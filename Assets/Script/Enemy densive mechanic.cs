using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Enemydensivemechanic : MonoBehaviour
{
    public EnemyCombat enemyCombat;
    public CombatScript combatscript;
    public bool isInvincible = false;
    public Transform enemyTransform;
    public Transform playerTransform;

    Coroutine dodgeCoroutine;

    float lastSeenAttackID;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyCombat.isEnraged) { return; }
        if (combatscript.AttackID != lastSeenAttackID && enemyCombat.Distance <= 5 && enemyCombat.readyToAttack)
        {
            if (dodgeCoroutine != null) { StopCoroutine(dodgeCoroutine); }
            dodgeCoroutine = StartCoroutine(DodgeRoutine());
            lastSeenAttackID = combatscript.AttackID;
            Debug.Log("enemy tried to dodge");
        }
    }

    IEnumerator DodgeRoutine()
    {
        int RandomNum = UnityEngine.Random.Range(0, 10);
        if (RandomNum < 5) { yield break; }
        enemyCombat.readyToAttack = false;
        yield return new WaitForSeconds(0.1f);
        isInvincible = true;
        yield return new WaitForSeconds(0.45f);
        isInvincible = false;
        enemyCombat.readyToAttack = true;
        if (enemyCombat.aggression < enemyCombat.attackThreshold)
        {
            RandomNum = UnityEngine.Random.Range(0, 3);
            switch (RandomNum)
            {
                case 0:
                    enemyCombat.Attack(1f);
                    break;
                case 1:
                    enemyCombat.ProjectileAttack(0.8f);
                    break;
                case 2:
                    break;
            }
        }
    }
}
