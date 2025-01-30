using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    private GameObject player;
    private Transform cameraTrans;

    private void Start()
    {
        this.player = GameObject.Find("Player");
        cameraTrans = this.transform;
    }

    private void Update()
    {
        if (this.player != null)
        {
            Vector3 pos = GetPositon(this.player);
            cameraTrans.position = new Vector3
                (pos.x, pos.y, this.transform.position.z);
        }

        MoveLimitation();
    }

    private Vector3 GetPositon(GameObject obj)
    {
        return obj.transform.position;
    }

    private void MoveLimitation() 
    {
        float posX = transform.position.x;
        float posY = transform.position.y;

        //
        if (transform.position.y >= 5.0f) { posY = 5.0f; }
        if (transform.position.y <= -2.5f) { posY = -2.5f; }
        if (transform.position.x >= 41.0f) { posX = 41.0f; }
        if (transform.position.x <= -28.7f) { posX = -28.7f; }

        transform.position = new Vector3(posX, posY, transform.position.z);
    }
}
