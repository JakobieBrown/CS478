using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    public TMP_InputField nameInput;
    public Toggle pvpToggle;

    public Toggle inviteOnlyToggle;
    public Toggle friendsToggle;
    public Toggle fofToggle;

    public Button submitButton;

    void Start()
    {
        submitButton.onClick.AddListener(OnSubmit);
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

        Debug.Log("Name: " + name);
        Debug.Log("PvP: " + isPvP);
        Debug.Log("Privacy: " + privacy);
    }
}