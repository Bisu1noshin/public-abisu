using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightBorneHitBox : MonoBehaviour
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
            bool hitFlag = GetComponentInParent<NightBoss>().GetHitFlag();
            if (hitFlag) { return; }

            EnemyObjectState e_state =
                 GetComponentInParent<NightBoss>().GetGameObjectState();

            PlayerObjectState p_state =
                other.GetComponentInParent<PlayerContllor>().GetPlayerState();

            e_state.SubHP(p_state.GetPlayerAtp());

            GetComponentInParent<NightBoss>().SetEnemyObjectState(e_state);
            Debug.Log("NightBossEnemyHP->" + e_state.GetHP());

            GetComponentInParent<NightBoss>().SetEnemyState2Hit();
        }
    }
}
