using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class StatsManager : MonoBehaviour
{
    [Header("Tab Buttons")]
    public Button cityTabButton;
    public Button mountainTabButton;
    public Button beachTabButton;

    [Header("Entry Template & Content")]
    public GameObject entryTemplate;
    public Transform contentParent;

    private string[] distances = { "200", "400", "800", "3000", "5000", "10000" };
    private string[] distanceLabels = { "200 M", "400 M", "800 M", "3 KM", "5 KM", "10 KM" };
    private string currentMap = "CityTrack";

    void Start()
    {
        cityTabButton.onClick.AddListener(() => ShowStats("CityTrack"));
        mountainTabButton.onClick.AddListener(() => ShowStats("MountainTrack"));
        beachTabButton.onClick.AddListener(() => ShowStats("BeachTrack"));

        // Show city by default
        ShowStats("CityTrack");

        // Hide the template
        entryTemplate.SetActive(false);
    }

    void ShowStats(string mapName)
    {
        currentMap = mapName;

        // Clear existing entries except the template
        foreach (Transform child in contentParent)
        {
            if (child.gameObject != entryTemplate)
                Destroy(child.gameObject);
        }

        // Create an entry for each distance
        for (int i = 0; i < distances.Length; i++)
        {
            string path = Application.persistentDataPath + "/" + mapName + "_" + distances[i] + "_besttime.json";

            string bestTimeDisplay = "No Time";
            if (File.Exists(path))
            {
                SaveData data = JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
                bestTimeDisplay = FormatTime(data.bestTime);
            }

            // Instantiate entry from template
            GameObject entry = Instantiate(entryTemplate, contentParent);
            entry.SetActive(true);

            // Fill in the fields
            entry.transform.Find("DistanceText").GetComponent<TMP_Text>().text = distanceLabels[i];
            entry.transform.Find("NameText").GetComponent<TMP_Text>().text = mapName.Replace("Track", "");
            entry.transform.Find("ScoreText").GetComponent<TMP_Text>().text = bestTimeDisplay;
        }
    }

    string FormatTime(float time)
    {
        int minutes = (int)(time / 60);
        int seconds = (int)(time % 60);
        int milliseconds = (int)((time * 100) % 100);
        return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }
}