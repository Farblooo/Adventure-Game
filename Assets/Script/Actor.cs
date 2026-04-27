using System.Transactions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Actor : MonoBehaviour
{
    public UnityEngine.UI.Image healthFill;
    public float currentHealth;
    public float maxHealth;
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

        if (healthFill != null)
        {
            healthFill.fillAmount = currentHealth / maxHealth;
        }

        if (currentHealth <= 0)
        {
            Death();
        }
    }

    void Death()
    {
        //Death function
        //TEMPORARY destroy objects
        Destroy(gameObject);
    }
}
