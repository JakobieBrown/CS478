using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Services.Multiplayer;
using UnityEngine.SceneManagement;
using static UnityEngine.LowLevelPhysics2D.PhysicsLayers;
using System.Collections.Generic;

public class SessionCreator : MonoBehaviour
{
    [Header("UI References")]
    public Toggle isPrivateToggle;
    public TMP_InputField maxPlayersInput;
    public TMP_InputField sessionNameInput;
    public Button submitButton;

    // Wrapper for Unity Button
    public void OnCreateSessionClicked()
    {
        CreateSessionAsync(); // fire and forget safely
    }

    private async void CreateSessionAsync()
    {
        if (MultiplayerService.Instance == null)
        {
            Debug.LogError("Multiplayer Services not initialized.");
            return;
        }

        submitButton.interactable = false;

        try
        {
            // Validate session name
            string sessionName = sessionNameInput.text;
            if (string.IsNullOrWhiteSpace(sessionName))
                throw new System.Exception("Session name cannot be empty.");

            // Parse max players
            if (!int.TryParse(maxPlayersInput.text, out int maxPlayers))
                throw new System.Exception("Invalid max players input.");

            maxPlayers = Mathf.Clamp(maxPlayers, 2, 8);

            //var playerProperties = new Dictionary<string, PlayerProperty>
            //{
            //    {
            //        PlayerNameModule.PropertyKey,
            //        new PlayerProperty(playerName, VisibilityPropertyOptions.Public)
            //    }
            //};

            // Build options
            var options = new SessionOptions
            {
                Name = sessionName,
                MaxPlayers = maxPlayers,
                IsPrivate = isPrivateToggle.isOn
            }.WithPlayerName();

            // Create session
            var session = await MultiplayerService.Instance.CreateSessionAsync(options);
            SessionState.CurrentSession = session;


            Debug.Log($"Session '{sessionName}' created!");
            Debug.Log($"IsPrivate: {session.IsPrivate}");
            Debug.Log($"Code: {session.Code}");
            Debug.Log(SessionState.CurrentSession.ToString());
            SceneManager.LoadScene("LobbyScene");
        }
        catch (SessionException e)
        {
            Debug.LogError($"Session failed: {e.Message}");
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
        finally
        {
            submitButton.interactable = true;
        }
    }
}