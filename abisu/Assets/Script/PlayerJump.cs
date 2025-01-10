using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    private GameObject player;
    private Rigidbody2D rb;
    private Animator anim;
    private PlayerInput.State state;
    private bool jumpFlag;
    private float jumpforce =  400.0f;
    private bool isGround;
    private bool crouchFlag;

    private void Start()
    {
        player = this.gameObject;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        jumpFlag = false;
    }

    private void FixedUpdate()
    {
        state = player.GetComponent<PlayerInput>().GetState();
        isGround = player.GetComponent<PlayerInput>().GetIsGround();
        crouchFlag = player.GetComponent<Player>().GetCrouchFlag();

        if (state != PlayerInput.State.Normal) { return; }

        if (isGround) { jumpFlag = true; }

        if (!isGround && rb.velocity.y < 0) 
        {
            anim.SetInteger("Anim_Trigger", 0);
            return;
        }

        if (jumpFlag &&! crouchFlag)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                Vector2 jumpForce = new(this.rb.velocity.x, 400.0f);
                this.rb.AddForce(jumpForce);
                anim.Play("PlayerJumpFoward", 0, 0);
                jumpFlag = false;
            }
        }
    }
}
