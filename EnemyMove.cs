using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    public int nextMove;
    void Awake()
    {
        rigid= GetComponent<Rigidbody2D>();
        
        Invoke("Think", 5);
    }

   
    void FixedUpdate()
    {
        //Move
        rigid.velocity=new Vector2(nextMove, rigid.velocity.y);
        // x축: 왼쪽으로 가기 -1, y축: 현재 y축의 속도를 그대로 기입. 0넣으면 큰일남

        //Platform Check
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));
     
        if (rayHit.collider == null)
        {
            Debug.Log("경고! 이 앞 낭떠러지.");
        }
    }

    //재귀 함수-딜레이 없이 호출하면 아주 위험
    void Think()
    {
        nextMove = Random.Range(-1, 2);

        Invoke("Think", 5);
    }
}
