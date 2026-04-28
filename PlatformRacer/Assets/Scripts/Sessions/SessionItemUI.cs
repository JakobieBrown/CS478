using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SessionItemUI : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text playerText;
    public Button selectButton;

    public void Setup(string name, int currentPlayers, int maxPlayers, Action onClick)
    {
        nameText.text = name;
        playerText.text = $"{currentPlayers}/{maxPlayers}";

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() => onClick());
    }
}