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
    private bool isGround;//������
    private bool jumpflag;
    //-------------------------------------
    //�C���X�y�N�^�[�Q�Ɖ�
    //-------------------------------------
    [SerializeField] private State state;//�X�e�[�^�X

    //-------------------------------------
    //�X�e�[�^�X�̒�`
    //-------------------------------------
    public enum State
    { 
        Non, Idle, Run, Attack, Crouch, Jump, Hit, Dash, Death
    };

    private void Start()
    {
        //������
        this.player = GetComponent<GameObject>();
        this.rb = GetComponent<Rigidbody2D>();
        this.state = State.Non;
    }
    private void FixedUpdate()
    {
        //�q�I�u�W�F�N�g���珰������擾����
        isGround = GetComponentInChildren<PlayerIsGround>().GetIsGround();

        if (isGround) { jumpflag = true; }
        //���ړ�
        {
            if (RunMove(isGround))
            {
                Debug.Log("Player->Run");
                state = State.Run;
            }
            else
            {
                Debug.Log("Player->Idle");
                state = State.Idle;
            }
        }

        //�W�����v
        {
            if(jumpflag)
            {
                if (JumpMove())
                {
                    Debug.Log("Player->Jump");
                    state = State.Jump;
                }
            }           
        }
    }

    private bool RunMove(bool isGround)
    {
        int key = 0;//�v���C���[�̌���
        float maxspeed = 8.0f;//�v���C���[�̍ő呬�x

        if (isGround)//���ƐڐG���Ă���Ƃ��̂݉��ړ�����
        {
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
                //��������͂�0��葽�����A���C���N����
                if (!Mathf.Approximately(Mathf.Abs(this.rb.velocity.x), 0))
                {
                    float velocity = this.rb.velocity.x;
                    this.rb.velocity = new Vector2(velocity * 0.96f, this.rb.velocity.y);
                }
            }

            if (maxspeed > Mathf.Abs(this.rb.velocity.x))
            {
                Vector2 force = new Vector2(15.0f * key, 0);//������͂̑傫��
                //�ő呬�x��菬������Η͂�������
                this.rb.AddForce(force);
            }
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
        Vector2 jumpforce = new(this.rb.velocity.x, 5.0f);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.rb.velocity = jumpforce;
            jumpflag = false;
            return true;
        }

        return false;
    }
    //-------------------------------------
    //�Q�Ɨp�֐�
    //-------------------------------------
    public State GetState()
    {
        //�Q�Ɨp
        return this.state;
    }
}
