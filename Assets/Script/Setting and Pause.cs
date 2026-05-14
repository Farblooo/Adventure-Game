using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingandPause : MonoBehaviour
{
    [Header("Pause")]
    public UnityEngine.UI.Image pauseScreenImage;
    Color pauseScreenImageColor;

    [Header("Master volume")]
    public AudioMixer audioMixer;
    public Slider masterVolumeSlider;
    Color masterVolumeSliderColor;

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
            EnableMenu();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && Time.timeScale < 0.1f)
        {
            Debug.Log("Unpaused");
            Time.timeScale = 1f;
            DisableMenus();
        }
    }

    public void SetVolume(float volume)
    {
        volume = Mathf.Clamp(volume, 0.0001f, 1f);
        audioMixer.SetFloat("Master Volume", Mathf.Log10(volume) * 20);
    }

    void EnableMenu()
    {
        pauseScreenImageColor.a = 0.7f;
        pauseScreenImage.color = pauseScreenImageColor;
    }

    void DisableMenus()
    {
        pauseScreenImageColor.a = 0f;
        pauseScreenImage.color = pauseScreenImageColor;
    }
}
