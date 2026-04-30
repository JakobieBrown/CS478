using UnityEngine;
using TMPro;

public class RaceTimer : MonoBehaviour
{
    public TextMeshProUGUI timerLabel;
    private float elapsedTime;
    private bool isRunning;

    public void StartTimer()
    {
        elapsedTime = 0;
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    void Update()
    {
        if (!isRunning) return;
        elapsedTime += Time.deltaTime;
        int minutes = (int)(elapsedTime / 60);
        int seconds = (int)(elapsedTime % 60);
        int milliseconds = (int)((elapsedTime * 100) % 100);
        timerLabel.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }

    public float GetTime() => elapsedTime;
}