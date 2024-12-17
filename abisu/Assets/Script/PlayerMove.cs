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
    private Animator anim;
    private Collider2D col;
    private bool isGround;//床判定
    private bool jumpFlag;
    private bool crouchFlag;
    //-------------------------------------
    //インスペクター参照可
    //-------------------------------------
    [SerializeField] private int state;//ステータス

    

    private void Start()
    {
        //初期化
        this.player = GetComponent<GameObject>();
        this.rb = GetComponent<Rigidbody2D>();
        this.state = 1;
        this.anim = GetComponent<Animator>();
        this.col = GetComponent<Collider2D>();
    }
    private void FixedUpdate()
    {
        //子オブジェクトから床判定を取得する
        isGround = GetComponentInChildren<PlayerIsGround>().GetIsGround();
        
        //床に触れているときジャンプできるようにする
        if (isGround) { jumpFlag = true; }

        //落ちる
        {
            if(!isGround && this.rb.velocity.y <= 0)
            {
                state = 2;
                ChangeAnim(state);
            }
        }

        //横移動
        {
            if (RunMove())
            {
                if (isGround)
                {
                    Debug.Log("Player->Run");
                    state = -1;
                    ChangeAnim(state);
                }   
            }
            else
            {
                if (isGround)
                {
                    Debug.Log("Player->Idle");
                    state = 1;
                    ChangeAnim(state);
                }
            }
        }

        //ジャンプ
        {
            if(jumpFlag)
            {
                if (JumpMove())
                {
                    Debug.Log("Player->Jump");
                    state = 4;
                    ChangeAnim(state);
                }
            }           
        }

        //しゃがむ
        {
            if(Crouchmove())
            {
                Debug.Log("Player->Crouch");
                state = 3;
                ChangeAnim(state);
            }
        }
    }
    private void ChangeAnim(int s_)
    {
        anim.SetInteger("Anim_Trigger", s_);
    }
    private bool RunMove()
    {
        int key = 0;//プレイヤーの向き
        float maxspeed = 8.0f;//プレイヤーの最大速度

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
            if(isGround)
            {
                //加わった力が0より多い時、摩擦を起こす
                if (!Mathf.Approximately(Mathf.Abs(this.rb.velocity.x), 0))
                {
                    float velocity = this.rb.velocity.x;
                    this.rb.velocity = new Vector2(velocity * 0.96f, this.rb.velocity.y);
                }
            }
        }

        if (maxspeed > Mathf.Abs(this.rb.velocity.x))
        {
            Vector2 force = new Vector2(15.0f * key, 0);//加える力の大きさ

            //最大速度より小さければ力を加える                                            
            this.rb.AddForce(force);
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
        Vector2 jumpforce = new(this.rb.velocity.x, 400.0f);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.rb.AddForce(jumpforce);
            jumpFlag = false;
            return true;
        }

        return false;
    }
    private bool Crouchmove()
    {
        if(Input.GetKey(KeyCode.DownArrow))
        {
            crouchFlag = true;
            return true;
        }
        else
        {
            crouchFlag = false;
        }

        return false;
    }
    //-------------------------------------
    //参照可能関数
    //-------------------------------------
    public int GetState()
    {
        //参照用
        return this.state;
    }
}
