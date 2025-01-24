using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeFindPlayer2AttackColliderConyllore : MonoBehaviour
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
            float playerPosX=
                other.gameObject.transform.parent.position.x;

            GetComponentInParent<SlimeContllore>().SetEnemy2Attack();
            GetComponentInParent<SlimeContllore>().SetMovePosX(playerPosX);
        }
    }
}
