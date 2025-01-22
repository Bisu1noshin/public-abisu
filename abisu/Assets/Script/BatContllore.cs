using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatContllore : MonoBehaviour
{
    //-----------------------
    //ステータスの定義
    //-----------------------

    private enum StateType
    {
        Non,
        Idle,
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
        EnterAttack,
        EnterHit,
        EnterDeath
    }

    private StateMachine<StateType,TriggerType> stateMachine;
    private EnemyObjectState enemyState;
    private Animator anim;//アニメーション
    private GameObject player;//プレイヤー
    private Collider2D NormalColl;//ヒットボックス
    private Collider2D AttackColl;//Attack時の攻撃範囲
    private Collider2D FindColl;//探索範囲用
    private bool hitFlag;
    private float invincibleTimeCnt;

    private void Start()
    {
        stateMachine = new StateMachine<StateType, TriggerType>(StateType.Idle);
        enemyState = new EnemyObjectState(100, 10);
        anim = GetComponent<Animator>();
        player = GameObject.Find("Player");
        hitFlag = false;

        // 遷移情報を登録
        stateMachine.AddTransition(StateType.Idle, StateType.Attack, TriggerType.EnterAttack);
        stateMachine.AddTransition(StateType.Idle, StateType.Hit, TriggerType.EnterHit);
        stateMachine.AddTransition(StateType.Idle, StateType.Death, TriggerType.EnterDeath);
        stateMachine.AddTransition(StateType.Attack, StateType.Idle, TriggerType.EnterIdle);
        stateMachine.AddTransition(StateType.Attack, StateType.Hit, TriggerType.EnterHit);
        stateMachine.AddTransition(StateType.Attack, StateType.Death, TriggerType.EnterDeath);
        stateMachine.AddTransition(StateType.Hit, StateType.Idle, TriggerType.EnterIdle);
        stateMachine.AddTransition(StateType.Hit, StateType.Death, TriggerType.EnterDeath);

        //Actionの登録
        stateMachine.SetupState(StateType.Idle, () => anim.Play("Bat_Fry", 0, 0), () => Debug.Log("ExitIdle"), deltaTime => EnemyIdle());
        stateMachine.SetupState(StateType.Attack, () => anim.Play("Bat_Attack", 0, 0), () => Debug.Log("ExitAttack"), deltaTime => EnemyAttack());
        stateMachine.SetupState(StateType.Hit, () => anim.Play("Bat_Hit", 0, 0), () => Debug.Log("ExitHit"), deltaTime => EnemyHit());
        stateMachine.SetupState(StateType.Death, () => anim.Play("Bat_Death", 0, 0), () => Debug.Log("ExitDeath"), deltaTime => EnemyDeath());
    }

    private void Update()
    {
        //ステートマシーンを更新
        stateMachine.Update(Time.deltaTime);

        //
        InvincibleTime(1.0f);
    }

    private void EnemyIdle() 
    {
        bool playerfind;
        //if()
    }
    private void EnemyAttack() { }
    private void EnemyHit() { }
    private void EnemyDeath() { }
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

    //

    private void Bat_AttackEnd() 
    {
        stateMachine.ExecuteTrigger(TriggerType.EnterIdle);
    }
    private void Bat_HitEnd() 
    {
        stateMachine.ExecuteTrigger(TriggerType.EnterIdle);
    }
    private void Bat_DeathEnd() { Destroy(gameObject); }


    //
    //
    //
    public EnemyObjectState GetEnemyState() { return enemyState; }
    public void SetEnemyObjectState(EnemyObjectState _state) 
    {
        if (_state == null) { return; }

        this.enemyState = _state; 
    }
    public void SetEnemyState2Hit()
    {
        if (stateMachine.GetState() == StateType.Hit){ return; }

        hitFlag = true;
        stateMachine.ExecuteTrigger(TriggerType.EnterHit);
    }
    public bool GetEnemyHitFlag() { return hitFlag; }
}
