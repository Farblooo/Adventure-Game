using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCombat : MonoBehaviour
{
    public Transform playerTransform;
    public GameObject hitbox;
    public float attackDelay = 0.4f;
    public float attackCooldown = 2f;
    public float aggression = 0f;
    public float attackThreshold = 0f;
    //public float maxHealth;
    //public float currentHealth;
    public int attackDmg = 1;
    public Actor actor;

    Vector3 enemyPos;
    Vector3 playerPos;
    float Distance;
    bool readyToAttack = true;
    bool isAttacking = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DisableHitbox();
        actor = GetComponent<Actor>();
    }

    // Update is called once per frame
    void Update()
    {
        enemyPos = transform.position;
        playerPos = playerTransform.position;

        Distance = (playerPos - enemyPos).magnitude;

        if (attackThreshold <= 0f) //only resets attack threshold after a attack
        {
            attackThreshold = Random.Range(0, 100);
        }

        if (Distance <= 5) //if the enemy is close then it would be more aggressive
        {
            aggression += (Time.deltaTime / 10);
        }
        else if(Distance > 5 && Distance < 18) //if enemy is too far away then it wouldn't be aggressive at all
        {
            aggression += (Time.deltaTime / 40);
        }

        if (aggression >= attackThreshold / 100f && Distance <= 5 && readyToAttack) //if aggression is higher than randomly generated threshold then the enemy would attack
        {
            Attack();
        }
        
    }

    void Attack()
    {
        isAttacking = true;
        readyToAttack = false;

        Debug.Log("Enemy tried to attack!");
        Invoke(nameof(EnableHitbox), 0.2f);
        Invoke(nameof(DisableHitbox), 0.5f);
        Invoke(nameof(ResetAttack), attackCooldown);

        aggression = 0f;
        attackThreshold = 0f;
    }

    public void OnHitPlayer(Collider Player)
    {
        if (!isAttacking) return;

        if (Player.TryGetComponent<PlayerActor>(out PlayerActor T))
        {
            T.TakeDamage(attackDmg);
            Debug.Log($"Player HP: {T.currentHealth}");
        }

        isAttacking= false;
    }

    void ResetAttack()
    {
        readyToAttack = true;
    }

    void EnableHitbox()
    {
        hitbox.SetActive(true);
    }

    void DisableHitbox()
    {
        hitbox.SetActive(false);
    }    
    

}
