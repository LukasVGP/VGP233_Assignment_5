using UnityEngine;

public class Customer : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float repelSpeed = 1.5f;

    [Header("Game Over Warning")]
    public Material normalMaterial;
    public Material warningMaterial;
    public float blinkInterval = 0.5f;

    private Table parentTable;
    private bool isRepelled = false;
    private bool isAtEndPoint = false;
    private float waitTimer = 0f;
    private float customerWaitTime;
    private bool isBlinking = false;
    private float blinkTimer = 0f;
    private Renderer customerRenderer;
    private float distanceTraveled = 0f;
    private Vector3 lastPosition;
    private Customer customerAhead;
    private float minDistanceFromCustomer = 1.5f; // Minimum distance to keep from customer ahead

    void Start()
    {
        customerRenderer = GetComponentInChildren<Renderer>();
        lastPosition = transform.position;
    }

    public void Initialize(Table table, float speed, float waitTime)
    {
        parentTable = table;
        moveSpeed = speed;
        customerWaitTime = waitTime;

        // Check for customer ahead
        customerAhead = parentTable.GetNextCustomerInLine(this);
    }

    void Update()
    {
        if (isRepelled)
        {
            HandleRepelMovement();
        }
        else if (isAtEndPoint)
        {
            HandleEndPointWaiting();
        }
        else
        {
            HandleForwardMovement();
        }

        // Track distance traveled for scoring
        distanceTraveled += Vector3.Distance(transform.position, lastPosition);
        lastPosition = transform.position;
    }

    void HandleForwardMovement()
    {
        // Check if we can move (ensure we don't collide with customer ahead)
        bool canMove = true;
        if (customerAhead != null)
        {
            float distanceToCustomerAhead = Vector3.Distance(transform.position, customerAhead.transform.position);
            if (distanceToCustomerAhead < minDistanceFromCustomer)
            {
                canMove = false;
            }
        }

        // If we can move, move toward end point
        if (canMove)
        {
            // Move toward end point
            Vector3 targetPosition = parentTable.customerEndPoint.position;
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            // Check if we've reached the end point
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                isAtEndPoint = true;
                waitTimer = 0f;
            }
        }
    }

    void HandleEndPointWaiting()
    {
        waitTimer += Time.deltaTime;

        // Start blinking after half the wait time
        if (waitTimer >= customerWaitTime * 0.5f && !isBlinking)
        {
            isBlinking = true;
        }

        // Handle blinking
        if (isBlinking)
        {
            blinkTimer += Time.deltaTime;
            if (blinkTimer >= blinkInterval)
            {
                if (customerRenderer.material == normalMaterial)
                {
                    customerRenderer.material = warningMaterial;
                }
                else
                {
                    customerRenderer.material = normalMaterial;
                }
                blinkTimer = 0f;
            }
        }

        // Trigger game over if wait time exceeded
        if (waitTimer >= customerWaitTime)
        {
            parentTable.TriggerGameOver();
        }
    }

    void HandleRepelMovement()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            parentTable.customerSpawnPoint.position,
            moveSpeed * repelSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, parentTable.customerSpawnPoint.position) < 0.1f)
        {
            parentTable.CustomerDestroyed(this);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Mug"))
        {
            // Calculate score based on progress from spawn to end
            parentTable.CustomerServed(this, distanceTraveled);

            // Start moving back
            transform.rotation = Quaternion.LookRotation(
                parentTable.customerSpawnPoint.position - transform.position
            );
            isRepelled = true;
            isAtEndPoint = false;

            Destroy(other.gameObject);
        }
    }
}