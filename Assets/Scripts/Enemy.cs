using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string enemyName;
    public float speed;
    public int health;
    public Sprite[] sprites;    
    public float maxShotDelay;
    public float curShotDelay;

    public GameObject bulletObjA;
    public GameObject bulletObjB;
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

            rigid.AddForce(dirVec.normalized * 10f,ForceMode2D.Impulse);
        }else if(enemyName == "C"){
            GameObject bulletR = Instantiate(bulletObjA,transform.position + Vector3.right * 0.3f,transform.rotation);
            GameObject bulletL = Instantiate(bulletObjA,transform.position + Vector3.left * 0.3f,transform.rotation);
            
            Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
            Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();

            Vector3 dirVecR = player.transform.position - transform.position;
            Vector3 dirVecL = player.transform.position - transform.position;

            rigidR.AddForce(dirVecR.normalized * 10f,ForceMode2D.Impulse);
            rigidL.AddForce(dirVecL.normalized * 10f,ForceMode2D.Impulse);
        }

        curShotDelay -= maxShotDelay;
    }

    void Reload(){
        curShotDelay += Time.deltaTime;
    }

    void OnHit(int dmg){
        health -= dmg;
        spriteRenderer.sprite = sprites[1];
        Invoke("ReturnSprite",0.1f);

        if(health<=0){
            Debug.Log("dead action");
            Destroy(gameObject);
            CancelInvoke("ReturnSprite");
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
