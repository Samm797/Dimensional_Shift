using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFixer : MonoBehaviour
{
    public Transform player;
    private float orthoSize = 8f;

    private Vector3 velocity = Vector3.zero;
    
    
    void LateUpdate()
    {
        Vector3 targetPosition = player.position;

        Camera.main.orthographicSize = orthoSize;

        Vector3 desiredPosition = new Vector3(targetPosition.x, targetPosition.y, targetPosition.z - 10f);

        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, 0.1f);

        transform.position = smoothedPosition;
    }
}
