using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeContllore : MonoBehaviour
{
    //-----------------------
    // ステータスの定義
    //-----------------------

    private enum StateType
    {
        Non,
        Idle,
        Attack,
        Run,
        Hit,
        Death
    }

    //-----------------------
    // トリガーの定義
    //-----------------------

    private enum TriggerType
    {
        EnterIdle,
        EnterAttack,
        EnterRun,
        EnterHit,
        EnterDeath
    }

    private StateMachine<StateType, TriggerType> stateMachine;
    private EnemyObjectState enemyState;
    private Animator anim;//アニメーション
    private GameObject player;//プレイヤー
    private Collider2D NormalColl;//ヒットボックス
    private Collider2D findPlayer2AttackColl;
    private Collider2D AttackColl;
    private bool hitFlag;
    private float invincibleTimeCnt;
    private bool findPlayer2RunFlag;
    private bool findPlayer2AttackFlag;
    private float movePosX;
    private float runTimeCnt;
    private bool colliderFlag;
    private float collTimeCnt;

    private void Start()
    {
        stateMachine = new StateMachine<StateType, TriggerType>(StateType.Idle);
        enemyState = new EnemyObjectState(10, 10);
        anim = GetComponent<Animator>();
        player = GameObject.Find("Player");
        NormalColl = transform.GetChild(0).GetComponent<Collider2D>();
        findPlayer2AttackColl = transform.GetChild(1).GetComponent<Collider2D>();
        AttackColl = transform.GetChild(2).GetComponent<Collider2D>();
        findPlayer2AttackColl.enabled = false;
        runTimeCnt = 0;

        // 遷移情報を登録
        stateMachine.AddTransition(StateType.Idle, StateType.Hit, TriggerType.EnterHit);
        stateMachine.AddTransition(StateType.Idle, StateType.Attack, TriggerType.EnterAttack);
        stateMachine.AddTransition(StateType.Idle, StateType.Run, TriggerType.EnterRun);
        stateMachine.AddTransition(StateType.Idle, StateType.Death, TriggerType.EnterDeath);
        stateMachine.AddTransition(StateType.Attack, StateType.Idle, TriggerType.EnterIdle);
        stateMachine.AddTransition(StateType.Attack, StateType.Hit, TriggerType.EnterHit);
        stateMachine.AddTransition(StateType.Attack, StateType.Death, TriggerType.EnterDeath);
        stateMachine.AddTransition(StateType.Run, StateType.Idle, TriggerType.EnterIdle);
        stateMachine.AddTransition(StateType.Run, StateType.Attack, TriggerType.EnterAttack);
        stateMachine.AddTransition(StateType.Run, StateType.Hit, TriggerType.EnterHit);
        stateMachine.AddTransition(StateType.Run, StateType.Death, TriggerType.EnterDeath);
        stateMachine.AddTransition(StateType.Hit, StateType.Idle, TriggerType.EnterIdle);
        stateMachine.AddTransition(StateType.Hit, StateType.Death, TriggerType.EnterDeath);

        //// Actionの登録
        stateMachine.SetupState(StateType.Idle, () => anim.Play("Slime_Idle", 0, 0), () => CollisonReset(), deltaTime => EnemyIdle());
        stateMachine.SetupState(StateType.Attack, () => anim.Play("Slime_Ability", 0, 0), () => CollisonReset(), deltaTime => EnemyAttack());
        stateMachine.SetupState(StateType.Death, () => anim.Play("Slime_Death", 0, 0), () => CollisonReset(), deltaTime => EnemyDeath());
        stateMachine.SetupState(StateType.Run, () => anim.Play("Slime_Run", 0, 0), () => CollisonReset(), deltaTime => EnemyRun());
        stateMachine.SetupState(StateType.Hit, () => anim.Play("Slime_Hit", 0, 0), () => CollisonReset(), deltaTime => EnemyHit());

    }

    private void Update()
    {
        // 死亡判定
        if (enemyState.GetHP() == 0)
        {
            stateMachine.ExecuteTrigger(TriggerType.EnterDeath);
            enemyState.SetHP(-1);
        }

        // ステートマシーンを更新
        stateMachine.Update(Time.deltaTime);

        // 無敵処理
        InvincibleTime(0.5f);

        // コライダーを元に戻す処理
        EnemyColliderContllore(1.0f);
    }

    private void EnemyIdle() { findPlayer2AttackColl.enabled = true; }
    private void EnemyRun() 
    { 
        findPlayer2AttackColl.enabled = true;

        if (enemyState.GetMoveCnt() % 3 == 0) 
        {
            stateMachine.ExecuteTrigger(TriggerType.EnterIdle);
        }

        transform.Translate(movePosX * Time.deltaTime, 0, 0);
    }
    private void EnemyAttack() { AttackColl.enabled = true; }
    private void EnemyHit() { }
    private void EnemyDeath() { CollisonReset(); }
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
    private void CollisonReset()
    {
        findPlayer2AttackColl.enabled = false;
        AttackColl.enabled = false;
    }
    private void EnemyColliderContllore(float ColliderEnabledCnt)
    {
        if (NormalColl.enabled == true) { return; }

        if (colliderFlag)
        {
            collTimeCnt += Time.deltaTime;

            if (collTimeCnt >= ColliderEnabledCnt)
            {
                colliderFlag = false;
                NormalColl.enabled = true;
                collTimeCnt = 0;
            }
        }
    }

    //

    private void Slime_IdleEnd() { stateMachine.ExecuteTrigger(TriggerType.EnterRun); }
    private void Slime_RunEnd() { enemyState.AddMoveCnt(); }
    private void Slime_HitEnd() { stateMachine.ExecuteTrigger(TriggerType.EnterIdle); }

    //
    public void SetEnemy2Attack() 
    {
        stateMachine.ExecuteTrigger(TriggerType.EnterAttack);
    }
    public bool GetHitFlag() { return hitFlag; }
    public EnemyObjectState GetGameObjectState() { return enemyState; }
    public void SetEnemyObjectState(EnemyObjectState _state) { enemyState = _state; }
    public Collider2D GetNormalCollider()
    {
        colliderFlag = true;
        return NormalColl;
    }
    public void SetEnemyState2Hit()
    {
        if (stateMachine.GetState() == StateType.Hit) { return; }
        if (stateMachine.GetState() == StateType.Death) { return; }

        hitFlag = true;
        stateMachine.ExecuteTrigger(TriggerType.EnterHit);
    }
    public void SetMovePosX(float posX)
    {
        if (posX < 0)
        {
            transform.localScale = new(-1, 1, 1);
        }
        else if (posX > 0)
        {
            transform.localScale = new(1, 1, 1);
        }
        movePosX = posX;
    }
}
