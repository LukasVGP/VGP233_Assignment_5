using UnityEngine;

public class Customer : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float destroyDelay = 2f;

    void Update()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Mug"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject, destroyDelay);
        }
    }
}