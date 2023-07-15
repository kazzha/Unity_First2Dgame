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

    void Update() // 1�ʿ� 60ȸ���� ����
    {
        //Jump
        if(Input.GetButtonDown("Jump") && !anim.GetBool("IsJumping")/*�������� ����*/)
        {
            rigid.AddForce(Vector2.up * jumpPower , ForceMode2D.Impulse);
            anim.SetBool("IsJumping", true);
        }

        //Stop Speed
        if(Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
            // x�࿡�� �����ҰŴϱ� .x ���̱�, f�ٿ��� �� �� �׷� �ȵ�
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

    void FixedUpdate() // 1�ʿ� 50ȸ ���� ���� -> �ܹ��� Ű �Է��� ������ 10������ �ȿ��� ���� �� ����
    {   
        //Move Speed
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right*h, ForceMode2D.Impulse);

        //Max Speed
        if (rigid.velocity.x > maxSpeed) // ������ �ִ� �ӵ�
        {
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        }
        else if(rigid.velocity.x < maxSpeed*(-1)) // ���� �ִ� �ӵ�
        {
            rigid.velocity = new Vector2(maxSpeed*(-1), rigid.velocity.y);
        }

        //Landing Platform
        if (rigid.velocity.y < 0)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
            //1�� ��ĭ�� ���� , GetMask ����: ���̰� �÷��̾ Hit �ϱ� ������

            //ratHit ������ �ݶ��̴����
            if (rayHit.collider != null)
            {
                //�÷��̾��� ���� ũ��
                if (rayHit.distance < 0.5f)
                {
                    anim.SetBool("IsJumping", false);
                }

            }
        }
    }
}
