using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIsGround : MonoBehaviour
{
    private Collider2D Coll;
    private bool exit;
    private bool stay;
    private bool enter;

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

        if(stay)
        {
            this.isGround = true;
        }

        if(exit)
        {
            this.isGround = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Ground")
        {
            enter = true;
            stay = false;
            exit = false;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Ground")
        {
            exit = true;
            enter = false;
            stay = false;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Ground")
        {
            stay = true;
            enter = false;
            exit = false;
        }
    }
    public bool GetIsGround()
    {
        return this.isGround;
    }
}
