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

###
