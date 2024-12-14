using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    //-------------------------------------
    //インスペクター参照不可
    //-------------------------------------
    private GameObject player;//プレイヤー
    private Rigidbody2D rb;//Rigidbody2D
    private bool isGround;//床判定
    private bool jumpflag;
    //-------------------------------------
    //インスペクター参照可
    //-------------------------------------
    [SerializeField] private State state;//ステータス

    //-------------------------------------
    //ステータスの定義
    //-------------------------------------
    public enum State
    { 
        Non, Idle, Run, Attack, Crouch, Jump, Hit, Dash, Death
    };

    private void Start()
    {
        //初期化
        this.player = GetComponent<GameObject>();
        this.rb = GetComponent<Rigidbody2D>();
        this.state = State.Non;
    }
    private void FixedUpdate()
    {
        //子オブジェクトから床判定を取得する
        isGround = GetComponentInChildren<PlayerIsGround>().GetIsGround();

        if (isGround) { jumpflag = true; }
        //横移動
        {
            if (RunMove(isGround))
            {
                Debug.Log("Player->Run");
                state = State.Run;
            }
            else
            {
                Debug.Log("Player->Idle");
                state = State.Idle;
            }
        }

        //ジャンプ
        {
            if(jumpflag)
            {
                if (JumpMove())
                {
                    Debug.Log("Player->Jump");
                    state = State.Jump;
                }
            }           
        }
    }

    private bool RunMove(bool isGround)
    {
        int key = 0;//プレイヤーの向き
        float maxspeed = 8.0f;//プレイヤーの最大速度

        if (isGround)//床と接触しているときのみ横移動する
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                //左に向く
                key = -1;
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                //右に向く
                key = 1;
            }
            else
            {
                //加わった力が0より多い時、摩擦を起こす
                if (!Mathf.Approximately(Mathf.Abs(this.rb.velocity.x), 0))
                {
                    float velocity = this.rb.velocity.x;
                    this.rb.velocity = new Vector2(velocity * 0.96f, this.rb.velocity.y);
                }
            }

            if (maxspeed > Mathf.Abs(this.rb.velocity.x))
            {
                Vector2 force = new Vector2(15.0f * key, 0);//加える力の大きさ
                //最大速度より小さければ力を加える
                this.rb.AddForce(force);
            }
        }

        switch (key)
        {
            case 1://右に向く
                transform.eulerAngles = new Vector3(0, 0, 0);
                break;
            case -1://左に向く
                transform.eulerAngles = new Vector3(0, 180, 0);
                break;
            default:break;
        } 

        if (!Mathf.Approximately(Mathf.Abs(this.rb.velocity.x), 0))
        {
            //力が加わっている
            return true;
        }

        return false;
    }
    private bool JumpMove()
    {
        Vector2 jumpforce = new(this.rb.velocity.x, 5.0f);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.rb.velocity = jumpforce;
            jumpflag = false;
            return true;
        }

        return false;
    }
    //-------------------------------------
    //参照用関数
    //-------------------------------------
    public State GetState()
    {
        //参照用
        return this.state;
    }
}
