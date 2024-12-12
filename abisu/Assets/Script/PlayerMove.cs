using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private GameObject player;
    private Rigidbody rb;

    [SerializeField] private State state;
    private enum State
    { 
        Non, Idle, Run, Attack, Crouch, Jump, Hit, Dash, Death
    };

    private void Start()
    {
        this.player = GetComponent<GameObject>();
        this.rb = GetComponent<Rigidbody>();
        this.state = State.Non;
    }
}
