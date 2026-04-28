using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using Unity.Services.Multiplayer;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WebSocketSharp;

public class JoinByCode : MonoBehaviour
{
    public TMP_InputField codeInput;
    public Button joinButton;

    public void Awake()
    {
        joinButton.onClick.AddListener(OnJoinByCodeClicked);
    }

    public void OnJoinByCodeClicked()
    {
        if (!codeInput.text.IsNullOrEmpty())
            JoinByCodeAsync();
        else
            Debug.Log("Code field is empty");
    }

    private async void JoinByCodeAsync()
    {
        if (MultiplayerService.Instance == null)
        {
            Debug.LogError("Services not initialized.");
            return;
        }

        string code = codeInput.text.Trim();

        if (string.IsNullOrEmpty(code))
        {
            Debug.LogError("Code is empty.");
            return;
        }

        try
        {
            var session = await MultiplayerService.Instance
                .JoinSessionByCodeAsync(code, new JoinSessionOptions().WithPlayerName());

            Debug.Log("Joined via code!");
            Debug.Log($"IsPrivate: {session.IsPrivate}");
            Debug.Log($"Code: {session.Code}");
            SessionState.CurrentSession = session;
            SceneManager.LoadScene("LobbyScene");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Join by code failed: {e.Message}");
        }
    }
}