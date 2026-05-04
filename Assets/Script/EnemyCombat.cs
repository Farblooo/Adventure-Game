using System.Collections;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum AttackType { Strike, Projectile};

public class EnemyCombat : MonoBehaviour
{
    public AttackType currentAttackType;
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
    bool readyToAttack = true;
    bool isAttacking = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DisableHitbox();
        actor = GetComponent<Actor>();
        enemyMovement = GetComponent<EnemyMovement>();

        normalWalkSpeed = enemyMovement.walkspeed;
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
        currentAttackType = AttackType.Strike;
        isAttacking = true;
        readyToAttack = false;

        Debug.Log("Enemy tried to attack!");
        Invoke(nameof(EnableHitbox), 0.2f);
        Invoke(nameof(DisableHitbox), 0.5f);
        Invoke(nameof(ResetAttack), attackCooldown);
        StartCoroutine(AttackLeapAnimation());

        aggression = 0f;
        attackThreshold = 0f;
    }

    public void OnHitPlayer(Collider Player)
    {
        if (!isAttacking) return;

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
    IEnumerator AttackLeapAnimation()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = transform.position + transform.forward * 2f;

        direction = endPos - startPos;

        transform.position += direction.normalized;

        yield return new WaitForSeconds(1f);

        transform.position = startPos;
    }

}
