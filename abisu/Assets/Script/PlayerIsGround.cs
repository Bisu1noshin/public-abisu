using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIsGround : MonoBehaviour
{
    //-------------------------------------
    //インスペクター参照不可
    //-------------------------------------
    private Collider2D Coll;
    private bool exit;
    private bool stay;
    private bool enter;

    //-------------------------------------
    //インスペクター参照可
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
    //参照可能関数
    //-------------------------------------
    public bool GetIsGround()
    {
        return this.isGround;
    }
}
