using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    public Transform player; 
    public Vector3 offset; 
    public float smoothSpeed = 0.125f; 
    void FixedUpdate()
    {
        if( player!= null)
        {
            Vector3 desiredPosition = player.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}
