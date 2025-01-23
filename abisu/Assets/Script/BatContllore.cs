using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatContllore : MonoBehaviour
{
    //-----------------------
    // ステータスの定義
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
    // トリガーの定義
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
    private bool hitFlag;
    private float invincibleTimeCnt;
    private float key;
    private float idleCnt;
    private float idleCntMax;

    [SerializeField] private GameObject shotPrefab;

    //-----------------------
    // 最初に1回だけ呼び出される関数
    //-----------------------

    private void Start()
    {
        stateMachine = new StateMachine<StateType, TriggerType>(StateType.Idle);
        enemyState = new EnemyObjectState(10, 10);
        anim = GetComponent<Animator>();
        player = GameObject.Find("Player");
        hitFlag = false;
        idleCnt = 0;
        idleCntMax = 5.0f;

        // 遷移情報を登録
        stateMachine.AddTransition(StateType.Idle, StateType.Attack, TriggerType.EnterAttack);
        stateMachine.AddTransition(StateType.Idle, StateType.Hit, TriggerType.EnterHit);
        stateMachine.AddTransition(StateType.Idle, StateType.Death, TriggerType.EnterDeath);
        stateMachine.AddTransition(StateType.Attack, StateType.Idle, TriggerType.EnterIdle);
        stateMachine.AddTransition(StateType.Attack, StateType.Hit, TriggerType.EnterHit);
        stateMachine.AddTransition(StateType.Attack, StateType.Death, TriggerType.EnterDeath);
        stateMachine.AddTransition(StateType.Hit, StateType.Idle, TriggerType.EnterIdle);
        stateMachine.AddTransition(StateType.Hit, StateType.Death, TriggerType.EnterDeath);

        // Actionの登録
        stateMachine.SetupState(StateType.Idle, () => anim.Play("Bat_Fry", 0, 0), () => Debug.Log("ExitIdle"), deltaTime => EnemyIdle());
        stateMachine.SetupState(StateType.Attack, () => anim.Play("Bat_Attack", 0, 0), () => Debug.Log("ExitAttack"), deltaTime => EnemyAttack());
        stateMachine.SetupState(StateType.Hit, () => anim.Play("Bat_Hit", 0, 0), () => Debug.Log("ExitHit"), deltaTime => EnemyHit());
        stateMachine.SetupState(StateType.Death, () => anim.Play("Bat_Death", 0, 0), () => Debug.Log("ExitDeath"), deltaTime => EnemyDeath());
    }

    //-----------------------
    // 毎フレーム呼び出されえる関数
    //-----------------------

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

    //---------------------------------
    // メソッド
    //---------------------------------

    private void EnemyIdle() 
    {
        idleCnt += Time.deltaTime;

        float move = 1.0f * Time.deltaTime;

        transform.eulerAngles = new(0, 0, 0);
        if (idleCnt >= idleCntMax) { transform.eulerAngles = new(0, 180, 0); move *= -1; }
        if (idleCnt >= idleCntMax * 2) { transform.eulerAngles = new(0, 0, 0); idleCnt = 0; }

        transform.position = new(transform.position.x + move, transform.position.y);
    }
    private void EnemyAttack() 
    {
        if (key < 0){ transform.eulerAngles = new(0, 180, 0); }
        if (key > 0) { transform.eulerAngles = new(0, 0, 0); }
    }
    private void EnemyHit() { }
    private void EnemyDeath() 
    {
        if (!GetComponent<Rigidbody2D>())
        {
            gameObject.AddComponent<Rigidbody2D>();
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 2;
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
    private void BatShotInstantiate(Vector2 AttackPos)
    {
        GameObject go = Instantiate(shotPrefab);
        go.transform.position = this.transform.position;
        go.GetComponent<BatShotController>().SetVector(AttackPos);
        go.GetComponent<BatShotController>().SetDestroyCnt(3.0f);
        go.GetComponent<BatShotController>().SetAttack(enemyState.GetAtp());
    }

    //---------------------------------
    // アニメーションイベント関数
    //---------------------------------

    private void Bat_AttackEnd() 
    {
        stateMachine.ExecuteTrigger(TriggerType.EnterIdle);
    }
    private void Bat_HitEnd() 
    {
        stateMachine.ExecuteTrigger(TriggerType.EnterIdle);
    }
    private void Bat_DeathEnd() { Destroy(gameObject); }

    //-----------------------------------
    // 参照用
    //-----------------------------------

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
    public void BatAttackTrigger(Vector2 AttackPos)
    {
        if (stateMachine.GetState() != StateType.Idle) { return; }

        BatShotInstantiate(AttackPos);
        key = AttackPos.x;
        stateMachine.ExecuteTrigger(TriggerType.EnterAttack);
    }
}
