using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField] private GameObject playerUI;
    [SerializeField] private Text GameOverText;
    [SerializeField] private Text TriAgainText;
    [SerializeField] private Text TitleCntText;
    [SerializeField] private Text ContinueText;
    [SerializeField] private Text TitleText;

    private PlayerAction inputActions;
    private Animator anim;
    private Vector2 moveInputValue;
    private float Horizontal;
    private float timeCnt;
    private int titleTImeCnt;
    private bool titleFlag;
    private bool continueFlag;
    private int nextFlag;
    private int changeColor;
    private bool changeSceneFlag;

    private void Start()
    {
        anim = GetComponent<Animator>();
        anim.Play("PlayerDeath", 0, 0);
        GameOverText.color = new Color32(0, 0, 0, 255);
        TriAgainText.color = new Color32(0, 0, 0, 255);
        ContinueText.color = new Color32(0, 0, 0, 255);
        TitleCntText.color = new Color32(0, 0, 0, 255);
        TitleText.color = new Color32(0, 0, 0, 255);
        changeColor = 0;
        titleTImeCnt = 15;
        timeCnt = 0;
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

        if (nextFlag >= 3) { changeSceneFlag = true; }
    }
    private void Update()
    {
        //アナログスティックを参照する
        Horizontal = moveInputValue.x;

        UIColorChange(GameOverText, 0, "Red");
        UIColorChange(TriAgainText, 1, "Red");
        UIColorChange(ContinueText, 2, "Green");
        UIColorChange(TitleText, 2, "Red");
        TryAgainCnt();
        
        if(changeSceneFlag)
        {
            GoMainGameScene();
            GoTitleScene();
        }
        MoveUI();
    }

    private void MoveUI()
    {
        if (nextFlag < 3) 
        {
            playerUI.transform.position = new(-10000, -10000);
            return;
        }

        if (nextFlag == 3)
        {
            playerUI.transform.position =
                new(950, 134);
            nextFlag++;
            return;
        }

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
    private void UIColorChange(Text text,int cnt,string color)
    {
        switch (color) 
        {
            case "Red":
                {
                    if (nextFlag >= cnt + 1)
                    {
                        text.color = new Color32(255, 0, 0, 255);
                        return;
                    }

                    if (nextFlag >= cnt)
                    {
                        if (changeColor < 255) { changeColor++; }
                        else if (changeColor == 255) 
                        { 
                            nextFlag = cnt + 1;
                            changeColor = 0;
                        }

                        text.color = new Color32((byte)changeColor, 0, 0, 255);
                    }
                    break;
                }
            case "Green":
                {
                    if (nextFlag >= cnt + 1)
                    {
                        text.color = new Color32(0, 255, 0, 255);
                        return;
                    }

                    if (nextFlag >= cnt)
                    {
                        if (changeColor < 255) { changeColor++; }
                        else if (changeColor == 255) { nextFlag = cnt + 1; }

                        text.color = new Color32(0, (byte)changeColor, 0, 255);
                    }
                    break;
                }
        }
        
    }
    private void TryAgainCnt()
    {
        if (nextFlag <= 3) { return; }

        timeCnt += Time.deltaTime;
        TitleCntText.color = new Color32(255, 0, 0, 255);

        if (timeCnt >= 1.0f)
        {
            titleTImeCnt--;
            TitleCntText.text = "" + titleTImeCnt ;
            timeCnt = 0.0f;
        }

        if (titleTImeCnt == 0)
        {
            SceneManager.LoadScene("Title");
        }
    }
    private void GoTitleScene()
    {
        if (titleFlag)
        {
            SceneManager.LoadScene("Title");
        }
    }
    private void GoMainGameScene()
    {
        if (continueFlag)
        {
            SceneManager.LoadScene("MainGame");
        }
    }

}
