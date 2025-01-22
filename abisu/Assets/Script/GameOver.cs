using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    [SerializeField] private GameObject playerUI;
    [SerializeField] private Text GameOverText;
    [SerializeField] private Text TriAgainText;

    private PlayerAction inputActions;
    private Animator anim;
    private Vector2 moveInputValue;
    private float Horizontal;
    private bool titleFlag;
    private bool continueFlag;
    private int nextFlag;
    private int changeColor;

    private void Start()
    {
        anim = GetComponent<Animator>();
        anim.Play("PlayerDeath", 0, 0);
        GameOverText.color = new Color32(0, 0, 0, 255);
        TriAgainText.color = new Color32(0, 0, 0, 255);
        changeColor = 0;
    }
    private void Awake()
    {
        inputActions = new PlayerAction();

        // Actionイベント登録
        inputActions.Player.Move.started += OnMove;
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;
        inputActions.Player.Attack.performed += OnAttack;

        // Input Actionを機能させるためには、
        // 有効化する必要がある
        inputActions.Enable();
    }
    private void OnDestroy()
    {
        // 自身でインスタンス化したActionクラスはIDisposableを実装しているので、
        // 必ずDisposeする必要がある
        inputActions.Dispose();
    }
    private void OnMove(InputAction.CallbackContext context)
    {
        moveInputValue = context.ReadValue<Vector2>();
    }
    private void OnAttack(InputAction.CallbackContext context)
    {
        nextFlag++;
    }
    private void Update()
    {
        //アナログスティックを参照する
        Horizontal = moveInputValue.x;

        UIColorChange(GameOverText, 1, 0, "Red");
        UIColorChange(TriAgainText, 2, 1, "Red");


        MoveUI();
    }

    private void MoveUI()
    {
        if (Horizontal < 0) 
        {
            continueFlag = true;
            titleFlag = false;

            playerUI.transform.position =
                new(950, 134);
        }
        if (Horizontal > 0) 
        {
            titleFlag = true;
            continueFlag = false;

            playerUI.transform.position =
                new(1883, 134);
        }
    }

    private void UIColorChange(Text text,int maxCnt,int minCnt,string color)
    {
        switch (color) 
        {
            case "Red":
                {
                    if (nextFlag >= maxCnt)
                    {
                        text.color = new Color32(255, 0, 0, 255);
                        return;
                    }

                    if (nextFlag >= minCnt)
                    {
                        if (changeColor < 255) { changeColor++; }
                        else if (changeColor == 255) { nextFlag = minCnt + 1; }

                        text.color = new Color32((byte)changeColor, 0, 0, 255);
                    }
                    break;
                }
            case "Green":
                {
                    if (nextFlag >= maxCnt)
                    {
                        text.color = new Color32(255, 0, 0, 255);
                        return;
                    }

                    if (nextFlag >= minCnt)
                    {
                        if (changeColor < 255) { changeColor++; }
                        else if (changeColor == 255) { nextFlag = minCnt + 1; }

                        text.color = new Color32(0, (byte)changeColor, 0, 255);
                    }
                    break;
                }
        }
        
    }

    

}
