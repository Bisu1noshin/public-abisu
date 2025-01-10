using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//--------------------------------------
//ステータスの管理用
//ステータスを参照させる
//コントローラーの情報を受け取る
//--------------------------------------
public class PlayerInput : MonoBehaviour
{
    //----------------------------------
    //インスペクター参照可能
    //----------------------------------

    [SerializeField] private State state;

    //----------------------------------
    //インスペクター参照不可
    //----------------------------------

    private bool isGround;
    private float Horizontal;
    private float Vertical;

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
        if (state ==State.Death)
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
