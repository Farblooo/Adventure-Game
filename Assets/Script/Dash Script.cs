using System.Collections;
using UnityEngine;
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
    }

    void DashDirection()
    {
        if (Input.GetKey(KeyCode.A)) //determine dash direction
        {
            currentDashType = dashType.sideDash;
            StartCoroutine(DashCoroutine(-playerTransform.right)); //left dash
        }
        else if (Input.GetKey(KeyCode.S))
        {
            currentDashType = dashType.backDash;
            StartCoroutine(DashCoroutine(-playerTransform.forward));//back dash
        }
        else if (Input.GetKey(KeyCode.D))
        {
            currentDashType = dashType.sideDash;
            StartCoroutine(DashCoroutine(playerTransform.right));//right dash
        }
        else //if no direction is pressed then forward will be the default dash
        {
            currentDashType = dashType.forwardDash;
            StartCoroutine(DashCoroutine(playerTransform.forward));
        }
    }

    IEnumerator DashCoroutine(Vector3 direction)
    {
        canDash = false; 
        dashTimer = 0f;

        while (dashTimer < dashDuration) //actual dash
        {
            playerTransform.position += direction * dashSpeed * Time.deltaTime;
            dashTimer += Time.deltaTime;
            yield return null;
        }
        isDashing = false;
        yield return new WaitForSeconds(dashCoolDown); //put dash on cooldown
        canDash = true;
    }
}
