using TMPro;
using UnityEngine;

public class RaceUIDisplay : MonoBehaviour
{

    private TextMeshProUGUI label;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        label = GetComponent<TextMeshProUGUI>();

        string map = PlayerPrefs.GetString("SelectedMap", "Unknown");
        string distance = PlayerPrefs.GetString("SelectedDistance", "Unknown");

        // Strip "Track" off the end for cleaner display
        string mapDisplay = map.Replace("Track", "");

        label.text = mapDisplay + "-" + distance;
    }

}
