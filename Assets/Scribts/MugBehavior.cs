using UnityEngine;

public class MugBehavior : MonoBehaviour
{
    private Transform handPosition;
    private Collider mugCollider;

    void Start()
    {
        mugCollider = GetComponent<Collider>();
        mugCollider.isTrigger = true;
        DisableAllPhysics();
    }

    void DisableAllPhysics()
    {
        Destroy(GetComponent<Rigidbody>());
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
        gameObject.AddComponent<Rigidbody>();
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.linearVelocity = direction * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall") || other.CompareTag("Customer"))
        {
            Destroy(gameObject);
        }
    }
}