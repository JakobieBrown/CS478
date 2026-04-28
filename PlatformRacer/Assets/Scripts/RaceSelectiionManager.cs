using Steamworks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using Unity.Services.Multiplayer;
using Unity.Netcode;
using System.Threading.Tasks;



public class RaceSelectiionManager : MonoBehaviour
{
    [Header("Map Buttons")]
    public Button beachButton;
    public Button cityButton;
    public Button mountainButton;

    [Header("Distance Buttons")] // 200m 400m 800m 3k ,5k, 10k
    public Button dist200MButton;
    public Button dist400MButton;
    public Button dist800MButton;
    public Button dist3KMButton;
    public Button dist5KMButton;
    public Button dist10KMButton;

    [Header("Start Button")]
    public Button startRaceButton;

    [Header("Pressed Color")]
    public Color selectedColor = new Color(0.2f, 0.6f, 1f);
    public Color normalColor = new Color(1f, 1f, 1f);

    private string selectedMap = "";
    private string selectedDistance = "";

    private ISession session;
    private bool isHost;

    void Start()
    {
        //Map Buttons
        beachButton.onClick.AddListener(() => SelectMap("BeachTrack", beachButton));
        cityButton.onClick.AddListener(() => SelectMap("CityTrack", cityButton));
        mountainButton.onClick.AddListener(() => SelectMap("MountainTrack", mountainButton));

        // Distance Buttons
        dist200MButton.onClick.AddListener(() => SelectDistance("200 M", dist200MButton));
        dist400MButton.onClick.AddListener(() => SelectDistance("400 M", dist400MButton));
        dist800MButton.onClick.AddListener(() => SelectDistance("800 M", dist800MButton));
        dist3KMButton.onClick.AddListener(() => SelectDistance("3 KM", dist3KMButton));
        dist5KMButton.onClick.AddListener(() => SelectDistance("5 KM", dist5KMButton));
        dist10KMButton.onClick.AddListener(() => SelectDistance("10 KM", dist10KMButton));

        // Start Button
        startRaceButton.onClick.AddListener(StartRace);
        startRaceButton.interactable = false;

        session = SessionState.CurrentSession;

        if (session == null)
        {
            Debug.LogError("No session found.");
            return;
        }


        SetInteractableState(session.IsHost);

    }


    void SetInteractableState(bool interactable)
    {
        // Map buttons
        beachButton.interactable = interactable;
        cityButton.interactable = interactable;
        mountainButton.interactable = interactable;

        // Distance buttons
        dist200MButton.interactable = interactable;
        dist400MButton.interactable = interactable;
        dist800MButton.interactable = interactable;
        dist3KMButton.interactable = interactable;
        dist5KMButton.interactable = interactable;
        dist10KMButton.interactable = interactable;
    }

    void SelectMap(string mapName, Button clicked)
    {
        selectedMap = mapName;

        //Reset all map buttons then highlight the clicked one
        SetButtonColor(beachButton, normalColor);
        SetButtonColor(cityButton, normalColor);
        SetButtonColor(mountainButton, normalColor);
        SetButtonColor(clicked, selectedColor);

        CheckReady();
    }

    void SelectDistance(string distance, Button clicked) 
    {
        selectedDistance = distance;
        
        SetButtonColor(dist200MButton, normalColor);
        SetButtonColor(dist400MButton, normalColor);
        SetButtonColor(dist800MButton, normalColor);
        SetButtonColor(dist3KMButton, normalColor);
        SetButtonColor(dist5KMButton, normalColor);
        SetButtonColor(dist10KMButton, normalColor);
        SetButtonColor (clicked, selectedColor);

        CheckReady() ;
    }

    void SetButtonColor(Button btn, Color color)
    {
        ColorBlock cb = btn.colors;
        cb.normalColor = color;
        cb.selectedColor = color;
        btn.colors = cb;
    }

    void CheckReady()
    {
        //startRaceButton.interactable = (selectedMap != "" && selectedDistance != "");
        Debug.Log("Map: " + selectedMap + " | Distance: " + selectedDistance);
        startRaceButton.interactable = (selectedMap != "" && selectedDistance != "");
    }

    async void StartRace()
    {
        if (!session.IsHost)
            return;

        PlayerPrefs.SetString("SelectedMap", selectedMap);
        PlayerPrefs.SetInt("SelectedDistance", ParseDistance(selectedDistance));
        PlayerPrefs.Save();

        string joinCode = await SessionManager.SetupHostRelay(session.MaxPlayers);

        var props = new Dictionary<string, SessionProperty>
    {
        { "relayCode", new SessionProperty(joinCode, VisibilityPropertyOptions.Member) },
        { "map",       new SessionProperty(selectedMap, VisibilityPropertyOptions.Member) },
        { "distance",  new SessionProperty(selectedDistance, VisibilityPropertyOptions.Member) },
        { "gameState", new SessionProperty("loading", VisibilityPropertyOptions.Member) }
    };

        session.AsHost().SetProperties(props);
        await session.AsHost().SavePropertiesAsync();

        // Wait for all clients to connect to Netcode relay
        int expectedClients = session.Players.Count - 1;
        float timeout = 15f;
        float elapsed = 0f;

        while (NetworkManager.Singleton.ConnectedClients.Count < expectedClients)
        {
            Debug.Log($"[StartRace] Waiting... {NetworkManager.Singleton.ConnectedClients.Count}/{expectedClients}");
            await Task.Delay(200);
            elapsed += 0.2f;

            if (elapsed >= timeout)
            {
                Debug.LogError("[StartRace] Timed out waiting for clients.");
                return;
            }
        }

        // All clients are on the relay — now tell everyone to load the scene
        var readyProps = new Dictionary<string, SessionProperty>
    {
        { "gameState", new SessionProperty("start", VisibilityPropertyOptions.Member) }
    };

        session.AsHost().SetProperties(readyProps);
        await session.AsHost().SavePropertiesAsync();

        // Host loads scene locally
        SceneManager.LoadScene(selectedMap, LoadSceneMode.Single);
    }

    public int ParseDistance(string input)
    {
        var parts = input.Split(' ');
        int value = int.Parse(parts[0]);
        string unit = parts[1];
        return unit switch
        {
            "M" => value,
            "KM" => value * 1000,
            _ => value
        };
    }

}
