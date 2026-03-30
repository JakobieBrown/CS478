using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderTextUpdater : MonoBehaviour
{
    public Slider slider;
    public TMP_Text valueText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize text at start
        UpdateText(slider.value);

        slider.onValueChanged.AddListener((float val) => UpdateText(val));
    }

    void UpdateText(float value)
    {
        int percentage = Mathf.RoundToInt(value);
        valueText.text = percentage + "%";
    }

    void OnDestroy()
    {
        slider.onValueChanged.RemoveAllListeners();
    }
}
