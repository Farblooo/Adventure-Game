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

    
    float dashDuration = 0.3f;
    float dashTimer = 0f;
    bool canDash = true;
    bool isDashing = false;

    float normalFOV = 70f;
    float dashFOV = 95f;
    float targetFOV;

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
            StartCoroutine(DashCoroutine(-playerTransform.right));
        }
        else if (Input.GetKey(KeyCode.S))
        {
            StartCoroutine(DashCoroutine(-playerTransform.forward));
        }
        else if (Input.GetKey(KeyCode.D))
        {
            StartCoroutine(DashCoroutine(playerTransform.right));
        }
        else //if no direction is pressed then forward will be the default dash
        {
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
