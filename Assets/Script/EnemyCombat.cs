using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum AttackType {Strike, Projectile};

public class EnemyCombat : MonoBehaviour
{
    private Coroutine TemperRegenRoutine;

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
    public int attackDmg = 1;
    public Actor actor;

    Vector3 enemyPos;
    Vector3 playerPos;
    Vector3 direction;
    public float Distance;
    float normalWalkSpeed;
    float projectileAggression;
    public bool readyToAttack = true;
    bool isAttacking = false;

    //------//
    //TEMPER//
    //------//

    public float temper = 1f;
    public Image temperFill;
    public bool isEnraged = false;
    Coroutine passiveTemperReductionCoroutine;
    public AudioSource audioSource;
    public AudioClip enragedSoundEffect;

    //-------//
    //TESTING//
    //-------//
    public Image aggroFill;

    [Header("Animation")]
    public const string ATTACK = "Armature|Snake_Attack_001";
    public const string PROJECTILE = "Armature|Snake_Projectile";

    [Header("Custom enemy setting")]
    public float CustomEnemyAttackDelay;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DisableHitbox();
        actor = GetComponent<Actor>();
        enemyMovement = GetComponent<EnemyMovement>();

        normalWalkSpeed = enemyMovement.walkspeed;

        originalColor = enemyRenderer.material.color;

        temperFill.fillAmount = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (temper < 1f) { temper = 1f; } //making sure temper doesn't go below 1
        if (temper > 2f) { temper = 2f; } //making sure temper doesn't go above 2
        if (temper >= 1.5f && !isEnraged)
        {
            Debug.Log("Is enraged!");
            StartCoroutine(Enraged());
            if (TemperRegenRoutine != null)
                StopCoroutine(TemperRegenRoutine);
        }

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
            aggression += (Time.deltaTime / 10 * temper);
        }
        else if(Distance > 5 && Distance < 18) //if enemy is too far away then it wouldn't be aggressive at all
        {
            aggression += (Time.deltaTime / 40 * temper);
            projectileAggression += (Time.deltaTime / 10 * temper);
        }

        if (aggression >= attackThreshold / 100f && Distance <= 5 && readyToAttack) //if aggression is higher than randomly generated threshold then the enemy would attack
        {
            Attack(0.4f);
        }
        if (projectileAggression >= attackThreshold / 100 && Distance >= 7 && readyToAttack)
        {
            ProjectileAttack(0.3f);
        }

        aggroFill.fillAmount = aggression;
        temperFill.fillAmount = temper - 1f;

    }

    public void Attack(float AttackDelay)
    {
        enemyMovement.lockedAnimation = true;
        enemyMovement.ChangeAnimationState(ATTACK);
        Invoke(nameof(EnemyMovementUnlockAnimation), 1f);
        currentAttackType = AttackType.Strike;
        isAttacking = true;
        readyToAttack = false;

        Debug.Log("Enemy tried to strike attack!");
        Invoke(nameof(EnableHitbox), AttackDelay + CustomEnemyAttackDelay); //attack takes 0.4 second to come out and attack stay active for 0.2 second
        Invoke(nameof(DisableHitbox), AttackDelay + 0.2f + CustomEnemyAttackDelay); //disable attack after 0.2 second
        Invoke(nameof(ResetAttack), attackCooldown);
        StartCoroutine(FlashRoutine(Color.red)); //indicate when enemy attack
        StartCoroutine(AttackLeapAnimation());

        aggression = 0f;
        attackThreshold = 0f;
    }

    public void ProjectileAttack(float ProjectileAttackDelay)
    {
        enemyMovement.lockedAnimation = true;
        enemyMovement.ChangeAnimationState(PROJECTILE);
        Invoke(nameof(EnemyMovementUnlockAnimation), 1.3f);
        currentAttackType = AttackType.Projectile;
        isAttacking = true;
        readyToAttack = false;

        Debug.Log("Enemy tried to throw a projectile attack!");
        Invoke(nameof(FireProjectile), ProjectileAttackDelay); //projectile takes 0.3 seconds to come out
        Invoke(nameof(ResetAttack), attackCooldown);
        StartCoroutine(FlashRoutine(Color.green));

        projectileAggression = 0f;
        attackThreshold = 0f;
    }

    public void OnHitPlayer(Collider Player) //if hitbox collides with player while active
    {
        if (!isAttacking) return;


        if (Player.TryGetComponent<Playerparryandblock>(out Playerparryandblock parry)) //if player is parrying
        {
            if (parry.TryParry(transform.position)) //detect if the parry is valid
            {
                Debug.Log("Parried a strike!");
                isAttacking = false;
                parry.SuccessfulParryStrike();
                ModifyTemper(1f);
                RestartTemperReduction();
                return;
            }
        }

        if (Player.TryGetComponent<PlayerActor>(out PlayerActor T))
        {
            T.TakeDamage(attackDmg, currentAttackType);
            if (parry.blocking) 
            { 
                ModifyTemper(-0.3f);
                RestartTemperReduction();
            }
            else 
            { 
                ModifyTemper(-0.1f);
                RestartTemperReduction();
            }
            Debug.Log($"Player HP: {T.currentHealth}");
        }

        if (Player.TryGetComponent<DashScript>(out DashScript dashscript))
        {
            if (dashscript.isDashing == true)
            {
                ModifyTemper(0.2f);
                RestartTemperReduction();
            }
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

    public void ModifyTemper(float amount)
    {
        if (isEnraged) { return; }
        temper += amount;
    }

    public void RestartTemperReduction()
    {
        if (TemperRegenRoutine != null)
        {
            StopCoroutine(TemperRegenRoutine);
        }
        TemperRegenRoutine = StartCoroutine(PassiveTemperReduction());
    }

    public IEnumerator PassiveTemperReduction()
    {
        if (isEnraged)
        {
            yield break;
        }
        yield return new WaitForSeconds(2f);
        while (true)
        {
            temper -= 0.02f * Time.deltaTime;
            yield return null;
        }
        
    }

    IEnumerator Enraged()
    {
        audioSource.PlayOneShot(enragedSoundEffect);
        int storedDmg = attackDmg;
        isEnraged = true;
        attackDmg += attackDmg / 2;
        yield return new WaitForSeconds(10);
        isEnraged = false;
        attackDmg = storedDmg;
        temper = 1f;
    }

    public void EnemyMovementUnlockAnimation()
    {
        enemyMovement.UnlockAnimation();
    }
}
