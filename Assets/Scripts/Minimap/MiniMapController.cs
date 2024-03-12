using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapController : MonoBehaviour
{
    [SerializeField] Transform playerTransfrom;
    [SerializeField] Vector3 cameraStartPos;

    private void Start()
    {
        cameraStartPos = transform.position;
    }

    private void LateUpdate()
    {
        transform.position = new Vector3(playerTransfrom.position.x, cameraStartPos.y, playerTransfrom.position.z);
    }
}
