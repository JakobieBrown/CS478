using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Multiplayer;
using Utilities;

public class SessionBrowser : MonoBehaviour
{
    public Button refreshButton;
    public Transform contentParent; // where session UI items go
    public GameObject sessionItemPrefab; // your UI prefab

    private List<ISessionInfo> sessions = new();
    private string selectedSessionId;


    const float pollInterval = 6f;

    CountdownTimer pollForUpdatesTimer = new CountdownTimer(pollInterval);

    void Start()
    {
        //refreshButton.onClick.AddListener(OnRefreshClicked);
        pollForUpdatesTimer.OnTimerStop += () => {
            RefreshSessionsAsync();
            PopulateUI();
            Debug.Log("PollTimerIsWorking");
            pollForUpdatesTimer.Start();
         };
    }

    private void Update()
    {
        pollForUpdatesTimer.Tick(Time.deltaTime);
    }

    //public void OnRefreshClicked()
    //{
    //    RefreshSessionsAsync();
    //}

    public void test()
    {
        Debug.Log("Test Succeeded");
    }

    private async void RefreshSessionsAsync()
    {
        if (MultiplayerService.Instance == null)
        {
            Debug.LogError("Services not initialized.");
            return;
        }

        //refreshButton.interactable = false;

        try
        {
            var result = await MultiplayerService.Instance.QuerySessionsAsync(
                new QuerySessionsOptions()
            );

            sessions = (List<ISessionInfo>)result.Sessions; 

            PopulateUI();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to fetch sessions: {e.Message}");
        }
        //finally
        //{
        //    refreshButton.interactable = true;
        //}
    }

    private void PopulateUI()
    {
        // Clear old
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        foreach (var session in sessions)
        {
            var item = Instantiate(sessionItemPrefab, contentParent);

            // You’ll need to customize this depending on your prefab
            var ui = item.GetComponent<SessionItemUI>();

            ui.Setup(
                session.Name,
                session.MaxPlayers - session.AvailableSlots,
                session.MaxPlayers,
                () => SelectSession(session.Id)
            );
        }
    }

    private void SelectSession(string sessionId)
    {
        selectedSessionId = sessionId;
        Debug.Log($"Selected session: {sessionId}");
    }

    public void OnJoinClicked()
    {
        JoinSelectedSessionAsync();
    }

    public void StartLobbyPollTimer()
    {
        RefreshSessionsAsync();
        PopulateUI();
        pollForUpdatesTimer.Start();
    }

    private async void JoinSelectedSessionAsync()
    {
        if (string.IsNullOrEmpty(selectedSessionId))
        {
            Debug.LogError("No session selected.");
            return;
        }

        try
        {
            var session = await MultiplayerService.Instance
                .JoinSessionByIdAsync(selectedSessionId, new JoinSessionOptions());

            Debug.Log("Joined session!");

            // Save + transition
            SessionState.CurrentSession = session;
            UnityEngine.SceneManagement.SceneManager.LoadScene("NextScene");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Join failed: {e.Message}");
        }
    }
}