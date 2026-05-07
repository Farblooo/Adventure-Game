using System;
using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class CombatScript : MonoBehaviour
{
    public Camera cam;
    public AudioSource windupAudioSource;
    public AudioSource audioSource;
    public PlayerMovement playerMovement;
    public Playerparryandblock playerParryandblock;

    //=========//
    //ANIMATION//
    //=========//
    Animator animator;

    public const string IDLE = "Player_idle";
    public const string ATTACK1 = "Player_Sword_Swing1";
    public const string ATTACK2 = "Player_Sword_Swing2";
    public const string ATTACK1WINDUP = "Player_Sword_Swing1_001";
    public const string ATTACK1RELEASE = "Player_Sword_Swing1_Release";
    public const string ATTACK2WINDUP = "Player_Sword_Swing2_Windup";
    public const string ATTACK2RELEASE = "Player_Sword_Swing2_002";

    string currentAnimationState;

    //=========//
    //ATTACKING//
    //=========//
    [Header("Attacking")]
    public float attackDistance = 3f;
    public float attackDelay = 0.4f;
    public float attackSpeed = 1f;
    public int attackDmg = 0;
    public LayerMask attackLayer;

    public GameObject hitEffect;
    public AudioClip swordSwing;
    public AudioClip hitSound;
    public AudioClip finishChargedSound;

    bool attacking = false;
    public bool readyToAttack = true;
    public bool isCharging = false;
    bool chargeSoundPlayed = false;
    int attackCount;
    float currentCharge = 0f;
    float chargeTime = 1f;
    float standardSpeed;


    //==================//
    //SWORD COLOR CHANGE//
    //==================//
    [Header("Sword Color")]
    public Renderer swordRenderer;
    public Color changeColor = Color.red;
    Color originalColor;
    float chargePercentage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = Camera.main;
        animator = GetComponentInChildren<Animator>();
        swordRenderer = transform.Find("Main Camera/hand and sword with animations/Cube.001")?.GetComponent<SkinnedMeshRenderer>();

        originalColor = swordRenderer.material.color;

        playerMovement = GetComponent<PlayerMovement>();

        standardSpeed = playerMovement.speed;
    }

    // Update is called once per frame
    void Update()
    {
        SetAnimations();


        if (Input.GetMouseButtonDown(0) && readyToAttack && !attacking) //If the player press/hold left click
        {
            isCharging = true;
            currentCharge = 0f;
            chargeSoundPlayed = false;            

            if (attackCount ==0 && readyToAttack && !attacking) //detect if attack is available and play windup animation accoding to attack count
            {
                readyToAttack = false;
                attacking = true;
                ChangeAnimationState(ATTACK1WINDUP);
            }
            else if(attackCount == 1 && readyToAttack && !attacking)
            {
                readyToAttack = false;
                attacking = true;
                ChangeAnimationState(ATTACK2WINDUP);
            }
        }

        if (isCharging && Input.GetMouseButton(0)) //if left click is hold down then increase current charge and plays a sound cue if fully charged
        {
            currentCharge += Time.deltaTime;

            if (currentCharge >= chargeTime && !chargeSoundPlayed) //play sound cue
            {
                audioSource.PlayOneShot(finishChargedSound);
                chargeSoundPlayed = true;
            }

        }

        if (Input.GetMouseButtonUp(0) && attacking && isCharging) //plays out attack
        {
            isCharging = false;

            if(currentCharge >= chargeTime) 
            {
                ChargeAttack();
            }
            else
            {
                Attack();
            }
        }

        if (isCharging && !windupAudioSource.isPlaying) //play windup sound when charging
        {
            windupAudioSource.Play();
        }
        if ((!isCharging || currentCharge >= chargeTime) && windupAudioSource.isPlaying) //stop windup sound when stop charging or fully charged
        {
            windupAudioSource.Stop();
        }

        if (isCharging)
        {
            chargePercentage = (currentCharge - 0.2f) / chargeTime;
            chargePercentage = Mathf.Clamp01(chargePercentage);

            swordRenderer.material.EnableKeyword("_EMISSION");
            swordRenderer.material.SetColor("_EmissionColor", changeColor * chargePercentage * 1.8f);
        }
        else if (!playerParryandblock.isParrying)
        {
            swordRenderer.material.color = originalColor;
            swordRenderer.material.SetColor("_EmissionColor", Color.black);
        }

        if (isCharging)
        {
            playerMovement.speed = standardSpeed / 2f;
        }
        else
        {
            playerMovement.speed = standardSpeed;
        }
    }

    public void Attack()
    {
        readyToAttack = false;
        attacking = true;
        attackDmg = 1;

        Invoke(nameof(ResetAttack), attackSpeed);
        Invoke(nameof(AttackRaycast), attackDelay);

        audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(swordSwing);
        audioSource.pitch = 1f;

        if (attackCount == 0)
        {
            ChangeAnimationState(ATTACK1RELEASE);
            attackCount++;
        }
        else
        {
            ChangeAnimationState(ATTACK2RELEASE);
            attackCount = 0;
        }
        
    }

    public void ChargeAttack()
    {
        readyToAttack = false;
        attacking = true;
        attackDmg = 2;

        Invoke(nameof(ResetAttack), attackSpeed);
        Invoke(nameof(AttackRaycast), attackDelay);

        audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(swordSwing);
        audioSource.pitch = 1f;

        if (attackCount == 0)
        {
            ChangeAnimationState(ATTACK1RELEASE);
            attackCount++;
        }
        else
        {
            ChangeAnimationState(ATTACK2RELEASE);
            attackCount = 0;
        }

    }

    void ResetAttack()
    {
        attacking = false;
        readyToAttack = true;
    }

    void AttackRaycast()
    {

        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, attackDistance, attackLayer))
        {
            HitTarget(hit.point);

            if (hit.transform.TryGetComponent<Actor>(out Actor T)) //out Actor T mean if founded the component than store it in the T variable
            { T.TakeDamage(attackDmg); }
        }

        
    }

    void HitTarget(Vector3 pos)
    {
        audioSource.pitch = 1;
        audioSource.PlayOneShot(hitSound);

        GameObject GO = Instantiate(hitEffect, pos, Quaternion.identity); //Quaternion.identity means no rotation
        Destroy(GO, 2);
    }

    public void ChangeAnimationState(string newState)
    {
        if (currentAnimationState == newState) return; //stop the same animation from interrupting itself

        currentAnimationState = newState;
        animator.CrossFadeInFixedTime(currentAnimationState, 0.2f);
    }

    void SetAnimations()
    {
        if(!attacking)
        {
            ChangeAnimationState(IDLE);
        }
    }
} 
