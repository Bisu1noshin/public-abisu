using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
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
    private Collider2D AttackColl;
    private bool hitFlag;
    private float invincibleTimeCnt;
    private bool findPlayer2RunFlag;
    private bool findPlayer2AttackFlag;
    private float movePosX;
    private float runTimeCnt;
    private bool colliderFlag;
    private float collTimeCnt;
    private int key;

    private void Start()
    {
        stateMachine = new StateMachine<StateType, TriggerType>(StateType.Idle);
        enemyState = new EnemyObjectState(10, 10);
        anim = GetComponent<Animator>();
        player = GameObject.Find("Player");
        NormalColl = transform.GetChild(0).GetComponent<Collider2D>();       
        AttackColl = transform.GetChild(1).GetComponent<Collider2D>();
        AttackColl.enabled = false;
        runTimeCnt = 0;
        key = (int)transform.localScale.x;

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
        stateMachine.SetupState(StateType.Run, () => anim.Play("Slime_Run", 0, 0), () => EnenmyRunExit(), deltaTime => EnemyRun());
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

    private void EnemyIdle() { }
    private void EnemyIdleExit()
    {
        //rayを飛ばす
        Vector2 enemyPos = new(transform.position.x, transform.position.y - 0.1f);
        Vector2 movePos = new(key, 0);

        movePosX = 1.0f;
        RaycastHit2D hit = Physics2D.Raycast(enemyPos, movePos, movePosX);
        Debug.DrawRay(enemyPos,movePos);

        if (hit.collider == null)
        {
            stateMachine.ExecuteTrigger(TriggerType.EnterRun);
            return;
        }

        movePosX = hit.point.x - enemyPos.x;

        if (hit.collider.tag == "Player")
        {
            stateMachine.ExecuteTrigger(TriggerType.EnterAttack);
            return;
        }

        Debug.Log(movePosX);
        key *= -1;
        stateMachine.ExecuteTrigger(TriggerType.EnterRun);
    }
    private void EnemyRun() 
    {
        float move = movePosX * Time.deltaTime;

        transform.Translate(move * key, 0, 0);
    }
    private void EnenmyRunExit() 
    {
        CollisonReset();
        transform.localScale = new(key, 1, 1);
    }
    private void EnemyAttack() 
    {
        AttackColl.enabled = true;
    }
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

    private void Slime_IdleEnd() { EnemyIdleExit(); stateMachine.ExecuteTrigger(TriggerType.EnterRun); }
    private void Slime_RunEnd() { stateMachine.ExecuteTrigger(TriggerType.EnterIdle); }
    private void Slime_HitEnd() { stateMachine.ExecuteTrigger(TriggerType.EnterIdle); }
    private void Slime_AttackEnd() { stateMachine.ExecuteTrigger(TriggerType.EnterIdle);}
    private void Slime_DeathEnd() { Destroy(gameObject); }
    private void Slime_AttackColl1() { AttackColl.transform.localPosition = new Vector3(0, -0.17f, 0); }
    private void Slime_AttackColl2() { AttackColl.transform.localPosition = new Vector3(0, 0.3f, 0); }
    private void Slime_AttackColl3() { AttackColl.transform.localPosition = new Vector3(0, 0.44f, 0); }    
    private void Slime_AttackColl4() { AttackColl.transform.localPosition = new Vector3(0, -0.13f, 0); }
    private void Slime_AttackColl5() { AttackColl.transform.localPosition = new Vector3(0, -0.44f, 0); }


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
