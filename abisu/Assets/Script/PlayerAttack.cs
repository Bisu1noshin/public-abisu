using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private GameObject player;
    private Animator anim;
    private Rigidbody2D rid;
    private bool isGround;
    private bool attackFlag;
    private bool jumpAttackFlag;
    private bool attackTime;
    private bool comboAttackFlag;

    private PlayerInput.State state;
    private float timeCnt;

    private void Start()
    {
        player = this.gameObject;
        anim = GetComponent<Animator>();
        rid = GetComponent<Rigidbody2D>();
        attackFlag = false;
        attackTime = true;
        comboAttackFlag = false;
        timeCnt = 0;
    }

    private void FixedUpdate()
    {
        state = player.GetComponent<PlayerInput>().GetState();
        isGround = player.GetComponent<PlayerInput>().GetIsGround();

        if (state != PlayerInput.State.Normal) { return; }

        //’nãUŒ‚
        {
            if (isGround)
            {
                if (!jumpAttackFlag)
                {
                    jumpAttackFlag = true;
                }

                if (attackFlag)
                {
                    timeCnt += Time.deltaTime * 1.0f;

                    if (timeCnt > 0.2f)
                    {
                        anim.Play("PlayerAttack3", 0, 0);
                        timeCnt = 0;
                        attackFlag = false;
                        comboAttackFlag = true;
                        return;
                    }
                    else
                    {
                        if (Input.GetKeyDown(KeyCode.B))
                        {
                            anim.Play("PlayerAttack2", 0, 0);
                            Debug.Log("combo");
                            attackFlag = false;
                            return;
                        }
                    }
                }

                if (comboAttackFlag)
                {
                    if (TimeCnt(0.5f))
                    {
                        comboAttackFlag = false;
                        return;
                    }

                    if (Input.GetKeyDown(KeyCode.B))
                    {
                        anim.Play("PlayerAttack1", 0, 0);
                        comboAttackFlag = false;
                        return;
                    }
                }
            }           
        }

        //‹ó’†UŒ‚
        {
            if (!isGround)
            {
                if (attackFlag && jumpAttackFlag)
                {
                    rid.gravityScale = 0;
                    rid.velocity = new(0, 0);
                    anim.Play("PlayerJumpAttack", 0, 0);
                    attackFlag = false;
                    jumpAttackFlag = false;
                }

            }
        }

        //‚µ‚á‚ª‚ÝUŒ‚
        {
            
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            attackFlag = true;
        }
    }
    private void PlayerJumpAttackEnd()
    {
        rid.gravityScale = 1;
    }

    private bool TimeCnt(float max)
    {
        timeCnt += Time.deltaTime;

        if(timeCnt>=max)
        {
            timeCnt = 0;
            return true;
        }

        return false;
    }
}
