using UnityEngine;
using System.IO;

public class ProfileManager : MonoBehaviour
{
    public static ProfileManager Instance;
    public ProfileSettings Profile { get; private set; }
    private string filePath;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadProfile();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void LoadProfile()
    {
        filePath = Application.persistentDataPath + "/playerprofile.json";
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            Profile = JsonUtility.FromJson<ProfileSettings>(json);
            Debug.Log("Profile loaded: " + Profile.playerName);
        }
        else
        {
            Profile = new ProfileSettings();
            Debug.Log("No profile found, using defaults");
        }
    }
    public void RefreshProfile()
    {
        LoadProfile();
    }
}
