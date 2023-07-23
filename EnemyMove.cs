using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRenderer;
    public int nextMove;
    void Awake()
    {
        rigid= GetComponent<Rigidbody2D>();
        anim= GetComponent<Animator>();
        spriteRenderer= GetComponent<SpriteRenderer>();
        Invoke("Think", 5);
    }

   
    void FixedUpdate()
    {
        //Move
        rigid.velocity=new Vector2(nextMove, rigid.velocity.y);
        // x축: 왼쪽으로 가기 -1, y축: 현재 y축의 속도를 그대로 기입. 0넣으면 큰일남

        //Platform Check
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove*0.2f, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));
     
        if (rayHit.collider == null)
        {
            Turn();
        }
    }

    //재귀 함수-딜레이 없이 호출하면 아주 위험
    void Think()
    {
        //Set Next Active
        nextMove = Random.Range(-1, 2);

        //Sprite Animation
        anim.SetInteger("WalkSpeed", nextMove); // nextMove를 WalkSpeed에 넣음

        //Flip Sprite
        if (nextMove != 0)
        {
            spriteRenderer.flipX = nextMove == 1;
        }

        // Recursive (재귀는 보통 맨 아래에 써주는게 좋음)
        float nextThinkTime = Random.Range(2f, 5f);
        Invoke("Think", nextThinkTime); // 생각하는 시간도 랜덤성 부여해서 몬스터 마다 각자 달라지는 조금 더 active함을 줄 수 있음.
    }

    void Turn()
    {
        nextMove *= -1; // 방향이 반대가 됨
        spriteRenderer.flipX = nextMove == 1;
        CancelInvoke();
        Invoke("Think", 2);
    }
}
