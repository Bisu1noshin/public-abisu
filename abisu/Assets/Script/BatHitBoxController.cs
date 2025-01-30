using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatHitBoxController : MonoBehaviour
{
    private Collider2D coll;

    private void Start()
    {
        coll = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Attack")
        {
            bool hitFlag = GetComponentInParent<BatContllore>().GetEnemyHitFlag();
            if (hitFlag) { return; }

            EnemyObjectState e_state =
                GetComponentInParent<BatContllore>().GetEnemyState();

            PlayerObjectState p_state =
                other.GetComponentInParent<PlayerContllor>().GetPlayerState();

            e_state.SubHP(p_state.GetPlayerAtp());

            GetComponentInParent<BatContllore>().SetEnemyObjectState(e_state);
            Debug.Log("BatEnemyHP->" + e_state.GetHP());

            GetComponentInParent<BatContllore>().SetEnemyState2Hit();

        }
    }
}
