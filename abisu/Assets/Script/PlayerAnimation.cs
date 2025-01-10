using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private GameObject player;
    private Animator anim;
    private PlayerMove.State state;
    private PlayerMove.State prestate;

    private void Start()
    {
        anim = GetComponent<Animator>();
        player = this.gameObject;
    }

    private void Update()
    {
        state = player.GetComponent<PlayerMove>().Get_State();

        if(prestate != state)
        {
            if (state == PlayerMove.State.Attack2)
            {
                anim.Play("PlayerAttack2", 0, 0);
            }
            if (state == PlayerMove.State.Attack3)
            {

            }
        }
        
        
        prestate = state;
    }

}
