using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using Unity.Services.Authentication;


[System.Serializable]
public class ProfileSettings
{
    public string playerName;
    public int body;
    public int skin;
    public int hairStyle;
    public int hairColor;
}

public class CharacterCustomizer : MonoBehaviour
{
    public Toggle[] bodyStyleToggles;
    public Toggle[] skinToneToggles;
    public Toggle[] hairStyleToggles;
    public Toggle[] hairColorToggles;

    public TMP_InputField nameInput;

    private string filePath;


    void Start()
    {
        filePath = Application.persistentDataPath + "/playerprofile.json";

        string json = File.ReadAllText(filePath);
        ProfileSettings settings = JsonUtility.FromJson<ProfileSettings>(json); //TODO: Change this to something better
        nameInput.text = settings.playerName;
    }

    void LoadSettings()
    {
        string json = File.ReadAllText(filePath);
        ProfileSettings settings = JsonUtility.FromJson<ProfileSettings>(json);
        Debug.Log("Name : " + settings.playerName);
        Debug.Log("Body  : " + settings.body);
        Debug.Log("Skin : " + settings.skin);
        Debug.Log("Hair Style : " + settings.hairStyle);
        Debug.Log("Hair Color : " + settings.hairColor);
    }

    void SaveProfile(ProfileSettings settings)
    {
        AuthenticationService.Instance.UpdatePlayerNameAsync(settings.playerName); // Save Name in authentication service
        string json = JsonUtility.ToJson(settings, true);
        File.WriteAllText(filePath, json);
        Debug.Log("Saved to " + filePath);
        if (ProfileManager.Instance != null)
        {
            ProfileManager.Instance.RefreshProfile();
        }
    }

    public void OnSubmit()
    {
        int body = GetSelectedToggle(bodyStyleToggles);
        int skin = GetSelectedToggle(skinToneToggles);
        int hairStyle = GetSelectedToggle(hairStyleToggles);
        int hairColor = GetSelectedToggle(hairColorToggles);
        string playerName = nameInput.text;

        ProfileSettings settings = new ProfileSettings()
        {
            playerName = playerName,
            body = body,
            hairStyle = hairStyle,
            hairColor = hairColor,
            skin = skin
        };

        string result =
            "Name: " + playerName + "\n" +
            "Body Type: " + body + "\n" +
            "Skin Color: " + skin + "\n" +
            "Hair Style: " + hairStyle + "\n" +
            "Hair Color: " + hairColor;

        Debug.Log(result);

        SaveProfile(settings);
        LoadSettings();
    }

    public int GetSelectedToggle(Toggle[] toggles)
    {
        for (int i = 0; i < toggles.Length; i++)
        {
            if (toggles[i].isOn)
                return i;
        }
        return 0;
    }


}
