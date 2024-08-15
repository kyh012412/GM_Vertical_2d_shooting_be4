https://www.youtube.com/watch?v=ETYzjbnLixY&list=PLO-mt5Iu5TeYI4dbYwWP8JqZMC9iuUIW2&index=31

be4

[에셋 다운 링크](https://assetstore.unity.com/packages/2d/characters/vertical-2d-shooting-assets-pack-188719)

### 2D 종스크롤 슈팅 - 플레이어 이동 구현하기 [B27+Assets]

#### 준비하기

1. spries 폴더 내에 아틀라스 설정
   1. Pixels per unit 24
   2. Filter mode Point
   3. Compression none
   4. Sprite Mode - multiple
2. 자르기
   1. pixel size 24,24
   2. padding 1,1
   3. slice apply
3. 첫번째 sprite를 하이라키 내로 드래그(Player)
4. 3D 아이콘 크기 줄이기

#### 플레이어 이동

1. Player.cs 생성

   ```cs
   public class Player : MonoBehaviour
   {
       public float speed;

       // Update is called once per frame
       void Update()
       {
           float h = Input.GetAxisRaw("Horizontal"); // -1,0,1
           float v = Input.GetAxisRaw("Vertical"); // -1,0,1
           Vector3 curPos = transform.position;
           Vector3 nextPos = new Vector3(h,v,0)*speed*Time.deltaTime;

           transform.position = curPos+nextPos;
       }
   }
   ```

1. inspector 상에서 speed값 3
1. 테스트
1. game tab에서 화면비율 9:16
1. 9:19비율도 준비해준다.

#### 경계설정

1. Player 객체내에
   1. box collider 2d 컴포넌트를 추가
   2. collider size 0.3 0.3
   3. rigidbody 2d
      1. bodytype :kinematic
2. 하이라키에 빈객체 추가(Border)
   1. border 내에 top bottom left right를 만들어주고
      1. 각각내에 box collider 2d를 넣어주고 사이즈와 위치 조절
         1. is trigger 체크
      2. 각각 내에 rigidbody 2d를 넣어준다.
         1. Body type : Static
3. Player 내에 rigidbody 2d > Body type : kinematic (스크립트로만 물리적 충돌 조정예정)

#### 경계 충돌 로직

1. Player.cs 추가

   ```cs
   public bool isTouchTop;
   public bool isTouchBottom;
   public bool isTouchRight;
   public bool isTouchLeft;

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
   ```

2. border 내에 자식들에게 Border 라는 태그를 추가해준다.

#### 애니메이션

1. spitre로부터 Center,Left,Right.anim을 만들어준다.
2. Center,Left,Right 서로 이동할수 있게 6개의 transition을 만들어주고
3. Input (int) 파라미터추가
4. 분기처리(-1일시 left로 0일시 center 1일시 right가 도착상태가되게끔)
5. transition 의 has exit time을 uncheck로 해준다.
6. transition duration을 0으로한다.
7. 적당한 곳에 animator에 트리거를 작동시켜준다.

   ```cs
   Animator anim;

   void Awake()
   {
   	anim = GetComponent<Animator>();
   }
   void Update()
   {
   	if(Input.GetButtonDown("Horizontal") || Input.GetButtonUp("Horizontal")){

   		anim.SetInteger("Input",(int)h);
   	}
   }
   ```

8. Background grid를 하이라키에 넣어준다.

### 2D 종스크롤 슈팅 - 총알발사 구현하기 [B28]

https://www.youtube.com/watch?v=JUG0GnsJHQw&list=PLO-mt5Iu5TeYI4dbYwWP8JqZMC9iuUIW2&index=32

#### 준비하기(프리펩)

1. Player Bullet 0을 하이라키에 드랍(Player Bullet A)
   1. 위치 초기화
   2. box collider 2d와 rigidbody 2d 추가
      1. Bullet은 추후 addforce로 날릴것이기에 Body Type은 Dynamic을 써야한다.
      2. is trigger 체크
2. Assets/Prefabs 폴더를 만들어 준다.
   1. 프리펩 : 재활용을 위해 에셋으로 저장된 게임 오브젝트
3. Prefabs 폴더내로 Player Bullet A를 끌어놔준다.
4. Player Bullet A를 복사 (Player Bullet B)
   1. Scale x 1.5, y 1.5
   2. collider size 0.3, 0.4
   3. sprite renderer내에 sprite를 Player Bullet 1로 변경
   4. Prefabs 폴더내로 드래그
      1. original prefab을 눌러서 새로운 prefab으로 만들어준다.
5. Prefab을 잘 만들었다면 하이라키상에서 Bullet 제거

#### 발사체 제거(오브젝트 삭제)

1. Bullet.cs생성
   ```cs
   public class Bullet : MonoBehaviour
   {
       void OnTriggerEnter2D(Collider2D other)
       {
           if(other.gameObject.CompareTag("BorderBullet")){
               Destroy(gameObject);
           }
       }
   }
   ```
1. 하이라키내에 border를 복사하여준다. (borderBullect)
   1. border보다 길이를 2씩증가시킨정도?
   2. 태그도 BorderBullet으로 변경
1. 미리 만들어논 prefab내에 Bullet.cs를 넣어준다.
1. 테스트 /정상

#### 발사체 생성(오브젝트 생성)

1. 코드

   ```cs
   public GameObject bulletObjA;
   public GameObject bulletObjB;

   void Update()
   {
   	Move();
   	Fire();
   }

   void Move(){
   	//...
   }

   void Fire(){
   	float forcePower = 10f;
   	// GameObject bulletA = Instantiate(bulletObjA,transform);
   	GameObject bullet = Instantiate(bulletObjA,transform.position,transform.rotation);
   	Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
   	rigid.AddForce(Vector2.up * forcePower,ForceMode2D.Impulse);
   }
   ```

1. inspector 내에서 bullet 객체 넣어주기

#### 발사체 다듬기

1. //fire 로직 수정

```cs
public float maxShotDelay;
public float curShotDelay;

void Update()
{
	//...
	Reload();
}

void Fire(){
	if(!Input.GetButton("Fire1")) return;

	if(curShotDelay < maxShotDelay) return;

	// before logic

	curShotDelay = 0;
}
```

1. inspector에서 max shot delay 조정 0.15

#### 발사체 파워

1. 코드

```cs
    public float power;

    void Fire(){
        if(!Input.GetButton("Fire1")) return;

        if(curShotDelay < maxShotDelay) return;

        switch(power){
            case 1:
                GameObject bullet = Instantiate(bulletObjA,transform.position,transform.rotation);
                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                rigid.AddForce(Vector2.up * 10f,ForceMode2D.Impulse);
                break;
            case 2:
                FireCase2();
                break;
            case 3:
                FireCase2();  
                GameObject bulletB = Instantiate(bulletObjB,transform.position,transform.rotation);
                Rigidbody2D rigidB = bulletB.GetComponent<Rigidbody2D>();
                rigidB.AddForce(Vector2.up * 10f,ForceMode2D.Impulse);
                break;
        }
        // curShotDelay -= maxShotDelay;
        curShotDelay = 0;
    }

    void FireCase2(){
        GameObject bulletR = Instantiate(bulletObjA,transform.position + Vector3.right*0.1f,transform.rotation);
        GameObject bulletL = Instantiate(bulletObjA,transform.position + Vector3.left*0.1f ,transform.rotation);
        Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
        rigidR.AddForce(Vector2.up * 10f,ForceMode2D.Impulse);
        rigidL.AddForce(Vector2.up * 10f,ForceMode2D.Impulse);
    }
```

1. 테스트 / 정상

### 2D 종스크롤 슈팅 - 적 비행기 만들기 [B29]

#### 준비하기

1. Enemy A,B,C를 하이라키 상에 드래그
2. Enemy B에는 Polygon collider 2d를 사용
   1. polygon수를 최적화하여 부하를 줄이기
3. sprites 아틀라스로가서 sprite editor탭을열어서
   1. 좌상단에 sprite editor를 눌러보면다른 값들이 잇는데
   2. *Custom Physics Shape*를 선택
   3. 원하는 sprite 한장 선택후 generate 버튼을 누르면 모양이 나온다.
   4. polygon을 처음 생성할때이모양을 따라간다.
   5. 모양 조정후 상단의 apply 선택
4. Enemy A
   1. circle collider 2d
5. Enemy A,B,C에
   1. rigidbody 2d 컴포넌트추가
   2. Enemy Tag 달아주기
   3. is trigger 체크

#### 적 기체 프리펩

1. Enemy.cs 추가

   ```cs
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
   ```

1. 객체별로
   1. sprites를 넣어주며 1번 인덱스에 hit sprite를 넣어준다.
   2. 각자 speed와 health를 지정해준다.
1. 지난번에 만들어둔 prefab bullet의 태그를 PlayerBullet으로 만들어주기
1. 테스트 후 prefab화

#### 적 기체 생성

1. GameManager 객체와 cs 생성

   ```cs
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
   ```

1. GameManager내에 변수를 inspector에서 초기화
1. 테스트

### 2D 종스크롤 슈팅 - 적 전투와 피격 이벤트 만들기 [B30]

#### 적 생성추가

1. 12시방향이아닌 3시 9시에도 적 spawnpoint 추가

   ```cs
   void SpawnEnemy(){
   	int ranEnemy = Random.Range(0,enemyObjs.Length);
   	int ranPoint = Random.Range(0,spawnPoints.Length);

   	GameObject enemy = Instantiate(enemyObjs[ranEnemy],spawnPoints[ranPoint].position,spawnPoints[ranPoint].rotation);
   	Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();

   	Enemy enemyLogic = enemy.GetComponent<Enemy>();

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
   ```

1. inspector 상에 연결
1. 테스트

#### 적 공격

1. Player Bullet B를 복사 (Enemy Bullet A)
   1. 수정시 Rename File 선택
   2. Sprite 는 Enemy Bullet 1로 변경
   3. collider size 0.2 , 0.25
   4. auto save가 켜져있는지 확인
2. Enemy Bullet A 복사 (Enemy Bullet B)
   1. Sprite Enemy Bullet 3으로 변경
   2. Collider는 Circle collider 2d로 변경
      1. is trigger 체크
3. EnemyBullet태그를 만들어주고 적용한다.
4. Enemy.cs

   ```cs
   public Sprite[] sprites;
   public float maxShotDelay;
   public float curShotDelay;

   public GameObject bulletObjA;
   public GameObject bulletObjB;
   public GameObject player;

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
   ```

5. GameManager에 player 등록
6. Enemy프리펩 내에
   1. Bullect 프리펩 등록
   2. enemyName 등록
   3. maxShotDelay에 수치 값주기

#### 피격 이벤트

1. Player.cs
   ```cs
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
   	}else if(other.gameObject.CompareTag("EnemyBullet") || other.gameObject.CompareTag("Enemy")){
   		gameObject.SetActive(false);
   		GameManager.instance.RespawnPlayer();
   	}
   }
   ```
2. GameManger.cs

   ```cs
   public void RespawnPlayer(){
   	Invoke("RespawnPlayerExe",2f);
   }

   void RespawnPlayerExe(){
   	player.transform.position = Vector3.down * 3.5f;
   	player.SetActive(true);
   }
   ```

### 2D 종스크롤 슈팅 - UI 간단하게 완성하기 [B31]

#### 목숨과 점수 UI 배치

1. Player.cs
2. 코드
3. life3
4. 하이라키에 canvas > text 추가(Score Text)
   1. 상단에 좌우꽉차게
   2. 라벨 999,999,999
   3. 볼드
   4. 높이 100
   5. 폰트크기 25
   6. 중앙정렬, 중앙정렬
5. Image도 3개추가(Life Icon)
   1. 앵커 좌상단
   2. 소스 Life
   3. 가로세로 100 100
   4. pos x 10 posy -10
      1. x105씩증가
6. Canvas에서 Canvas Scaler값을
   1. Scale with Screen Size : 기준 해동사도의 UI 크기 유지
   2. (해상도와무관하게 크기를 유지할 수 있음)
   3. 현재는 1080 1920 사용
7. Score Text 복사 (GameOver Text)
   1. 앵커 화면 정중앙 (최대 x)
   2. pos y 30
   3. 폰트 크기 100
   4. 높이 150
8. Canvas 내에 Button 추가(Retry)
   1. pos y -150
   2. 가로 300 세로 200
   3. 이미지 소스 Button
9. 아틀라스에서 User Interface 아래 > Button 같은경우
   1. ![[Pasted image 20240811213925.png]]
   2. 이렇게 조정을 해주어야한다.(파란선과 녹색선의 위치를 잘 봐야한다.)
   3. 조정 후 apply
10. Retry 이하의 Text
    1. 볼드
    2. 라벨 REtry?
    3. 크기 65
11. Canvas 내에 빈 객체 추가(GameOver Set)
    1. 이하로 Retry와 GameOvet Text를 넣어준다.

#### UI 로직

https://www.youtube.com/watch?v=qXa7y1Que6s&list=PLO-mt5Iu5TeYI4dbYwWP8JqZMC9iuUIW2&index=35

1. 코드
2. GameManager.cs

   ```cs
   public Text scoreText;
   public Image[] lifeImage;
   public GameObject gameOverSet;

   void Update()
   {
   	curSpawnDelay+=Time.deltaTime;

   	if(curSpawnDelay > maxSpawnDelay){
   		//...
   	}

   	// #.UI Score Update
   	Player playerLogic = player.GetComponent<Player>();
   	scoreText.text = string.Format("{0:n0}",playerLogic.score);
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

   public void GameOver(){
   	gameOverSet.SetActive(true);
   }

   public void GameRetry(){
   	SceneManager.LoadScene(0);
   }
   ```

3. Enemy.cs

   ```cs
   public int enemyScore;

   void OnHit(int dmg){
   	health -= dmg;
   	spriteRenderer.sprite = sprites[1];
   	Invoke("ReturnSprite",0.1f);

   	if(health<=0){
   		Player playerLogic = GameManager.instance.player.GetComponent<Player>();
   		playerLogic.score += enemyScore;
   		Destroy(gameObject);
   	}
   }
   ```

4. Player.cs

   ```cs
   void OnTriggerEnter2D(Collider2D other)
   {
   	if(other.gameObject.CompareTag("Border")){
   		//...
   	}else if(other.gameObject.CompareTag("EnemyBullet") || other.gameObject.CompareTag("Enemy")){
   		life--;
   		GameManager.instance.UpdateLifeIcon(life);

   		if(life==0){
   			GameManager.instance.GameOver();
   		}else{
   			GameManager.instance.RespawnPlayer();
   		}

   		gameObject.SetActive(false);
   		Destroy(other.gameObject);
   	}
   }
   ```

5. GameManager의 inspector에서 연결
6. Enemy prefab에서 enemyScore 연결
7. 테스트
   1. 한번에 2개의 미사일을 맞으면 라이프가 2번감소됨

#### 예외 처리

1. 중복피격을 방지하기위해 player내에 bool 변수추가
2. player.cs

   ```cs
   public bool isHit;

   void OnTriggerEnter2D(Collider2D other)
   {

   	if(other.gameObject.CompareTag("Border")){
   		//...
   	}else if(other.gameObject.CompareTag("EnemyBullet") || other.gameObject.CompareTag("Enemy")){
   		if(isHit) return;

   		isHit = true;
   		life--;

   		//...

   	}

   }
   ```

3. GameManager.cs

   ```cs
   void RespawnPlayerExe(){

   	player.transform.position = Vector3.down * 3.5f;

   	player.SetActive(true);



   	Player playerLogic = player.GetComponent<Player>();

   	playerLogic.isHit = false;

   }
   ```

### 2D 종스크롤 슈팅 - 아이템과 필살기 구현하기 [B32]

#### 준비하기

1. 아틀라스를 나눠준다. (Items)
2. 하이라키내에 객체 추가
   1. (Boom, Power, Coin)을
   2. (Item Boom, Item Power, Item Coin)으로 객체명을 바꿔준다.
   3. box collider 2d 추가
      1. Power와 Boom은 collider size 0.6, 0.5로변경
      2. coin은 Circle collider
      3. 모두 is trigger 체크
   4. rigidbody 2d 추가
      1. gravity scale 0
   5. item.cs 추가
3. Item.cs 생성

   ```cs
   public class Item : MonoBehaviour
   {
       public enum ItemType {Power,Boom,Coin}
       public ItemType type;
       Rigidbody2D rigid;

       void Awake()
       {
           rigid = GetComponent<Rigidbody2D>();
           rigid.velocity = Vector2.down * 0.1f;
       }
   }
   ```

4. Animation 넣어주기
5. Item Tag 달아주기

#### 충돌 로직

1. Player.cs

   ```cs
   public int maxPower;
   public int power;

   void OnTriggerEnter2D(Collider2D other)
   {
   	if(other.gameObject.CompareTag("Border")){
   		//... 
   	}else if(other.gameObject.CompareTag("EnemyBullet") || other.gameObject.CompareTag("Enemy")){
   		//...
   	}else if(other.gameObject.CompareTag("Item")){
   		Item item = other.gameObject.GetComponent<Item>();
   		switch(item.type){
   			case Item.ItemType.Coin:
   				score += 1000;
   				break;
   			case Item.ItemType.Power:
   				if(power == maxPower){
   					score += 500;
   				}else{
   					power++;
   				}
   				break;
   			case Item.ItemType.Boom:
   				break;
   		}
   	}
   }
   ```

#### 필살기

1. Boom 아틀라스의
   1. pixel per unit 크기를 24> 15로 변경
2. Boom을 하이라키에 끌어놓기 (BoomEffect)
   1. 위치 초기화
   2. order in layer값 -1
   3. animation 추가
      1. Speed 3
   4. 비활성화
3. background 의 order in layer값 -2
4. Player에 변수로 BoomEffect(Game Object) 추가
   1. `public GameObject boomEffect;`
   2. maxPower를 inspector에서 설정 3
   3. 기본 power 1
5. Enemy.cs (코드)
   1. OnHit 메서드 접근제어자를 public으로 변경
   2. `public void OnHit(int dmg)`
6. Player.cs

   ```cs
   public int maxBoom;
   public int boom;
   public GameObject boomEffect;
   public bool isBoomTime;

   void Update()
   {
   	Move();
   	Fire();
   	Boom();
   	Reload();
   }

   void Boom(){
   	if(!Input.GetButton("Fire2")) return;
   	if(isBoomTime) return;
   	if(boom <= 0) return;

   	boom--;
   	GameManager.instance.UpdateBoomIcon(boom);
   	isBoomTime=true;

   	// #1. Effect visible
   	boomEffect.SetActive(true);
   	Invoke("OffBoomEffect",4f);

   	// #2.Remove
   	GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
   	for(int i=0;i<enemies.Length;i++){
   		Enemy enemyLogic = enemies[i].GetComponent<Enemy>();
   		enemyLogic.OnHit(1000);
   	}

   	// #3. Remove Enemy Bullet
   	GameObject[] bullets = GameObject.FindGameObjectsWithTag("EnemyBullet");
   	for(int i=0;i<bullets.Length;i++){
   		Destroy(bullets[i]);
   	}
   }

   void OffBoomEffect(){
   	boomEffect.SetActive(false);
   	isBoomTime=false;
   }

   void OnTriggerEnter2D(Collider2D other)
   {
   	if(other.gameObject.CompareTag("Border")){
   		//...
   	}else if(other.gameObject.CompareTag("EnemyBullet") || other.gameObject.CompareTag("Enemy")){
   		//...
   	}else if(other.gameObject.CompareTag("Item")){
   		Item item = other.gameObject.GetComponent<Item>();
   		switch(item.type){
   			case Item.ItemType.Coin:
   				//...
   			case Item.ItemType.Power:
   				//...
   			case Item.ItemType.Boom:
   				if(boom == maxBoom){
   					score += 500;
   				}else{
   					boom++;
   					GameManager.instance.UpdateBoomIcon(boom);
   				}
   				break;
   		}

   		Destroy(other.gameObject);
   	}
   }
   ```

   1. enter collision 분기
   2. fire2

7. 폭탄 UI 생성
8. Life Icon 복사 (Boom Icon)
   1. 앵커 우상단
   2. pos x -10, pos y -10
   3. 소스 이미지 Boom 4
9. Boom Icon을 Max 개수에 맞게 복사와 배치
10. GameManger에 연결
11. GameManager.cs
12. `UpdateBoomIcon`메서드 연결
13. Boom 1,2,3 알파값을 0인상태로 시작
14. 테스트 / 정상
15. Item 3개 prefab화

#### 아이템 드랍

1. Enemy.cs 내에 GameObject (Item 들 추가)
2. (이하에 코드추가)

#### 예외 처리

1. Enemy.cs

   ```cs
   public void OnHit(int dmg){
   	if(health <=0) return;
   	health -= dmg;
   	spriteRenderer.sprite = sprites[1];
   	Invoke("ReturnSprite",0.1f);

   	if(health<=0){
   		Player playerLogic = GameManager.instance.player.GetComponent<Player>();
   		playerLogic.score += enemyScore;

   		// #.Random Ratio Item Drop
   		int ran = Random.Range(0,10);
   		if(ran < 3){
   			Debug.Log("item didn't dropped");
   			//....
   		}
   		else if(ran < 6){ // Coin
   			Instantiate(itemCoin,transform.position,Quaternion.identity);
   		}
   		else if(ran < 8){ // Power
   			Instantiate(itemPower,transform.position,Quaternion.identity);
   		}
   		else if(ran < 10){ // Boom
   			Instantiate(itemCoin,transform.position,Quaternion.identity);
   		}

   		Destroy(gameObject);
   	}
   }
   ```

### 2D 종스크롤 슈팅 - 원근감있는 무한 배경 만들기 [B33]

https://www.youtube.com/watch?v=KUQAULcpYZU&list=PLO-mt5Iu5TeYtWvM9eN-xnwRbyUAMWd3b&index=7

#### 준비하기

1. 빈 객체 3개 Back A,B,C
   1. 각각의 객체내에 sprite를 넣어주기
   2. bottom 부터 order를 -5 -4 -3
   3. 3개씩 만들어주기
   4. 0번때는 y -10
   5. 2번대는 y 10
2. BackGround.cs

   ```cs
   public class BackGround : MonoBehaviour
   {
       public float speed;

       void Update()
       {
           Vector3 curPos = transform.position;
           Vector3 nextPos = Vector3.down * speed * Time.deltaTime;
           transform.position = curPos + nextPos;
       }
   }
   ```

#### 스크롤링

1. BackGround.cs

   ```cs
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
   ```

1. _카메라 높이를 가져오는 방법_
   1. Camera.main.orthographicSize
1. end가 0 start가 2

#### 패럴랙스

1. Parallax : 거리에 따른 상대적 속도를 활용한 기술
2. Speed값을 4,2,1로 바깥 테두리를 빠른속도를 준다.

###
