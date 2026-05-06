using System.Collections;
using UnityEngine;

public class Playerparryandblock : MonoBehaviour
{
    Animator animator;

    public bool isParrying = false;

    public float parryWindow = 0.1f;
    public const string PARRY = "Player_sword_parry";

    float parryCoolDown = 1.5f;
    public bool canParry = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canParry)
        {
            canParry = false;
            Invoke(nameof(ResetParry), parryCoolDown);
            StartCoroutine(Parry());
            Debug.Log("Tried to parry");
        }
    }

    void ResetParry()
    {
        canParry = true;
    }

    IEnumerator Parry()
    {
        animator.CrossFadeInFixedTime(PARRY, 0.2f);
        isParrying = true;

        yield return new WaitForSeconds(parryWindow);

        isParrying = false;
    }

    public bool TryParry(Vector3 attackPos)
    {
        if (!isParrying) return false;

        Vector3 direction = (attackPos - transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, direction);

        return dot > 0.5f;
    }

    public void SuccessfulParry()
    {
        canParry = true;
        StopAllCoroutines();
    }
}
