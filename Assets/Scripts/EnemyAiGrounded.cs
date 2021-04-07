using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAiGrounded : MonoBehaviour
{
    [SerializeField] float detectionCircle;

    public Transform target;
    Rigidbody2D rb;
    private LayerMask playerLayer;

    private float targetDirection = 0f;
    [SerializeField] float movementSpeed;
    [SerializeField] float speedLimit;
    [SerializeField] float normalMovementSpeed;
    public Transform pathPost1;                     //Left patrol point
    public Transform pathPost2;                     //Right patrol point
    public Transform nextTarget;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerLayer = 3;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (CheckIfTargetIsInRange())
        {
            if (target.position.x > rb.position.x) targetDirection = 1f;
            else targetDirection = -1f;

            rb.AddForce(new Vector2(targetDirection * movementSpeed, 0));
            ClampSpeed();
        }else
        {       
            SelectNewPatrolTarget();
            TravelToCheckPoint();
         }

    }

    private bool CheckIfTargetIsInRange()
    {
        if (Physics2D.OverlapCircle(target.position, detectionCircle, playerLayer)) return true;

        return false;
    }

    /*private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(rb.position, detectionCircle);
    }   */
 
    private void ClampSpeed()
    {
        float temp = rb.velocity.x;
        if (Mathf.Abs(temp) >= Mathf.Abs(speedLimit))
            rb.velocity = new Vector2(speedLimit, 0);
    }

    private void TravelToCheckPoint()
    {
        if(nextTarget.position.x < rb.position.x)
        {
            rb.AddForce(new Vector2(-movementSpeed/ normalMovementSpeed, 0));
        }
        else rb.AddForce(new Vector2(movementSpeed/ normalMovementSpeed, 0));
    }
    private void SelectNewPatrolTarget()
    {
        if (nextTarget == null) nextTarget = pathPost1;
        if (nextTarget == pathPost1 && rb.position.x < pathPost1.position.x) nextTarget = pathPost2;
        else if (nextTarget == pathPost2 && rb.position.x > pathPost2.position.x) nextTarget = pathPost1;
    }
}
