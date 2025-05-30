﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightBorneAttackColliderContllore : MonoBehaviour
{
    private Collider2D coll;//コライダー
    void Start()
    {
        //初期化
        coll = GetComponent<Collider2D>();
    }

    //他のコライダーが範囲に侵入した時に呼び出す
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")//コライダーのタグがプレイヤーだった時
        {
            //プレイヤーの状態を取得
            PlayerContllor_1.State state =
                other.gameObject.GetComponentInParent<PlayerContllor_1>().GetState();

            //プレイヤーの状態がNormalの時、ダメージを与える
            if (state == PlayerContllor_1.State.Normal)
            {
                Debug.Log("Hit");
                EnemyObjectState _state = GetComponentInParent<NightBoss>().GetGameObjectState();
                other.gameObject.GetComponentInParent<PlayerContllor_1>().PlayerHit1();
                other.gameObject.GetComponentInParent<PlayerContllor_1>().SubPlayerHP(_state.GetAtp());
            }

            //プレイヤーの状態がDashの時、回避成功関数を呼び出す
            if (state == PlayerContllor_1.State.Dash)
            {
                Debug.Log("Brink");
                other.gameObject.GetComponentInParent<PlayerContllor_1>().SuccessDash();
                Collider2D coll =
                    GetComponentInParent<NightBoss>().GetNormalCollider();
                coll.enabled = false;
            }

        }
    }
}
