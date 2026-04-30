using UnityEngine;
using TMPro;
using System.Collections;

public class RaceCountdown : MonoBehaviour
{
    public TMP_Text countdownText;
    public AudioClip countdownBeep;
    public AudioClip goSound;
    public AudioClip backgroundMusic;
    private AudioSource audioSource;
    private bool hasStarted = false;
    public RaceTimer raceTimer;
    public static bool RaceStarted { get; private set; } = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }


    public void BeginCountdown()
    {
        Debug.Log("BeginCountdown called, hasStarted: " + hasStarted);
        if (hasStarted) return;
        hasStarted = true;
        RaceStarted = false;
        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()

    {
        countdownText.text = "3";
        audioSource.PlayOneShot(countdownBeep);
        yield return new WaitForSeconds(1f);

        countdownText.text = "2";
        audioSource.PlayOneShot(countdownBeep);
        yield return new WaitForSeconds(1f);
        
        countdownText.text = "1";
        audioSource.PlayOneShot(countdownBeep);
        yield return new WaitForSeconds(1f);

        countdownText.text = "GO!";
        audioSource.PlayOneShot(goSound);
        yield return new WaitForSeconds(1f);

        RaceStarted = true;

        countdownText.gameObject.SetActive(false);

        audioSource.clip = backgroundMusic;
        audioSource.loop = true;
        audioSource.Play();

        if (raceTimer != null)
        {
            raceTimer.StartTimer();
        }
    }
}