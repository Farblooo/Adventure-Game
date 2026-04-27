using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class PlayerActor : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
    float deathScreenFade = 0f;

    public CanvasGroup deathScreen;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0)
        {
            Death();
        }
    }

    void Death()
    {
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
}

