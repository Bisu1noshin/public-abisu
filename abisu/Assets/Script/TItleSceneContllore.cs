using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleSceneContllore : MonoBehaviour
{
    [SerializeField] private Text Action;
    [SerializeField] private Text Monstar;
    [SerializeField] private Text pushA;

    private PlayerAction inputActions;
    private float preColorR = 15;
    private float preColorG = 0;
    private float preColorB = 38;

    private int nextFlag;
    private void Start()
    {
        nextFlag = 0;
        Action.transform.localPosition = new Vector3(342, -690, 0);
        Monstar.transform.localPosition = new Vector3(-295, 620, 0);
        pushA.color = new Color32(15, 0, 38, 255);
    }

    private void Awake()
    {
        // Actionスクリプトのインスタンス生成
        inputActions = new PlayerAction();

        // Actionイベント登録
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
    private void OnAttack(InputAction.CallbackContext context)
    {
        nextFlag++;
    }
    private void Update()
    {
        MoveActionUI();
        MoveMonstarUI();
        MovePushAUI(); 
        GoMainGame();
    }

    private void MoveActionUI()
    {
        if (nextFlag >= 1) 
        {
            Action.transform.localPosition = new Vector3(342, -170, 0);
            return;
        }

        if (Action.transform.localPosition.y >= -170) { nextFlag++; return; }

        Action.transform.localPosition = new Vector3
            (Action.transform.localPosition.x, Action.transform.localPosition.y + 1, Action.transform.localPosition.z);
    }
    private void MoveMonstarUI()
    {
        if (nextFlag >= 1) 
        {
            Monstar.transform.localPosition = new Vector3(-295, 100, 0);
            return;
        }

        if (Monstar.transform.localPosition.y <= 100) { return; }

        Monstar.transform.localPosition = new Vector3
            (Monstar.transform.localPosition.x, Monstar.transform.localPosition.y - 1, Monstar.transform.localPosition.z);
    }
    private void MovePushAUI() 
    {
        if (nextFlag < 1) { return; }
        if (nextFlag >= 2) { pushA.color = new Color32(104, 93, 8, 255);return; }    

        if (preColorR >= 104) { nextFlag++; return; }

        float R = (104 - 15) / 60;
        float G = (93 - 0) / 60;
        float B = (8 - 38) / 60;

        preColorR += R;
        preColorG += G;
        preColorB += B;

        pushA.color = new Color32((byte)preColorR, (byte)preColorG, (byte)preColorB, 255);
    }
    private void GoMainGame()
    {
        if (nextFlag < 3) { return; }

        SceneManager.LoadScene("MainGame");
    }
}
