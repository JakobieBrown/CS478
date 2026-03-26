using Steamworks;
using UnityEngine;

public class SteamLobbyManager : MonoBehaviour
{
    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<LobbyEnter_t> lobbyEntered;
    protected Callback<LobbyChatUpdate_t> lobbyUpdated;

    public static CSteamID CurrentLobbyID;

    void Start()
    {
        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        lobbyUpdated = Callback<LobbyChatUpdate_t>.Create(OnLobbyUpdated);
    }

    public void CreateLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 4);
    }

    void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
            return;

        CurrentLobbyID = new CSteamID(callback.m_ulSteamIDLobby);

        // Example metadata
        SteamMatchmaking.SetLobbyData(CurrentLobbyID, "HostAddress", SteamUser.GetSteamID().ToString());
    }

    void OnLobbyEntered(LobbyEnter_t callback)
    {
        CurrentLobbyID = new CSteamID(callback.m_ulSteamIDLobby);

        // Load lobby scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("LobbyScene");
    }

    void OnLobbyUpdated(LobbyChatUpdate_t callback)
    {
        // Trigger UI refresh
    }
}