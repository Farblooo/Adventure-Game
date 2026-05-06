using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum AttackType { Strike, Projectile};

public class EnemyCombat : MonoBehaviour
{
    public AttackType currentAttackType;
    public Renderer enemyRenderer;
    Color originalColor;
    public GameObject projectile;
    public Transform projectileFirepoint;
    public EnemyMovement enemyMovement;
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
    Vector3 direction;
    float Distance;
    float normalWalkSpeed;
    float projectileAggression;
    bool readyToAttack = true;
    bool isAttacking = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DisableHitbox();
        actor = GetComponent<Actor>();
        enemyMovement = GetComponent<EnemyMovement>();

        normalWalkSpeed = enemyMovement.walkspeed;

        originalColor = enemyRenderer.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        enemyPos = transform.position;
        playerPos = playerTransform.position;

        Distance = (playerPos - enemyPos).magnitude;

        if (!readyToAttack) //Stops enemy from walking when attacking
        {
            enemyMovement.walkspeed = 0f;
        }
        else
        {
            enemyMovement.walkspeed = normalWalkSpeed;
        }

        if (attackThreshold <= 0f) //only resets attack threshold after a attack
        {
            attackThreshold = UnityEngine.Random.Range(0, 100);
        }

        if (Distance <= 5) //if the enemy is close then it would be more aggressive
        {
            aggression += (Time.deltaTime / 10);
        }
        else if(Distance > 5 && Distance < 18) //if enemy is too far away then it wouldn't be aggressive at all
        {
            aggression += (Time.deltaTime / 40);
            projectileAggression += (Time.deltaTime / 10);
        }

        if (aggression >= attackThreshold / 100f && Distance <= 5 && readyToAttack) //if aggression is higher than randomly generated threshold then the enemy would attack
        {
            Attack();
        }
        if (projectileAggression >= attackThreshold / 100 && Distance >= 7 && readyToAttack)
        {
            ProjectileAttack();
        }
    }

    void Attack()
    {
        currentAttackType = AttackType.Strike;
        isAttacking = true;
        readyToAttack = false;

        Debug.Log("Enemy tried to strike attack!");
        Invoke(nameof(EnableHitbox), 0.4f);
        Invoke(nameof(DisableHitbox), 0.6f);
        Invoke(nameof(ResetAttack), attackCooldown);
        StartCoroutine(FlashRoutine(Color.red));
        StartCoroutine(AttackLeapAnimation());

        aggression = 0f;
        attackThreshold = 0f;
    }

    void ProjectileAttack()
    {
        currentAttackType = AttackType.Projectile;
        isAttacking = true;
        readyToAttack = false;

        Debug.Log("Enemy tried to throw a projectile attack!");
        Invoke(nameof(FireProjectile), 0.2f);
        Invoke(nameof(ResetAttack), attackCooldown);
        StartCoroutine(FlashRoutine(Color.green));

        projectileAggression = 0f;
        attackThreshold = 0f;
    }

    public void OnHitPlayer(Collider Player)
    {
        if (!isAttacking) return;


        if (Player.TryGetComponent<Playerparryandblock>(out Playerparryandblock parry))
        {
            if (parry.TryParry(transform.position))
            {
                Debug.Log("Parried a strike!");
                isAttacking = false;
                parry.SuccessfulParry();
                return;
            }
        }

        if (Player.TryGetComponent<PlayerActor>(out PlayerActor T))
        {
            T.TakeDamage(attackDmg, currentAttackType);
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

    void FireProjectile()
    {
        Instantiate(projectile, projectileFirepoint.position, projectileFirepoint.rotation);
    }

    IEnumerator AttackLeapAnimation()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = transform.position + transform.forward * 2f;

        direction = endPos - startPos;

        transform.position += direction.normalized;

        yield return new WaitForSeconds(1f);

        transform.position = startPos;
    }

    IEnumerator FlashRoutine(Color flashColor)
    {
        enemyRenderer.material.color = flashColor;

        yield return new WaitForSeconds(2f);

        enemyRenderer.material.color = originalColor;
    }

}
