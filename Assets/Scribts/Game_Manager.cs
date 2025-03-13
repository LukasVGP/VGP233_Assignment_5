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
        public float spawnInterval = 5f;
        public float customerMoveSpeed = 1f;
        public int customersToServeBeforeNextTable = 5;
        [HideInInspector] public int customersServed = 0;
        [HideInInspector] public bool isActive = false;
    }

    [Header("Table Settings")]
    public List<TableSettings> tables = new List<TableSettings>();

    [Header("Game Settings")]
    public float customerWaitTimeBeforeGameOver = 5f;
    public GameObject gameOverScreen;

    public static GameManager Instance { get; private set; }
    public PointSystem Points { get; private set; }

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

        // Initially, only the first table is active
        if (tables.Count > 0)
        {
            tables[0].isActive = true;
        }
    }

    private void Start()
    {
        // Initialize all tables
        foreach (var tableSetting in tables)
        {
            tableSetting.table.Initialize(
                tableSetting.maxCustomers,
                tableSetting.spawnInterval,
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
        // Find which table setting this belongs to
        foreach (var tableSetting in tables)
        {
            if (tableSetting.table == table)
            {
                tableSetting.customersServed++;

                // Check if we should activate the next table
                int tableIndex = tables.IndexOf(tableSetting);
                if (tableIndex < tables.Count - 1 &&
                    tableSetting.customersServed >= tableSetting.customersToServeBeforeNextTable)
                {
                    tables[tableIndex + 1].isActive = true;
                }

                break;
            }
        }

        // Award points based on completion percentage
        Points.AddPointsForCustomer(completionPercentage);
    }

    public bool IsTableActive(Table table)
    {
        foreach (var tableSetting in tables)
        {
            if (tableSetting.table == table)
            {
                return tableSetting.isActive;
            }
        }
        return false;
    }

    public void TriggerGameOver()
    {
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
        }
        Debug.Log("Game Over! Final score: " + Points.GetTotalPoints());

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