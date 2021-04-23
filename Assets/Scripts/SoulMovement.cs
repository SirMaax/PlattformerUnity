using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulMovement : MonoBehaviour
{
    [SerializeField] float movementSpeed;
    [SerializeField] float rotationSpeed;
    [SerializeField] float animationTransitionSpeed;
    [SerializeField] float idleMovement;
    [SerializeField] float linearDrag;
    public Rigidbody2D rb;
    public Animator animator;
    public Particle_Soul particleFly;
    public ParticleSystem particleStand;

    public float angle;
    private float horizontalMovement = 0f;
    private float vertialMovement = 0f;
    private bool toggleParticlesOnce = false;
    // Update is called once per frame
    void Update()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        vertialMovement = Input.GetAxisRaw("Vertical");

        if (horizontalMovement == 0 && vertialMovement == 0)
        {
            rb.drag = linearDrag;
            
            if(rb.velocity.x <= animationTransitionSpeed && rb.velocity.y <= animationTransitionSpeed)animator.SetFloat("Speed", 0);
        }
        else
        {
            animator.SetFloat("Speed", 1);
            rb.drag = 0f;
        }
    }
    private void FixedUpdate()
    {
        Movement();
        rotateToKeyPresse();
        if(horizontalMovement == 0 && vertialMovement == 0) MoveIdle();
    }


    private void Movement()
    {

        if (horizontalMovement != 0 || vertialMovement != 0)
        {
            if (toggleParticlesOnce)
            {
                toggleParticlesOnce = false;
                particleFly.ToggleParticlesFly();
            }
            
            Vector2 directionPoint = new Vector2(rb.position.x + horizontalMovement, rb.position.y + vertialMovement);

            Vector2 direction = directionPoint - rb.position;

            direction.Normalize();

            rb.AddForce(direction * movementSpeed);
        }
    }
    
    private void rotateToKeyPresse()
    {

        Vector2 pointA = rb.position;
        Vector2 pointB = new Vector2(rb.position.x + rb.velocity.x, rb.position.y + rb.velocity.y);

        

        angle = Mathf.Atan2(pointA.y - pointB.y, pointA.x - pointB.x) * Mathf.Rad2Deg;
        Quaternion quat = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, quat, rotationSpeed * Time.deltaTime);
    }

    private void MoveIdle()
    {
        rb.AddForce(new Vector2(UnityEngine.Random.Range(-idleMovement, idleMovement), UnityEngine.Random.Range(-idleMovement, idleMovement)));
        if (!toggleParticlesOnce)
        {
            toggleParticlesOnce = true;
            particleFly.ToggleParticlesFly();
            ToggleParticleStand();

        }
    }

    private void ToggleParticleStand()
    {
        if (particleStand.isEmitting) particleStand.enableEmission = false;
        else particleStand.enableEmission = true;
    }
}
