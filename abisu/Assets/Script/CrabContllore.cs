using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabContllore : MonoBehaviour
{
    //-----------------------
    // ステータスの定義
    //-----------------------

    private enum StateType
    {
        Non,
        Idle,
        Attack1,
        Attack2,
        Attack3,
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
        EnterAttack1,
        EnterAttack2,
        EnterAttack3,
        EnterRun,
        EnterHit,
        EnterDeath
    }

    private StateMachine<StateType, TriggerType> stateMachine;
    private EnemyObjectState enemyState;
    private Animator anim;//アニメーション
    private GameObject player;//プレイヤー
    private Collider2D NormalColl;//ヒットボックス
    private Collider2D findPlayer2RunColl;
    private Collider2D findPlayer2AttackColl;
    private bool hitFlag;
    private float invincibleTimeCnt;
    private bool findPlayer2RunFlag;
    private bool findPlayer2AttackFlag;
    private float movePosX;

    private void Start()
    {
        stateMachine = new StateMachine<StateType, TriggerType>(StateType.Idle);
        enemyState = new EnemyObjectState(10, 10);
        anim = GetComponent<Animator>();
        player = GameObject.Find("Player");
        findPlayer2RunColl = transform.GetChild(1).GetComponent<Collider2D>();
        findPlayer2AttackColl = transform.GetChild(2).GetComponent<Collider2D>();
        findPlayer2RunColl.enabled = false;
        findPlayer2AttackColl.enabled = false;

        // 遷移情報を登録
        stateMachine.AddTransition(StateType.Idle, StateType.Hit, TriggerType.EnterHit);
        stateMachine.AddTransition(StateType.Idle, StateType.Run, TriggerType.EnterRun);
        stateMachine.AddTransition(StateType.Idle, StateType.Death, TriggerType.EnterDeath);
        stateMachine.AddTransition(StateType.Attack1, StateType.Idle, TriggerType.EnterIdle);
        stateMachine.AddTransition(StateType.Attack1, StateType.Hit, TriggerType.EnterHit);
        stateMachine.AddTransition(StateType.Attack1, StateType.Death, TriggerType.EnterDeath);
        stateMachine.AddTransition(StateType.Attack2, StateType.Idle, TriggerType.EnterIdle);
        stateMachine.AddTransition(StateType.Attack2, StateType.Hit, TriggerType.EnterHit);
        stateMachine.AddTransition(StateType.Attack2, StateType.Death, TriggerType.EnterDeath);
        stateMachine.AddTransition(StateType.Attack3, StateType.Idle, TriggerType.EnterIdle);
        stateMachine.AddTransition(StateType.Attack3, StateType.Hit, TriggerType.EnterHit);
        stateMachine.AddTransition(StateType.Attack3, StateType.Death, TriggerType.EnterDeath);
        stateMachine.AddTransition(StateType.Run, StateType.Idle, TriggerType.EnterIdle);
        stateMachine.AddTransition(StateType.Run, StateType.Attack1, TriggerType.EnterAttack1);
        stateMachine.AddTransition(StateType.Run, StateType.Attack2, TriggerType.EnterAttack2);
        stateMachine.AddTransition(StateType.Run, StateType.Attack3, TriggerType.EnterAttack3);
        stateMachine.AddTransition(StateType.Run, StateType.Hit, TriggerType.EnterHit);
        stateMachine.AddTransition(StateType.Run, StateType.Death, TriggerType.EnterDeath);
        stateMachine.AddTransition(StateType.Hit, StateType.Idle, TriggerType.EnterIdle);
        stateMachine.AddTransition(StateType.Hit, StateType.Death, TriggerType.EnterDeath);

        // Actionの登録
        stateMachine.SetupState(StateType.Idle, () => anim.Play("Crab_Idle", 0, 0), () => Debug.Log("ExitIdle"), deltaTime => EnemyIdle());
        stateMachine.SetupState(StateType.Attack1, () => anim.Play("Crab_Attack1", 0, 0), () => Debug.Log("ExitAttack1"), deltaTime => EnemyAttack1());
        stateMachine.SetupState(StateType.Attack2, () => anim.Play("Crab_Attack2", 0, 0), () => Debug.Log("ExitAttack2"), deltaTime => EnemyAttack2());
        stateMachine.SetupState(StateType.Attack3, () => anim.Play("Crab_Attack3", 0, 0), () => Debug.Log("ExitAttack3"), deltaTime => EnemyAttack3());
        stateMachine.SetupState(StateType.Death, () => anim.Play("Crab_Death", 0, 0), () => Debug.Log("ExitDeath"), deltaTime => EnemyDeath());
        stateMachine.SetupState(StateType.Run, () => anim.Play("Crab_Run", 0, 0), () => Debug.Log("ExitRun"), deltaTime => EnemyHit());
        stateMachine.SetupState(StateType.Hit, () => anim.Play("Crab_Hit", 0, 0), () => Debug.Log("ExitHit"), deltaTime => EnemyRun());

    }

    private void Update()
    {
        //ステートマシーンを更新
        stateMachine.Update(Time.deltaTime);

        //無敵処理
        InvincibleTime(0.5f);

        if (enemyState.GetHP() == 0)
        {
            stateMachine.ExecuteTrigger(TriggerType.EnterDeath);
            enemyState.SetHP(-1);
        }
    }

    private void EnemyIdle()
    {
        enemyState.AddMoveCnt();
        findPlayer2RunColl.enabled = true;
    }
    private void EnemyAttack1(){ }
    private void EnemyAttack2() { }
    private void EnemyAttack3() { }
    private void EnemyDeath() { }
    private void EnemyHit() { }
    private void EnemyRun() 
    {

        findPlayer2AttackColl.enabled = true;

        if (findPlayer2AttackFlag) 
        {
            int seed = enemyState.GetMoveCnt();
            switch(seed % 3)
            {
                case 0:
                    findPlayer2AttackFlag = false;
                    stateMachine.ExecuteTrigger(TriggerType.EnterAttack1);
                    break;
                case 1:
                    findPlayer2AttackFlag = false;
                    stateMachine.ExecuteTrigger(TriggerType.EnterAttack2);
                    break;
                case 2:
                    findPlayer2AttackFlag = false;
                    stateMachine.ExecuteTrigger(TriggerType.EnterAttack3);
                    break;
                default:
                    break;
            }
            return;
        }

        EnemyMove(movePosX);
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
    private void EnemyMove(float posX) 
    {
        transform.position = new
            (transform.position.x + posX, transform.position.y);
    }

    private void Crab_IdleEnd() 
    {        
        if (findPlayer2RunFlag)
        {
            findPlayer2RunColl.enabled = false;
            findPlayer2RunFlag = false;
            stateMachine.ExecuteTrigger(TriggerType.EnterRun);
        }
    }
    private void Crab_AttackEnd()
    {
        stateMachine.ExecuteTrigger(TriggerType.EnterIdle);
    }

    public void SetMovePosX(float posX)
    {
        movePosX = posX;
    }

    public void SetFindPlayer2Run()
    {
        findPlayer2RunFlag = true;
    }
    public void SetFindPlayer2Attack()
    {
        findPlayer2AttackFlag = true;
    }
}
