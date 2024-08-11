using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int dmg;
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("BorderBullet")){
            Destroy(gameObject);
        }
    }
}
