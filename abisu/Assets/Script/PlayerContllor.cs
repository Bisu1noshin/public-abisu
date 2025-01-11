using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
//--------------------------------------
//�X�e�[�^�X�̊Ǘ��p
//�R���|�[�l���g�Ƃ��Ĉ���
//�R���g���[���[�̏����󂯎��
//���̃X�N���v�g��enabled���Ȃ�
//--------------------------------------
public class PlayerContllor : MonoBehaviour
{
    //----------------------------------
    //�C���X�y�N�^�[�Q�Ɖ\
    //----------------------------------

    [SerializeField] private State state;
    [SerializeField] private bool jumpFlag;

    //----------------------------------
    //�C���X�y�N�^�[�Q�ƕs��
    //----------------------------------

    private Rigidbody2D rb;
    private Animator anim;
    private Collider2D idlecoll;
    private Collider2D crouchcoll;
    private float jumpforce = 400.0f;
    private bool isGround;
    private bool crouchFlag;
    private float Horizontal;
    private float Vertical;
    private float maxspeed;
    private float speed;
    private int key;
    private bool attackFlag;
    private bool jumpAttackFlag;
    private bool attackTime;
    private bool comboAttackFlag;
    private float timeCnt;

    //----------------------------------
    //�X�e�[�^�X�̒�`
    //----------------------------------

    public enum State
    {
        Non,Normal,Dash,Hit,Death
    };

    private void Start()
    {
        if(this.gameObject == null)
        {
            state = State.Non;
            return;
        }

        state = State.Normal;

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        idlecoll = transform.GetChild(1).gameObject.GetComponent<Collider2D>();
        crouchcoll = transform.GetChild(2).gameObject.GetComponent<Collider2D>();

        idlecoll.enabled = false;
        crouchcoll.enabled = false;
        jumpFlag = false;
        crouchFlag = false;
        speed = 15.0f;
        maxspeed = 8.0f;
        key = 0;
        attackFlag = false;
        attackTime = true;
        comboAttackFlag = false;
        timeCnt = 0;
    }

    private void Update()
    {
        //�X�e�[�^�X��Non�̎��͖�������
        if(state == State.Non)
        {
            Debug.Log("�v���C������������Ă܂���");
            return;
        }

        //�X�e�[�^�X��Death�̎��͖�������
        if (state == State.Death)
        {
            Debug.Log("�v���C�������ɂ܂���");
            return;
        }

        //��������Q�Ƃ���
        isGround = GetComponentInChildren<PlayerIsGround>().GetIsGround();

        //�A�i���O�X�e�B�b�N���Q�Ƃ���
        Horizontal = Input.GetAxis("Horizontal");
        Vertical = Input.GetAxis("Vertical");      
    }

    private void FixedUpdate()
    {
        PlayerMove();
        PlayerJump();
        PlayerAttack();
        CollisionContllor();
    }

    private void PlayerMove() {

        //���ړ�
        {
            if (Horizontal != 0)
            {
                if (Mathf.Abs(rb.velocity.x) <= maxspeed)
                {
                    Vector2 force = new(speed * Horizontal, rb.velocity.y);
                    rb.AddForce(force);
                    anim.SetInteger("Anim_Trigger", -1);
                }
            }
            else
            {
                if (isGround)
                {
                    anim.SetInteger("Anim_Trigger", 1);
                }
            }
        }

        //������ς���
        {
            if (Horizontal > 0)
            {
                key = 1;
            }
            else if (Horizontal < 0)
            {
                key = -1;
            }
            else
            {
                if (isGround)
                {
                    //��������͂�0��葽�����A���C���N����
                    if (!Mathf.Approximately(Mathf.Abs(this.rb.velocity.x), 0))
                    {
                        float velocityX = this.rb.velocity.x;
                        this.rb.velocity = new(velocityX * 0.96f, this.rb.velocity.y);
                    }
                }
                else
                {
                    if (Vertical < 0)
                    {
                        this.rb.velocity = new(0, this.rb.velocity.y);
                    }
                }
            }

            switch (key)
            {
                case 1://�E�Ɍ���
                    transform.eulerAngles = new(0, 0, 0);
                    break;
                case -1://���Ɍ���
                    transform.eulerAngles = new(0, 180, 0);
                    break;
                default: break;
            }
        }

        //���Ⴊ��
        {
            if (isGround)
            {
                if (Vertical >= 0)
                {
                    anim.SetBool("Crouch", false);
                    crouchFlag = false;
                }
                else
                {
                    anim.SetBool("Crouch", true);
                    this.rb.velocity = new(0, this.rb.velocity.y);
                    crouchFlag = true;
                }
            }
        }
    }
    private void PlayerJump() {

        //���Ⴊ��ł���ꍇ�͏������s��Ȃ�
        if (crouchFlag) { return; }

        //�n�ʂɐڐG���Ă��鎞�W�����v�ł���悤�ɂ���
        if (isGround) { jumpFlag = true; }

        //�����̏���
        if (!isGround && rb.velocity.y < 0)//�������Ă���Ƃ�
        {
            anim.SetInteger("Anim_Trigger", 0);
            return;//�W�����v�͈�x����
        }

        //�W�����v�̏���
        if (jumpFlag)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Vector2 jumpForce = new(this.rb.velocity.x, jumpforce);
                this.rb.AddForce(jumpForce);
                anim.Play("PlayerJumpFoward", 0, 0);
                jumpFlag = false;
            }
        }
    }
    private void PlayerAttack(){

        //�n��U��
        {
            if (isGround)
            {
                if (!jumpAttackFlag)
                {
                    jumpAttackFlag = true;
                }

                if (attackFlag)
                {
                    timeCnt += Time.deltaTime * 1.0f;

                    if (timeCnt > 0.2f)
                    {
                        anim.Play("PlayerAttack3", 0, 0);
                        timeCnt = 0;
                        attackFlag = false;
                        comboAttackFlag = true;
                        return;
                    }
                    else
                    {
                        if (Input.GetKeyDown(KeyCode.B))
                        {
                            anim.Play("PlayerAttack2", 0, 0);
                            Debug.Log("combo");
                            attackFlag = false;
                            return;
                        }
                    }
                }

                if (comboAttackFlag)
                {
                    if (TimeCnt(0.5f))
                    {
                        comboAttackFlag = false;
                        return;
                    }

                    if (Input.GetKeyDown(KeyCode.B))
                    {
                        anim.Play("PlayerAttack1", 0, 0);
                        comboAttackFlag = false;
                        return;
                    }
                }
            }
        }

        //�󒆍U��
        {
            if (!isGround)
            {
                if (attackFlag && jumpAttackFlag)
                {
                    rb.gravityScale = 0;
                    rb.velocity = new(0, 0);
                    anim.Play("PlayerJumpAttack", 0, 0);
                    attackFlag = false;
                    jumpAttackFlag = false;
                }

            }
        }

        //���Ⴊ�ݍU��
        {
            if (crouchFlag)
            {
                if (attackFlag)
                {
                    anim.Play("PlayerCrouchAttack", 0, 0);
                    attackFlag = false;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            attackFlag = true;
        }
    }
    private void PlayerMagic()
    {
        
    }
    private void CollisionContllor()
    {
        if (crouchFlag)
        {
            idlecoll.enabled = false;
            crouchcoll.enabled = true;
        }
        else 
        {
            idlecoll.enabled = true;
            crouchcoll.enabled = false;
        }
    }
    private void PlayerJumpAttackEnd()
    {
        rb.gravityScale = 1;
    }
    private bool TimeCnt(float max)
    {
        timeCnt += Time.deltaTime;

        if (timeCnt >= max)
        {
            timeCnt = 0;
            return true;
        }

        return false;
    }

    //---------------------------------------
    //�Q�b�^�[�֐�
    //---------------------------------------
    public bool GetIsGround() { return this.isGround; }
    public float GetHorizontal(){ return this.Horizontal; }
    public float GetVertical() { return this.Vertical; }
    public State GetState(){ return this.state; }
    //---------------------------------------
    //�Z�b�^�[�֐�(�ێ�Ǘ�!!!!!!)
    //---------------------------------------
    public void SetState(State s_)
    {
        //������Non�Ȃ疳������
        if (s_ == State.Non) { return; }

        //�X�e�[�^�X��Death�Ȃ疳������
        if (state == State.Death) { return; }

        //��𒆂͔�e���Ȃ�����Normal�ȊO�͖�������
        if (state == State.Dash)
        {
            if (s_ != State.Normal)
            {
                return;
            }
        }

        //�X�e�[�^�X�Ɉ�����������
        this.state = s_;
    }
}
