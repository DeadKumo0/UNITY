using UnityEngine;
using UnityEngine.Timeline;

public class DroneController : MonoBehaviour
{
    public float liftSpeed = 5f;
    public float moveSpeed = 5f;
    public float rotationSpeed = 50f;

    public Transform cameraTransform; 
    public float cameraTiltAngle = 10f; 
    public float cameraTiltSpeed = 5f;

    public float gravity = 9.8f; 
    private bool isPoweredOn = false;
    private bool isAirborne = false;
    private Vector3 initialCameraRotation;

    private Vector3 smoothVelocity;
    public float smoothTime = 0.1f; 

    private float cameraPitch = 0f;
    private Vector3 velocity;

    private Rigidbody rb;

    public GameObject markerPrefab; 
    public float markerLifetime = 5f; 






    void Start()
    {
        if (cameraTransform != null)
        {
            initialCameraRotation = cameraTransform.localEulerAngles;
        }

        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleInput();
        HandleCameraTiltAndFollow();
        HandleMouseLook();
        HandleMarkerPlacement();
    }

    private void FixedUpdate()
    {
        ApplyGravity();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            isPoweredOn = !isPoweredOn;
            Debug.Log(isPoweredOn ? "Drone powered on" : "Drone powered off");
        }

        if (!isPoweredOn)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            return;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            isAirborne = true;
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, liftSpeed, rb.linearVelocity.z);
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, -liftSpeed, rb.linearVelocity.z);
        }
        else if (isAirborne)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        }

        if (!isAirborne) return;

        float moveForward = Input.GetAxis("Vertical") * moveSpeed;
        float moveRight = Input.GetAxis("Horizontal") * moveSpeed;

        Vector3 movement = transform.forward * moveForward + transform.right * moveRight;
        rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);

        if (Input.GetKey(KeyCode.Z))
        {
            transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.C))
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }

    private void HandleCameraTiltAndFollow()
    {
        if (cameraTransform == null) return;

        float tiltX = Input.GetAxis("Vertical") * -cameraTiltAngle;
        float tiltZ = Input.GetAxis("Horizontal") * cameraTiltAngle;

        Vector3 targetLocalRotation = initialCameraRotation + new Vector3(tiltX, 0, tiltZ);
        Vector3 currentLocalRotation = cameraTransform.localEulerAngles;

        currentLocalRotation.x = Mathf.LerpAngle(currentLocalRotation.x, targetLocalRotation.x, cameraTiltSpeed * Time.deltaTime);
        currentLocalRotation.z = Mathf.LerpAngle(currentLocalRotation.z, targetLocalRotation.z, cameraTiltSpeed * Time.deltaTime);

        cameraTransform.localEulerAngles = currentLocalRotation;
    }

    private void HandleMouseLook()
    {
        if (cameraTransform == null) return;

        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

        transform.Rotate(Vector3.up, mouseX);

        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -45f, 45f);

        Vector3 cameraRotation = cameraTransform.localEulerAngles;
        cameraRotation.x = cameraPitch;
        cameraTransform.localEulerAngles = cameraRotation;
    }

    private void HandleMarkerPlacement()
    {
        if (Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(0))
        {
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                GameObject marker = Instantiate(markerPrefab, hitInfo.point, Quaternion.identity);
                Debug.Log("Marker placed at: " + hitInfo.point);

                Destroy(marker, 5f);
            }
        }
    }


    private void ApplyGravity()
    {
        if (!isPoweredOn)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y - gravity * Time.deltaTime, rb.linearVelocity.z);
        }
    }
}
