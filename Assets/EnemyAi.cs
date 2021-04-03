using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAi : MonoBehaviour
{
    public Transform target;

    public float speed = 200f;
    int currentWayPoint = 0;
    public float nextWayPointDistance = 3f;
    bool reachedEndOfWayPoint = false;

    Path path;
    Seeker seeker;
    Rigidbody2D rigidbody;
    
    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rigidbody = GetComponent<Rigidbody2D>();

        //              Name of Meth, wait time until its called, how often it is called
        InvokeRepeating("UpdatePath", 0f, .5f);
    }
    
    private void UpdatePath()
    {
        if (seeker.IsDone())
        {
            //Current Pos           // Target pos           //Function that is called upon reaching end of path
            seeker.StartPath(rigidbody.position, target.position, OnPathComplete);
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
        if(path == null)
        {
            return;
        }
                            //Total amount of wayPoints
        if(currentWayPoint >= path.vectorPath.Count)
        {
            reachedEndOfWayPoint = true;
            return;
        }
        else
        {
            reachedEndOfWayPoint = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWayPoint] - rigidbody.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        float distance = Vector2.Distance(rigidbody.position, path.vectorPath[currentWayPoint]);

        rigidbody.AddForce(force);

        if(distance < nextWayPointDistance)
        {
            currentWayPoint++;
        }

    }
}
