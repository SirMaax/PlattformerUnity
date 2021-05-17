using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAi : MonoBehaviour
{
    [SerializeField] float health;
    public Rigidbody2D rb;
    public Transform leftFromBoss;
    public Transform rightFromBoss;

    public PlayerMovement player;

    //Burrowing from left to right
    public float maxDistanceToTheSide = 3;
    public float minDistanceToTheSide = 1;
    public Vector2 nextSurfacePosition;
    public float particleMovementSpeed = 3f;
    public ParticleSystem movingParticleSystem;
    public bool movingToNewPoint = false;
    // Start is called before the first frame update
    void Start()
    {
        nextSurfacePosition.y = -1;
        //startBurrow();
        SurfaceAtDiffrentLocation();

    }
    private void Update()
    {
        //CheckIfPlayerIsNear();
    }

 
    void FixedUpdate()
    {
        if (movingToNewPoint) MoveParticleSystem();
    }
    public IEnumerator startBurrow()
    {
        SurfaceAtDiffrentLocation();
        yield return new WaitForSeconds(8f);
    }
    private void CheckIfPlayerIsNear()
    {
        float playerX = player.transform.position.x;
        float playerY = player.transform.position.y;

        if (playerX > leftFromBoss.position.x && playerX < rb.position.x && playerY < leftFromBoss.position.y) Debug.Log("Player is left from Boss");//Do light attack to the left;
        else if (playerX > rb.position.x && playerX < rightFromBoss.position.x  && playerY < rightFromBoss.position.y) Debug.Log("Player is right from Boss");//Do light attack to the left;

    }
    private void SurfaceAtDiffrentLocation()
    {
        nextSurfacePosition.x = Random.Range(minDistanceToTheSide - rb.position.x, maxDistanceToTheSide + rb.position.x);
        movingToNewPoint = true;
        Debug.Log("Setet new Location at " + nextSurfacePosition.x);
        

    }
    //Only do this while something is false or true ^^
    private void MoveParticleSystem()
    {
        if (rb.position.x > nextSurfacePosition.x - particleMovementSpeed && rb.position.x < nextSurfacePosition.x + particleMovementSpeed)
        {
            movingToNewPoint = false;
            SurfaceAtDiffrentLocation();
            Debug.Log("Reached destination!, setting new point");
            return;
        }
        if(rb.position.x < nextSurfacePosition.x)
        {
            transform.position = new Vector3(transform.position.x + particleMovementSpeed * Time.deltaTime, transform.position.y, 0);
        }
        else if(rb.position.x > nextSurfacePosition.x)
        {
            transform.position = new Vector3( (transform.position.x - particleMovementSpeed * Time.deltaTime), transform.position.y, 0);
        }
    }
    void Die()
    {

    }
}
