using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightBoss : MonoBehaviour
{
    //-----------------------
    //ステータスの定義
    //-----------------------

    private enum StateType
    {
        Non,
        Idle,
        Run,
        Hit,
        Attack,
        Death
    }

    //-----------------------
    //トリガーの定義
    //-----------------------

    private enum TriggerType
    {
        EnterIdle,
        EnterRun,
        EnterAttack,
        EnterHit,
        EnterDeath
    }

    //-----------------------
    //メンバ変数
    //-----------------------

    private StateMachine<StateType, TriggerType> stateMachine;//ステートマシーン
    private EnemyObjectState enemyState;//ステータスクラス
    private Animator anim;//アニメーション
    private GameObject player;//プレイヤー
    private Collider2D NormalColl;//ヒットボックス
    private Collider2D AttackColl;//Attack時の攻撃範囲
    private Collider2D RunColl;//Run時の攻撃範囲
    private int key = 1;
    private bool onRun;
    private bool colliderFlag;
    private bool hitFlag;
    private float collTimeCnt;
    private float invincibleTimeCnt;

    //-----------------------
    //最初に1回だけ呼び出される関数
    //-----------------------

    private void Start()
    {
        //初期化
        stateMachine = new StateMachine<StateType, TriggerType>(StateType.Idle);
        enemyState = new EnemyObjectState(100, 10);
        anim = GetComponent<Animator>();
        player = GameObject.Find("Player");
        NormalColl = transform.GetChild(0).GetComponent<Collider2D>();
        AttackColl = transform.GetChild(1).GetComponent<Collider2D>();
        RunColl = transform.GetChild(2).GetComponent<Collider2D>();
        hitFlag = false;

        // 遷移情報を登録
        stateMachine.AddTransition(StateType.Idle, StateType.Attack, TriggerType.EnterAttack);
        stateMachine.AddTransition(StateType.Idle, StateType.Run, TriggerType.EnterRun);
        stateMachine.AddTransition(StateType.Idle, StateType.Death, TriggerType.EnterDeath);
        stateMachine.AddTransition(StateType.Idle, StateType.Hit, TriggerType.EnterHit);
        stateMachine.AddTransition(StateType.Attack, StateType.Idle, TriggerType.EnterIdle);
        stateMachine.AddTransition(StateType.Attack, StateType.Hit, TriggerType.EnterHit);
        stateMachine.AddTransition(StateType.Attack, StateType.Death, TriggerType.EnterDeath);
        stateMachine.AddTransition(StateType.Run, StateType.Hit, TriggerType.EnterHit);
        stateMachine.AddTransition(StateType.Run, StateType.Idle, TriggerType.EnterIdle);
        stateMachine.AddTransition(StateType.Run, StateType.Death, TriggerType.EnterDeath);
        stateMachine.AddTransition(StateType.Hit, StateType.Idle, TriggerType.EnterIdle);
        stateMachine.AddTransition(StateType.Hit, StateType.Death, TriggerType.EnterDeath);

        //Actionの登録
        stateMachine.SetupState(StateType.Idle, () => anim.Play("NightBorne_Idle", 0, 0), () => Debug.Log("OnExit ; Idle"), deltaTime => EnemyIdle());
        stateMachine.SetupState(StateType.Attack, () => anim.Play("NightBorne_Idle2Attack", 0, 0), () => Debug.Log("OnExit ; Attack"), deltaTime => EnemyAttack());
        stateMachine.SetupState(StateType.Run, () => anim.Play("NightBorne_Idle2Run", 0, 0), () => Debug.Log("OnExit ; Attack"), deltaTime => EnemyRun());
        stateMachine.SetupState(StateType.Hit, () => anim.Play("NightBorne_Hit", 0, 0), () => Debug.Log("OnExit ; Hit"), deltaTime => EnemyHit());
        stateMachine.SetupState(StateType.Death, () => anim.Play("NightBorne_Death", 0, 0), () => Debug.Log("OnExit ; Death"), deltaTime => EnemyDeath());

    }

    //-----------------------
    //毎フレーム呼び出されえる関数
    //-----------------------

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) enemyState.SubHP(enemyState.GetHP());

        //トリガーを呼ぶ
        if (Input.GetKey(KeyCode.J)) stateMachine.ExecuteTrigger(TriggerType.EnterRun);
        if (Input.GetKey(KeyCode.K)) stateMachine.ExecuteTrigger(TriggerType.EnterAttack);
        if (enemyState.GetHP() == 0)
        {
            stateMachine.ExecuteTrigger(TriggerType.EnterDeath);
            enemyState.SetHP(-1);
        }

        //ステートマシーンを更新
        stateMachine.Update(Time.deltaTime);

        //コライダーを元に戻す処理
        EnemyColliderContllore();

        //攻撃された後の無敵処理
        InvincibleTime(3.0f);
    }

    //---------------------------------
    //メソッド
    //---------------------------------

    private void EnemyIdle()
    {
        //プレイヤーの位置に合わせて向きを変更
        if (player.transform.position.x <= transform.position.x) { key = -1; }
        else { key = 1; }
        transform.localScale = new(key, 1, 1);

        //コライダー
        NormalColl.enabled = true;
        AttackColl.enabled = false;
        RunColl.enabled = false;

    }
    private void EnemyAttack() 
    {
        //Attack時の攻撃力の変更
        enemyState.SetATP(10);
    }
    private void EnemyRun()
    {
        if (onRun)
        {
            transform.Translate(0.4f * key, 0, 0);

            //コライダー
            NormalColl.enabled = true;
            AttackColl.enabled = false;
            RunColl.enabled = true;
        }

        //Run時の攻撃力の変更
        enemyState.SetATP(5);
    }
    private void EnemyHit() 
    {
        NormalColl.enabled = false;
    }
    private void EnemyDeath()
    {
        //コライダー
        NormalColl.enabled = false;
        AttackColl.enabled = false;
        RunColl.enabled = false;
    }
    private void EnemyColliderContllore()
    {
        if (NormalColl.enabled == true) { return; }

        if (colliderFlag)
        {
            collTimeCnt += Time.deltaTime;

            if (collTimeCnt >= 3.0f) 
            {
                colliderFlag = false;
                NormalColl.enabled = true;
                collTimeCnt = 0;
            }
        }
    }
    private void InvincibleTime(float maxCnt)
    {
        if (hitFlag)
        {
            invincibleTimeCnt += Time.deltaTime;

            if (invincibleTimeCnt >= maxCnt)
            {
                hitFlag = false;
                invincibleTimeCnt = 0;
            }
        }
    }

    //---------------------------------
    //アニメーションイベント関数
    //---------------------------------

    private void NightBorne_IdleEnd()
    {
        //シード値の取得
        int moveCnt = enemyState.GetMoveCnt();

        //シード値に合わせて行動を変更
        if (moveCnt % 3 == 0)
        {
            stateMachine.ExecuteTrigger(TriggerType.EnterAttack);
        }
        else
        {
            stateMachine.ExecuteTrigger(TriggerType.EnterRun);
        }
    }
    private void NightBorne_RunStart() { onRun = true; }
    private void NightBorne_RunEnd()
    {
        onRun = false;

        //Idleに戻し、シード値を更新
        stateMachine.ExecuteTrigger(TriggerType.EnterIdle);
        enemyState.AddMoveCnt();
    }
    private void NightBorne_AttackEnd()
    {
        //Idleに戻し、シード値を更新
        stateMachine.ExecuteTrigger(TriggerType.EnterIdle);
        enemyState.AddMoveCnt();
    }
    private void NightyBorne_AttackCheckStart()
    {
        //攻撃判定のコライダーを起動させる
        NormalColl.enabled = true;
        AttackColl.enabled = true;
        RunColl.enabled = false;
    }
    private void NightyBorne_AttackCheckEnd()
    {
        //攻撃用に使ったコライダーをもどす
        NormalColl.enabled = true;
        AttackColl.enabled = false;
        RunColl.enabled = false;
    }
    private void NightBorne_HitEnd() 
    {
        //Idleに戻し、シード値を更新
        stateMachine.ExecuteTrigger(TriggerType.EnterIdle);
    }
    private void Nightborne_DeathEnd() { Destroy(this.gameObject); }

    //---------------------------------
    //ゲッター関数
    //---------------------------------
    public EnemyObjectState GetGameObjectState()
    {
        //GameObjectStateを返す
        return this.enemyState;
    }
    public void SetEnemyObjectState(EnemyObjectState _state) 
    {
        if (_state == null) { return; }

        this.enemyState = _state;
    }
    public Collider2D GetNormalCollider()
    {
        if (NormalColl == null) { return null; }

        colliderFlag = true;
        return NormalColl;
    }
    public void SetEnemyState2Hit() 
    {
        if (stateMachine.GetState() == StateType.Hit) { return ; }

        hitFlag = true;
        stateMachine.ExecuteTrigger(TriggerType.EnterHit);
    }
    public bool GetHitFlag() { return hitFlag; }
}
