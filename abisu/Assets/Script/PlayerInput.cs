using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//--------------------------------------
//�X�e�[�^�X�̊Ǘ��p
//�X�e�[�^�X���Q�Ƃ�����
//�R���g���[���[�̏����󂯎��
//--------------------------------------
public class PlayerInput : MonoBehaviour
{
    //----------------------------------
    //�C���X�y�N�^�[�Q�Ɖ\
    //----------------------------------

    [SerializeField] private State state;

    //----------------------------------
    //�C���X�y�N�^�[�Q�ƕs��
    //----------------------------------

    private bool isGround;
    private float Horizontal;
    private float Vertical;

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
        if (state ==State.Death)
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
