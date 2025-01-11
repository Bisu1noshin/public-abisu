using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIsGround : MonoBehaviour
{
    //-------------------------------------
    //�C���X�y�N�^�[�Q�ƕs��
    //-------------------------------------
    private Collider2D Coll;
    private bool exit;
    private bool stay;
    private bool enter;

    //-------------------------------------
    //�C���X�y�N�^�[�Q�Ɖ�
    //-------------------------------------
    [SerializeField] private bool isGround;

    private void Start()
    {
        this.Coll = GetComponent<Collider2D>();
        this.exit = false;
        this.enter = false;
        this.stay = false;
        this.isGround = false;
    }

    private void Update()
    { 
        if(GetIsGround())
        {
            Debug.Log("isGround");
        }

        if(enter)
        {
            this.isGround = true;
        }

        if(exit)
        {
            this.isGround = false;
        }

        enter = false;
        stay = false;
        exit = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Ground")
        {
            enter = true;           
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Ground")
        {
            exit = true;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Ground")
        {
            stay = true;
        }
    }
    //-------------------------------------
    //�Q�Ɖ\�֐�
    //-------------------------------------
    public bool GetIsGround()
    {
        return this.isGround;
    }
}
