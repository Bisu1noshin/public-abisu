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
            PlayerContllor_1.State state =
                other.gameObject.GetComponentInParent<PlayerContllor_1>().GetState();

            //プレイヤーの状態がNormalの時、ダメージを与える
            if (state == PlayerContllor_1.State.Normal)
            {
                Debug.Log("Hit");
                EnemyObjectState _state = GetComponentInParent<SlimeContllore>().GetGameObjectState();
                other.gameObject.GetComponentInParent<PlayerContllor_1>().PlayerHit1();
                other.gameObject.GetComponentInParent<PlayerContllor_1>().SubPlayerHP(_state.GetAtp());
            }

            //プレイヤーの状態がDashの時、回避成功関数を呼び出す
            if (state == PlayerContllor_1.State.Dash)
            {
                Debug.Log("Brink");
                other.gameObject.GetComponentInParent<PlayerContllor_1>().SuccessDash();
                Collider2D coll =
                    GetComponentInParent<SlimeContllore>().GetNormalCollider();
                coll.enabled = false;
            }
        }
    }
}
