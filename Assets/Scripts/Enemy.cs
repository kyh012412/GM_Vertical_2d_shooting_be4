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

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Fire();
        Reload();
    }

    void Fire(){
        if(curShotDelay < maxShotDelay) return;

        if(enemyName == "A"){
            GameObject bullet = objectManager.MakeObj(ObjectManager.Type.BulletEnemyA);
            //Instantiate(bulletObjA,transform.position,transform.rotation);
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
        spriteRenderer.sprite = sprites[1];
        Invoke("ReturnSprite",0.1f);

        if(health<=0){
            Player playerLogic = GameManager.instance.player.GetComponent<Player>();
            playerLogic.score += enemyScore;
            GameObject item=null;

            // #.Random Ratio Item Drop
            int ran = Random.Range(0,10);
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
}
