using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabHitBoxColliderContllore : MonoBehaviour
{
    private Collider2D col;
    private void Start()
    {
        col = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Attack")
        {
            bool hitFlag = GetComponentInParent<CrabContllore>().GetHitFlag();
            if (hitFlag) { return; }

            EnemyObjectState e_state =
                 GetComponentInParent<CrabContllore>().GetGameObjectState();

            PlayerObjectState p_state =
                other.GetComponentInParent<PlayerContllor_1>().GetPlayerState();

            e_state.SubHP(p_state.GetPlayerAtp());

            GetComponentInParent<CrabContllore>().SetEnemyObjectState(e_state);
            Debug.Log("CrabEnemyHP->" + e_state.GetHP());

            GetComponentInParent<CrabContllore>().SetEnemyState2Hit();
        }
    }
}
