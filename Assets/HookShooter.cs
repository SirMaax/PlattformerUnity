using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookShooter : MonoBehaviour
{
    public PlayerMovement player;
    public Rigidbody2D rb;
    public Hook2 hook;

    [SerializeField] float hookForce;
    [SerializeField] float coyoteStickInputTime;            //How long the last input is keept when hook is triggerd. 
    //That if you relaese the stick at the same time as the rt you still got the correct last input;

    Vector2 direction;
    Vector2 oldDirection;
    Vector2 move;
    public bool hookAtTarget = false;
    bool hookActive = false;
    bool aimingActive = false;

    float airtime = 0;
    [SerializeField] float maxAirtime;


    private void Update()
    {
        if (aimingActive) StartCoroutine(keepDirection());
        if (aimingActive) updateDirection();

        
    }
    private void FixedUpdate()
    {
        if (!hookAtTarget && hookActive) ApplyForceToHook();
        DrawAimingLine();
    }
    public void aimingActiveToggle()
    {
        aimingActive = true;
    }
    public void shootHook()
    {
        if (direction == Vector2.zero && oldDirection == Vector2.zero) return;
        //hook.enabled = true;
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
        hookAtTarget = false;
        Vector2 temp = player.rigidBody.position;
        temp.y += 0.5f;
        rb.position = temp;

        //rb.position = player.rigidBody.position;
        aimingActive = false;
        hookActive = true;
        airtime = 0;
        hook.ResetVar();
        if (direction == Vector2.zero && oldDirection != Vector2.zero) direction = oldDirection;

    }
    public void updateDirection()
    {
        move = player.move;
        move *= 5;
        direction = move;
        direction.Normalize();
        direction *= hookForce;
    }
    private void ApplyForceToHook()
    {
        if (airtime < maxAirtime)
        {
            airtime++;
            //rb.AddForce(direction);
            rb.velocity = direction;
        }
        else if(!hookAtTarget)
        {
            rb.gravityScale = 1;
        }
    }
    
    public void setHookAtTarget()
    {
        hookAtTarget = true;
        rb.gravityScale = 0;
    }

    IEnumerator keepDirection()
    {
       if(direction == Vector2.zero)
        {
            yield return new WaitForSeconds(coyoteStickInputTime);
        }
        oldDirection = direction;
        yield return null;
    }
    public void ResetHook()
    {
        hookAtTarget = false;
        hook.enabled = false;
        hookActive = false;
    }
    public void VelocityToZero()
    {
        rb.velocity = Vector2.zero;
    }

    private void DrawAimingLine()
    {
        Debug.DrawLine(player.rigidBody.position, move * 59);
    }
}
