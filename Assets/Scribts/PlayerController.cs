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
            TryGetMug();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
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
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionDistance);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Barrel") && !hasMug)
            {
                heldMug = Instantiate(mugPrefab, handPosition.position, handPosition.rotation);
                MugBehavior mugBehavior = heldMug.GetComponent<MugBehavior>();
                mugBehavior.SetHeld(true);  // Now passing the required boolean parameter
                heldMug.transform.parent = handPosition;
                Physics.IgnoreCollision(heldMug.GetComponent<Collider>(), GetComponent<Collider>(), true);
                hasMug = true;
                break;
            }
        }
    }

    void TryServeMug()
    {
        if (!hasMug || heldMug == null) return;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionDistance);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Table"))
            {
                StartSliding();
                break;
            }
        }
    }

    void StartSliding()
    {
        heldMug.transform.parent = null;
        Rigidbody mugRb = heldMug.GetComponent<Rigidbody>();
        mugRb.isKinematic = false;

        float tableHeight = GameObject.FindGameObjectWithTag("Table").transform.position.y + 0.5f;
        heldMug.transform.position = new Vector3(heldMug.transform.position.x, tableHeight, heldMug.transform.position.z);

        mugRb.linearVelocity = currentDirection * slidingSpeed;
        mugRb.useGravity = false;

        hasMug = false;
        heldMug = null;
    }

    public Vector3 GetCurrentDirection()
    {
        return currentDirection;
    }
}