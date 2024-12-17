using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    //-------------------------------------
    //�C���X�y�N�^�[�Q�ƕs��
    //-------------------------------------
    private GameObject player;//�v���C���[
    private Rigidbody2D rb;//Rigidbody2D
    private Animator anim;
    private Collider2D col;
    private bool isGround;//������
    private bool jumpFlag;
    private bool crouchFlag;
    //-------------------------------------
    //�C���X�y�N�^�[�Q�Ɖ�
    //-------------------------------------
    [SerializeField] private int state;//�X�e�[�^�X

    

    private void Start()
    {
        //������
        this.player = GetComponent<GameObject>();
        this.rb = GetComponent<Rigidbody2D>();
        this.state = 1;
        this.anim = GetComponent<Animator>();
        this.col = GetComponent<Collider2D>();
    }
    private void FixedUpdate()
    {
        //�q�I�u�W�F�N�g���珰������擾����
        isGround = GetComponentInChildren<PlayerIsGround>().GetIsGround();
        
        //���ɐG��Ă���Ƃ��W�����v�ł���悤�ɂ���
        if (isGround) { jumpFlag = true; }

        //������
        {
            if(!isGround && this.rb.velocity.y <= 0)
            {
                state = 2;
                ChangeAnim(state);
            }
        }

        //���ړ�
        {
            if (RunMove())
            {
                if (isGround)
                {
                    Debug.Log("Player->Run");
                    state = -1;
                    ChangeAnim(state);
                }   
            }
            else
            {
                if (isGround)
                {
                    Debug.Log("Player->Idle");
                    state = 1;
                    ChangeAnim(state);
                }
            }
        }

        //�W�����v
        {
            if(jumpFlag)
            {
                if (JumpMove())
                {
                    Debug.Log("Player->Jump");
                    state = 4;
                    ChangeAnim(state);
                }
            }           
        }

        //���Ⴊ��
        {
            if(Crouchmove())
            {
                Debug.Log("Player->Crouch");
                state = 3;
                ChangeAnim(state);
            }
        }
    }
    private void ChangeAnim(int s_)
    {
        anim.SetInteger("Anim_Trigger", s_);
    }
    private bool RunMove()
    {
        int key = 0;//�v���C���[�̌���
        float maxspeed = 8.0f;//�v���C���[�̍ő呬�x

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            //���Ɍ���
            key = -1;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            //�E�Ɍ���
            key = 1;
        }
        else
        {
            if(isGround)
            {
                //��������͂�0��葽�����A���C���N����
                if (!Mathf.Approximately(Mathf.Abs(this.rb.velocity.x), 0))
                {
                    float velocity = this.rb.velocity.x;
                    this.rb.velocity = new Vector2(velocity * 0.96f, this.rb.velocity.y);
                }
            }
        }

        if (maxspeed > Mathf.Abs(this.rb.velocity.x))
        {
            Vector2 force = new Vector2(15.0f * key, 0);//������͂̑傫��

            //�ő呬�x��菬������Η͂�������                                            
            this.rb.AddForce(force);
        }

        switch (key)
        {
            case 1://�E�Ɍ���
                transform.eulerAngles = new Vector3(0, 0, 0);
                break;
            case -1://���Ɍ���
                transform.eulerAngles = new Vector3(0, 180, 0);
                break;
            default:break;
        } 

        if (!Mathf.Approximately(Mathf.Abs(this.rb.velocity.x), 0))
        {
            //�͂�������Ă���
            return true;
        }

        return false;
    }
    private bool JumpMove()
    {
        Vector2 jumpforce = new(this.rb.velocity.x, 400.0f);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.rb.AddForce(jumpforce);
            jumpFlag = false;
            return true;
        }

        return false;
    }
    private bool Crouchmove()
    {
        if(Input.GetKey(KeyCode.DownArrow))
        {
            crouchFlag = true;
            return true;
        }
        else
        {
            crouchFlag = false;
        }

        return false;
    }
    //-------------------------------------
    //�Q�Ɖ\�֐�
    //-------------------------------------
    public int GetState()
    {
        //�Q�Ɨp
        return this.state;
    }
}
