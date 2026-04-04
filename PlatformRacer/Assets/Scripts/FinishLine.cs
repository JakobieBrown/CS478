using UnityEngine;

public class FinishLine : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        transform.position = new Vector3(
            PlayerPrefs.GetInt("SelectedDistance"),
            transform.position.y,
            transform.position.z
        );

        Debug.Log("Awake set: " + transform.position);
    }

    void Start()
    {
        Debug.Log("Start sees: " + transform.position);
    }

    void Update()
    {
        Debug.Log("Update sees: " + transform.position);
    }
}
