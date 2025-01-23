using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabFind2AttackColliderContllore : MonoBehaviour
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
            GetComponentInParent<CrabContllore>().SetFindPlayer2Attack();
        }
    }
}
