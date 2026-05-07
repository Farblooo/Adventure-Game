using System;
using System.Collections;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class PlayerActor : MonoBehaviour
{
    public DashScript dashScript;
    public EnemyCombat enemyCombat;
    public UnityEngine.UI.Image healthFill;
    public UnityEngine.UI.Image hurtImage;
    public float maxHealth;
    public float currentHealth;
    float deathScreenFade = 0f;

    public CanvasGroup deathScreen;
    public AudioClip hurtSoundEffect;
    public AudioClip deathSoundEffect;
    public AudioClip gameoverSoundEffect;

    AudioSource audioSource;
    Color hurtImageColor;
    Coroutine hurtFadeRoutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hurtImageColor = hurtImage.color;
        currentHealth = maxHealth;
        hurtImageColor.a = 0f;
        hurtImage.color = hurtImageColor;

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void TakeDamage(int amount, AttackType attackType)
    {
        if (dashScript.isDashing == true)
        {
            if (dashScript.currentDashType == DashScript.dashType.sideDash && attackType == AttackType.Strike)
            {
                Debug.Log("Dodged a strike");
                return;
            }
            else if (dashScript.currentDashType == DashScript.dashType.forwardDash)
            {
                Debug.Log("Dodged a attack");
                return;
            }
        }
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        healthFill.fillAmount = currentHealth / maxHealth; //Modify healthbar


        if (currentHealth / maxHealth < 0.77 && hurtImageColor.a <= 0.3) //Hurt effect starts stronger the lower health is
        {
            hurtImageColor.a = 0.3f;
        }
        else if (currentHealth / maxHealth < 0.5 && hurtImageColor.a <= 0.5)
        {
            hurtImageColor.a = 0.5f;
        }
        else if (currentHealth / maxHealth < 0.77 && hurtImageColor.a <= 0.7)
        {
            hurtImageColor.a = 0.7f;
        }

        hurtImageColor.a += 0.2f; //Indicate when take damage
        hurtImage.color = hurtImageColor;


        if (hurtFadeRoutine != null)
        {
            StopCoroutine(hurtFadeRoutine);
        }

        hurtFadeRoutine = StartCoroutine(HurtEffectFade());

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

    IEnumerator HurtEffectFade()
    {
        yield return new WaitForSeconds(1f);
        while (hurtImageColor.a > 0)
        {
            hurtImageColor.a -= 0.05f * Time.deltaTime;
            hurtImage.color = hurtImageColor;
            hurtImageColor.a = Mathf.Clamp01(hurtImageColor.a);
            yield return null;
        }
    }
}

