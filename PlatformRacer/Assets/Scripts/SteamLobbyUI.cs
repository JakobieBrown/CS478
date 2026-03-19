using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using System.Collections.Generic;

public class SteamLobbyUI : MonoBehaviour
{
    private RectTransform playerListContainer;
    private Button readyButton;
    private Button startButton;

    private Dictionary<CSteamID, GameObject> playerEntries = new Dictionary<CSteamID, GameObject>();

    void Start()
    {
        CreateUI();
        RefreshPlayerList();
    }

    void Update()
    {
        SteamAPI.RunCallbacks();
    }

    #region UI Creation

    void CreateUI()
    {
        // Canvas
        GameObject canvasGO = new GameObject("Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        // Panel
        GameObject panel = CreateUIObject("LobbyPanel", canvasGO.transform);
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.sizeDelta = new Vector2(400, 500);
        panelRect.anchoredPosition = Vector2.zero;

        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.7f);

        // Player List Container
        GameObject listGO = CreateUIObject("PlayerList", panel.transform);
        playerListContainer = listGO.GetComponent<RectTransform>();
        playerListContainer.anchorMin = new Vector2(0.1f, 0.3f);
        playerListContainer.anchorMax = new Vector2(0.9f, 0.9f);
        playerListContainer.offsetMin = Vector2.zero;
        playerListContainer.offsetMax = Vector2.zero;

        VerticalLayoutGroup layout = listGO.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 5;
        layout.childControlHeight = true;
        layout.childControlWidth = true;

        ContentSizeFitter fitter = listGO.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // Ready Button
        readyButton = CreateButton(panel.transform, "Ready", new Vector2(0.25f, 0.1f), OnReadyClicked);

        // Start Button (Host only)
        startButton = CreateButton(panel.transform, "Start Game", new Vector2(0.75f, 0.1f), OnStartClicked);
    }

    GameObject CreateUIObject(string name, Transform parent)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent);

        RectTransform rect = go.AddComponent<RectTransform>();
        rect.localScale = Vector3.one;

        return go;
    }

    Button CreateButton(Transform parent, string text, Vector2 anchor, UnityEngine.Events.UnityAction onClick)
    {
        GameObject btnGO = CreateUIObject(text + "Button", parent);
        RectTransform rect = btnGO.GetComponent<RectTransform>();

        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.sizeDelta = new Vector2(120, 40);

        Image img = btnGO.AddComponent<Image>();
        img.color = Color.gray;

        Button button = btnGO.AddComponent<Button>();
        button.onClick.AddListener(onClick);

        // Text
        GameObject textGO = CreateUIObject("Text", btnGO.transform);
        Text txt = textGO.AddComponent<Text>();
        txt.text = text;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = Color.white;
        txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");

        RectTransform txtRect = textGO.GetComponent<RectTransform>();
        txtRect.anchorMin = Vector2.zero;
        txtRect.anchorMax = Vector2.one;
        txtRect.offsetMin = Vector2.zero;
        txtRect.offsetMax = Vector2.zero;

        return button;
    }

    #endregion

    #region Player List

    public void RefreshPlayerList()
    {
        if (SteamLobbyManager.CurrentLobbyID == CSteamID.Nil)
            return;

        int count = SteamMatchmaking.GetNumLobbyMembers(SteamLobbyManager.CurrentLobbyID);

        // Clear old
        foreach (Transform child in playerListContainer)
            Destroy(child.gameObject);

        playerEntries.Clear();

        for (int i = 0; i < count; i++)
        {
            CSteamID member = SteamMatchmaking.GetLobbyMemberByIndex(SteamLobbyManager.CurrentLobbyID, i);
            CreatePlayerEntry(member);
        }
    }

    void CreatePlayerEntry(CSteamID steamId)
    {
        GameObject entry = CreateUIObject("PlayerEntry", playerListContainer);

        LayoutElement layout = entry.AddComponent<LayoutElement>();
        layout.preferredHeight = 40;

        Image bg = entry.AddComponent<Image>();
        bg.color = new Color(1, 1, 1, 0.1f);

        string name = SteamFriends.GetFriendPersonaName(steamId);
        string ready = SteamMatchmaking.GetLobbyMemberData(SteamLobbyManager.CurrentLobbyID, steamId, "Ready");

        // Text
        GameObject textGO = CreateUIObject("Name", entry.transform);
        Text txt = textGO.AddComponent<Text>();
        txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        txt.alignment = TextAnchor.MiddleLeft;
        txt.color = Color.white;

        txt.text = $"{name} {(ready == "true" ? "[READY]" : "")}";

        RectTransform rect = textGO.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(10, 0);
        rect.offsetMax = new Vector2(-10, 0);

        playerEntries[steamId] = entry;
    }

    #endregion

    #region Buttons

    void OnReadyClicked()
    {
        string current = SteamMatchmaking.GetLobbyMemberData(
            SteamLobbyManager.CurrentLobbyID,
            SteamUser.GetSteamID(),
            "Ready"
        );

        string newState = (current == "true") ? "false" : "true";

        SteamMatchmaking.SetLobbyMemberData(
            SteamLobbyManager.CurrentLobbyID,
            "Ready",
            newState
        );

        RefreshPlayerList();
    }

    void OnStartClicked()
    {
        if (!IsHost()) return;

        SteamMatchmaking.SetLobbyData(
            SteamLobbyManager.CurrentLobbyID,
            "StartGame",
            "true"
        );
    }

    bool IsHost()
    {
        return SteamMatchmaking.GetLobbyOwner(SteamLobbyManager.CurrentLobbyID) == SteamUser.GetSteamID();
    }

    #endregion
}