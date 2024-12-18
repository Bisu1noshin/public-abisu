using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitBox : MonoBehaviour
{
    private GameObject player;
    private GameObject idle;
    private GameObject crouch;
    private Collider2D idlecoll;
    private Collider2D crouchcoll;
    private bool playerHit;
    private int state;

    private void Start()
    {
        this.playerHit = false;
        this.player = this.gameObject;
        this.idle = transform.GetChild(1).gameObject;
        this.crouch = transform.GetChild(2).gameObject;
        idlecoll = idle.GetComponent<Collider2D>();
        crouchcoll = crouch.GetComponent<Collider2D>();

        idlecoll.enabled = false;
        crouchcoll.enabled = false;
    }

    private void Update()
    {
        state = player.GetComponent<PlayerMove>().GetState();

        if(state == 3)
        {
            idlecoll.enabled = false;
            crouchcoll.enabled = true;
        }
        else
        {
            idlecoll.enabled = true;
            crouchcoll.enabled = false;
        }
    }
}
