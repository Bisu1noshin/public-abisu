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
    private Collider2D Attack1Coll;
    private Collider2D Attack2Coll;
    private Collider2D Attack3Coll;
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
        findPlayer2RunColl = transform.GetChild(1).GetComponent<Collider2D>();
        findPlayer2AttackColl = transform.GetChild(2).GetComponent<Collider2D>();
        Attack1Coll = transform.GetChild(3).GetComponent<Collider2D>();
        Attack2Coll = transform.GetChild(4).GetComponent<Collider2D>();
        Attack3Coll = transform.GetChild(5).GetComponent<Collider2D>();
        findPlayer2RunColl.enabled = false;
        findPlayer2AttackColl.enabled = false;
        runTimeCnt = 0;

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
        stateMachine.SetupState(StateType.Idle, () => anim.Play("Crab_Idle", 0, 0), () => CollisonReset(), deltaTime => EnemyIdle());
        stateMachine.SetupState(StateType.Attack1, () => anim.Play("Crab_Attack1", 0, 0), () => CollisonReset(), deltaTime => EnemyAttack1());
        stateMachine.SetupState(StateType.Attack2, () => anim.Play("Crab_Attack2", 0, 0), () => CollisonReset(), deltaTime => EnemyAttack2());
        stateMachine.SetupState(StateType.Attack3, () => anim.Play("Crab_Attack3", 0, 0), () => CollisonReset(), deltaTime => EnemyAttack3());
        stateMachine.SetupState(StateType.Death, () => anim.Play("Crab_Death", 0, 0), () => CollisonReset(), deltaTime => EnemyDeath());
        stateMachine.SetupState(StateType.Run, () => anim.Play("Crab_Run", 0, 0), () => CollisonReset(), deltaTime => EnemyRun());
        stateMachine.SetupState(StateType.Hit, () => anim.Play("Crab_Hit", 0, 0), () => CollisonReset(), deltaTime => EnemyHit());

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

    private void EnemyIdle()
    {
        enemyState.AddMoveCnt();
        findPlayer2RunColl.enabled = true;
    }
    private void EnemyAttack1(){ Attack1Coll.enabled = true; }
    private void EnemyAttack2() { Attack2Coll.enabled = true; }
    private void EnemyAttack3() { Attack3Coll.enabled = true; }
    private void EnemyDeath() { CollisonReset(); }
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

        runTimeCnt += Time.deltaTime;

        if (runTimeCnt >= 1.0f) 
        {
            runTimeCnt = 0;
            stateMachine.ExecuteTrigger(TriggerType.EnterIdle);
        }

        transform.Translate(movePosX * Time.deltaTime, 0, 0);
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
    private void CollisonReset() 
    {
        findPlayer2RunColl.enabled = false;
        findPlayer2AttackColl.enabled = false;
        Attack1Coll.enabled = false;
        Attack2Coll.enabled = false;
        Attack3Coll.enabled = false;
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

    private void Crab_AttackEnd()
    {
        stateMachine.ExecuteTrigger(TriggerType.EnterIdle);
    }
    private void Crab_HitEnd() { stateMachine.ExecuteTrigger(TriggerType.EnterIdle); }
    private void Crab_DeathEnd() { Destroy(gameObject); }

    public bool GetHitFlag() { return hitFlag; }
    public void SetMovePosX(float posX)
    {
        if (posX < 0)
        {
            transform.localScale = new(-1, 1, 1);
        }
        else if(posX > 0)
        {
            transform.localScale = new(1, 1, 1);
        }
        movePosX = posX;
    }
    public EnemyObjectState GetGameObjectState() { return enemyState; }
    public Collider2D GetNormalCollider() 
    {
        colliderFlag = true;
        return NormalColl;
    }
    public void SetFindEnemy2Run()
    {
        findPlayer2RunFlag = true;
        stateMachine.ExecuteTrigger(TriggerType.EnterRun);
    }
    public void SetFindEnemy2Attack()
    {
        findPlayer2AttackFlag = true;
    }
    public void SetEnemyObjectState(EnemyObjectState _state) { enemyState = _state; }
    public void SetEnemyState2Hit() 
    {
        if (stateMachine.GetState() == StateType.Hit) { return; }
        if (stateMachine.GetState() == StateType.Death) { return; }

        hitFlag = true;
        stateMachine.ExecuteTrigger(TriggerType.EnterHit);
    }
}
