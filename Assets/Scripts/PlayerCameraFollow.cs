using UnityEngine;

public class PlayerCameraFollow : MonoBehaviour
{
    public Transform cameraTransform; 
    public Vector3 offset = new Vector3(0f,0f,-10f); 
    public float followSpeed = 0.0125f;

    void Start()
    {
        if (cameraTransform == null)
        {
            Debug.LogError("Camera reference is missing! Please assign the camera in the Inspector.");
        }
    }

    void Update()
    {
        if (cameraTransform != null)
        {
            Vector3 targetPosition = transform.position + offset;
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPosition, followSpeed * Time.deltaTime);
            cameraTransform.LookAt(transform);
        }
    }
}
