using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using Unity.Services.Relay;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SessionManager : MonoBehaviour
{

    [Header("UI")]
    public TMP_Text sessionCodeText;
    public Button copyCodeButton;
    public Button leaveButton;

    [Header("Player List")]
    [SerializeField] Transform playerListContainer;
    [SerializeField] GameObject playerListItemPrefab;

    private List<GameObject> spawnedItems = new List<GameObject>();

    private ISession session;

    bool hasJoinedRelay = false;


    void Start()
    {
        session = SessionState.CurrentSession;

        if (session == null)
        {
            Debug.LogError("No session found.");
            return;
        }

        // Set session code
        sessionCodeText.text = session.Code;

        // Hook buttons
        copyCodeButton.onClick.AddListener(CopyCode);
        leaveButton.onClick.AddListener(OnLeaveClicked);

        session.PlayerJoined += OnPlayerChanged;
        session.PlayerHasLeft += OnPlayerChanged;

        session.SessionPropertiesChanged += OnSessionUpdated;

        RefreshPlayerList();
    }


    private async void OnSessionUpdated()
    {
        if (session == null) return;

        // Client joins relay when relayCode appears
        if (!hasJoinedRelay && !session.IsHost)
        {
            if (session.Properties.TryGetValue("relayCode", out var relayProp))
            {
                hasJoinedRelay = true;
                Debug.Log("[OnSessionUpdated] Joining relay: " + relayProp.Value);
                await JoinRelay(relayProp.Value);
            }
        }

        // Everyone (except host who already loaded) loads scene when gameState = "start"
        if (!session.IsHost)
        {
            if (session.Properties.TryGetValue("gameState", out var stateProp) &&
                stateProp.Value == "start")
            {
                if (session.Properties.TryGetValue("map", out var mapProp))
                {
                    Debug.Log("[OnSessionUpdated] Loading scene: " + mapProp.Value);
                    SceneManager.LoadScene(mapProp.Value, LoadSceneMode.Single);
                }
            }
        }
    }

    private void CopyCode()
    {
        if (string.IsNullOrEmpty(session.Code))
            return;

        GUIUtility.systemCopyBuffer = session.Code;
        Debug.Log("Copied session code!");
    }

    public void OnLeaveClicked()
    {
        LeaveSessionAsync();
    }

    private async void LeaveSessionAsync()
    {
        if (session == null)
            return;

        try
        {
            await session.LeaveAsync();

            Debug.Log("Left session");

            SessionState.CurrentSession = null;

            SceneManager.LoadScene("MainMenu"); // or wherever
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to leave session: {e.Message}");
        }
    }

    void OnPlayerChanged(string _)
    {
        RefreshPlayerList();
    }

    void RefreshPlayerList()
    {
        if (session == null)
            return;

        // Clear old UI
        foreach (var item in spawnedItems)
        {
            Destroy(item);
        }
        spawnedItems.Clear();

        // Rebuild list
        foreach (var player in session.Players)
        {
            var go = Instantiate(playerListItemPrefab, playerListContainer);
            var ui = go.GetComponent<PlayerListItemUI>();

            bool isHost = session.Host == player.Id;
            ui.SetPlayer(player.GetPlayerName(), isHost);

            spawnedItems.Add(go);
        }
    }

    void OnDestroy()
    {
        if (session != null)
        {
            session.PlayerJoined -= OnPlayerChanged;
            session.PlayerHasLeft -= OnPlayerChanged;
        }
    }


    public static async Task<string> SetupHostRelay(int maxPlayers)
    {
        try
        {
            var allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers-1);
            var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

            // Use the correct overload of SetRelayServerData
            transport.SetRelayServerData(
                allocation.ServerEndpoints[0].Host,
                (ushort)allocation.ServerEndpoints[0].Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData,
                allocation.ConnectionData, // Host connection data (can be the same as ConnectionData for host)
                true // Use secure connection
            );

            NetworkManager.Singleton.StartHost();

            return joinCode;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to setup host relay: {e.Message}");
            throw;
        }
    }

    public static async Task JoinRelay(string joinCode)
    {
        try
        {
            Debug.Log("[JoinRelay] Starting with code: " + joinCode);
            var allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(
                allocation.ServerEndpoints[0].Host,
                (ushort)allocation.ServerEndpoints[0].Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData,
                allocation.HostConnectionData,
                true
            );

            NetworkManager.Singleton.StartClient();
            Debug.Log("[JoinRelay] StartClient called.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[JoinRelay] Failed: {e.Message}");
        }
    }
}