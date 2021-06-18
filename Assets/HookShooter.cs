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
    float maxAirtime = 250000;


    private void Update()
    {
        if (aimingActive) StartCoroutine(keepDirection());
        if (aimingActive) updateDirection();

        Debug.Log(direction);
    }
    private void FixedUpdate()
    {
        if (!hookAtTarget && hookActive) ApplyForceToHook();
    }
    public void aimingActiveToggle()
    {
        aimingActive = true;
    }
    public void shootHook()
    {
        if (direction == Vector2.zero && oldDirection == Vector2.zero) return;
        //hook.enabled = true;
        rb.position = player.rigidBody.position;
        aimingActive = false;
        hookActive = true;
        airtime = 0;
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
            rb.AddForce(direction);
        }
    }
    
    public void setHookAtTarget()
    {
        hookAtTarget = true;
    }

    IEnumerator keepDirection()
    {
       if(direction == Vector2.zero)
        {
            yield return new WaitForSeconds(coyoteStickInputTime);
        }
        oldDirection = direction;
        Debug.Log("One run through");
        yield return null;
    }
    public void ResetHook()
    {
        hookAtTarget = false;
        hook.enabled = false;
        hookActive = false;
    }
 
}
