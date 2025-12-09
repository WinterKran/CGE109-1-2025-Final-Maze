using UnityEngine;

public class TriggerBlock : MonoBehaviour
{
    public enum BlockType { StartTimer, WinPoint }
    public BlockType blockType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager gm = FindObjectOfType<GameManager>();

            if (blockType == BlockType.StartTimer)
            {
                gm.StartCountdown();
            }
            else if (blockType == BlockType.WinPoint)
            {
                gm.PlayerWin();
            }
        }
    }
}
