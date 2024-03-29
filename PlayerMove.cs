using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public GameManager gameManager;
    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioDamaged;
    public AudioClip audioItem;
    public AudioClip audioDie;
    public AudioClip audioFinish;
    public float maxSpeed;
    public float jumpPower;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    CircleCollider2D circleCollider;
    AudioSource audioSource;

    void Awake()
    {
        rigid= GetComponent<Rigidbody2D>();
        spriteRenderer= GetComponent<SpriteRenderer>();
        anim= GetComponent<Animator>();
        circleCollider = GetComponent<CircleCollider2D>();
        audioSource= GetComponent<AudioSource>();
    }

    void Playsound(string soundName)
    {
        switch (soundName)
        {
            case "JUMP":
                audioSource.clip= audioJump;
                break;
            case "ATTACK":
                audioSource.clip= audioAttack;
                break;
            case "DAMAGED":
                audioSource.clip= audioDamaged;
                break;
            case "ITEM":
                audioSource.clip= audioItem;
                break;
            case "DIE":
                audioSource.clip= audioDie;
                break;
            case "FINISH":
                audioSource.clip= audioFinish;
                break;
        }
        audioSource.Play();
    }

    void Update() // 1초에 60회정도 돈다
    {
        //Jump
        if(Input.GetButtonDown("Jump") && !anim.GetBool("IsJumping")/*무한점프 막기*/)
        {
            rigid.AddForce(Vector2.up * jumpPower , ForceMode2D.Impulse);
            anim.SetBool("IsJumping", true);
            Playsound("JUMP");
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
        if (Input.GetButton("Horizontal"))
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag=="Enemy")
        {
            // Attack
            if (rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
            {
                OnAttack(collision.transform);
            }
            else // Damaged
            {
                OnDamaged(collision.transform.position);
            }
        }
        if (collision.gameObject.tag == "Spike")
        {
            OnDamaged(collision.transform.position);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
       if(collision.gameObject.tag == "Item")
        {
            //Point
            bool isBronze = collision.gameObject.name.Contains("Bronze");
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");

            if(isBronze)
            {
                gameManager.stagePoint += 50;
            }
            else if (isSilver)
            {
                gameManager.stagePoint += 100;
            }
            else if (isGold)
            {
                gameManager.stagePoint += 300;
            }

            // Deactive Item
            collision.gameObject.SetActive(false);

            // sound
            Playsound("ITEM");
        }
        else if (collision.gameObject.tag=="Finish")
        {
            // Next stage
            gameManager.NextStage();

            // sound
            Playsound("FINISH");
        }
    }
    void OnDamaged(Vector2 targetPos)
    {
        // Health Down
        gameManager.HealthDown();

        // Change Layer (Immortal Active)
        gameObject.layer = 9;

        // View Alpha - 마지막 숫자(0.4f)는 투명도 (알파값)
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        // Reaction Force
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1; // 0보다 크면 1, 아니면 -1
        rigid.AddForce(new Vector2(dirc, 1)*7, ForceMode2D.Impulse);

        // Animation
        anim.SetTrigger("doDamaged");

        Invoke("OffDamaged", 2);

        //sound
        Playsound("DAMAGED");

        //point
        gameManager.totalPoint -= 100;

    }

    void OnAttack(Transform enemy)
    {
        // Point 
        gameManager.stagePoint += 300;

        // Enemy Die
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();

        //sound
        Playsound("ATTACK");

    }
    void OffDamaged()
    {
        gameObject.layer = 8;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }
    public void OnDie()
    {
        // sound
        Playsound("DIE");

        //Sprite Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        //Sprite Flip Y
        spriteRenderer.flipY = true;

        //Collider Disable
        circleCollider.enabled = false;

        //Die Effect Jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

    }
    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }
}
