using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitCheck : MonoBehaviour
{
    private bool playerHit;
    private Collider2D coll;

    private void Start()
    {
        playerHit = false;
        coll = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (playerHit)
        {
            playerHit = false;
        }

    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Enemy")
        {
            Debug.Log("Hit");
            playerHit = true;
        }
    }
    public bool GetPlayerHit()
    {
        return this.playerHit;
    }
}
