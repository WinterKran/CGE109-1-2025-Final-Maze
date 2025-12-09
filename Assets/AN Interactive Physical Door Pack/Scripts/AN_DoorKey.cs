using UnityEngine;

public class AN_DoorKey : MonoBehaviour
{
    [Header("Key Settings")]
    [Tooltip("True = Red Key, False = Blue Key")]
    public bool isRedKey = true;

    private AN_HeroInteractive hero;

    [Header("Interaction Settings")]
    public float interactDistance = 2f;
    public float viewAngle = 45f;

    void Start()
    {
        hero = FindObjectOfType<AN_HeroInteractive>();

        if (hero == null)
            Debug.LogWarning("AN_DoorKey: No AN_HeroInteractive found in scene!");
    }

    void Update()
    {
        if (hero == null) return;

        if (Input.GetKeyDown(KeyCode.E) && NearView())
        {
            if (isRedKey) hero.RedKey = true;
            else hero.BlueKey = true;

            Destroy(gameObject);
        }
    }

    bool NearView()
    {
        Camera cam = Camera.main;
        if (cam == null) return false;

        Vector3 toKey = transform.position - cam.transform.position;

        float distance = toKey.magnitude;
        if (distance > interactDistance) return false;

        float angle = Vector3.Angle(cam.transform.forward, toKey);
        if (angle > viewAngle) return false;

        // Optional: Check line of sight
        // if (Physics.Raycast(cam.transform.position, toKey.normalized, distance, obstacleMask)) return false;

        return true;
    }
}
