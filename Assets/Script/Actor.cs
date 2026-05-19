using System.Transactions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Actor : MonoBehaviour
{
    public EnemyCombat enemyCombat;
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
        float floatAmount = amount;
        enemyCombat.ModifyTemper(floatAmount * 0.05f);
        enemyCombat.RestartTemperReduction();

        if (enemyCombat.isEnraged)
        {
            amount = amount * 2;
        }

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
