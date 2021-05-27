using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int MaxHP = 10;
    public float playerSpeed = 8f;
    public float jumpPower = 5f;
    public float jumpTimeLimit = 0.1f;//점프할 수 있는 최대 시간
    public float knockBackPower = 5f;
    public bool isUnBeatTime;

    int HP;
    float direction = 1f;
    //float isGround = 0f;
    float jumpTimer = 0f;//점프하고 있는 시간
    bool isJump = false;//점프 상태
    bool isDead = false;//사망 상태
    bool isDamaged = false;
    bool isSwamp = false;

    Rigidbody2D playerRigidbody;
    SpriteRenderer spriteRenderer;
    Vector2 responPosition;

    // Start is called before the first frame update
    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        responPosition  = new Vector2();

        HP = MaxHP;
    }

    // Update is called once per frame
    void Update()
    {
        if (HP <= 0) //플레이어이HP가 0보다 작을 때
        {
            Die(); //사망 함수 실행
        }

        if (isDead) //사망처리
        {
            return;
        }

        if (!isSwamp && !isDamaged && Input.GetButtonDown("Jump"))//점프 버튼을 눌렀을 때 점프 상태를 true로 변경
        {
            isJump = true;
        }

        if(!isDamaged)
        {
            Move();
            Jump();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)//일반 충돌 감지
    {
        if(collision.contacts[0].normal.y > 0.7f)//어떤 콜라이더와 닿았고, 충돌 표면이 위쪽을 보고 있을 때
        {
            //isGround = collision.transform.position.y;
            jumpTimer = 0f;//점프 타이머 초기화
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Respon" && !isDead)
        {
            responPosition = other.transform.position;
        }

        if (!isUnBeatTime && other.tag == "Enemy" && !isDead) // 충돌한 오브젝트의 태그가 Enemy이고 사망상태가 아닐 때
        {
            Damaged();

            int reaction = transform.position.x - other.transform.position.x > 0 ? 1 : -1;

            playerRigidbody.AddForce(new Vector2(reaction, 1) * knockBackPower, ForceMode2D.Impulse);//넉백

            if (HP > 0)
            {
                isUnBeatTime = true;
                StartCoroutine("UnBeatTime");
            }
        }

        if (!isUnBeatTime && other.tag == "Swamp" && !isDead)
        {
            isSwamp = true;
        }

        if (!isUnBeatTime && other.tag == "DeadLine" && !isDead)
        {
            Damaged();
            
            transform.position = responPosition;//리스폰

            if (HP > 0)
            {
                isUnBeatTime = true;
                StartCoroutine("FallUnBeatTime");

                isSwamp = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!isUnBeatTime && other.tag == "Swamp" && !isDead)
        {
            isSwamp = false;
        }
    }

    IEnumerator UnBeatTime()
    {
        Debug.Log("무적");    

        float countTime = 0f;

        while (countTime < 10)
        {
            if (countTime % 2 == 0)
                spriteRenderer.color = new Color32(255, 255, 255, 90);
            else
                spriteRenderer.color = new Color32(255, 255, 255, 180);

            if(countTime == 3)
            {
                playerRigidbody.velocity = Vector2.zero;
                isDamaged = false;
            }

            yield return new WaitForSeconds(0.2f);

            countTime++;
        }   

        spriteRenderer.color = new Color32(255, 255, 255, 255);

        isUnBeatTime = false;

        yield return null;
    }

    IEnumerator FallUnBeatTime()
    {
        Debug.Log("무적");

        float countTime = 0f;

        while (countTime < 10)
        {
            if (countTime % 2 == 0)
                spriteRenderer.color = new Color32(255, 255, 255, 90);
            else
                spriteRenderer.color = new Color32(255, 255, 255, 180);

            if (countTime == 9)
            {
                playerRigidbody.velocity = Vector2.zero;
                isDamaged = false;
            }

            yield return new WaitForSeconds(0.2f);

            countTime++;
        }

        spriteRenderer.color = new Color32(255, 255, 255, 255);

        isUnBeatTime = false;

        yield return null;
    }

    private void Die()//사망
    {
        playerRigidbody.velocity = Vector2.zero;//플레이어 속도를 제로로 변경
        isDead = true;//사망 상태 true
    }

    private void Move()//움직임
    {
        float xInput = Input.GetAxis("Horizontal");

        if (xInput < 0)
        {
            direction = -1f;
        }
        else if(xInput > 0)
        {
            direction = 1f;
        }

        Vector2 vector2 = new Vector2(direction, 1);
        transform.localScale = vector2;

        float moveSpeed = xInput * playerSpeed * Time.deltaTime;

        transform.Translate(new Vector2(moveSpeed, 0));
    }

    private void Jump()//점프
    {
        if (!isJump)//점프 상태가 아닐 때
        { 
            return;
        }

        if (Input.GetButtonUp("Jump") || jumpTimer >= jumpTimeLimit)//점프 키를 때었을 때나 점프 타이머가 최대 값을 넘었을 때
        {
            jumpTimer = jumpTimeLimit;//점프 타이머를 최대 값으로 설정 > 점프 타이머가 최대 값을 넘지 않았을 때 이단 점프 방지
            isJump = false;//점프 상태 변경
            return;
        }

        playerRigidbody.velocity = Vector2.zero;
        playerRigidbody.AddForce(Vector2.up * jumpPower * ((jumpTimer * 10) + 1f), ForceMode2D.Impulse);

        jumpTimer += Time.deltaTime;
    }

    public void Damaged()
    {
        isDamaged = true;

        HP--;//HP 1 감소

        Debug.Log("체력 : " + HP);
    }
}