using TMPro;
using UnityEngine;

public class PlayerListItemUI : MonoBehaviour
{
    [SerializeField] TMP_Text playerNameText;

    public void SetPlayer(string playerName, bool isHost)
    {
        playerName = playerName.Split("#")[0];
        playerNameText.text = isHost ? $"{playerName} (Host)" : playerName;
    }
}