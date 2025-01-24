using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeAttackColliderContllore : MonoBehaviour
{
    private Collider2D coll;

    private void Start()
    {
        coll = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            //プレイヤーの状態を取得
            PlayerContllor.State state =
                other.gameObject.GetComponentInParent<PlayerContllor>().GetState();

            //プレイヤーの状態がNormalの時、ダメージを与える
            if (state == PlayerContllor.State.Normal)
            {
                Debug.Log("Hit");
                EnemyObjectState _state = GetComponentInParent<SlimeContllore>().GetGameObjectState();
                other.gameObject.GetComponentInParent<PlayerContllor>().PlayerHit1();
                other.gameObject.GetComponentInParent<PlayerContllor>().SubPlayerHP(_state.GetAtp());
            }

            //プレイヤーの状態がDashの時、回避成功関数を呼び出す
            if (state == PlayerContllor.State.Dash)
            {
                Debug.Log("Brink");
                other.gameObject.GetComponentInParent<PlayerContllor>().SuccessDash();
                Collider2D coll =
                    GetComponentInParent<SlimeContllore>().GetNormalCollider();
                coll.enabled = false;
            }
        }
    }
}
