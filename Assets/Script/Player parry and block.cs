using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class Playerparryandblock : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public CombatScript combatScript;
    public Camera cam;
    public AudioSource audioSource;
    public AudioClip parrySoundEffect;

    public bool isParrying = false;

    public float parryWindow = 1f;

    float parryCoolDown = 1f;
    public bool canParry = true;

    [Header("Render")]
    public Renderer swordRenderer;
    public Color colorChange = Color.deepSkyBlue;
    Color originialColor;
    float changePercentage;

    public GameObject enemyProjectile;
    GameObject deflectedProjectile;

    public Transform reflectedProjectileFirePoint;


    float finalParryTimeScale = 1f;
    float startParryTimeScale = 0.5f;

    //--------//
    //BLOCKING//
    //--------//

    public bool blocking = false;

    [Header("Animation")]
    public Animator animator;

    public const string PARRY = "Armature_R|Player_Parry_001";
    public const string PARRY_SUCCESS = "Armature_R|Player_Parry_Success";
    public const string IDLE = "Armature_R|Player_idle";

    string currentAnimationState;
    public bool lockedAnimation = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originialColor = swordRenderer.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        SetAnimations();
        if (Input.GetKeyDown(KeyCode.E) && canParry && combatScript.readyToAttack)
        {
            canParry = false;
            Invoke(nameof(ResetParry), parryCoolDown);
            StartCoroutine(Parry());
            Debug.Log("Tried to parry");
        }

        if (!canParry)
        {
            playerMovement.speed = playerMovement.speed / 3f;
        }

        if (Input.GetKey(KeyCode.Mouse1) && canParry && combatScript.readyToAttack)
        {
            blocking = true;
            foreach (Material mat in swordRenderer.materials)
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", Color.green);
            }
        }
    }

    void ResetParry()
    {
        canParry = true;
    }

    IEnumerator Parry()
    {
        isParrying = true;

        ChangeAnimationState(PARRY);
        lockedAnimation = true;
        Invoke(nameof(UnlockAnimation), 2.3f);

        foreach (Material mat in swordRenderer.materials)
        {
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", colorChange);
        }

        yield return new WaitForSeconds(parryWindow);

        isParrying = false;
    }

    public bool TryParry(Vector3 attackPos)
    {
        if (!isParrying) return false;

        Vector3 direction = (attackPos - transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, direction);

        return dot > 0.35f;
    }

    public void SuccessfulParry()
    {
        deflectedProjectile = Instantiate(enemyProjectile, reflectedProjectileFirePoint.position, cam.transform.rotation);
        deflectedProjectile.TryGetComponent<EnemyProjectile>(out EnemyProjectile projectile);
        projectile.damage += projectile.damage / 2;
        projectile.isDeflected = true;
        canParry = true;
        isParrying = false;
        StopAllCoroutines();
        StartCoroutine(ParrySlowDown());
        audioSource.PlayOneShot(parrySoundEffect);
        UnlockAnimation();
        ChangeAnimationState(PARRY_SUCCESS);
    }

    public void SuccessfulParryStrike()
    {
        canParry = true;
        isParrying = false;
        StopAllCoroutines();
        StartCoroutine(ParrySlowDown());
        audioSource.PlayOneShot(parrySoundEffect);
        ChangeAnimationState(PARRY_SUCCESS);
    }

    IEnumerator ParrySlowDown()
    {
        float elapsed = 0f;

        while (elapsed < 1f)
        {
            elapsed += Time.unscaledDeltaTime;

            Time.timeScale = Mathf.Lerp(startParryTimeScale, finalParryTimeScale, elapsed / 1f);
            yield return null;
        }

        Time.timeScale = 1f;

    }

    public void ChangeAnimationState(string newState)
    {
        if (currentAnimationState == newState) return; //stop the same animation from interrupting itself

        currentAnimationState = newState;
        animator.CrossFadeInFixedTime(currentAnimationState, 0.1f);
    }

    void SetAnimations()
    {
        if (combatScript.readyToAttack && canParry && !lockedAnimation)
        {
            ChangeAnimationState(IDLE);
        }
    }

    void UnlockAnimation()
    {
        lockedAnimation = false;
    }
}
