using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatShotController : MonoBehaviour
{
    private Collider2D coll;
    private Vector2 AttackPos;
    private float destroyCnt;
    private float timeCnt;
    private int Atp;

    private void Start()
    {
        coll = GetComponent<Collider2D>();
        transform.eulerAngles = new Vector3(0, 0, 0);
        destroyCnt = 1;
        timeCnt = 0;
    }

    
    
    private void Update()
    {
        timeCnt += Time.deltaTime;

        if (timeCnt >= destroyCnt) { Destroy(gameObject); }

        transform.position = new
            (transform.position.x + AttackPos.x * Time.deltaTime, transform.position.y + AttackPos.y * Time.deltaTime);
        LotationAngle();
    }

    private void LotationAngle()
    {
        transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + 5);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")//コライダーのタグがプレイヤーだった時
        {
            //プレイヤーの状態を取得
            PlayerContllor_1.State state =
                other.GetComponentInParent<PlayerContllor_1>().GetState();

            //プレイヤーの状態がNormalの時、ダメージを与える
            if (state == PlayerContllor_1.State.Normal)
            {
                Debug.Log("Hit");
                other.GetComponentInParent<PlayerContllor_1>().PlayerHit1();
                other.GetComponentInParent<PlayerContllor_1>().SubPlayerHP(Atp);
            }

            //プレイヤーの状態がDashの時、回避成功関数を呼び出す
            if (state == PlayerContllor_1.State.Dash)
            {
                Debug.Log("Brink");
                other.gameObject.GetComponentInParent<PlayerContllor_1>().SuccessDash();
            }
        }

        if (other.tag == "Ground")
        {
            Destroy(gameObject);
        }
    }
    public void SetVector(Vector2 attackpos)
    {
        AttackPos = attackpos;
    }
    public void SetDestroyCnt(float maxcnt)
    {
        destroyCnt = maxcnt;
    }
    public void SetAttack(int atp)
    {
        Atp = atp;
    }
}
