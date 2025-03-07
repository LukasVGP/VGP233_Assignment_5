using UnityEngine;

public class MugBehavior : MonoBehaviour
{
    [Header("Sliding Settings")]
    public float slidingSpeed = 20f;

    private Transform handPosition;

    void Start()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    void LateUpdate()
    {
        if (handPosition != null)
        {
            transform.position = handPosition.position;
            transform.rotation = handPosition.rotation;
        }
    }

    public void SetHeld(bool held)
    {
        if (held)
        {
            handPosition = GameObject.FindGameObjectWithTag("Player").transform.Find("HandPosition");
            transform.parent = handPosition;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
        else
        {
            handPosition = null;
            transform.parent = null;
        }
    }

    public void StartSliding(Vector3 direction, float speed)
    {
        SetHeld(false);
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.linearVelocity = direction * slidingSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall") || other.CompareTag("Customer"))
        {
            Destroy(gameObject);
        }
    }
}