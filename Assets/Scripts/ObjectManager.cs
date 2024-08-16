using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager instance;
    public enum Type {EnemyA, EnemyB, EnemyC, EnemyBoss, ItemPower, ItemBoom, ItemCoin, BulletPlayerA, BulletPlayerB, BulletEnemyA, BulletEnemyB,BulletBossA,BulletBossB,BulletFollower};
    public GameObject enemyCPrefab;
    public GameObject enemyBPrefab;
    public GameObject enemyAPrefab;
    public GameObject enemyBossPrefab;

    public GameObject itemCoinPrefab;
    public GameObject itemPowerPrefab;
    public GameObject itemBoomPrefab;

    public GameObject bulletPlayerAPrefab;
    public GameObject bulletPlayerBPrefab;
    public GameObject bulletEnemyAPrefab;
    public GameObject bulletEnemyBPrefab;
    public GameObject bulletBossAPrefab;
    public GameObject bulletBossBPrefab;
    public GameObject bulletFollowerPrefab;

    GameObject[] enemyC;
    GameObject[] enemyB;
    GameObject[] enemyA;
    GameObject[] enemyBoss;
    GameObject[] itemCoin;
    GameObject[] itemPower;
    GameObject[] itemBoom;

    GameObject[] bulletPlayerA;
    GameObject[] bulletPlayerB;
    GameObject[] bulletEnemyA;
    GameObject[] bulletEnemyB;
    GameObject[] bulletBossA;
    GameObject[] bulletBossB;
    GameObject[] bulletFollower;
    List<GameObject[]> pools;

    void Awake()
    {
        if(instance==null){
            instance=this;
            //my pools definition
            pools = new List<GameObject[]>(Enum.GetValues(typeof(Type)).Length);
            for(int i=0;i<Enum.GetValues(typeof(Type)).Length;i++){
                pools.Add(null);
            }
            enemyA = new GameObject[10];
            enemyB = new GameObject[10];
            enemyC = new GameObject[20];
            enemyBoss = new GameObject[5];

            itemCoin = new GameObject[20];
            itemPower = new GameObject[10];
            itemBoom = new GameObject[10];

            bulletPlayerA = new GameObject[100];
            bulletPlayerB = new GameObject[100];        
            bulletEnemyA = new GameObject[100];
            bulletEnemyB = new GameObject[100];
            bulletBossA = new GameObject[50];
            bulletBossB = new GameObject[1000];
            bulletFollower = new GameObject[100];
            Generate();
        }
    }

    // 수정 필요
    void Generate(){
        // #1.Enemy
        for(int i=0;i<enemyC.Length;i++){
            enemyC[i] = Instantiate(enemyCPrefab);
            enemyC[i].SetActive(false);
        }
        
        for(int i=0;i<enemyB.Length;i++){
            enemyB[i] = Instantiate(enemyBPrefab);
            enemyB[i].SetActive(false);
        }
        
        for(int i=0;i<enemyA.Length;i++){
            enemyA[i] = Instantiate(enemyAPrefab);
            enemyA[i].SetActive(false);
        }

        for(int i=0;i<enemyBoss.Length;i++){
            enemyBoss[i] = Instantiate(enemyBossPrefab);
            enemyBoss[i].SetActive(false);
        }
        
        // #2.Item
        for(int i=0;i<itemCoin.Length;i++){
            itemCoin[i] = Instantiate(itemCoinPrefab);
            itemCoin[i].SetActive(false);
        }
        
        for(int i=0;i<itemPower.Length;i++){
            itemPower[i] = Instantiate(itemPowerPrefab);
            itemPower[i].SetActive(false);
        }
        
        for(int i=0;i<itemBoom.Length;i++){
            itemBoom[i] = Instantiate(itemBoomPrefab);
            itemBoom[i].SetActive(false);
        }
        
        // #3.Bullet
        for(int i=0;i<bulletPlayerA.Length;i++){
            bulletPlayerA[i] = Instantiate(bulletPlayerAPrefab);
            bulletPlayerA[i].SetActive(false);
        }
        
        for(int i=0;i<bulletPlayerB.Length;i++){
            bulletPlayerB[i] = Instantiate(bulletPlayerBPrefab);
            bulletPlayerB[i].SetActive(false);
        }
        
        for(int i=0;i<bulletEnemyA.Length;i++){
            bulletEnemyA[i] = Instantiate(bulletEnemyAPrefab);
            bulletEnemyA[i].SetActive(false);
        }
        
        for(int i=0;i<bulletEnemyB.Length;i++){
            bulletEnemyB[i] = Instantiate(bulletEnemyBPrefab);
            bulletEnemyB[i].SetActive(false);
        }
        for(int i=0;i<bulletBossA.Length;i++){
            bulletBossA[i] = Instantiate(bulletBossAPrefab);
            bulletBossA[i].SetActive(false);
        }
        for(int i=0;i<bulletBossB.Length;i++){
            bulletBossB[i] = Instantiate(bulletBossBPrefab);
            bulletBossB[i].SetActive(false);
        }
        for(int i=0;i<bulletFollower.Length;i++){
            bulletFollower[i] = Instantiate(bulletFollowerPrefab);
            bulletFollower[i].SetActive(false);
        }
        
        pools[0] = enemyA;
        pools[1] = enemyB;
        pools[2] = enemyC;
        pools[3] = enemyBoss;
        pools[4] = itemPower;
        pools[5] = itemBoom;
        pools[6] = itemCoin;
        pools[7] = bulletPlayerA;
        pools[8] = bulletPlayerB;
        pools[9] = bulletEnemyA;
        pools[10] = bulletEnemyB;
        pools[11] = bulletBossA;
        pools[12] = bulletBossB;
        pools[13] = bulletFollower;
    }

    public GameObject MakeObj(Type type){
        GameObject[] targetPool = pools[(int)type];
        if(targetPool.Length==0) {
            GameObject target = InitOnePrefab(type);
            //추가 로직 필요
        }

        for(int i=0;i<targetPool.Length;i++){
            if(!targetPool[i].activeSelf){
                GameObject target = targetPool[i];
                target.SetActive(true);
                return target ;
            }
        }
        return null;
    }

    public GameObject InitOnePrefab(Type typeNum){
        switch(typeNum){
            case Type.EnemyC:
                return Instantiate(enemyCPrefab);
            case Type.EnemyB:
                return Instantiate(enemyBPrefab);
            case Type.EnemyA:
                return Instantiate(enemyAPrefab);
            case Type.EnemyBoss:
                return Instantiate(enemyBossPrefab);
            case Type.ItemPower:
                return Instantiate(itemPowerPrefab);
            case Type.ItemBoom:
                return Instantiate(enemyCPrefab);
            case Type.ItemCoin:
                return Instantiate(itemCoinPrefab);
            case Type.BulletPlayerA:
                return Instantiate(bulletPlayerAPrefab);
            case Type.BulletPlayerB:
                return Instantiate(bulletPlayerBPrefab);
            case Type.BulletEnemyA:
                return Instantiate(bulletEnemyAPrefab);
            case Type.BulletEnemyB:
                return Instantiate(bulletEnemyBPrefab);
            case Type.BulletFollower:
                return Instantiate(bulletFollowerPrefab);
            case Type.BulletBossA:
                return Instantiate(bulletBossAPrefab);
            case Type.BulletBossB:
                return Instantiate(bulletBossBPrefab);
            default :
                Debug.Log("에러발생");
                break;
        }
        return null;
    }

    public GameObject[] GetPool(Type type){
        return pools[(int)type];
    }
}
