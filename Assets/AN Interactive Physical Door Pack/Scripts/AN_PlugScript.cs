using UnityEngine;

public class AN_PlugScript : MonoBehaviour
{
    [Header("Plug Settings")]
    public bool OneTime = false;

    [Tooltip("Point where the player holds the plug")]
    public Transform HeroHandsPosition;

    [Tooltip("Trigger socket collider")]
    public Collider Socket;

    [Tooltip("Controlled Door")]
    public AN_DoorScript DoorObject;

    Rigidbody rb;

    bool isHeld = false;
    bool isConnected = false;
    bool canInteract = true;

    float followSmooth = 15f;
    float rotateSmooth = 12f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (HeroHandsPosition == null)
            Debug.LogError("Assign HeroHandsPosition!");

        if (Socket == null)
            Debug.LogError("Assign Socket Collider!");

        if (DoorObject == null)
            Debug.LogError("Assign DoorObject!");
    }

    void Update()
    {
        if (canInteract)
            HandleInteraction();

        if (isConnected)
        {
            LockToSocket();
        }
    }

    // ----------------------------
    // Interaction Logic (Press E)
    // ----------------------------
    void HandleInteraction()
    {
        // Pick up plug
        if (!isHeld && NearView() && Input.GetKeyDown(KeyCode.E))
        {
            isConnected = false;
            isHeld = true;
            rb.useGravity = false;
        }

        // Drop plug manually
        if (isHeld && Input.GetKeyDown(KeyCode.E))
        {
            isHeld = false;
            rb.useGravity = true;
        }

        // Follow player's hands smoothly
        if (isHeld)
            FollowHands();
    }

    // ----------------------------
    // Smooth following to hand
    // ----------------------------
    void FollowHands()
    {
        // Smooth Lerp position
        Vector3 targetPos = HeroHandsPosition.position;
        rb.MovePosition(Vector3.Lerp(transform.position, targetPos, followSmooth * Time.deltaTime));

        // Smooth rotate
        Quaternion targetRot = HeroHandsPosition.rotation;
        rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRot, rotateSmooth * Time.deltaTime));
    }

    // ----------------------------
    // Lock plug into socket
    // ----------------------------
    void LockToSocket()
    {
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Snap into socket
        transform.position = Socket.transform.position;
        transform.rotation = Socket.transform.rotation;

        // Door opens when plugged
        DoorObject.isOpened = true;
    }

    // ----------------------------
    // Detect socket connection
    // ----------------------------
    private void OnTriggerEnter(Collider other)
    {
        if (other == Socket && isHeld)
        {
            isHeld = false;
            isConnected = true;

            if (OneTime)
                canInteract = false;

            // Add torque to door
            if (DoorObject.rbDoor != null)
                DoorObject.rbDoor.AddRelativeTorque(0, 0, 20f);
        }
    }

    // ----------------------------
    // Player can interact if near
    // ----------------------------
    bool NearView()
    {
        if (Camera.main == null) return false;

        Vector3 dir = transform.position - Camera.main.transform.position;
        float dist = dir.magnitude;

        if (dist > 3f) return false;

        float angle = Vector3.Angle(Camera.main.transform.forward, dir);
        return angle < 35f;
    }
}
