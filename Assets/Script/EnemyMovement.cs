using System;
using TMPro;
using TreeEditor;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public Transform groundCheck;
    public Transform PlayerTransform;
    public LayerMask groundMask;

    Vector3 PlayerPosition;
    Vector3 EnemyPosition;
    Vector3 Direction;
    Vector3 currentDirection;
    float walkspeed = 5f;
    float Distance;
    float StrafeTimer;
    float StrafeDuration = 2f;
    float StrafeDirection;
    float groundDistance = 0.2f;
    float moveTimer = 0f;
    bool isStrafing = false;
    bool isGrounded;
    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PlayerPosition = PlayerTransform.position;
        EnemyPosition = transform.position;
        Direction = PlayerPosition - EnemyPosition;
        Distance = Direction.magnitude;


        if (moveTimer <= 0f)
        {
            moveTimer = UnityEngine.Random.Range(0.05f, 0.1f);
            if (Distance > 8 && Distance < 18) //if the enemy is not too far or too close then move closer to player
            {
                currentDirection = Direction.normalized;
                //EnemyPosition += Direction.normalized * walkspeed * Time.deltaTime; //normalize gives direction
            }
            else if (Distance <= 8 && Distance > 5) //slows down if closer to player
            {
                currentDirection = Direction.normalized / 2;
                //EnemyPosition += Direction.normalized * (walkspeed / 2) * Time.deltaTime;
            }
            else if (Distance < 3) //walk back if too too close
            {
                currentDirection = -Direction.normalized / 2;
                //EnemyPosition -= Direction.normalized * (walkspeed / 2) * Time.deltaTime;
            }
        }

        if (Distance <= 5 && Distance > 3) // starts strafing if too close
        {
            if (!isStrafing)
            {
                StrafeDirection = UnityEngine.Random.value < 0.5f ? -1f : 1f; //picks a random value, move right if positive and moves left if negative
                StrafeTimer = StrafeDuration;
                isStrafing = true;
            }

            currentDirection = (transform.right * StrafeDirection) / 3;
            //EnemyPosition += transform.right * StrafeDirection * (walkspeed / 3) * Time.deltaTime;

            StrafeTimer -= Time.deltaTime;
            if (StrafeTimer <= 0)
            {
                isStrafing = false;
            }
        }

        EnemyPosition += currentDirection * walkspeed * Time.deltaTime;

        moveTimer -= Time.deltaTime;

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask); //make sure the enemy is grounded
        if (!isGrounded)
        {
            EnemyPosition.y -= 0.1f;
        }

        transform.position = EnemyPosition;

        Direction.y = 0f;
        transform.rotation = Quaternion.LookRotation(Direction); //make the enemy always look at player
    }
}
