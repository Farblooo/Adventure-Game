using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class PlayerActor : MonoBehaviour
{
    public UnityEngine.UI.Image healthFill;
    public float maxHealth;
    public float currentHealth;
    float deathScreenFade = 0f;

    public CanvasGroup deathScreen;
    public AudioClip hurtSoundEffect;
    public AudioClip deathSoundEffect;
    public AudioClip gameoverSoundEffect;

    AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        healthFill.fillAmount = currentHealth / maxHealth;

        if (currentHealth > 0)
        {
            audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(hurtSoundEffect);
            audioSource.pitch = 1f;
        }

        if (currentHealth <= 0)
        {
            Death();
        }
    }

    void Death()
    {
        audioSource.pitch = 1f;
        audioSource.PlayOneShot(deathSoundEffect);

        StartCoroutine(PlayGameoverSoundEffectDelayed());

        StartCoroutine(DeathScreenFadeIn());
        //Death function
    }

    IEnumerator DeathScreenFadeIn()
    {
        while (deathScreenFade < 1f)
        {
            deathScreenFade += Time.deltaTime;
            deathScreen.alpha = deathScreenFade;
            deathScreenFade = Mathf.Clamp01(deathScreenFade);
            yield return null;
        }
        Time.timeScale = 0f;
    }

    IEnumerator PlayGameoverSoundEffectDelayed()
    {
        yield return new WaitForSeconds(0.5f);
        audioSource.PlayOneShot(gameoverSoundEffect);
    }
}

