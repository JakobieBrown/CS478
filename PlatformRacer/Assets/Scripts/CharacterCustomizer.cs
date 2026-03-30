using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterCustomizer : MonoBehaviour
{
    public Toggle[] bodyStyleToggles;
    public Toggle[] skinToneToggles;
    public Toggle[] hairStyleToggles;
    public Toggle[] hairColorToggles;

    public TMP_InputField nameInput;
    

    public void OnSubmit()
    {
        string body = GetSelectedToggle(bodyStyleToggles);
        string skin = GetSelectedToggle(skinToneToggles);
        string hairStyle = GetSelectedToggle(hairStyleToggles);
        string hairColor = GetSelectedToggle(hairColorToggles);
        string playerName = nameInput.text;

        string result =
            "Name: " + playerName + "\n" +
            "Body Type: " + body + "\n" +
            "Skin Color: " + skin + "\n" +
            "Hair Style: " + hairStyle + "\n" +
            "Hair Color: " + hairColor;

        Debug.Log(result);
    }

    public string GetSelectedToggle(Toggle[] toggles)
    {
        foreach (Toggle t in toggles)
        {
            if (t.isOn)
                return t.name;
        }
        return "None";
    }


}
