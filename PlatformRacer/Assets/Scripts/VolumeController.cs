using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class VolumeController : MonoBehaviour
{
    public AudioMixer mixer;
    public Slider slider;       // 0–100 slider
    public TMP_Text percentText;

    void Start()
    {
        slider.onValueChanged.AddListener(SetVolume);

        // Initialize UI on start
        SetVolume(slider.value);
    }

    public void SetVolume(float value)
    {
        // 1. Update percent text (0–100)
        int percent = Mathf.RoundToInt(value);
        percentText.text = percent + "%";

        // 2. Convert 0–100 → 0–1 for audio mixer
        float normalized = value / 100f;

        // 3. Prevent log(0) issue
        if (normalized <= 0.0001f)
        {
            mixer.SetFloat("MasterVolume", -80f); // silence
        }
        else
        {
            float dB = Mathf.Log10(normalized) * 20f;
            mixer.SetFloat("MasterVolume", dB);
        }
    }
}