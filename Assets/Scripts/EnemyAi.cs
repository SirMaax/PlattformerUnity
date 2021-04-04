using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAi : MonoBehaviour
{

    [SerializeField] float detectionCircle;
    
    public Transform target;
    public LayerMask playerLayer;
    PlayerMovement player;
    Path path;
    Seeker seeker;
    Rigidbody2D rigidBody;

    public float speed = 200f;
    private int currentWayPoint = 0;
    public float nextWayPointDistance = 3f;
    bool reachedEndOfWayPoint = false;
    private bool playerInReach = false;


    
    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rigidBody = GetComponent<Rigidbody2D>();

        //              Name of Meth, wait time until its called, how often it is called
        InvokeRepeating("UpdatePath", 0f, .5f);
    }
    
    private void UpdatePath()
    {
        if (seeker.IsDone())
        {
            //Current Pos           // Target pos           //Function that is called upon reaching end of path
            seeker.StartPath(rigidBody.position, target.position, OnPathComplete);
        }
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWayPoint = 0;
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        //Checks if player is in Reach
        CheckIfPlayerIsInReach();
        
        if (playerInReach)
        {
            PathFinding();
        }

    }

    private void PathFinding()
    {
        if (path == null)
        {
            return;
        }
        //Total amount of wayPoints
        if (currentWayPoint >= path.vectorPath.Count)
        {
            reachedEndOfWayPoint = true;
            return;
        }
        else
        {
            reachedEndOfWayPoint = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWayPoint] - rigidBody.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        float distance = Vector2.Distance(rigidBody.position, path.vectorPath[currentWayPoint]);

        rigidBody.AddForce(force);

        if (distance < nextWayPointDistance)
        {
            currentWayPoint++;
        }
    }

    private void CheckIfPlayerIsInReach()
    {
        if (Physics2D.OverlapCircle(rigidBody.position, detectionCircle, playerLayer)) playerInReach = true;
        else playerInReach = false;
    }

    /* private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(rigidBody.position,detectionCircle);
    }       */
}
