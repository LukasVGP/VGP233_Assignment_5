using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class TableSettings
    {
        public Table table;
        public int maxCustomers = 3;
        public float initialSpawnInterval = 5f;
        [HideInInspector] public float currentSpawnInterval;
        public float customerMoveSpeed = 1f;
        [HideInInspector] public int customersServed = 0;
        [HideInInspector] public bool isActive = false;
    }

    [Header("Table Settings")]
    public List<TableSettings> tables = new List<TableSettings>();

    [Header("Difficulty Settings")]
    public float initialSpawnInterval = 5f;
    public float minSpawnInterval = 0.8f;                  // Fastest possible spawn rate
    public float intervalDecreasePerCustomer = 0.1f;       // How much to decrease interval per customer
    public int customersBeforeRandomTables = 7;            // After this many customers per table, use random tables

    [Header("Game Settings")]
    public float customerWaitTimeBeforeGameOver = 5f;
    public GameObject gameOverScreen;

    public static GameManager Instance { get; private set; }
    public PointSystem Points { get; private set; }

    private int totalCustomersServed = 0;
    private bool useRandomTables = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Points = GetComponent<PointSystem>();
        if (Points == null)
        {
            Points = gameObject.AddComponent<PointSystem>();
        }
    }

    private void Start()
    {
        // Initialize all tables
        foreach (var tableSetting in tables)
        {
            tableSetting.currentSpawnInterval = initialSpawnInterval;
            tableSetting.isActive = true;  // Start with all tables active but with high interval

            tableSetting.table.Initialize(
                tableSetting.maxCustomers,
                tableSetting.currentSpawnInterval,
                tableSetting.customerMoveSpeed,
                customerWaitTimeBeforeGameOver
            );
        }

        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(false);
        }
    }

    public void OnCustomerServed(Table table, float completionPercentage)
    {
        totalCustomersServed++;

        // Find which table setting this belongs to and update its customers served count
        TableSettings servedTable = null;
        foreach (var tableSetting in tables)
        {
            if (tableSetting.table == table)
            {
                tableSetting.customersServed++;
                servedTable = tableSetting;
                break;
            }
        }

        // Check if we should switch to random table selection
        if (!useRandomTables)
        {
            bool allTablesReachedThreshold = true;
            foreach (var tableSetting in tables)
            {
                if (tableSetting.customersServed < customersBeforeRandomTables)
                {
                    allTablesReachedThreshold = false;
                    break;
                }
            }

            if (allTablesReachedThreshold)
            {
                useRandomTables = true;
                Debug.Log("Switching to random table spawning!");
            }
        }

        // Increase difficulty by decreasing spawn intervals for all tables
        foreach (var tableSetting in tables)
        {
            tableSetting.currentSpawnInterval = Mathf.Max(
                minSpawnInterval,
                tableSetting.currentSpawnInterval - intervalDecreasePerCustomer
            );
            tableSetting.table.UpdateSpawnInterval(tableSetting.currentSpawnInterval);

            Debug.Log($"Table {tables.IndexOf(tableSetting) + 1} spawn interval: {tableSetting.currentSpawnInterval}s");
        }

        // Award points based on completion percentage
        Points.AddPointsForCustomer(completionPercentage);
    }

    public bool ShouldSpawnAtTable(Table table)
    {
        if (useRandomTables)
        {
            // When in random mode, GameManager decides which table to spawn customers at
            return false; // Let the RandomSpawnUpdate method handle spawning
        }

        // Otherwise, each table handles its own spawning
        return true;
    }

    void Update()
    {
        if (useRandomTables)
        {
            RandomSpawnUpdate();
        }
    }

    private float randomSpawnTimer = 0f;
    private float currentRandomInterval = 2.5f; // Start value for random spawning

    void RandomSpawnUpdate()
    {
        // Gradually decrease the random spawn interval
        currentRandomInterval = Mathf.Max(
            minSpawnInterval * 0.8f, // Allow random spawning to be slightly faster
            initialSpawnInterval - (totalCustomersServed * intervalDecreasePerCustomer * 0.5f)
        );

        randomSpawnTimer += Time.deltaTime;
        if (randomSpawnTimer >= currentRandomInterval)
        {
            SpawnAtRandomTable();
            randomSpawnTimer = 0f;
        }
    }

    void SpawnAtRandomTable()
    {
        // Create a list of tables that can accept more customers
        List<TableSettings> availableTables = new List<TableSettings>();
        foreach (var table in tables)
        {
            if (table.table.CanAcceptCustomer())
            {
                availableTables.Add(table);
            }
        }

        if (availableTables.Count > 0)
        {
            // Choose a random table from available tables
            int randomIndex = Random.Range(0, availableTables.Count);
            availableTables[randomIndex].table.ForceSpawnCustomer();

            Debug.Log($"Randomly spawned customer at table {tables.IndexOf(availableTables[randomIndex]) + 1}");
        }
    }

    public void TriggerGameOver()
    {
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
        }
        Debug.Log($"Game Over! Final score: {Points.GetTotalPoints()} with {totalCustomersServed} customers served.");

        // Disable all table spawning
        foreach (var tableSetting in tables)
        {
            tableSetting.table.SetActive(false);
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}