using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeHitBoxContllore : MonoBehaviour
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
            bool hitFlag = GetComponentInParent<SlimeContllore>().GetHitFlag();
            if (hitFlag) { return; }

            EnemyObjectState e_state =
                 GetComponentInParent<SlimeContllore>().GetGameObjectState();

            PlayerObjectState p_state =
                other.GetComponentInParent<PlayerContllor>().GetPlayerState();

            e_state.SubHP(p_state.GetPlayerAtp());

            GetComponentInParent<SlimeContllore>().SetEnemyObjectState(e_state);
            Debug.Log("SlimeEnemyHP->" + e_state.GetHP());

            GetComponentInParent<SlimeContllore>().SetEnemyState2Hit();
        }
    }
}
