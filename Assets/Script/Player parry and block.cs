using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;

public class Playerparryandblock : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public CombatScript combatScript;
    public Camera cam;
    Animator animator;

    public bool isParrying = false;

    public float parryWindow = 1f;

    float parryCoolDown = 1.5f;
    public bool canParry = true;

    public Renderer swordRenderer;
    public Color colorChange = Color.deepSkyBlue;
    Color originialColor;
    float changePercentage;

    public GameObject enemyProjectile;
    GameObject deflectedProjectile;

    public Transform reflectedProjectileFirePoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponentInChildren<Animator>();

        swordRenderer = transform.Find("Main Camera/hand and sword with animations/Cube.001")?.GetComponent<SkinnedMeshRenderer>();
        originialColor = swordRenderer.material.color;
    }

    // Update is called once per frame
    void Update()
    {
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
    }

    void ResetParry()
    {
        canParry = true;
    }

    IEnumerator Parry()
    {
        isParrying = true;

        swordRenderer.material.EnableKeyword("_EMISSION");
        swordRenderer.material.SetColor("_EmissionColor", colorChange);

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
        deflectedProjectile = Instantiate(enemyProjectile, reflectedProjectileFirePoint.position, cam.transform.rotation);
        deflectedProjectile.TryGetComponent<EnemyProjectile>(out EnemyProjectile projectile);
        projectile.damage += projectile.damage / 2;
        projectile.isDeflected = true;
        canParry = true;
        isParrying = false;
        StopAllCoroutines();
    }

    public void SuccessfulParryStrike()
    {
        canParry = true;
        isParrying = false;
        StopAllCoroutines();
    }
}
