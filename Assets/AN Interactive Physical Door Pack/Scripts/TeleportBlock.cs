using UnityEngine;

public class TeleportBlock : MonoBehaviour
{
    [Tooltip("ตำแหน่งที่ต้องการให้ Player ไป (Block 2)")]
    public Transform targetPoint;

    private void OnTriggerEnter(Collider other)
    {
        // ตรวจสอบว่าเป็น Player หรือไม่ (Player ต้องมี Collider)
        if (other.CompareTag("Player"))
        {
            other.transform.position = targetPoint.position;
            Debug.Log("Teleported to Block 2");
        }
    }
}
