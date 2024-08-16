using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Follower : MonoBehaviour
{
    public float maxShotDelay;
    public float curShotDelay;

    public Vector3 followPos;
    public int followDelay;
    public Transform parent;
    public Queue<Vector3> parentPos;
    
    void Awake()
    {
        parentPos = new Queue<Vector3>();
    }

    void Update()
    {
        Watch();
        Follow();
        Fire();
        Reload();
    }

    public void Watch(){
        //Queue = FIFO (First)

        // # Input Pos
        if(!parentPos.Contains(parent.position))
            parentPos.Enqueue(parent.position);

        // # Output Pos
        if(parentPos.Count>followDelay)
            followPos = parentPos.Dequeue();
        else if(parentPos.Count>followDelay){
            followPos = parent.position;
        }
    }

    public void Follow(){
        transform.position = followPos;
    }

    void Fire(){
        // if(!Input.GetButton("Fire1")) return;
        if(curShotDelay < maxShotDelay) return;

        GameObject bullet = ObjectManager.instance.MakeObj(ObjectManager.Type.BulletFollower);
        bullet.transform.position = transform.position;
        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        rigid.AddForce(Vector2.up * 10f,ForceMode2D.Impulse);

        // curShotDelay -= maxShotDelay;
        curShotDelay = 0;
    }

    void Reload(){
        curShotDelay += Time.deltaTime;
    }
}
