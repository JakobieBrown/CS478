using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class SettingsManager : MonoBehaviour
{
    [Header("Audio")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Gameplay")]
    public Slider zoomSlider;
    public Slider uiSizeSlider;

    [Header("Display")]
    public Slider resolutionSlider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Auido Listeners
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);

        // Gameplay Listeners
        zoomSlider.onValueChanged.AddListener(SetZoom);
        uiSizeSlider.onValueChanged.AddListener(SetUISize);

        // Display Listeners
        resolutionSlider.onValueChanged.AddListener(SetResolution);
    }

    // AUDIO

    void SetMasterVolume(float value)
    {
        AudioListener.volume = value;
    }

    void SetMusicVolume(float value)
    {
        Debug.Log("Music Volume: " + value);
    }

    void SetSFXVolume (float value)
    {
        Debug.Log("SFX Volume: " + value);
    }

    void SetZoom(float value)
    {
        Camera.main.fieldOfView = Mathf.Lerp(60f, 30f, value);
    }

    void SetUISize (float value)
    {
        CanvasScaler scaler = FindFirstObjectByType<CanvasScaler>();
        scaler.scaleFactor = Mathf.Lerp(0.8f,1.5f,value);
    }

    void SetResolution (float value)
    {
        Debug.Log("Screen Resolution: " + value);
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
