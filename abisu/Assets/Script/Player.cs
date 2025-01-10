using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private GameObject player;
    private Rigidbody2D rb;
    private Animator anim;
    private PlayerInput.State state;
    private float maxspeed;
    private float speed;
    private int key;
    private bool isGround;
    private bool crouchFlag;
    private float Horizontal;
    private float Vertical;

    private void Start()
    {
        player = this.gameObject;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        crouchFlag = false;
        speed = 15.0f;
        maxspeed = 8.0f;
        key = 0;
    }

    private void FixedUpdate()
    {
        state = player.GetComponent<PlayerInput>().GetState();
        isGround= player.GetComponent<PlayerInput>().GetIsGround();
        Horizontal= player.GetComponent<PlayerInput>().GetHorizontal();
        Vertical = player.GetComponent<PlayerInput>().GetVertical();

        if (state != PlayerInput.State.Normal) { return;}
        
        //‰¡ˆÚ“®
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
                if(isGround)
                {
                    anim.SetInteger("Anim_Trigger", 1);
                }
            }
        }

        //Œü‚«‚ð•Ï‚¦‚é
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
                    //‰Á‚í‚Á‚½—Í‚ª0‚æ‚è‘½‚¢ŽžA–€ŽC‚ð‹N‚±‚·
                    if (!Mathf.Approximately(Mathf.Abs(this.rb.velocity.x), 0))
                    {
                        float velocityX = this.rb.velocity.x;
                        this.rb.velocity = new (velocityX * 0.98f, this.rb.velocity.y);
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
                case 1://‰E‚ÉŒü‚­
                    transform.eulerAngles = new (0, 0, 0);
                    break;
                case -1://¶‚ÉŒü‚­
                    transform.eulerAngles = new (0, 180, 0);
                    break;
                default: break;
            }
        }

        //‚µ‚á‚ª‚Þ
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

    public bool GetCrouchFlag()
    {
        return this.crouchFlag;
    }
}
