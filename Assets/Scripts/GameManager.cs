using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject[] enemyObjs;
    public Transform[] spawnPoints;
    public float maxSpawnDelay;
    public float curSpawnDelay;

    public GameObject player;
    public Text scoreText;
    public Image[] lifeImage;
    public Image[] boomImage;
    public GameObject gameOverSet;

    void Awake()
    {
        if(instance==null){
            instance=this;
        }
    }
    
    void Update()
    {
        curSpawnDelay+=Time.deltaTime;

        if(curSpawnDelay > maxSpawnDelay){
            SpawnEnemy();
            maxSpawnDelay = Random.Range(0.5f,3f);
            curSpawnDelay -= maxSpawnDelay;
        }

        // #.UI Score Update
        Player playerLogic = player.GetComponent<Player>();
        scoreText.text = string.Format("{0:n0}",playerLogic.score);
    }

    void SpawnEnemy(){
        int ranEnemy = Random.Range(0,enemyObjs.Length);
        int ranPoint = Random.Range(0,spawnPoints.Length);

        GameObject enemy = Instantiate(enemyObjs[ranEnemy],spawnPoints[ranPoint].position,spawnPoints[ranPoint].rotation);
        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();

        Enemy enemyLogic = enemy.GetComponent<Enemy>();
        enemyLogic.player = player;

        if(ranPoint == 5 || ranPoint == 6){
            rigid.velocity = Vector2.left * enemyLogic.speed + Vector2.down;
            enemy.transform.Rotate(Vector3.back*90);
        }else if(ranPoint == 7 || ranPoint == 8){
            rigid.velocity = Vector2.right * enemyLogic.speed + Vector2.down;
            enemy.transform.Rotate(Vector3.forward*90);
        }else{
            rigid.velocity = Vector2.down * enemyLogic.speed;
        }
    }

    public void UpdateLifeIcon(int life){        
        // #.UI Life Init Disable
        for(int index=0;index<3;index++){
            lifeImage[index].color= new Color(1,1,1,0);
        }

        // #.UI Life Init Active
        for(int index=0;index<life;index++){
            lifeImage[index].color= new Color(1,1,1,1);
        }
    }

    public void UpdateBoomIcon(int boom){        
        // #.UI Life Init Disable
        for(int index=0;index<3;index++){
            boomImage[index].color= new Color(1,1,1,0);
        }

        // #.UI Life Init Active
        for(int index=0;index<boom;index++){
            boomImage[index].color= new Color(1,1,1,1);
        }
    }

    public void RespawnPlayer(){
        Invoke("RespawnPlayerExe",2f);
    }

    void RespawnPlayerExe(){
        player.transform.position = Vector3.down * 3.5f;
        player.SetActive(true);

        Player playerLogic = player.GetComponent<Player>();
        playerLogic.isHit = false;
    }

    public void GameOver(){
        gameOverSet.SetActive(true);
    }

    public void GameRetry(){
        SceneManager.LoadScene(0);
    }
}
