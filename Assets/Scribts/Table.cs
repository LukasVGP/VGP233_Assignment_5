using System.Collections.Generic;
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

    private float spawnTimer;
    private int currentCustomers = 0;

    private int maxCustomers;
    private float spawnInterval;
    private float customerMoveSpeed;
    private float customerWaitTime;

    private bool isActive = false;
    private Queue<Customer> customerQueue = new Queue<Customer>();

    public void Initialize(int maxCustomers, float spawnInterval, float customerMoveSpeed, float waitTime)
    {
        this.maxCustomers = maxCustomers;
        this.spawnInterval = spawnInterval;
        this.customerMoveSpeed = customerMoveSpeed;
        this.customerWaitTime = waitTime;

        spawnTimer = 0f;
        isActive = true;
    }

    public void SetActive(bool active)
    {
        isActive = active;
    }

    public void UpdateSpawnInterval(float newInterval)
    {
        spawnInterval = newInterval;
    }

    public bool CanAcceptCustomer()
    {
        return isActive && currentCustomers < maxCustomers;
    }

    public void ForceSpawnCustomer()
    {
        if (CanAcceptCustomer())
        {
            SpawnCustomer();
        }
    }

    void Update()
    {
        if (isActive && GameManager.Instance.ShouldSpawnAtTable(this))
        {
            HandleCustomerSpawning();
        }
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
        GameObject customerObj = Instantiate(customerPrefab, customerSpawnPoint.position, customerSpawnPoint.rotation);
        Customer customerScript = customerObj.GetComponent<Customer>();
        customerScript.Initialize(this, customerMoveSpeed, customerWaitTime);
        customerQueue.Enqueue(customerScript);
        currentCustomers++;
    }

    public void CustomerDestroyed(Customer customer)
    {
        currentCustomers--;
        customerQueue.Dequeue();
    }

    public Vector3 GetSlideDirection()
    {
        return (slideEndPoint.position - slideStartPoint.position).normalized;
    }

    public Vector3 GetSlideStartPosition()
    {
        return slideStartPoint.position;
    }

    public void CustomerServed(Customer customer, float distanceTraveled)
    {
        // Calculate percentage of path completed (0 = at spawn, 1 = at end)
        float totalDistance = Vector3.Distance(customerSpawnPoint.position, customerEndPoint.position);
        float completionPercentage = distanceTraveled / totalDistance;

        GameManager.Instance.OnCustomerServed(this, completionPercentage);
    }

    public Customer GetNextCustomerInLine(Customer currentCustomer)
    {
        // Find the customer ahead in the queue
        Customer[] customers = customerQueue.ToArray();
        for (int i = 0; i < customers.Length - 1; i++)
        {
            if (customers[i] == currentCustomer && i > 0)
            {
                return customers[i - 1];
            }
        }
        return null;
    }

    public void TriggerGameOver()
    {
        GameManager.Instance.TriggerGameOver();
    }
}