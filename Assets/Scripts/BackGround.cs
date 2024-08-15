using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGround : MonoBehaviour
{
    public float speed;
    public int startIndex;
    public int endIndex;
    public Transform[] sprites;

    float viewHeight;

    void Awake()
    {
        viewHeight = Camera.main.orthographicSize*2;
    }

    void Update()
    {
        Move();
        Scrolling();
    }

    void Move(){
        Vector3 curPos = transform.position;
        Vector3 nextPos = Vector3.down * speed * Time.deltaTime;
        transform.position = curPos + nextPos;
    }

    void Scrolling(){
        if(sprites[endIndex].position.y < viewHeight * (-1)){
            // #.Sprite ReUse
            Vector3 backSpritePos = sprites[startIndex].localPosition;
            Vector3 frontSpritePos = sprites[endIndex].localPosition;
            sprites[endIndex].transform.localPosition = backSpritePos + Vector3.up*10;

            // #.Cursor Index Change
            startIndex=++startIndex%3;
            endIndex=++endIndex%3;
        }
    }
}
