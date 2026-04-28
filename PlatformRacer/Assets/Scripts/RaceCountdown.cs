using UnityEngine;
using TMPro;
using System.Collections;
using Unity.Netcode;

public class RaceCountdown : NetworkBehaviour
{
    public TMP_Text countdownText;
    public AudioClip countdownBeep;
    public AudioClip goSound;
    public AudioClip backgroundMusic;
    private AudioSource audioSource;
    public RaceTimer raceTimer;

    private PlayerController[] players;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public override void OnNetworkSpawn()
    {
        // Only the host kicks off the countdown
        if (IsHost)
        {
            // Small delay to ensure all clients have finished spawning
            StartCoroutine(WaitThenBegin());
        }
    }

    IEnumerator WaitThenBegin()
    {
        yield return new WaitForSeconds(1f);
        BeginCountdownClientRpc();
    }

    [ClientRpc]
    void BeginCountdownClientRpc()
    {
        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        // Disable player input at start

        countdownText.gameObject.SetActive(true);

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

        // Enable player input on GO

        yield return new WaitForSeconds(1f);

        countdownText.gameObject.SetActive(false);
        ReleaseLocalPlayer();
        audioSource.clip = backgroundMusic;
        audioSource.loop = true;
        audioSource.Play();
        raceTimer.StartTimer();
    }

    void ReleaseLocalPlayer()
    {
        var allPlayers = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        foreach (var player in allPlayers)
        {
            if (player.IsOwner)
            {
                player.stateMachine.ChangeStates("PlayerDefault");
                break;
            }
        }
    }

}