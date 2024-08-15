using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            GameObject bullet = Instantiate(bulletObjA,transform.position,transform.rotation);
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();

            Vector3 dirVec = player.transform.position - transform.position;

            rigid.AddForce(dirVec.normalized * 4f,ForceMode2D.Impulse);
        }else if(enemyName == "C"){
            GameObject bulletR = Instantiate(bulletObjA,transform.position + Vector3.right * 0.3f,transform.rotation);
            GameObject bulletL = Instantiate(bulletObjA,transform.position + Vector3.left * 0.3f,transform.rotation);
            
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

            // #.Random Ratio Item Drop
            int ran = Random.Range(0,10);
            if(ran < 3){
                Debug.Log("item didn't dropped");
                //....
            }
            else if(ran < 6){ // Coin
                Instantiate(itemCoin,transform.position,Quaternion.identity);
            }
            else if(ran < 8){ // Power
                Instantiate(itemPower,transform.position,Quaternion.identity);
            }
            else if(ran < 10){ // Boom
                Instantiate(itemCoin,transform.position,Quaternion.identity);
            }

            Destroy(gameObject);
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
                Destroy(gameObject);
                break;
            case "PlayerBullet":
                Bullet bullet = other.gameObject.GetComponent<Bullet>();
                OnHit(bullet.dmg);

                Destroy(other.gameObject);
                break;
        }
    }
}
