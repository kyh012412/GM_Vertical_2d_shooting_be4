using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public int health;
    public Sprite[] sprites;

    SpriteRenderer spriteRenderer;
    Rigidbody2D rigid;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = Vector2.down * speed;
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
