using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

[System.Serializable]
public class ServerSettings
{
    public string playerName;
    public bool isPvp;
    public string privacy;
}
public class UIController : MonoBehaviour
{
    public TMP_InputField nameInput;
    public Toggle pvpToggle;

    public Toggle inviteOnlyToggle;
    public Toggle friendsToggle;
    public Toggle fofToggle;

    public Button submitButton;
    private string filePath;

    void Start()
    {
        filePath = Application.persistentDataPath + "/serversettings.json";
        submitButton.onClick.AddListener(OnSubmit);
    }

    void SaveSettings(ServerSettings settings)
    {
        string json = JsonUtility.ToJson(settings, true);
        File.WriteAllText(filePath, json);
        Debug.Log("Saved to " + filePath);
    }
    
    void LoadSettings()
    {
        string json = File.ReadAllText(filePath);
        ServerSettings settings = JsonUtility.FromJson<ServerSettings>(json);
        Debug.Log("Name : " + settings.playerName);
        Debug.Log("Pvp  : " + settings.isPvp);
        Debug.Log("Privacy : " + settings.privacy);
            
    }

    void OnSubmit()
    {
        string name = nameInput.text;
        bool isPvP = pvpToggle.isOn;

        string privacy = "None";

        if (inviteOnlyToggle.isOn)
            privacy = "Invite Only";
        else if (friendsToggle.isOn)
            privacy = "Friends";
        else if (fofToggle.isOn)
            privacy = "Friends of friends";


        ServerSettings settings = new ServerSettings()
        {
            playerName = name,
            isPvp = isPvP,
            privacy = privacy
        };

        SaveSettings(settings);
        LoadSettings();

    }
}