using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class DashScript : MonoBehaviour
{
    public Camera cam;
    public CombatScript combatScript;
    public Transform playerTransform;
    public float dashSpeed;
    public float dashCoolDown;
    public enum dashType {forwardDash, sideDash, backDash};
    public dashType currentDashType;

    float dashDuration = 0.2f;
    float dashTimer = 0f;
    bool canDash = true;
    public bool isDashing = false;

    float normalFOV = 70f;
    float dashFOV = 95f;
    float fovSpeed = 2f;

    bool isBackDashing = false;
    Coroutine currentDashRoutine;
    Coroutine backDashRoutine;

    [Header("Dash Distance")]
    public float forwardDashDistance = 10f;
    public float sideDashDistance = 20f;
    public float backdashDistance = 6f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && combatScript.isCharging == false) //dash if allowed when player press shift
        {
            DashDirection();
            isDashing = true;
        }

        if (isDashing)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, dashFOV, Time.deltaTime * fovSpeed);
        }
        else
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, normalFOV, Time.deltaTime * fovSpeed);
        }

        if (Input.GetKey(KeyCode.Mouse0) && isBackDashing)
        {
            currentDashRoutine = StartCoroutine(DashCoroutine(playerTransform.forward, forwardDashDistance));
        }
    }

    void DashDirection()
    {
        if (Input.GetKey(KeyCode.A)) //determine dash direction
        {
            currentDashType = dashType.sideDash;
            currentDashRoutine = StartCoroutine(DashCoroutine(-playerTransform.right, sideDashDistance)); //left dash
        }
        else if (Input.GetKey(KeyCode.S))
        {
            isBackDashing = true;
            currentDashType = dashType.backDash;
            currentDashRoutine = StartCoroutine(DashCoroutine(-playerTransform.forward, backdashDistance));//back dash
        }
        else if (Input.GetKey(KeyCode.D))
        {
            currentDashType = dashType.sideDash;
            currentDashRoutine = StartCoroutine(DashCoroutine(playerTransform.right, sideDashDistance));//right dash
        }
        else //if no direction is pressed then forward will be the default dash
        {
            currentDashType = dashType.forwardDash;
            currentDashRoutine = StartCoroutine(DashCoroutine(playerTransform.forward, forwardDashDistance));
        }
    }

    IEnumerator DashCoroutine(Vector3 direction, float dashDistance)
    {
        if (currentDashRoutine != null)
        {
            StopCoroutine(currentDashRoutine);
        }

        canDash = false; 
        dashTimer = 0f;

        while (dashTimer < dashDuration) //actual dash
        {
            playerTransform.position += direction * dashDistance * Time.deltaTime;
            dashTimer += Time.deltaTime;
            yield return null;
        }
        isDashing = false;
        isBackDashing = false;
        yield return new WaitForSeconds(dashCoolDown); //put dash on cooldown
        canDash = true;
    }
}
