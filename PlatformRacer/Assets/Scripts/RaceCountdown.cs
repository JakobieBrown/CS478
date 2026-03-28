using UnityEngine;
using TMPro;
using System.Collections;

public class RaceCountdown : MonoBehaviour
{
    public TMP_Text countdownText;
    public AudioClip countdownBeep;
    public AudioClip goSound;
    private AudioSource audioSource;
    private bool hasStarted = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }


    public void BeginCountdown()
    {
        Debug.Log("BeginCountdown called, hasStarted: " + hasStarted);
        if (hasStarted) return;
        hasStarted = true;
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

        countdownText.gameObject.SetActive(false);
    }
}