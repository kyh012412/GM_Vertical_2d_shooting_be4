using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;


    public int stage;
    public Animator stageAnim;
    public Animator clearAnim;
    public Animator fadeAnim;
    public Transform playerPos;
    public GameObject[] enemyObjs;
    public Transform[] spawnPoints;
    public float nextSpawnDelay;
    public float curSpawnDelay;

    public GameObject player;
    public Text scoreText;
    public Image[] lifeImage;
    public Image[] boomImage;
    public GameObject gameOverSet;

    public ObjectManager objectManager;

    public List<Spawn> spawnList;
    public int spawnIndex;
    public bool spawnEnd;

    void Awake()
    {
        if(instance==null){
            instance=this;
            spawnList = new List<Spawn>();
            StageStart();
        }
    }

    public void StageStart(){
        // # Stage UI Load
        stageAnim.SetTrigger("On");
        stageAnim.GetComponent<Text>().text = "Stage "+stage+"\nStart";
        clearAnim.GetComponent<Text>().text = "Stage "+stage+"\nClear";

        // # Enemy Spawn File Read
        ReadSpawnFile();

        // # Fade In (밝아지는 것)
        fadeAnim.SetTrigger("In");
    }

    public void StageEnd(){
        // # Clear UI Load
        clearAnim.SetTrigger("On");

        // # Fade Out (어두워 지는 것)
        fadeAnim.SetTrigger("Out");

        // # Player Repos
        player.transform.position = playerPos.position;

        // # Stage Increment
        stage++;
        if(stage > 2)
            Invoke("GameOver",6f);
        else
            Invoke("StageStart",5f);
    }

    void ReadSpawnFile(){
        // #1.변수 초기화
        spawnList.Clear();
        spawnIndex=0;
        spawnEnd=false;

        // #2.리스폰 파일 읽기
        TextAsset textFile = Resources.Load("Stage " + stage) as TextAsset; //as Text는 자료형 검증 .. 만약에 아닐시 null처리가 됨
        StringReader stringReader = new StringReader(textFile.text);

        while(stringReader != null){
            string line = stringReader.ReadLine();
            Debug.Log(line);

            if(line == null)
                break;            

            // # 리스폰 데이터 생성
            Spawn spawnData = new Spawn();
            spawnData.delay = float.Parse(line.Split('/')[0]);
            spawnData.type = (ObjectManager.Type)int.Parse(line.Split('/')[1]);
            spawnData.point = int.Parse(line.Split('/')[2]);

            // # 텍스트 파일 닫기
            spawnList.Add(spawnData);

            // # 첫번째 스폰 딜레이 적용
            nextSpawnDelay = spawnList[0].delay;
        }

        //#.텍스트 파일 닫기
        stringReader.Close();
    }
    
    void Update()
    {
        curSpawnDelay+=Time.deltaTime;

        if(curSpawnDelay > nextSpawnDelay && !spawnEnd){
            SpawnEnemy();
            curSpawnDelay = 0;
            
            // curSpawnDelay -= nextSpawnDelay
        }

        // #.UI Score Update
        Player playerLogic = player.GetComponent<Player>();
        scoreText.text = string.Format("{0:n0}",playerLogic.score);
    }

    void SpawnEnemy(){       
        int enemyPoint = spawnList[spawnIndex].point;
        GameObject enemy = objectManager.MakeObj(spawnList[spawnIndex].type); // 0~2
        enemy.transform.position = spawnPoints[enemyPoint].position;
        
        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();

        Enemy enemyLogic = enemy.GetComponent<Enemy>();
        enemyLogic.player = player;
        enemyLogic.objectManager=objectManager;

        if(enemyPoint == 5 || enemyPoint == 6){
            rigid.velocity = Vector2.left * enemyLogic.speed + Vector2.down;
            enemy.transform.Rotate(Vector3.back*90);
        }else if(enemyPoint == 7 || enemyPoint == 8){
            rigid.velocity = Vector2.right * enemyLogic.speed + Vector2.down;
            enemy.transform.Rotate(Vector3.forward*90);
        }else{
            rigid.velocity = Vector2.down * enemyLogic.speed;
        }

        // # 리스폰 인덱스 증가
        spawnIndex++;
        if(spawnIndex == spawnList.Count){
            spawnEnd = true;
            return;
        }

        // # 다음 리스폰딜레이 갱신
        nextSpawnDelay = spawnList[spawnIndex].delay;
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

    public void CallExplosion(Vector3 pos,ObjectManager.Type type){
        GameObject explosion = objectManager.MakeObj(ObjectManager.Type.Explosion);
        Explosion explosionLogic = explosion.GetComponent<Explosion>();

        explosion.transform.position = pos;
        explosionLogic.StartExplosion(type);
    }
}
