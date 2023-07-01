using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float maxSpeed;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;

    void Awake()
    {
        rigid= GetComponent<Rigidbody2D>();
        spriteRenderer= GetComponent<SpriteRenderer>();
        anim= GetComponent<Animator>();
    }

    void Update() // 1초에 60회정도 돈다
    {
        if(Input.GetButtonUp("Horizontal"))
        {
            //Stop Speed
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
            // x축에만 적용할거니까 .x 붙이기, f붙여야 됨 안 그럼 안됨
        }

        //Animation
        if (Mathf.Abs(rigid.velocity.x) < 0.3)
        {
            anim.SetBool("IsWalking", false);
        }
        else { anim.SetBool("IsWalking", true); }
    }

    void FixedUpdate() // 1초에 50회 정도 돈다 -> 단발적 키 입력을 넣으면 10프레임 안에서 씹힐 수 있음
    {   
        //Move Speed
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right*h, ForceMode2D.Impulse);

        //Max Speed
        if (rigid.velocity.x > maxSpeed) // 오른쪽 최대 속도
        {
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        }
        else if(rigid.velocity.x < maxSpeed*(-1)) // 왼쪽 최대 속도
        {
            rigid.velocity = new Vector2(maxSpeed*(-1), rigid.velocity.y);
        }

        //Direction Sprite
        if (Input.GetButtonDown("Horizontal"))
        {
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }
    }
}
