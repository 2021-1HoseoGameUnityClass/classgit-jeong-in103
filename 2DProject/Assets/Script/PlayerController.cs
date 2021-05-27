using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int MaxHP = 10;
    public float playerSpeed = 8f;
    public float jumpPower = 5f;
    public float jumpTimeLimit = 0.1f;//������ �� �ִ� �ִ� �ð�
    public float knockBackPower = 5f;
    public bool isUnBeatTime;

    int HP;
    float direction = 1f;
    //float isGround = 0f;
    float jumpTimer = 0f;//�����ϰ� �ִ� �ð�
    bool isJump = false;//���� ����
    bool isDead = false;//��� ����
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
        if (HP <= 0) //�÷��̾���HP�� 0���� ���� ��
        {
            Die(); //��� �Լ� ����
        }

        if (isDead) //���ó��
        {
            return;
        }

        if (!isSwamp && !isDamaged && Input.GetButtonDown("Jump"))//���� ��ư�� ������ �� ���� ���¸� true�� ����
        {
            isJump = true;
        }

        if(!isDamaged)
        {
            Move();
            Jump();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)//�Ϲ� �浹 ����
    {
        if(collision.contacts[0].normal.y > 0.7f)//� �ݶ��̴��� ��Ұ�, �浹 ǥ���� ������ ���� ���� ��
        {
            //isGround = collision.transform.position.y;
            jumpTimer = 0f;//���� Ÿ�̸� �ʱ�ȭ
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Respon" && !isDead)
        {
            responPosition = other.transform.position;
        }

        if (!isUnBeatTime && other.tag == "Enemy" && !isDead) // �浹�� ������Ʈ�� �±װ� Enemy�̰� ������°� �ƴ� ��
        {
            Damaged();

            int reaction = transform.position.x - other.transform.position.x > 0 ? 1 : -1;

            playerRigidbody.AddForce(new Vector2(reaction, 1) * knockBackPower, ForceMode2D.Impulse);//�˹�

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
            
            transform.position = responPosition;//������

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
        Debug.Log("����");    

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
        Debug.Log("����");

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

    private void Die()//���
    {
        playerRigidbody.velocity = Vector2.zero;//�÷��̾� �ӵ��� ���η� ����
        isDead = true;//��� ���� true
    }

    private void Move()//������
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

    private void Jump()//����
    {
        if (!isJump)//���� ���°� �ƴ� ��
        { 
            return;
        }

        if (Input.GetButtonUp("Jump") || jumpTimer >= jumpTimeLimit)//���� Ű�� ������ ���� ���� Ÿ�̸Ӱ� �ִ� ���� �Ѿ��� ��
        {
            jumpTimer = jumpTimeLimit;//���� Ÿ�̸Ӹ� �ִ� ������ ���� > ���� Ÿ�̸Ӱ� �ִ� ���� ���� �ʾ��� �� �̴� ���� ����
            isJump = false;//���� ���� ����
            return;
        }

        playerRigidbody.velocity = Vector2.zero;
        playerRigidbody.AddForce(Vector2.up * jumpPower * ((jumpTimer * 10) + 1f), ForceMode2D.Impulse);

        jumpTimer += Time.deltaTime;
    }

    public void Damaged()
    {
        isDamaged = true;

        HP--;//HP 1 ����

        Debug.Log("ü�� : " + HP);
    }
}