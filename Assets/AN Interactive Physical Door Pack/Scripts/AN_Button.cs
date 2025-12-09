using UnityEngine;

public class AN_Button : MonoBehaviour
{
    [Header("Button / Lever / Valve Modes")]
    public bool isValve = false;
    public bool isLever = false;

    [Header("Lock / Door Control")]
    public bool Locked = false;
    public AN_DoorScript DoorObject;

    [Header("Ramp / Elevator Objects")]
    public Transform RampObject;

    [Header("Valve Settings")]
    public float ValveSpeed = 10f;
    public bool xRotation = true;      // Rotates on X
    public bool yPosition = false;     // Moves in Y
    public float max = 90f;
    public float min = 0f;
    public float speed = 5f;

    private float current = 0f;
    private float startYPosition;
    private Quaternion startQuat;
    private Quaternion rampStartQuat;
    private bool valveActionFlag = true;

    [Header("Door States")]
    public bool CanOpen = true;
    public bool CanClose = true;
    public bool isOpened = false;

    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();

        if (RampObject != null)
        {
            startYPosition = RampObject.position.y;
            rampStartQuat = RampObject.rotation;
        }

        startQuat = transform.rotation;
    }

    void Update()
    {
        if (Locked) return;

        if (isValve)
        {
            HandleValve();
        }
        else
        {
            HandleButtonOrLever();
        }
    }

    // --------------------------
    // BUTTON / LEVER LOGIC
    // --------------------------
    void HandleButtonOrLever()
    {
        if (!Input.GetKeyDown(KeyCode.E)) return;
        if (!NearView()) return;
        if (DoorObject == null) return;
        if (!DoorObject.Remote) return;

        DoorObject.Action();

        // Lever Animation
        if (isLever && anim != null)
        {
            anim.SetBool("LeverUp", DoorObject.isOpened);
        }
        else if (anim != null) // Button Animation
        {
            anim.SetTrigger("ButtonPress");
        }
    }

    // --------------------------
    // VALVE LOGIC
    // --------------------------
    void HandleValve()
    {
        if (RampObject == null) return;

        bool holdingE = Input.GetKey(KeyCode.E) && NearView();

        if (holdingE)
        {
            if (valveActionFlag)
            {
                if (!isOpened && CanOpen && current < max)
                    current += speed * Time.deltaTime;

                if (isOpened && CanClose && current > min)
                    current -= speed * Time.deltaTime;

                if (current >= max)
                {
                    current = max;
                    isOpened = true;
                    valveActionFlag = false;
                }
                else if (current <= min)
                {
                    current = min;
                    isOpened = false;
                    valveActionFlag = false;
                }
            }
        }
        else
        {
            // Auto return motion
            if (!isOpened && current > min)
                current -= speed * Time.deltaTime;

            if (isOpened && current < max)
                current += speed * Time.deltaTime;

            valveActionFlag = true;
        }

        UpdateValveObject();
    }

    // --------------------------
    // APPLY ROTATION / POSITION
    // --------------------------
    void UpdateValveObject()
    {
        // Rotate the valve handle itself
        transform.rotation = startQuat * Quaternion.Euler(0f, 0f, current * ValveSpeed);

        if (RampObject == null) return;

        if (xRotation)
        {
            RampObject.rotation = rampStartQuat * Quaternion.Euler(current, 0f, 0f);
        }
        else if (yPosition)
        {
            RampObject.position = new Vector3(RampObject.position.x,
                                              startYPosition + current,
                                              RampObject.position.z);
        }
    }

    // --------------------------
    // PLAYER INTERACTION RANGE
    // --------------------------
    bool NearView()
    {
        Camera cam = Camera.main;
        if (cam == null) return false;

        Vector3 toObject = transform.position - cam.transform.position;
        float distance = toObject.magnitude;

        if (distance > 2f) return false;

        float angle = Vector3.Angle(cam.transform.forward, toObject);
        return angle < 45f;
    }
}
