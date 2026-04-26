using UnityEngine;
using UnityEngine.UI;
public class MinimapIcon : MonoBehaviour
{
    [SerializeField] private Color iconColor = Color.red;
    private Image minimapDisplay;
    private RectTransform iconRect;
    private int trackLength;
    private float trackHeight = 20f;
    private GameObject iconObject;

    void Start()
    {
        trackLength = PlayerPrefs.GetInt("SelectedDistance", 500);

        Image[] allImages = FindObjectsByType<Image>(FindObjectsSortMode.None);
        foreach (Image img in allImages)
        {
            if (img.gameObject.name == "MinimapImage")
            {
                minimapDisplay = img;
                break;
            }
        }

        if (minimapDisplay == null)
        {
            Debug.LogError("MinimapImage not found in scene!");
            return;
        }

        // Create the icon dot dynamically
        iconObject = new GameObject("MinimapDot");
        iconObject.transform.SetParent(minimapDisplay.transform, false);
        Image dot = iconObject.AddComponent<Image>();
        dot.color = iconColor;
        iconRect = iconObject.GetComponent<RectTransform>();
        iconRect.sizeDelta = new Vector2(10, 10);
    }

    void Update()
    {
        if (minimapDisplay == null || iconRect == null) return;

        float xNorm = Mathf.Clamp01(transform.position.x / trackLength);
        float yNorm = Mathf.Clamp01((transform.position.y + trackHeight / 2f) / trackHeight);

        Rect mapRect = minimapDisplay.rectTransform.rect;
        iconRect.anchoredPosition = new Vector2(
            xNorm * mapRect.width - mapRect.width / 2f,
            yNorm * mapRect.height - mapRect.height / 2f
        );
    }

    void OnDestroy()
    {
        if (iconObject != null)
            Destroy(iconObject);
    }
}