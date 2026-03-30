using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenController : MonoBehaviour
{
    public void GoToLobby()
    {
        SceneManager.LoadScene("LobbyScene");
    }

    public void GoToProfile()
    {
        SceneManager.LoadScene("ProfileScene");
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

}
