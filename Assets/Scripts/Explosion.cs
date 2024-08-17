using UnityEngine;

public class Explosion : MonoBehaviour
{
    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void OnEnable()
    {
        Invoke("Disable",2f);
    }

    void Disable(){
        gameObject.SetActive(false);
    }

    public void StartExplosion(ObjectManager.Type target){
        anim.SetTrigger("OnExplosion");

        switch(target){
            case ObjectManager.Type.EnemyA:
                transform.localScale = Vector3.one*0.7f;
                break;
            case ObjectManager.Type.EnemyB:
            case ObjectManager.Type.Player:
                transform.localScale = Vector3.one*1f;
                break;
            case ObjectManager.Type.EnemyC:
                transform.localScale = Vector3.one*2f;
                break;
            case ObjectManager.Type.EnemyBoss:
                transform.localScale = Vector3.one * 3f;
                break;
            default:
                Debug.Log("예외 발생");
                break;
        }
    }
}
