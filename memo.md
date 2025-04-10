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

### 2D 종스크롤 슈팅 - 최적화의 기본, 오브젝트 풀링 (어려움!) [B34]

#### 오브젝트 풀링이란?

1. 객체를 생성 삭제하면
   1. 쓰레기가 쌓이고 garbage collector가 실행될때 잠깐 끈기는데
   2. 쓰레기가 많으면 렉이 심하게 걸릴수 있음

#### 풀 생성

1. ObjectManager 생성(빈객체)
2. ObjectManager.cs

   ```cs
   public class ObjectManager : MonoBehaviour
   {
       public GameObject enemyCPrefab;
       public GameObject enemyBPrefab;
       public GameObject enemyAPrefab;

       public GameObject itemCoinPrefab;
       public GameObject itemPowerPrefab;
       public GameObject itemBoomPrefab;

       public GameObject bulletPlayerAPrefab;
       public GameObject bulletPlayerBPrefab;
       public GameObject bulletEnemyAPrefab;
       public GameObject bulletEnemyBPrefab;

       GameObject[] enemyC;
       GameObject[] enemyB;
       GameObject[] enemyA;

       GameObject[] itemCoin;
       GameObject[] itemPower;
       GameObject[] itemBoom;

       GameObject[] bulletPlayerA;
       GameObject[] bulletPlayerB;
      
       GameObject[] bulletEnemyA;
       GameObject[] bulletEnemyB;

       void Awake()
       {
           enemyA = new GameObject[10];
           enemyB = new GameObject[10];
           enemyC = new GameObject[20];

           itemCoin = new GameObject[20];
           itemPower = new GameObject[10];
           itemBoom = new GameObject[10];

           bulletPlayerA = new GameObject[100];
           bulletPlayerB = new GameObject[100];
          
           bulletEnemyA = new GameObject[100];
           bulletEnemyB = new GameObject[100];
          
           Generate();
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
       }
   }
   ```

#### 풀 사용하기

1. 오브젝트 풀에 접근할 수 잇는 함수 생성
2. 모든생성은 ObjectManager를 사용하며
3. prefab의 경우 미리등록할수없기에
4. 생성해줄때(GameManager가) ObjectManager를 연결해준다.
5. 삭제 대신에 비활성화로 처리한다.
6. 재사용을 위해 회전을 정상화하고, 속도를 정상화, health를 정상화한다.

#### 로직 정리

1. GameManager.cs

   ```cs
   public ObjectManager objectManager;

   void SpawnEnemy(){
   	int ranEnemy = Random.Range(0,enemyObjs.Length);
   	int ranPoint = Random.Range(0,spawnPoints.Length);
   	GameObject enemy = objectManager.MakeObj((ObjectManager.Type)ranEnemy); // 0~2
   	enemy.transform.position = spawnPoints[ranPoint].position;

   	Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();

   	Enemy enemyLogic = enemy.GetComponent<Enemy>();
   	enemyLogic.objectManager=objectManager;
   	//...
   }
   ```

2. Player.cs

   ```cs
   public ObjectManager objectManager;

   void Fire(){
   	if(curShotDelay < maxShotDelay) return;
   	switch(power){
   		case 1:
   			GameObject bullet = objectManager.MakeObj(ObjectManager.Type.BulletPlayerA);
   			bullet.transform.position = transform.position;
   			//...
   			break;
   		case 2:
   			FireCase2();
   			break;
   		case 3:
   			FireCase2();  
   			GameObject bulletB = objectManager.MakeObj(ObjectManager.Type.BulletPlayerB);
   			bulletB.transform.position = transform.position;
   			//...
   			break;
   	}

   	// curShotDelay -= maxShotDelay;
   	curShotDelay = 0;
   }

   void FireCase2(){
   	GameObject bulletR = objectManager.MakeObj(ObjectManager.Type.BulletPlayerA);
   	GameObject bulletL = objectManager.MakeObj(ObjectManager.Type.BulletPlayerA);

   	bulletR.transform.position = transform.position  + Vector3.right*0.1f;
   	bulletL.transform.position = transform.position  + Vector3.left*0.1f;

   	Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
   	Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
   	rigidR.AddForce(Vector2.up * 10f,ForceMode2D.Impulse);
   	rigidL.AddForce(Vector2.up * 10f,ForceMode2D.Impulse);
   }

   //active 인것만 Deactivate하기
   void DeactivateGroup(GameObject[] targetGroup){
   	for(int i=0;i<targetGroup.Length;i++){
   		if(!targetGroup[i].activeSelf) return;

   		Enemy enemyLogic = targetGroup[i].GetComponent<Enemy>();
   		if(enemyLogic) {
   			enemyLogic.OnHit(1000);
   			return;
   		}

   		targetGroup[i].SetActive(false);
   	}
   }

   void Boom(){
   	//...

   	// #1. Effect visible
   	boomEffect.SetActive(true);
   	Invoke("OffBoomEffect",4f);

   	// #2.Remove
   	DeactivateGroup(objectManager.GetPool(ObjectManager.Type.EnemyA));
   	DeactivateGroup(objectManager.GetPool(ObjectManager.Type.EnemyB));
   	DeactivateGroup(objectManager.GetPool(ObjectManager.Type.EnemyC));

   	// #3. Remove Enemy Bullet
   	DeactivateGroup(objectManager.GetPool(ObjectManager.Type.BulletEnemyA));
   	DeactivateGroup(objectManager.GetPool(ObjectManager.Type.BulletEnemyB));
   }

   void OnTriggerEnter2D(Collider2D other)
   {
   	if(other.gameObject.CompareTag("Border")){
   		//...
   	}else if(other.gameObject.CompareTag("EnemyBullet") || other.gameObject.CompareTag("Enemy")){
   		//...
   		gameObject.SetActive(false);
   		other.gameObject.SetActive(false);
   	}else if(other.gameObject.CompareTag("Item")){
   		//...
   		other.gameObject.SetActive(false);
   	}
   }
   ```

3. Enemy.cs

   ```cs
   public ObjectManager objectManager;

   void Fire(){
   	if(curShotDelay < maxShotDelay) return;

   	if(enemyName == "A"){
   		GameObject bullet = objectManager.MakeObj(ObjectManager.Type.BulletEnemyA);
   		bullet.transform.position=transform.position;
   		Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();

   		Vector3 dirVec = player.transform.position - transform.position;

   		rigid.AddForce(dirVec.normalized * 4f,ForceMode2D.Impulse);
   	}else if(enemyName == "C"){
   		GameObject bulletR = objectManager.MakeObj(ObjectManager.Type.BulletEnemyA);
   		GameObject bulletL = objectManager.MakeObj(ObjectManager.Type.BulletEnemyA);

   		bulletR.transform.position = transform.position + Vector3.right * 0.3f;
   		bulletL.transform.position = transform.position + Vector3.left * 0.3f;

   		//...
   	}

   	curShotDelay -= maxShotDelay;
   }

   public void OnHit(int dmg){
   	if(health <=0) return;
   	health -= dmg;
   	spriteRenderer.sprite = sprites[1];
   	Invoke("ReturnSprite",0.1f);

   	if(health<=0){
   		Player playerLogic = GameManager.instance.player.GetComponent<Player>();
   		playerLogic.score += enemyScore;
   		GameObject item=null;

   		// #.Random Ratio Item Drop
   		int ran = Random.Range(0,10);
   		if(ran < 3){
   			Debug.Log("item didn't dropped");
   			//....
   		}
   		else if(ran < 6){ // Coin
   			item = objectManager.MakeObj(ObjectManager.Type.ItemCoin);
   		}
   		else if(ran < 8){ // Power
   			item = objectManager.MakeObj(ObjectManager.Type.ItemPower);
   		}
   		else if(ran < 10){ // Boom
   			item = objectManager.MakeObj(ObjectManager.Type.ItemBoom);
   		}
   		gameObject.SetActive(false);
   		transform.rotation = quaternion.identity;

   		if(!item) return;
   		item.transform.position  = transform.position;
   		// CancelInvoke("ReturnSprite");
   	}
   }

   void OnTriggerEnter2D(Collider2D other)
   {
   	switch(other.gameObject.tag){
   		case "BorderBullet":
   			gameObject.SetActive(false);
   			transform.rotation = quaternion.identity;
   			break;
   		case "PlayerBullet":
   			Bullet bullet = other.gameObject.GetComponent<Bullet>();
   			OnHit(bullet.dmg);

   			other.gameObject.SetActive(false);
   			break;
   	}
   }

   void OnEnable()
   {
   	switch(enemyName){
   		case "C":
   			health=40;
   			break;
   		case "B":
   			health=10;
   			break;
   		case "A":
   			health=3;
   			break;
   	}
   }
   ```

4. Item.cs

   ```cs
   public class Item : MonoBehaviour
   {
       public enum ItemType {Power,Boom,Coin};
       public ItemType type;
       Rigidbody2D rigid;

       void Awake()
       {
           rigid = GetComponent<Rigidbody2D>();
       }

       void OnEnable()
       {
           rigid.velocity = Vector2.down * 1f;
       }
   }
   ```

5. Bullet.cs
   ```cs
   public class Bullet : MonoBehaviour
   {
   	public int dmg;
   	void OnTriggerEnter2D(Collider2D other)
   	{
   		if(other.gameObject.CompareTag("BorderBullet")){
   			gameObject.SetActive(false);
   		}
   	}
   }
   ```

### 2D 종스크롤 슈팅 - 텍스트파일을 이용한 커스텀 배치 구현 [B35]

#### 구조체

1. Spawn.cs
   ```cs
   public class Spawn
   {
       public float delay;
       public ObjectManager.Type type;
       public int point; // 인덱스
   }
   ```
2.

#### 텍스트 데이터

1. 텍스트 데이터 (Stage 0)를 Asset/Resources 폴더에 넣어준다.
   1. 런타임에서 불러오는 에셋이 저장된 폴더

```
1,0,1
0.2,0,1
0.2,0,1
0.2,0,1
0.2,0,1
1,0,3
0.2,0,3
0.2,0,3
0.2,0,3
0.2,0,3
2,1,2
```

3. GameManager.cs에서 파일을 읽을 수 있게 코드를 추가해준다.

   ```cs
   public List<Spawn> spawnList;
   public int spawnIndex;
   public bool spawnEnd;

   void Awake()
   {
   	if(instance==null){
   		instance=this;
   		spawnList = new List<Spawn>();
   		ReadSpawnFile();
   	}
   }

   void ReadSpawnFile(){
   	// #1.변수 초기화
   	spawnList.Clear();
   	spawnIndex=0;
   	spawnEnd=false;

   	// #2.리스폰 파일 읽기
   	TextAsset textFile = Resources.Load("Stage 0") as TextAsset; //as Text는 자료형 검증 .. 만약에 아닐시 null처리가 됨

   	StringReader stringReader = new StringReader(textFile.text);

   	while(stringReader != null){
   		string line = stringReader.ReadLine();
   		Debug.Log(line);

   		if(line == null)
   			break;

   		// # 리스폰 데이터 생성
   		Spawn spawnData = new Spawn();
   		spawnData.delay = float.Parse(line.Split(',')[0]);
   		spawnData.type = (ObjectManager.Type)int.Parse(line.Split(',')[1]);
   		spawnData.point = int.Parse(line.Split(',')[2]);

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

   	if(curSpawnDelay > nextSpawnDelay){
   		SpawnEnemy();
   		nextSpawnDelay = Random.Range(0.5f,3f);
   		curSpawnDelay = 0;
   		// curSpawnDelay -= nextSpawnDelay;
   	}

   	// #.UI Score Update
   	Player playerLogic = player.GetComponent<Player>();
   	scoreText.text = string.Format("{0:n0}",playerLogic.score);
   }
   ```

1. _TextAsset : 텍스트 파일 에셋 클래스_
1. _StringReader : 파일 내의 문자열 데이터 읽기 클래스_
1. _ReadLine() : 텍스트 데이터를 한 줄씩 반환.(자동 줄 바꿈)_

#### 데이터 적용

1. GameManager.cs

   ```cs
   void ReadSpawnFile(){
   	// #1.변수 초기화
   	spawnList.Clear();
   	spawnIndex=0;
   	spawnEnd=false;

   	// #2.리스폰 파일 읽기
   	TextAsset textFile = Resources.Load("Stage 0") as TextAsset; //as Text는 자료형 검증 .. 만약에 아닐시 null처리가 됨

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
   ```

1. ObjectManager.cs
   ```cs
   //초기화 부분중 일부수정
   pools[0] = enemyA;
   pools[1] = enemyB;
   pools[2] = enemyC;
   ```

### 2D 종스크롤 슈팅 - 따라다니는 보조무기 만들기 [B36]

#### 준비하기

1. 하이라키에 Follwer 객체를 만들어준다. (스프라이트추가)
   1. order in layer 9
2. 기본 PalayerBulletA를 복사 (Follower Bullet)
   1. Sprite는 Follower Bullet으로 변경
   2. box collider 2d크기 수정
      1. 0.15, 0.5

#### 기본 작동 구현

1. Follower.cs 1. (이하)
   `

#### 팔로우 로직

1. _Queue 사용법_
   1. enqueue와 dequeue를 사용
2. Follower

   ```cs
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
           curShotDelay = 0;
       }

       void Reload(){
           curShotDelay += Time.deltaTime;
       }
   }
   ```

#### 파워 적용

1. follower 넣어주기
2. 코드상 power++ 하는쪽에 다음 메서드호출

```cs
    void AddFollower(){
        if(power<4) return;
        if(power>6) return;

        followers[power-4].SetActive(true);
    }
```

3. Fire 메서드 내에서
4. Case 3: 를 default로 변경

### 2D 종스크롤 슈팅 - 탄막을 뿜어대는 보스 만들기 [B37]

#### 준비하기

1. Boss sprite를 하이라키에 넣어주고 (Enemy Boss)
   1. 애니메이션을 추가해준다.
   2. 0,1,2를 Boss_Idle로 저장해준다.
   3. animator로 가서 Hit animation을 Hit 1프레임으로 조정
      1. any state > Hit로 transition과 함께 OnHit condition 사용
      2. Hit > Idle로 갈때는 condition이 없고 Has Exit Time을 켜준다.
   4. capsule collider 2d를 사용
      1. horizontal
      2. 2.7, 1.5
   5. rigidbody 2d를 넣어준다.
      1. gravity scale 0
   6. 태그 설정

#### 패턴 흐름 1

1. 보스의 총알Prefab을 만들기위해 기존것을 복사 (Enemy Bullet C)
   1. 스프라이트 변경
   2. 콜라이더 변경
   3. flipy true
2. (Enemy Bullet C 복사하여 D도 만들어준다.
3. Enemy.cs

   ```cs
   Animator anim;

   void Awake()
   {
   	//...
   	if(enemyName == "Boss"){
   		anim = GetComponent<Animator>();
   	}
   }

   void Update()
   {
   	if(enemyName == "Boss")
   		return;
   	Fire();
   	Reload();
   }
   //...
   		// #.Random Ratio Item Drop

   		int ran = enemyName == "Boss" ? 0 : Random.Range(0,10);
   		//...

   void OnTriggerEnter2D(Collider2D other)
   {
   	switch(other.gameObject.tag){
   		case "BorderBullet":
   			if(enemyName == "B") break;
   			gameObject.SetActive(false);
   			transform.rotation = quaternion.identity;
   			break;
   		case "PlayerBullet":
   			Bullet bullet = other.gameObject.GetComponent<Bullet>();
   			OnHit(bullet.dmg);

   			other.gameObject.SetActive(false);
   			break;
   	}
   }
   ```

4. 테스트 후 Boss Prefab화

#### 오브젝트 풀링 사용

1. Boss도 prefab화 하여 bullet과 함께 풀링사용

#### 패턴흐름 2

1. Enemy.cs

```cs
    void OnEnable()
    {
        switch(enemyName){
            case "Boss":
                health = 3000;
                Invoke("Stop",2f);
                break;
            case "C":
                health=40;
                break;
            case "B":
                health=10;
                break;
            case "A":
                health=3;
                break;
        }
    }

    void Stop(){
        if(!gameObject.activeSelf){
            return;
        }

        Rigidbody2D rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = Vector2.zero;

        Invoke("Think",2);
    }

    void Think(){
        patternIndex = (patternIndex+1) % 4;
        curPatternCount = 0;

        switch(patternIndex){
            case 0:
                FireForward();
                break;
            case 1:
                FireShot();
                break;
            case 2:
                FireArc();
                break;
            case 3:
                FireAround();
                break;
            default:
                Debug.Log("예외 발생");
                break;
        }
    }

    void FireForward(){
        Debug.Log("앞으로 4발 발사");
        curPatternCount++;
        if(curPatternCount< maxPatternCount[patternIndex])
            Invoke("FireForward",2f);
        else
            Invoke("Think",3f);
    }

    void FireShot(){
        Debug.Log("플레이어 방향으로 샷건.");
        curPatternCount++;
        if(curPatternCount< maxPatternCount[patternIndex])
            Invoke("FireShot",3.5f);
        else
            Invoke("Think",3f);
    }

    void FireArc(){
        Debug.Log("부채모양으로 발사.");
        curPatternCount++;
        if(curPatternCount< maxPatternCount[patternIndex])
            Invoke("FireArc",0.15f);
        else
            Invoke("Think",3f);
    }

    void FireAround(){
        Debug.Log("원 형태로 전체 공격");
        curPatternCount++;
        if(curPatternCount< maxPatternCount[patternIndex])
            Invoke("FireArc",0.7f);
        else
            Invoke("Think",3f);
    }
}
```

#### 패턴 구현

1. Enemy.cs

   ```cs
   void FireForward(){
   	// # Fire 4 Bullet Forward
   	Debug.Log("앞으로 4발 발사");

   	GameObject bulletR = objectManager.MakeObj(ObjectManager.Type.BulletBossA);
   	GameObject bulletL = objectManager.MakeObj(ObjectManager.Type.BulletBossA);
   	GameObject bulletRR = objectManager.MakeObj(ObjectManager.Type.BulletBossA);
   	GameObject bulletLL = objectManager.MakeObj(ObjectManager.Type.BulletBossA);

   	bulletR.transform.position = transform.position + Vector3.right * 0.62f;
   	bulletL.transform.position = transform.position + Vector3.left * 0.62f;
   	bulletRR.transform.position = transform.position + Vector3.right * 0.84f;
   	bulletLL.transform.position = transform.position + Vector3.left * 0.84f;

   	Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
   	Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
   	Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();
   	Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();

   	// Vector3 dirVecR = player.transform.position - transform.position;
   	// Vector3 dirVecL = player.transform.position - transform.position;
   	// Vector3 dirVecRR = player.transform.position - transform.position;
   	// Vector3 dirVecLL = player.transform.position - transform.position;

   	rigidR.AddForce(Vector2.down * 8f,ForceMode2D.Impulse);
   	rigidL.AddForce(Vector2.down * 8f,ForceMode2D.Impulse);
   	rigidRR.AddForce(Vector2.down * 8f,ForceMode2D.Impulse);
   	rigidLL.AddForce(Vector2.down * 8f,ForceMode2D.Impulse);

   	//코드 수정필요

   	// # Pattern Counting
   	curPatternCount++;

   	if(curPatternCount< maxPatternCount[patternIndex])
   		Invoke("FireForward",2f);
   	else
   		Invoke("Think",3f);
   }

   void FireShot(){
   	// # Fire 5 Random Shotgun Bullet to Player
   	Debug.Log("플레이어 방향으로 샷건.");

   	for(int i=0;i<5;i++){
   		GameObject bullet = objectManager.MakeObj(ObjectManager.Type.BulletEnemyB);
   		bullet.transform.position=transform.position;

   		Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
   		Vector2 dirVec = player.transform.position - transform.position;
   		Vector2 ranVec = new Vector2(Random.Range(-0.5f,0.5f),Random.Range(0f,2f));
   		dirVec += ranVec;
   		rigid.AddForce(dirVec.normalized * 5f,ForceMode2D.Impulse);
   	}

   	// # Pattern Counting
   	curPatternCount++;

   	if(curPatternCount< maxPatternCount[patternIndex])
   		Invoke("FireShot",3.5f);
   	else
   		Invoke("Think",3f);
   }

   void FireArc(){
   	// # Fire Arc Continue Fire
   	Debug.Log("부채모양으로 발사.");

   	GameObject bullet = objectManager.MakeObj(ObjectManager.Type.BulletEnemyA);
   	bullet.transform.position=transform.position;
   	bullet.transform.rotation = quaternion.identity;

   	Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
   	Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 10 * curPatternCount/maxPatternCount[patternIndex]),-1);
   	rigid.AddForce(dirVec.normalized * 5f,ForceMode2D.Impulse);

   	// # Pattern Counting
   	curPatternCount++;

   	if(curPatternCount< maxPatternCount[patternIndex])
   		Invoke("FireArc",0.15f);
   	else
   		Invoke("Think",3f);
   }

   void FireAround(){
   	// # Fire Around
   	Debug.Log("부채모양으로 발사.");

   	int roundNumA = 50;
   	int roundNumB = 40;
   	int roundNum = curPatternCount%2==0? roundNumA: roundNumB;

   	for(int i=0;i<roundNum;i++){
   		GameObject bullet = objectManager.MakeObj(ObjectManager.Type.BulletBossB);
   		bullet.transform.position=transform.position;
   		bullet.transform.rotation = quaternion.identity;

   		Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
   		Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 2 * i / roundNum),Mathf.Sin(Mathf.PI * 2 * i / roundNum));
   		rigid.AddForce(dirVec.normalized * 2f,ForceMode2D.Impulse);

   		Vector3 rotVec = Vector3.forward * 360 * i / roundNum +Vector3.forward*90;
   		bullet.transform.Rotate(rotVec);
   	}

   	Debug.Log("원 형태로 전체 공격");
   	curPatternCount++;

   	if(curPatternCount< maxPatternCount[patternIndex])
   		Invoke("FireArc",0.7f);
   	else
   		Invoke("Think",3f);
   }
   ```

### 2D 종스크롤 슈팅 - 모바일 슈팅게임 만들기 [BE4]

#### 플레이어 무적 시간

1. Player.cs

```cs
    public bool isRespawnTime; // 무적 타임을 처리할 변수

    void OnEnable()
    {
        Unbeatable();
        Invoke("Unbeatable",3f);
    }

    void Unbeatable(){
        isRespawnTime = !isBoomTime;
        if(isRespawnTime){
            spriteRenderer.color = new Color(1,1,1,0.5f);
        }else{
            spriteRenderer.color = new Color(1,1,1,1);
        }
    }
    //...
	     if(other.gameObject.CompareTag("EnemyBullet") || other.gameObject.CompareTag("Enemy")){
			if(isRespawnTime) return;
		}
```

#### 폭발 효과

1. Explosion 0을 하이라키에 드랍
   1. 애니메이션을 만들어주고
   2. animator내로 가서 기본값을 empty state로 변경해준다.
   3. any state > Explosion 연결
2. Explosion.cs

   ```cs
   public class Explosion : MonoBehaviour
   {
       Animator anim;

       void Awake()
       {
           anim = GetComponent<Animator>();
       }

       public void StartExplosion(string target){
           anim.SetTrigger("OnExplosion");

           switch(target){
               case "A":
                   transform.localScale = Vector3.one*0.7f;
                   break;
               case "B":
               case "P":
                   transform.localScale = Vector3.one*1f;
                   break;
               case "C":
                   transform.localScale = Vector3.one*2f;
                   break;
               case "Boss":
                   transform.localScale = Vector3.one * 3f;
                   break;
               default:
                   Debug.Log("예외 발생");
                   break;
           }
       }
   }
   ```

3. Prefab화

#### 모바일 컨트롤 UI(이동)

1. Canvas 내에 Render Mode - Screen Space - Overlay로 되어있음
   1. (Scene 내에서 Canvas가 Screen World보다 크게보임 )
2. 이값을 Screen Space - Camera로 변경
   1. Main Camera도 넣어준다.
   2. order in layer 높은값 (다른것에 가려지지 않기 위해서)
3. Canvas 내에 이미지 추가 (Joy Panel)
   1. 소스 JoyPad
   2. 가로 세로 300 300
   3. 앵커 좌하단
   4. pos x 25 y 25
   5. 알파값 170
4. Joy Panel내에 버튼추가

   1. 알파값 0
   2. 이하 텍스트 삭제
   3. Button 컴포넌트 내에
      1. Transition None
      2. Navigation None
   4. 가로세로 100 100
   5. 이버튼을 9개까지 복사후 분산배치
      1. 하나를 완성한후 copy component후 Paste하는쪽이 편함
   6. 각 event trigger를 넣고

      ```cs
      public void JoyPanel(int type){
      	for(int i=0;i<9;i++){
      		joyControl[i] = i == type;
      	}
      }

      public void JoyDown(){
      	isControl = true;
      }

      public void JoyUp(){
      	isControl = false;
      }
      ```

#### 모바일 컨트롤 UI(발사)

1. Canvas 이하에 버튼 추가 (Button A)
   1. 이하의 텍스트 제거
   2. 이미지 소스 후 set native size
   3. 200, 200
   4. 앵커 우하단
   5. pos x -25 y 25
   6. 알파값 170
2. Canvas 이하에 버튼 추가 (Button B)
   1. 이하의 텍스트 제거
   2. 이미지 소스 후 set native size
   3. 200, 150
   4. 앵커 우하단
   5. pos x -25 y 235
   6. 알파값 170
3. 위와 마찬가지로 function을 만들어서 event trigger를 통하여 호출

#### 스테이지 관리

1. Canvas 내에 Text 추가 (Stage Text)
   1. 라벨 STAGE 0 Start
   2. 볼드 130
   3. 가로 700 세로 300
   4. 하얀색
   5. 중앙정렬, 중앙정렬
2. Stage Text 복사 Clear Text
   1. 라벨
      1. STAGE 0
         Clear!!
3. 위 2개에 애니메이션을 줄 예정
4. Animation 폴더내에 Animator 생성(Text)
5. Animation도 만들어준다.(Text)
6. Idle 상태를 디폴트로 연결
7. Text.anim을 정의
8. 코드
9. Fade Black을 하이라키에 드롭
   1. Scale x 70, y 100
   2. order in layer를 1로 변경
   3. 애니 메이터 추가 (Fade)
   4. 애니메이션 추가 (Fade In, Fade Out)
10. Fade
    1. Idle State 추가
    2. 파라미터 In Out
    3. 애니메이션 제작
    4. spriterenderer에서 alpha값만을 조정해서
11. Player Pos 만들어주고 연결
12. GameManager.cs

```cs
    public int stage;
    public Animator stageAnim;
    public Animator clearAnim;
    public Animator fadeAnim;
    public Transform playerPos;
   
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
		//...

        // #2.리스폰 파일 읽기
        TextAsset textFile = Resources.Load("Stage " + stage) as TextAsset; //as Text는 자료형 검증 .. 만약에 아닐시 null처리가 됨
	}
```

#### 모바일

###
