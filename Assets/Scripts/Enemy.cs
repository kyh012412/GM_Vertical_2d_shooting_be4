using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    public string enemyName;
    public int enemyScore;
    public float speed;
    public int health;
    public Sprite[] sprites;    
    public float maxShotDelay;
    public float curShotDelay;

    public GameObject bulletObjA;
    public GameObject bulletObjB;
    public GameObject itemCoin;
    public GameObject itemPower;
    public GameObject itemBoom;
    public GameObject player;

    public ObjectManager objectManager;
    SpriteRenderer spriteRenderer;
    Animator anim;

    public int patternIndex;
    public int curPatternCount;
    public int[] maxPatternCount;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if(enemyName == "Boss"){            
            anim = GetComponent<Animator>();
        }
    }

    void Update()
    {
        if(enemyName == "Boss")
            return;
        Fire();
        Reload();
    }

    void Fire(){
        if(curShotDelay < maxShotDelay) return;

        if(enemyName == "A"){
            GameObject bullet = objectManager.MakeObj(ObjectManager.Type.BulletEnemyA);
            bullet.transform.position=transform.position;
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();

            Vector3 dirVec = player.transform.position - transform.position;

            rigid.AddForce(dirVec.normalized * 4f,ForceMode2D.Impulse);
        }else if(enemyName == "C"){
            GameObject bulletR = objectManager.MakeObj(ObjectManager.Type.BulletEnemyA);
            GameObject bulletL = objectManager.MakeObj(ObjectManager.Type.BulletEnemyA);
            
            bulletR.transform.position = transform.position + Vector3.right * 0.3f;
            bulletL.transform.position = transform.position + Vector3.left * 0.3f;
            
            Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
            Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();

            Vector3 dirVecR = player.transform.position - transform.position;
            Vector3 dirVecL = player.transform.position - transform.position;

            rigidR.AddForce(dirVecR.normalized * 4f,ForceMode2D.Impulse);
            rigidL.AddForce(dirVecL.normalized * 4f,ForceMode2D.Impulse);
        }

        curShotDelay -= maxShotDelay;
    }

    void Reload(){
        curShotDelay += Time.deltaTime;
    }

    public void OnHit(int dmg){
        if(health <=0) return;
        health -= dmg;

        if(enemyName == "Boss"){
            anim.SetTrigger("OnHit");
        }else{
            spriteRenderer.sprite = sprites[1];
            Invoke("ReturnSprite",0.1f);
        }

        if(health<=0){
            Player playerLogic = GameManager.instance.player.GetComponent<Player>();
            playerLogic.score += enemyScore;
            GameObject item=null;

            // #.Random Ratio Item Drop
            int ran = enemyName == "Boss" ? 0 : Random.Range(0,10);
            if(ran < 3){
                Debug.Log("item didn't dropped");
                //....
            }
            else if(ran < 6){ // Coin
                item = objectManager.MakeObj(ObjectManager.Type.ItemCoin);
            }
            else if(ran < 8){ // Power
                item = objectManager.MakeObj(ObjectManager.Type.ItemPower);
            }
            else if(ran < 10){ // Boom
                item = objectManager.MakeObj(ObjectManager.Type.ItemBoom);
            }
            
            gameObject.SetActive(false);
            transform.rotation = quaternion.identity;

            if(!item) return;
            item.transform.position  = transform.position;
            // CancelInvoke("ReturnSprite");
        }
    }

    void ReturnSprite(){
        spriteRenderer.sprite = sprites[0];
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        switch(other.gameObject.tag){
            case "BorderBullet":
                if(enemyName == "Boss") break;
                gameObject.SetActive(false);
                transform.rotation = quaternion.identity;
                break;
            case "PlayerBullet":
                Bullet bullet = other.gameObject.GetComponent<Bullet>();
                OnHit(bullet.dmg);

                other.gameObject.SetActive(false);
                break;
        }
    }

    void OnEnable()
    {
        switch(enemyName){
            case "Boss":
                health = 3000;
                Invoke("Stop",2f);
                break;
            case "C":
                health=40;
                break;
            case "B":
                health=10;
                break;
            case "A":
                health=3;
                break;
        }
    }

    void Stop(){
        if(!gameObject.activeSelf){
            return;
        }

        Rigidbody2D rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = Vector2.zero;

        Invoke("Think",2);
    }

    void Think(){
        patternIndex = (patternIndex+1) % 4;
        curPatternCount = 0;

        switch(patternIndex){
            case 0:
                FireForward();
                break;
            case 1:
                FireShot();
                break;
            case 2:
                FireArc();
                break;
            case 3:
                FireAround();                
                break;
            default:
                Debug.Log("예외 발생");
                break;
        }
    }

    void FireForward(){
        // # Fire 4 Bullet Forward
        Debug.Log("앞으로 4발 발사");

        GameObject bulletR = objectManager.MakeObj(ObjectManager.Type.BulletBossA);
        GameObject bulletL = objectManager.MakeObj(ObjectManager.Type.BulletBossA);
        GameObject bulletRR = objectManager.MakeObj(ObjectManager.Type.BulletBossA);
        GameObject bulletLL = objectManager.MakeObj(ObjectManager.Type.BulletBossA);
        
        bulletR.transform.position = transform.position + Vector3.right * 0.62f;
        bulletL.transform.position = transform.position + Vector3.left * 0.62f;
        bulletRR.transform.position = transform.position + Vector3.right * 0.84f;
        bulletLL.transform.position = transform.position + Vector3.left * 0.84f;
        
        Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();

        // Vector3 dirVecR = player.transform.position - transform.position;
        // Vector3 dirVecL = player.transform.position - transform.position;
        // Vector3 dirVecRR = player.transform.position - transform.position;
        // Vector3 dirVecLL = player.transform.position - transform.position;

        rigidR.AddForce(Vector2.down * 8f,ForceMode2D.Impulse);
        rigidL.AddForce(Vector2.down * 8f,ForceMode2D.Impulse);
        rigidRR.AddForce(Vector2.down * 8f,ForceMode2D.Impulse);
        rigidLL.AddForce(Vector2.down * 8f,ForceMode2D.Impulse);
        //코드 수정필요

        // # Pattern Counting
        curPatternCount++;
        if(curPatternCount< maxPatternCount[patternIndex])
            Invoke("FireForward",2f);
        else 
            Invoke("Think",3f);
    }

    void FireShot(){
        // # Fire 5 Random Shotgun Bullet to Player
        Debug.Log("플레이어 방향으로 샷건.");

        for(int i=0;i<5;i++){
            GameObject bullet = objectManager.MakeObj(ObjectManager.Type.BulletEnemyB);
            bullet.transform.position=transform.position;

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector2 dirVec = player.transform.position - transform.position;
            Vector2 ranVec = new Vector2(Random.Range(-0.5f,0.5f),Random.Range(0f,2f));
            dirVec += ranVec;
            rigid.AddForce(dirVec.normalized * 5f,ForceMode2D.Impulse);
        }

        // # Pattern Counting
        curPatternCount++;

        if(curPatternCount< maxPatternCount[patternIndex])
            Invoke("FireShot",3.5f);
        else 
            Invoke("Think",3f);
    }

    void FireArc(){
        // # Fire Arc Continue Fire
        Debug.Log("부채모양으로 발사.");

        GameObject bullet = objectManager.MakeObj(ObjectManager.Type.BulletEnemyA);
        bullet.transform.position=transform.position;
        bullet.transform.rotation = quaternion.identity;

        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 10 * curPatternCount/maxPatternCount[patternIndex]),-1);
        rigid.AddForce(dirVec.normalized * 5f,ForceMode2D.Impulse);

        // # Pattern Counting
        curPatternCount++;

        if(curPatternCount< maxPatternCount[patternIndex])
            Invoke("FireArc",0.15f);
        else 
            Invoke("Think",3f);
    }

    void FireAround(){
        // # Fire Around
        Debug.Log("부채모양으로 발사.");
        int roundNumA = 50;
        int roundNumB = 40;
        int roundNum = curPatternCount%2==0? roundNumA: roundNumB;

        for(int i=0;i<roundNum;i++){
            GameObject bullet = objectManager.MakeObj(ObjectManager.Type.BulletBossB);
            bullet.transform.position=transform.position;
            bullet.transform.rotation = quaternion.identity;

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 2 * i / roundNum),Mathf.Sin(Mathf.PI * 2 * i / roundNum));
            rigid.AddForce(dirVec.normalized * 2f,ForceMode2D.Impulse);

            Vector3 rotVec = Vector3.forward * 360 * i / roundNum +Vector3.forward*90;
            bullet.transform.Rotate(rotVec);
        }

        Debug.Log("원 형태로 전체 공격");
        
        curPatternCount++;
        if(curPatternCount< maxPatternCount[patternIndex])
            Invoke("FireArc",0.7f);
        else 
            Invoke("Think",3f);
    }
}
