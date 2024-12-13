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
    }
    private Vector3 GetPositon(GameObject obj)
    {
        return obj.transform.position;
    }
}
