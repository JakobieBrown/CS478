using Steamworks;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject startMenu;
    public GameObject hostMenu;
    public GameObject joinMenu;
    public GameObject settingsMenu;

    public void ShowMainMenu() 
    { 
        mainMenu.SetActive(true);
        startMenu.SetActive(false);
        hostMenu.SetActive(false);
        joinMenu.SetActive(false);
        settingsMenu.SetActive(false);
    }

    public void ShowStartMenu()
    {
        mainMenu.SetActive(false);
        startMenu.SetActive(true);
        hostMenu.SetActive(false);
        joinMenu.SetActive(false);
        settingsMenu.SetActive(false);
    }

    public void ShowHostMenu()
    {
        mainMenu.SetActive(false);
        startMenu.SetActive(false);
        hostMenu.SetActive(true);
        joinMenu.SetActive(false);
        settingsMenu.SetActive(false);
    }
    public void ShowJoinMenu()
    {
        mainMenu.SetActive(false);
        startMenu.SetActive(false);
        hostMenu.SetActive(false);
        joinMenu.SetActive(true);
        settingsMenu.SetActive(false);
    }
    public void ShowSettingsMenu()
    {
        mainMenu.SetActive(false);
        startMenu.SetActive(false);
        hostMenu.SetActive(false);
        joinMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void ExitGame()
    {
      Debug.Log("Quit Game");
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}