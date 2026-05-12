using UnityEngine;
using UnityEngine.UIElements;

public class SettingandPause : MonoBehaviour
{
    public UnityEngine.UI.Image pauseScreenImage;
    Color pauseScreenImageColor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pauseScreenImageColor.a = 0f;
        pauseScreenImage.color = pauseScreenImageColor;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && Time.timeScale > 0.9f)
        {
            Debug.Log("Paused");
            Time.timeScale = 0f;
            pauseScreenImageColor.a = 0.7f;
            pauseScreenImage.color = pauseScreenImageColor;
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && Time.timeScale < 0.1f)
        {
            Debug.Log("Unpaused");
            Time.timeScale = 1f;
            pauseScreenImageColor.a = 0f;
            pauseScreenImage.color = pauseScreenImageColor;
        }
    }
}
