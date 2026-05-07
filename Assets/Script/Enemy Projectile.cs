using UnityEngine;
using UnityEngine.SubsystemsImplementation;

public class EnemyProjectile : MonoBehaviour
{
    public Playerparryandblock playerParryandBlock;
    public float projectileSpeed;
    public float projectileLifetime;
    public int damage;


    public bool isDeflected = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, projectileLifetime); //despawn after awhile
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * projectileSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isDeflected)
        {
            if (other.TryGetComponent<PlayerActor>(out PlayerActor player))
            {
                other.TryGetComponent<Playerparryandblock>(out Playerparryandblock parry);
                if (parry.TryParry(transform.position))
                {
                    parry.enemyProjectile = gameObject;
                    parry.SuccessfulParry();
                    Debug.Log("Parried a projectile!");
                    Destroy(gameObject);
                    return;
                }
                player.TakeDamage(damage, AttackType.Projectile);
            }
            Destroy(gameObject);
        }

        if (other.CompareTag("Enemy") && isDeflected)
        {
            if (other.TryGetComponent<Actor>(out Actor enemy))
            {
                enemy.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}
