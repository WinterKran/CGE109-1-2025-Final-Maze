using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI messageText;    // ข้อความ Win / Lose
    public TextMeshProUGUI timerText;      // ตัวแสดงเวลา **NEW**

    private bool gameEnded = false;
    private Coroutine countdownRoutine;

    // เริ่มจับเวลา 15 วิ
    public void StartCountdown()
    {
        if (countdownRoutine == null)
            countdownRoutine = StartCoroutine(CountdownTimer());
    }

    private IEnumerator CountdownTimer()
    {
        float timeLeft = 20f;

        // ทำให้ timer โชว์ตอนเริ่ม
        timerText.gameObject.SetActive(true);

        while (timeLeft > 0 && !gameEnded)
        {
            timeLeft -= Time.deltaTime;

            // แสดงผลเวลาแบบทศนิยม 0 ตำแหน่ง (เช่น 14, 13, 12...)
            timerText.text = "Time: " + Mathf.Ceil(timeLeft).ToString();

            yield return null;
        }

        // ถ้าหมดเวลาแล้วยังไม่ Win → แพ้
        if (!gameEnded)
        {
            gameEnded = true;
            messageText.text = "YOU LOSE!";
            timerText.text = "Time: 0";
            
            StartCoroutine(EndGameReset());
        }
    }

    // ชน Block 3 → ชนะ
    public void PlayerWin()
    {
        if (gameEnded) return;

        gameEnded = true;
        messageText.text = "WIN!";
        StartCoroutine(EndGameExit());
    }

    private IEnumerator EndGameReset()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator EndGameExit()
    {
        yield return new WaitForSeconds(3f);
        Application.Quit();
    }
}
