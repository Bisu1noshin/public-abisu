using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
//--------------------------------------
//ステータスの管理用
//コンポーネントとして扱う
//コントローラーの情報を受け取る
//このスクリプトはenabledしない
//--------------------------------------
public class PlayerContllor : MonoBehaviour
{
    //----------------------------------
    //インスペクター参照可能
    //----------------------------------

    [SerializeField] private State state;
    [SerializeField] private bool jumpFlag;

    //----------------------------------
    //インスペクター参照不可
    //----------------------------------

    private Rigidbody2D rb;
    private Animator anim;
    private Collider2D idlecoll;
    private Collider2D crouchcoll;
    private float jumpforce = 400.0f;
    private bool isGround;
    private bool crouchFlag;
    private float Horizontal;
    private float Vertical;
    private float maxspeed;
    private float speed;
    private int key;
    private bool attackFlag;
    private bool jumpAttackFlag;
    private bool attackTime;
    private bool comboAttackFlag;
    private float timeCnt;

    //----------------------------------
    //ステータスの定義
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

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        idlecoll = transform.GetChild(1).gameObject.GetComponent<Collider2D>();
        crouchcoll = transform.GetChild(2).gameObject.GetComponent<Collider2D>();

        idlecoll.enabled = false;
        crouchcoll.enabled = false;
        jumpFlag = false;
        crouchFlag = false;
        speed = 15.0f;
        maxspeed = 8.0f;
        key = 0;
        attackFlag = false;
        attackTime = true;
        comboAttackFlag = false;
        timeCnt = 0;
    }

    private void Update()
    {
        //ステータスがNonの時は無視する
        if(state == State.Non)
        {
            Debug.Log("プレイヤが生成されてません");
            return;
        }

        //ステータスがDeathの時は無視する
        if (state == State.Death)
        {
            Debug.Log("プレイヤが死にました");
            return;
        }

        //床判定を参照する
        isGround = GetComponentInChildren<PlayerIsGround>().GetIsGround();

        //アナログスティックを参照する
        Horizontal = Input.GetAxis("Horizontal");
        Vertical = Input.GetAxis("Vertical");      
    }

    private void FixedUpdate()
    {
        PlayerMove();
        PlayerJump();
        PlayerAttack();
        CollisionContllor();
    }

    private void PlayerMove() {

        //横移動
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
                if (isGround)
                {
                    anim.SetInteger("Anim_Trigger", 1);
                }
            }
        }

        //向きを変える
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
                    //加わった力が0より多い時、摩擦を起こす
                    if (!Mathf.Approximately(Mathf.Abs(this.rb.velocity.x), 0))
                    {
                        float velocityX = this.rb.velocity.x;
                        this.rb.velocity = new(velocityX * 0.96f, this.rb.velocity.y);
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
                case 1://右に向く
                    transform.eulerAngles = new(0, 0, 0);
                    break;
                case -1://左に向く
                    transform.eulerAngles = new(0, 180, 0);
                    break;
                default: break;
            }
        }

        //しゃがむ
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
    private void PlayerJump() {

        //しゃがんでいる場合は処理を行わない
        if (crouchFlag) { return; }

        //地面に接触している時ジャンプできるようにする
        if (isGround) { jumpFlag = true; }

        //落下の処理
        if (!isGround && rb.velocity.y < 0)//落下しているとき
        {
            anim.SetInteger("Anim_Trigger", 0);
            return;//ジャンプは一度だけ
        }

        //ジャンプの処理
        if (jumpFlag)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Vector2 jumpForce = new(this.rb.velocity.x, jumpforce);
                this.rb.AddForce(jumpForce);
                anim.Play("PlayerJumpFoward", 0, 0);
                jumpFlag = false;
            }
        }
    }
    private void PlayerAttack(){

        //地上攻撃
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

        //空中攻撃
        {
            if (!isGround)
            {
                if (attackFlag && jumpAttackFlag)
                {
                    rb.gravityScale = 0;
                    rb.velocity = new(0, 0);
                    anim.Play("PlayerJumpAttack", 0, 0);
                    attackFlag = false;
                    jumpAttackFlag = false;
                }

            }
        }

        //しゃがみ攻撃
        {
            if (crouchFlag)
            {
                if (attackFlag)
                {
                    anim.Play("PlayerCrouchAttack", 0, 0);
                    attackFlag = false;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            attackFlag = true;
        }
    }
    private void PlayerMagic()
    {
        
    }
    private void CollisionContllor()
    {
        if (crouchFlag)
        {
            idlecoll.enabled = false;
            crouchcoll.enabled = true;
        }
        else 
        {
            idlecoll.enabled = true;
            crouchcoll.enabled = false;
        }
    }
    private void PlayerJumpAttackEnd()
    {
        rb.gravityScale = 1;
    }
    private bool TimeCnt(float max)
    {
        timeCnt += Time.deltaTime;

        if (timeCnt >= max)
        {
            timeCnt = 0;
            return true;
        }

        return false;
    }

    //---------------------------------------
    //ゲッター関数
    //---------------------------------------
    public bool GetIsGround() { return this.isGround; }
    public float GetHorizontal(){ return this.Horizontal; }
    public float GetVertical() { return this.Vertical; }
    public State GetState(){ return this.state; }
    //---------------------------------------
    //セッター関数(保守管理!!!!!!)
    //---------------------------------------
    public void SetState(State s_)
    {
        //引数がNonなら無視する
        if (s_ == State.Non) { return; }

        //ステータスがDeathなら無視する
        if (state == State.Death) { return; }

        //回避中は被弾しないためNormal以外は無視する
        if (state == State.Dash)
        {
            if (s_ != State.Normal)
            {
                return;
            }
        }

        //ステータスに引数を代入する
        this.state = s_;
    }
}
