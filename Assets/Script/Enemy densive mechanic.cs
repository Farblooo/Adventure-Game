using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class Enemydensivemechanic : MonoBehaviour
{
    public EnemyCombat enemyCombat;
    public CombatScript combatscript;
    public bool isInvincible = false;

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
        if (combatscript.AttackID != lastSeenAttackID)
        {
            if (dodgeCoroutine != null) { StopCoroutine(dodgeCoroutine); }
            dodgeCoroutine = StartCoroutine(DodgeRoutine());
            lastSeenAttackID = combatscript.AttackID;
        }
    }

    IEnumerator DodgeRoutine()
    {
        Debug.Log("Player fired a attack");
        yield return new WaitForSeconds(0.1f);
        isInvincible = true;
        yield return new WaitForSeconds(0.2f);
        isInvincible = false;
    }
}
