using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatFindCollideController : MonoBehaviour
{
    private Collider2D coll;
    void Start()
    {
        coll = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Vector2 AtttackPos = other.transform.position - transform.parent.position;
            GetComponentInParent<BatContllore>().BatAttackTrigger(AtttackPos);
        }
    }
}
