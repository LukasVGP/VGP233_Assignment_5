using UnityEngine;

public class Table : MonoBehaviour
{
    [Header("Table Settings")]
    public Transform slideStartPoint;
    public Transform slideEndPoint;

    [Header("Customer Settings")]
    public GameObject customerPrefab;
    public Transform customerSpawnPoint;
    public Transform customerEndPoint;
    public int maxCustomers = 3;
    public float spawnInterval = 5f;
    public float customerMoveSpeed = 1f;

    private float spawnTimer;
    private int currentCustomers = 0;

    void Start()
    {
        spawnTimer = spawnInterval; // Start with immediate spawn
    }

    void Update()
    {
        HandleCustomerSpawning();
    }

    void HandleCustomerSpawning()
    {
        if (currentCustomers >= maxCustomers) return;

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            SpawnCustomer();
            spawnTimer = 0f;
        }
    }

    void SpawnCustomer()
    {
        GameObject customer = Instantiate(customerPrefab, customerSpawnPoint.position, customerSpawnPoint.rotation);
        Customer customerScript = customer.GetComponent<Customer>();
        customerScript.Initialize(this);
        currentCustomers++;
    }

    public void CustomerDestroyed()
    {
        currentCustomers--;
    }

    public Vector3 GetSlideDirection()
    {
        return (slideEndPoint.position - slideStartPoint.position).normalized;
    }

    public Vector3 GetSlideStartPosition()
    {
        return slideStartPoint.position;
    }
}