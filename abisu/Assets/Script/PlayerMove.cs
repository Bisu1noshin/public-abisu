using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private GameObject player;
    private Rigidbody2D rb;
    private bool isGround;

    [SerializeField] private State state;
    private enum State
    { 
        Non, Idle, Run, Attack, Crouch, Jump, Hit, Dash, Death
    };

    private void Start()
    {
        this.player = GetComponent<GameObject>();
        this.rb = GetComponent<Rigidbody2D>();
        this.state = State.Non;
    }
    private void FixedUpdate()
    {
        isGround = GetComponentInChildren<PlayerIsGround>().GetIsGround();

        if (RunMove(isGround))
        {
            Debug.Log("Player->Run");
            state = State.Run;
        }
        else
        {
            state = State.Idle;
        }
    }

    private bool RunMove(bool isGround)
    {
        int key = 0;

        if(isGround)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                key = -1;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                key = 1;
            }
        }
        
        Vector2 force = new Vector2(30.0f * key, 0);
        this.rb.AddForce(force);
        
        switch(key)
        {
            case 0:
                break;
            case 1:
                transform.eulerAngles = new Vector3(0, 0, 0);
                break;
            case -1:
                transform.eulerAngles = new Vector3(0, 180, 0);
                break;
            default:break;
        }

        if (key != 0)
        {
            return true;
        }
        return false;
    }

    private void ChangeAnim(State s_)
    {
        switch(s_)
        {
            case State.Idle:

                break;
            case State.Run:

                break;
        }
    }
}
