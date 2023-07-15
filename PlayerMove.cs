using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float maxSpeed;
    public float jumpPower;
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
        //Jump
        if(Input.GetButtonDown("Jump") && !anim.GetBool("IsJumping")/*무한점프 막기*/)
        {
            rigid.AddForce(Vector2.up * jumpPower , ForceMode2D.Impulse);
            anim.SetBool("IsJumping", true);
        }

        //Stop Speed
        if(Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
            // x축에만 적용할거니까 .x 붙이기, f붙여야 됨 안 그럼 안됨
        }

        //Animation
        if (Mathf.Abs(rigid.velocity.x) < 0.3)
        {
            anim.SetBool("IsWalking", false);
        }
        else { anim.SetBool("IsWalking", true); }

        //Direction Sprite
        if (Input.GetButtonDown("Horizontal"))
        {
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }
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

        //Landing Platform
        if (rigid.velocity.y < 0)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
            //1은 한칸을 뜻함 , GetMask 이유: 레이가 플레이어도 Hit 하기 때문에

            //ratHit 정보가 콜라이더라면
            if (rayHit.collider != null)
            {
                //플레이어의 절반 크기
                if (rayHit.distance < 0.5f)
                {
                    anim.SetBool("IsJumping", false);
                }

            }
        }
    }
}
