using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
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
    [SerializeField] private Vector2 Verlocity;
    [SerializeField] private int playerHP;
    [SerializeField] private int playerMP;


    //----------------------------------
    //インスペクター参照不可
    //----------------------------------

    private Rigidbody2D rb;
    private Animator anim;
    private Collider2D idlecoll;
    private Collider2D crouchcoll;
    private Collider2D attack1coll;
    private Collider2D attack2coll;
    private Collider2D attack3coll;
    private Collider2D attackCrouchcoll;
    private Collider2D attackJumpcoll;
    private PlayerAction inputActions;
    private PlayerObjectState playerObjectState;
    private bool isMoveAddVerocity;
    private float jumpforce = 600.0f;
    private bool isGround;
    private bool crouchFlag;
    private float Horizontal;
    private float Vertical;
    private float maxspeed;
    private float speed;
    private int key;
    private bool attackFlag;
    private int jumpAttackCnt;
    private bool attackTime;
    private int comboAttackCnt;
    private float magicCnt;
    private bool magicFlag;
    private float timeCnt;
    private float preMagicCnt = 0;
    private float susutainMagicTime = 15f;

    private Vector2 moveInputValue;
    private bool onJumpFlag;
    private bool onAttackFlag;
    private bool onMagicFlag;

    //----------------------------------
    //ステータスの定義
    //----------------------------------

    public enum State
    {
        Non, Normal, Dash, Hit, Death
    };

    private void Start()
    {
        if (this.gameObject == null)
        {
            state = State.Non;
            return;
        }

        state = State.Normal;

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        idlecoll = transform.GetChild(1).gameObject.GetComponent<Collider2D>();
        crouchcoll = transform.GetChild(2).gameObject.GetComponent<Collider2D>();
        attack1coll = transform.GetChild(3).gameObject.GetComponent<Collider2D>();
        attack2coll = transform.GetChild(4).gameObject.GetComponent<Collider2D>();
        attack3coll = transform.GetChild(5).gameObject.GetComponent<Collider2D>();
        attackCrouchcoll = transform.GetChild(6).gameObject.GetComponent<Collider2D>();
        attackJumpcoll = transform.GetChild(7).gameObject.  GetComponent<Collider2D>();
        AttackCollisionContllor(null);
        playerObjectState = new PlayerObjectState(100, 10, 100);

        isMoveAddVerocity = false;
        idlecoll.enabled = false;
        crouchcoll.enabled = false;
        jumpFlag = false;
        crouchFlag = false;
        speed = 15.0f;
        maxspeed = 8.0f;
        key = 0;
        attackFlag = true;
        attackTime = true;
        comboAttackCnt = 0;
        timeCnt = 0;
        onJumpFlag = false;
        onAttackFlag = false;
        onMagicFlag = false;
    }
    private void Awake()
    {
        // Actionスクリプトのインスタンス生成
        inputActions = new PlayerAction();

        // Actionイベント登録
        inputActions.Player.Move.started += OnMove;
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;
        inputActions.Player.Jump.performed += OnJump;
        inputActions.Player.Attack.performed += OnAttack;
        inputActions.Player.Magic.started += StartedMagic;
        inputActions.Player.Magic.canceled += CanceledMagic;
        inputActions.Player.Dash.performed += OnDash;

        // Input Actionを機能させるためには、
        // 有効化する必要がある
        inputActions.Enable();
    }
    private void OnDestroy()
    {
        // 自身でインスタンス化したActionクラスはIDisposableを実装しているので、
        // 必ずDisposeする必要がある
        inputActions.Dispose();
    }
    private void OnMove(InputAction.CallbackContext context)
    {
        moveInputValue = context.ReadValue<Vector2>();
    }
    private void OnJump(InputAction.CallbackContext context)
    {
        if (state != State.Normal) { return; }

        onJumpFlag = true;
    }
    private void OnAttack(InputAction.CallbackContext context)
    {
        if (state != State.Normal) { return; }

        onAttackFlag = true;
    }
    private void StartedMagic(InputAction.CallbackContext context)
    {
        
    }
    private void CanceledMagic(InputAction.CallbackContext context) 
    {
        if (state != State.Normal) { return; }

        onMagicFlag = true;
        isMoveAddVerocity = false;
    }
    private void OnDash(InputAction.CallbackContext context)
    {
        IsDash();
    }

    //-----------------------------------
    //毎フレーム呼び出す
    //-----------------------------------

    private void Update()
    {
        //ステータスがNonの時は無視する
        if (state == State.Non)
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

        //HPが0になったらステータスをDeathにする
        if (playerObjectState.GetPlayerHP() == 0) 
        {
            PlayerDeath();
            state = State.Death;
        }

        //アナログスティックを参照する
        Horizontal = moveInputValue.x;
        Vertical = moveInputValue.y;

        //攻撃、魔法中の移動の制限
        if(isMoveAddVerocity)
        {
            Horizontal *= 0.1f;
        }

        //魔法のチャージアクション
        ChargeMagicCnt();

        //デバッグ用

        Verlocity = rb.velocity;
        playerHP = playerObjectState.GetPlayerHP();
        playerMP = playerObjectState.GetPlayerMP();



        if (magicCnt > 0)
        {
            Debug.Log(magicCnt * 100 + "%");
        }
        else { Debug.Log("nopush 'S'"); }
    }

    //-----------------------------------
    //物理演算
    //-----------------------------------

    private void FixedUpdate()
    {
        if (state == State.Normal) 
        {
            PlayerMove();
            PlayerCrouch();
            PlayerJump();
            PlayerAttack();
            PlayerMagic();
        }

        HitBoxCollisionContllor();          
    }

    //-----------------------------------
    //メソッド
    //-----------------------------------

    private void PlayerMove() {

        //横移動
        {
            if (Horizontal != 0)
            {
                if (Mathf.Abs(rb.velocity.x) <= maxspeed)
                {
                    Vector2 force = new(speed * Horizontal, 0);
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
    }
    private void PlayerCrouch()
    {
        //しゃがむ
        {
            if (isGround)
            {
                if (Vertical <= -0.5f)
                {
                    anim.SetBool("Crouch", true);
                    this.rb.velocity = new(0, this.rb.velocity.y);
                    crouchFlag = true;
                }
                else
                {
                    anim.SetBool("Crouch", false);
                    crouchFlag = false;
                }
            }
        }
    }
    private void PlayerJump() {

        //しゃがんでいる場合は処理を行わない
        if (crouchFlag) { onJumpFlag = false; return; }

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
            if (onJumpFlag)
            {
                Vector2 jumpForce = new(this.rb.velocity.x, jumpforce);
                this.rb.AddForce(jumpForce);
                anim.Play("PlayerJumpFoward", 0, 0);
                jumpFlag = false;
                onJumpFlag = false;
            }
        }
    }
    private void PlayerAttack()
    {
        if (isGround) { jumpAttackCnt = 3; }

        if (onAttackFlag)
        {
            NormalAttack();
            CrouchAttack();
            JumpAttack();
            onAttackFlag = false;
            isMoveAddVerocity = true;
        }
    }
    private void NormalAttack()
    {
        if (!isGround) { return; }
        if (crouchFlag) { return; }

        if (attackFlag)
        {
            switch (comboAttackCnt)
            {
                case 0:
                    anim.Play("PlayerAttack2", 0, 0);
                    comboAttackCnt = 1;
                    attackFlag = false;
                    break;
                case 1:
                    anim.Play("PlayerAttack3", 0, 0);
                    comboAttackCnt = 2;
                    attackFlag = false;
                    break;
                case 2:
                    anim.Play("PlayerAttack1", 0, 0);
                    comboAttackCnt = 0;
                    attackFlag = false;
                    break;
            }
        }
    }
    private void CrouchAttack()
    {
        if (!crouchFlag) { return; }

        anim.Play("PlayerCrouchAttack", 0, 0);
        attackFlag = false;
    }
    private void JumpAttack()
    {
        if (isGround) { return; }

        if (jumpAttackCnt > 0)
        {
            rb.gravityScale = 0;
            rb.velocity = new(0, 0);
            anim.Play("PlayerJumpAttack", 0, 0);
            attackFlag = false;
            jumpAttackCnt--;
        }
    }
    private void PlayerMagic()
    {
        if (crouchFlag) { return; }
        if (!isGround) { return; }

        preMagicCnt *= 100;

        ChargeMagic();
        
        if (onMagicFlag)
        {
            if (preMagicCnt < susutainMagicTime)
            {
                FastMagic();
                onMagicFlag = false;
            }
            
            if (preMagicCnt >= susutainMagicTime)
            {
                SustainMagic();
                onMagicFlag = false;
            }          
        }

        preMagicCnt = magicCnt;
    }
    private void ChargeMagicCnt() 
    {
        //ジャンプ、しゃがんだらカウントをリセット
        if (state != State.Normal) { magicCnt = 0; return; }
        if (!isGround) { magicCnt = 0; return; }
        if (crouchFlag) { magicCnt = 0; return; }
        
        //holdした時間を0から1で返す
        magicCnt = inputActions.Player.Magic.GetTimeoutCompletionPercentage();
    }
    private void ChargeMagic()
    {
        if (!isGround) { return; }

        if (magicCnt > 0)
        {
            if (magicFlag)
            {
                anim.Play("PlayerMagicCharge", 0, 0);
            }
            magicFlag = false;
            isMoveAddVerocity = true;
        }

        if (magicCnt == 0) 
        {
            magicFlag = true;
        }
        
    }
    private void FastMagic()
    {
        anim.Play("PlayerMagicFast", 0, 0);
    }
    private void SustainMagic() 
    {
        anim.Play("PlayerMagicSustain", 0, 0);
    }
    private void HitBoxCollisionContllor()
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
    private void AttackCollisionContllor(Collider2D coll) 
    {
        attack1coll.enabled = false;
        attack2coll.enabled = false;
        attack3coll.enabled = false;
        attackCrouchcoll.enabled = false;
        attackJumpcoll.enabled = false;

        if (coll == null) { return; }
        coll.enabled = true;

    }
    private void IsDash()
    {
        if (state != State.Normal) { return; }

        state = State.Dash;

        anim.Play("PlayerDash", 0, 0);

        this.rb.gravityScale = 0;
        this.rb.velocity = new(8 * key, 0);

    } 
    private void PlayerDeath()
    {
        anim.Play("PlayerDeath", 0, 0);
    }

    //---------------------------------------
    //ゲッター関数
    //---------------------------------------

    public State GetState() { return this.state; }

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

    public void SubPlayerHP(int atp)
    {
        playerObjectState.SubPlayerHP(atp);
        Debug.Log("PlayerHP->" + playerObjectState.GetPlayerHP());
    }

    public void SuccessDash()
    {
        playerObjectState.SetPlayerAtp(20);
    }

    public void PlayerHit1()
    {
        anim.Play("PlayerHit1", 0, 0);

        rb.gravityScale = 0;
        state = State.Hit;
    }

    //---------------------------------------
    //アニメーションイベント関数
    //---------------------------------------

    private void PlayerAttack1End() { comboAttackCnt = 0; isMoveAddVerocity = false; AttackCollisionContllor(null); }
    private void PlayerAttack2End() { comboAttackCnt = 0; isMoveAddVerocity = false; AttackCollisionContllor(null); }
    private void PlayerAttack3End() { comboAttackCnt = 0; isMoveAddVerocity = false; AttackCollisionContllor(null); }
    private void PlayerAttack1true() { attackFlag = true; }
    private void PlayerAttack2true() { attackFlag = true; }
    private void PlayerAttack3true() { attackFlag = true; }
    private void PlayerJumpAttackEnd() { attackFlag = true; rb.gravityScale = 2; isMoveAddVerocity = false; AttackCollisionContllor(null); }
    private void PlayerCrouchAttackEnd() { attackFlag = true; isMoveAddVerocity = false; AttackCollisionContllor(null); }
    private void PlayerDashEnd() 
    { 
        state = State.Normal;
        rb.gravityScale = 2;
        isMoveAddVerocity = false;
        attackFlag = true;
        comboAttackCnt = 0;
        AttackCollisionContllor(null);
    }
    private void PlayerHitEnd() 
    { 
        state = State.Normal;
        rb.gravityScale = 2;
        isMoveAddVerocity = false;
        attackFlag = true;
        comboAttackCnt = 0;
        AttackCollisionContllor(null);
    }
    private void Attack1ColliderStart() { AttackCollisionContllor(attack1coll); }
    private void Attack2ColliderStart(){ AttackCollisionContllor(attack2coll); }
    private void Attack3ColliderStart(){ AttackCollisionContllor(attack3coll); }
    private void AttackCrouchColliderStart(){ AttackCollisionContllor(attackCrouchcoll); }
    private void AttackJumpColliderStart(){ AttackCollisionContllor(attackJumpcoll); }

}
