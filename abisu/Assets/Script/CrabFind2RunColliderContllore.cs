using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabFind2RunColliderContllore : MonoBehaviour
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
            float posx =
                other.transform.position.x - transform.parent.position.x;

            GetComponentInParent<CrabContllore>().SetMovePosX(posx);
            GetComponentInParent<CrabContllore>().SetFindEnemy2Run();
        }
    }
}
