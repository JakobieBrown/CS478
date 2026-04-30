using UnityEngine;
using TMPro;
using System.IO;

public class ResultsManager : MonoBehaviour
{
    public GameObject resultsPanel;
    public TextMeshProUGUI finalTimeText;
    public TextMeshProUGUI bestTimeText;

    string path;

    void Start()
    {
        string map = PlayerPrefs.GetString("SelectedMap", "Unknown");
        int distance = PlayerPrefs.GetInt("SelectedDistance", 0);
        path = Application.persistentDataPath + "/" + map + "_" + distance + "_besttime.json";
        resultsPanel.SetActive(false);
    }
    public void BackToMenu()
    {
        if (Unity.Netcode.NetworkManager.Singleton != null)
        {
            Unity.Netcode.NetworkManager.Singleton.Shutdown();
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuScene");
    }

    public void ShowResults(float finalTime)
    {
        resultsPanel.SetActive(true);

        finalTimeText.text = "Your Time: " + FormatTime(finalTime);

        float bestTime = LoadBestTime();

        if (bestTime == 0 || finalTime < bestTime)
        {
            SaveBestTime(finalTime);
            bestTime = finalTime;
        }

        bestTimeText.text = "Best Time: " + FormatTime(bestTime);
    }

    string FormatTime(float time)
    {
        int minutes = (int)(time / 60);
        int seconds = (int)(time % 60);
        int milliseconds = (int)((time * 100) % 100);

        return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }

    void SaveBestTime(float time)
    {
        SaveData data = new SaveData();
        data.bestTime = time;

        File.WriteAllText(path, JsonUtility.ToJson(data));
    }

    float LoadBestTime()
    {
        if (!File.Exists(path)) return 0f;

        SaveData data = JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
        return data.bestTime;
    }
}

[System.Serializable]
public class SaveData
{
    public float bestTime;
}