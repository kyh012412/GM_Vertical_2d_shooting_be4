using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public bool isTouchTop;
    public bool isTouchBottom;
    public bool isTouchRight;
    public bool isTouchLeft;

    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
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
