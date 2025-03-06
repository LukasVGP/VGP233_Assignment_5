using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 180f;

    [Header("Interaction Settings")]
    public GameObject mugPrefab;
    public Transform handPosition;
    public float interactionDistance = 2f;
    public float slidingSpeed = 10f;

    private GameObject heldMug;
    private Rigidbody rb;
    private bool hasMug = false;
    private Vector3 currentDirection = Vector3.forward;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
        Debug.Log("PlayerController initialized");
    }

    void Update()
    {
        HandleRotation();
        HandleMovement();
        HandleInteractions();
    }

    void HandleRotation()
    {
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
            currentDirection = transform.forward;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            currentDirection = transform.forward;
        }
    }

    void HandleMovement()
    {
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 movement = transform.forward * verticalInput * moveSpeed;
        rb.MovePosition(rb.position + movement * Time.deltaTime);
    }

    void HandleInteractions()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E key pressed");
            TryGetMug();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space key pressed");
            TryServeMug();
        }

        if (hasMug && heldMug != null)
        {
            heldMug.transform.position = handPosition.position;
            heldMug.transform.rotation = handPosition.rotation;
        }
    }

    void TryGetMug()
    {
        Debug.Log("TryGetMug called");
        Debug.Log($"Player position: {transform.position}");
        Debug.Log($"Interaction distance: {interactionDistance}");

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionDistance);
        Debug.Log($"Found {hitColliders.Length} colliders in range");

        foreach (var hitCollider in hitColliders)
        {
            Debug.Log($"Checking collider: {hitCollider.gameObject.name} with tag: {hitCollider.tag}");
            if (hitCollider.CompareTag("Barrel") && !hasMug)
            {
                Debug.Log("Found barrel, spawning mug");
                heldMug = Instantiate(mugPrefab, handPosition.position, handPosition.rotation);
                Debug.Log($"Mug spawned at position: {heldMug.transform.position}");

                MugBehavior mugBehavior = heldMug.GetComponent<MugBehavior>();
                mugBehavior.SetHeld(true);
                heldMug.transform.parent = handPosition;
                Physics.IgnoreCollision(heldMug.GetComponent<Collider>(), GetComponent<Collider>(), true);
                hasMug = true;
                Debug.Log("Mug successfully attached to player");
                break;
            }
        }
    }

    void TryServeMug()
    {
        if (!hasMug || heldMug == null)
        {
            Debug.Log("No mug to serve");
            return;
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionDistance);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Table"))
            {
                Debug.Log("Found table, starting slide");
                StartSliding();
                break;
            }
        }
    }

    void StartSliding()
    {
        Debug.Log("StartSliding called");
        heldMug.transform.parent = null;
        Rigidbody mugRb = heldMug.GetComponent<Rigidbody>();
        mugRb.isKinematic = false;

        float tableHeight = GameObject.FindGameObjectWithTag("Table").transform.position.y + 0.5f;
        heldMug.transform.position = new Vector3(heldMug.transform.position.x, tableHeight, heldMug.transform.position.z);

        mugRb.linearVelocity = currentDirection * slidingSpeed;
        mugRb.useGravity = false;

        hasMug = false;
        heldMug = null;
        Debug.Log("Mug sliding initiated");
    }

    public Vector3 GetCurrentDirection()
    {
        return currentDirection;
    }
}