using UnityEngine;

public class Customer : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float repelSpeed = 1.5f;

    private Table parentTable;
    private bool isRepelled = false;

    public void Initialize(Table table)
    {
        parentTable = table;
    }

    void Update()
    {
        float currentSpeed = isRepelled ? moveSpeed * repelSpeed : moveSpeed;
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);

        if (isRepelled && Vector3.Distance(transform.position, parentTable.customerSpawnPoint.position) < 0.1f)
        {
            parentTable.CustomerDestroyed();
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Mug"))
        {
            transform.position = parentTable.customerSpawnPoint.position;
            transform.rotation = parentTable.customerSpawnPoint.rotation;
            isRepelled = true;
            Destroy(other.gameObject);
        }
    }
}