using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public GameObject[] enemyObjs;
    public Transform[] spawnPoints;
    public float maxSpawnDelay;
    public float curSpawnDelay;
    
    void Update()
    {
        curSpawnDelay+=Time.deltaTime;

        if(curSpawnDelay > maxSpawnDelay){
            SpawnEnemy();
            maxSpawnDelay = Random.Range(0.5f,3f);
            curSpawnDelay -= maxSpawnDelay;
        }
    }

    void SpawnEnemy(){
        int ranEnemy = Random.Range(0,enemyObjs.Length);
        int ranPoint = Random.Range(0,spawnPoints.Length);
        Instantiate(enemyObjs[ranEnemy],spawnPoints[ranPoint].position,spawnPoints[ranPoint].rotation);
    }
}
