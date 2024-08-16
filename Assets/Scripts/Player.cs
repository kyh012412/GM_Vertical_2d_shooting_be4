using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool isTouchTop;
    public bool isTouchBottom;
    public bool isTouchRight;
    public bool isTouchLeft;

    public int life;
    public int score;
    public float speed;
    public int maxPower;
    public int power;
    public int maxBoom;
    public int boom;
    public float maxShotDelay;
    public float curShotDelay;

    public GameObject bulletObjA;
    public GameObject bulletObjB;
    public GameObject boomEffect;
    public GameObject[] followers;

    public bool isHit;
    public bool isBoomTime;

    Animator anim;
    public ObjectManager objectManager;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        Move();
        Fire();
        Boom();
        Reload();
    }

    void Move(){
        float h = Input.GetAxisRaw("Horizontal"); // -1,0,1
        float v = Input.GetAxisRaw("Vertical"); // -1,0,1

        if(isTouchRight && h==1){
            h=0;
        }
        else if(isTouchLeft && h==-1){
            h=0;
        }

        if(isTouchTop && v==1){
            v=0;
        }
        else if(isTouchBottom && v==-1){
            v=0;
        }

        Vector3 curPos = transform.position;
        Vector3 nextPos = new Vector3(h,v,0)*speed*Time.deltaTime;

        transform.position = curPos+nextPos;

        if(Input.GetButtonDown("Horizontal") || Input.GetButtonUp("Horizontal")){
            anim.SetInteger("Input",(int)h);
        }
    }

    void Fire(){
        // if(!Input.GetButton("Fire1")) return;
        if(curShotDelay < maxShotDelay) return;
        switch(power){
            case 1:
                GameObject bullet = objectManager.MakeObj(ObjectManager.Type.BulletPlayerA);
                bullet.transform.position = transform.position;
                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                rigid.AddForce(Vector2.up * 10f,ForceMode2D.Impulse);
                break;
            case 2:
                FireCase2();
                break;
            default:
                FireCase2();   
                GameObject bulletB = objectManager.MakeObj(ObjectManager.Type.BulletPlayerB);
                bulletB.transform.position = transform.position;
                Rigidbody2D rigidB = bulletB.GetComponent<Rigidbody2D>();
                rigidB.AddForce(Vector2.up * 10f,ForceMode2D.Impulse);
                break;
        }

        // curShotDelay -= maxShotDelay;
        curShotDelay = 0;
    }

    void FireCase2(){
        GameObject bulletR = objectManager.MakeObj(ObjectManager.Type.BulletPlayerA);
        GameObject bulletL = objectManager.MakeObj(ObjectManager.Type.BulletPlayerA);

        bulletR.transform.position = transform.position  + Vector3.right*0.1f;
        bulletL.transform.position = transform.position  + Vector3.left*0.1f;

        Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
        rigidR.AddForce(Vector2.up * 10f,ForceMode2D.Impulse);
        rigidL.AddForce(Vector2.up * 10f,ForceMode2D.Impulse);
    }

    void Reload(){
        curShotDelay += Time.deltaTime;
    }

    void Boom(){
        if(!Input.GetButton("Fire2")) return;
        if(isBoomTime) return;
        if(boom <= 0) return;

        boom--;
        GameManager.instance.UpdateBoomIcon(boom);
        isBoomTime=true;

        // #1. Effect visible
        boomEffect.SetActive(true);
        Invoke("OffBoomEffect",4f);

        // #2.Remove
        DeactivateGroup(objectManager.GetPool(ObjectManager.Type.EnemyA));
        DeactivateGroup(objectManager.GetPool(ObjectManager.Type.EnemyB));
        DeactivateGroup(objectManager.GetPool(ObjectManager.Type.EnemyC));
        
        // #3. Remove Enemy Bullet
        DeactivateGroup(objectManager.GetPool(ObjectManager.Type.BulletEnemyA));
        DeactivateGroup(objectManager.GetPool(ObjectManager.Type.BulletEnemyB));
    }

    void OffBoomEffect(){
        boomEffect.SetActive(false);
        isBoomTime=false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Border")){
            switch(other.gameObject.name){
                case "Top":
                    isTouchTop=true;
                    break;
                case "Bottom":
                    isTouchBottom=true;
                    break;
                case "Right":
                    isTouchRight=true;
                    break;
                case "Left":
                    isTouchLeft=true;
                    break;
            }
        }else if(other.gameObject.CompareTag("EnemyBullet") || other.gameObject.CompareTag("Enemy")){
            if(isHit) return;

            isHit = true;
            life--;
            GameManager.instance.UpdateLifeIcon(life);

            if(life==0){
                GameManager.instance.GameOver();
            }else{
                GameManager.instance.RespawnPlayer();
            }
            gameObject.SetActive(false);
            other.gameObject.SetActive(false);
        }else if(other.gameObject.CompareTag("Item")){
            Item item = other.gameObject.GetComponent<Item>();
            switch(item.type){
                case Item.ItemType.Coin:
                    score += 1000;
                    break;
                case Item.ItemType.Power:
                    if(power == maxPower){
                        score += 500;
                    }else{
                        power++;
                        AddFollower();
                    }
                    break;
                case Item.ItemType.Boom:
                    if(boom == maxBoom){
                        score += 500;
                    }else{
                        boom++;
                        GameManager.instance.UpdateBoomIcon(boom);
                    }
                    break;
            }

            other.gameObject.SetActive(false);
        }
    }
    void AddFollower(){
        if(power<4) return;
        if(power>6) return;

        followers[power-4].SetActive(true);
    }

    //active 인것만 Deactivate하기
    void DeactivateGroup(GameObject[] targetGroup){
        for(int i=0;i<targetGroup.Length;i++){
            if(!targetGroup[i].activeSelf) return;

            Enemy enemyLogic = targetGroup[i].GetComponent<Enemy>();
            if(enemyLogic) {
                enemyLogic.OnHit(1000);
                return;
            }

            targetGroup[i].SetActive(false);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Border")){
            switch(other.gameObject.name){
                case "Top":
                    isTouchTop=false;
                    break;
                case "Bottom":
                    isTouchBottom=false;
                    break;
                case "Right":
                    isTouchRight=false;
                    break;
                case "Left":
                    isTouchLeft=false;
                    break;
            }
        }
    }
}
