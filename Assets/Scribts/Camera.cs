using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0f, 10f, -5f);
    public float rotationSmoothSpeed = 5f;

    private void LateUpdate()
    {
        if (target == null) return;

        // Position
        Vector3 desiredPosition = target.position + (target.rotation * offset);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        // Rotation
        Quaternion desiredRotation = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, rotationSmoothSpeed * Time.deltaTime);
    }
}