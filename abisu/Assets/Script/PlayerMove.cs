using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    //-------------------------------------
    //インスペクター参照不可
    //-------------------------------------
    private GameObject player;
    private Rigidbody2D rb;
    private bool isGround;
    //-------------------------------------
    //インスペクター参照可
    //-------------------------------------
    [SerializeField] private State state;
    public enum State
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
        float maxspeed = 8.0f;

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
        
        Vector2 force = new Vector2(15.0f * key, 0);

        switch (key)
        {
            case 0:
                if(!Mathf.Approximately(Mathf.Abs(this.rb.velocity.x),0))
                {
                    float velocity = this.rb.velocity.x;
                    this.rb.velocity = new Vector2(velocity * 0.96f, 0);
                }
                break;
            case 1:
                transform.eulerAngles = new Vector3(0, 0, 0);
                break;
            case -1:
                transform.eulerAngles = new Vector3(0, 180, 0);
                break;
            default:break;
        }

        if (maxspeed > Mathf.Abs(this.rb.velocity.x))
        {
            this.rb.AddForce(force);
        }

        if (!Mathf.Approximately(Mathf.Abs(this.rb.velocity.x), 0))
        {
            return true;
        }

        return false;
    }

    public State GetState()
    {
        return this.state;
    }
}
