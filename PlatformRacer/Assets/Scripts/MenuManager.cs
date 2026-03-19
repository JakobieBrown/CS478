using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private GameObject canvas;

    private GameObject mainMenu;
    private GameObject startMenu;
    private GameObject hostMenu;
    private GameObject joinMenu;
    private GameObject settingsMenu;

    void Start()
    {
        CreateCanvas();
        CreateMenus();

        ShowPanel(mainMenu);
    }

    #region Canvas

    void CreateCanvas()
    {
        canvas = new GameObject("Canvas");
        var c = canvas.AddComponent<Canvas>();
        c.renderMode = RenderMode.ScreenSpaceOverlay;

        canvas.AddComponent<CanvasScaler>();
        canvas.AddComponent<GraphicRaycaster>();
    }

    #endregion

    #region Menu Creation

    void CreateMenus()
    {
        mainMenu = CreatePanel("MainMenu");
        startMenu = CreatePanel("StartMenu");
        hostMenu = CreatePanel("HostMenu");
        joinMenu = CreatePanel("JoinMenu");
        settingsMenu = CreatePanel("SettingsMenu");

        CreateMainMenu();
        CreateStartMenu();
        CreateHostMenu();
        CreateJoinMenu();
        CreateSettingsMenu();
    }

    GameObject CreatePanel(string name)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(canvas.transform);

        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        VerticalLayoutGroup layout = panel.AddComponent<VerticalLayoutGroup>();
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.spacing = 15;

        ContentSizeFitter fitter = panel.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        return panel;
    }

    #endregion

    #region UI Helpers

    Button CreateButton(Transform parent, string text, UnityEngine.Events.UnityAction action)
    {
        GameObject btnGO = new GameObject(text);
        btnGO.transform.SetParent(parent);

        Image img = btnGO.AddComponent<Image>();
        img.color = Color.gray;

        Button btn = btnGO.AddComponent<Button>();
        btn.onClick.AddListener(action);

        RectTransform rect = btnGO.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(200, 50);

        GameObject txtGO = new GameObject("Text");
        txtGO.transform.SetParent(btnGO.transform);

        Text txt = txtGO.AddComponent<Text>();
        txt.text = text;
        txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = Color.white;

        RectTransform txtRect = txtGO.GetComponent<RectTransform>();
        txtRect.anchorMin = Vector2.zero;
        txtRect.anchorMax = Vector2.one;
        txtRect.offsetMin = Vector2.zero;
        txtRect.offsetMax = Vector2.zero;

        return btn;
    }

    GameObject CreateLabel(Transform parent, string text)
    {
        GameObject txtGO = new GameObject("Label");
        txtGO.transform.SetParent(parent);

        Text txt = txtGO.AddComponent<Text>();
        txt.text = text;
        txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = Color.white;

        RectTransform rect = txtGO.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(300, 40);

        return txtGO;
    }

    #endregion

    #region Menu Layouts

    void CreateMainMenu()
    {
        CreateLabel(mainMenu.transform, "MAIN MENU");

        CreateButton(mainMenu.transform, "Start", () => ShowPanel(startMenu));
        CreateButton(mainMenu.transform, "Profile", () => SceneManager.LoadScene("ProfileScene"));
        CreateButton(mainMenu.transform, "Settings", () => ShowPanel(settingsMenu));
        CreateButton(mainMenu.transform, "Exit", Application.Quit);
    }

    void CreateStartMenu()
    {
        CreateLabel(startMenu.transform, "START");

        CreateButton(startMenu.transform, "Host", () => ShowPanel(hostMenu));
        CreateButton(startMenu.transform, "Join", () => ShowPanel(joinMenu));
        CreateButton(startMenu.transform, "Solo Play", () => SceneManager.LoadScene("LobbyScene"));

        CreateButton(startMenu.transform, "Back", () => ShowPanel(mainMenu));
    }

    void CreateHostMenu()
    {
        CreateLabel(hostMenu.transform, "HOST LOBBY");

        // Placeholder for saved settings
        CreateLabel(hostMenu.transform, "[Saved Lobby Settings List]");

        // Form fields (simplified placeholders)
        CreateLabel(hostMenu.transform, "Lobby Name: (TODO Input)");
        CreateLabel(hostMenu.transform, "Invite Type: Invite / Friends / Friends of Friends");

        CreateButton(hostMenu.transform, "Create Lobby", () =>
        {
            // TODO: Call Steam lobby creation
            SceneManager.LoadScene("LobbyScene");
        });

        CreateButton(hostMenu.transform, "Back", () => ShowPanel(startMenu));
    }

    void CreateJoinMenu()
    {
        CreateLabel(joinMenu.transform, "JOIN LOBBY");

        // Placeholder list
        CreateLabel(joinMenu.transform, "[Lobby List Here]");

        CreateButton(joinMenu.transform, "Refresh", () =>
        {
            // TODO: Query Steam lobbies
        });

        CreateButton(joinMenu.transform, "Join Selected", () =>
        {
            // TODO: Join selected lobby
            SceneManager.LoadScene("LobbyScene");
        });

        CreateButton(joinMenu.transform, "Back", () => ShowPanel(startMenu));
    }

    void CreateSettingsMenu()
    {
        CreateLabel(settingsMenu.transform, "SETTINGS");

        CreateLabel(settingsMenu.transform, "[Settings Options Here]");

        CreateButton(settingsMenu.transform, "Apply", () => ShowPanel(mainMenu));
        CreateButton(settingsMenu.transform, "Back", () => ShowPanel(mainMenu));
    }

    #endregion

    #region Panel Control

    void ShowPanel(GameObject panel)
    {
        mainMenu.SetActive(false);
        startMenu.SetActive(false);
        hostMenu.SetActive(false);
        joinMenu.SetActive(false);
        settingsMenu.SetActive(false);

        panel.SetActive(true);
    }

    #endregion
}