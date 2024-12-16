using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    //private Animator anim;
    //private PlayerMove.State prestate;

    //[SerializeField] private string trigger;
    //[SerializeField] private PlayerMove.State state;

    //private void Start()
    //{
    //    anim = GetComponent<Animator>();
    //    prestate = PlayerMove.State.Non;
    //}
    //private void Update()
    //{
    //    bool animChange = false;

    //    state = GetComponent<PlayerMove>().GetState();
    //    trigger = AnimTrigger(state);

    //    if (prestate != state)
    //    {
    //        animChange = true;
    //    }

    //    if (animChange)
    //    {
    //        AnimTriggerAllfalse();

    //        if (trigger != null)
    //        {
    //            anim.SetBool(trigger, true);
    //        }
    //    }

    //    prestate = state;
    //}
    //private void AnimTriggerAllfalse()
    //{
    //    anim.SetBool("Run", false);
    //    anim.SetBool("Jump", false);
    //    anim.SetBool("Fall", false);
    //}
    //private string AnimTrigger(PlayerMove.State _s)
    //{
    //    string name = null;
    //    switch (_s)
    //    {
    //        case PlayerMove.State.Run:
    //            name = "Run";
    //            break;
    //        case PlayerMove.State.Jump:
    //            name = "Jump";
    //            break;
    //        case PlayerMove.State.Fall:
    //            name = "Fall";
    //            break;

    //        default: break;
    //    }
    //    return name;
    //}
}
