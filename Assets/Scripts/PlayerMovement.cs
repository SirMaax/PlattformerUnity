using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    public Rigidbody2D rigidBody;

    [SerializeField] float runSpeed = 40f;                   //Controlls RunSpeed for player
    [SerializeField] float jumpHeight = 50f;                //Controls jumpforce
    [SerializeField] float jumpDuration = 5f;               //The length of the maximum jump
    [SerializeField] float downMovementForce = 5;           //The force of how fast the player is pulled towards earth while pressing down
    [SerializeField] float minimumJumpHeight = 0f;          //The minimum distance a player always jumps when pressing the jump button
    [SerializeField] float jumpHeightRecument = 0.1f;       //The longer a jump goes on the jumpHeight is reduced
    [SerializeField] float dashForce = 0f;                  //Intial force behind a dash
    [SerializeField] float dashCooldown = 0f;               //How long the cooldown between dashes is
    [SerializeField] float dashMultiply = 0.1f;             //How far the player dashes
    [SerializeField] float moveFromWallAway = 0f;           //When sliding down the wall, player is set a few pixels away from the wall.    Maybe unnecessary ;D
    [SerializeField] float distanceWallJumpX = 0f;          //Distance the player will jump away from the wall
    [SerializeField] float wallJumpHeight = 0f;             //Height player jumps with a wallJump
    [SerializeField] float pogoHeight = 0f;                 //When hitting something with sword in the air, how far the player is send high;


    private float horizontalMovement = 0f;                  //Important for moving left and right take input
    private float lastHorizontMovement = 0f;                //Important for knowing which direction the next dash is gonna go, when not moving
    private float vertialMovement = 1f;                     //Goes from 3 to -3 -> Only controls down movement right now

    private float dashButtonPressed = 0f;                   //0 When right should button is not pressed. 1 When pressed
    private bool downMovement = false;                      //True = Player is pressing down , False not
    private bool dashReady = true;                          //True -> PLayer can dash
    private float dashCounter = 25f;                        //Counts intervalls between dashes
    private bool dashOnlyOnceInAir = true;                  //Allows the player to only dash once in air
    private bool doOnlyOnceTouchingWall = false;            //Used to stop Movement when hitting Wall, but only once per walltouch

    public bool grounded = true;                            //True when grounded in air false
    public bool airborn = false;                            //When jumping player is airborn
    private float minimumJump = 0f;                         //Counter for minimumJump
    private float currentJumpDuration = 0f;                 //Length of current Jump


    public bool touchWallLeft = false;                      //IF player is touching wall from the left
    public bool touchWallRight = false;                     //IF player is touching wall from the right
    public float wallJump = 0f;                             //0 = No touch , 1 = left Wall ,2 = right wall
    public Vector2 lastPositionOnWall;                      //Holds the position where a wall was touched last
    public bool weirdGroundWallJump = false;                //When starting a jump next to a wall

    [SerializeField] float maxDownSpeed = 0f; 





    private bool DoOnlyOnce = false;                  //TEST

    public Animator animator;
    // Update is called once per frame
    void Update()
    {
        //Tells in which direction the player is facing
        if (horizontalMovement < 0) lastHorizontMovement = -1;
        if (horizontalMovement > 0) lastHorizontMovement = 1;

        horizontalMovement = Input.GetAxisRaw("Horizontal") * runSpeed;
        vertialMovement = Input.GetAxisRaw("Vertical") * runSpeed;

        animator.SetFloat("Speed", Mathf.Abs(horizontalMovement));

        //Disables consecuent dashing while pressing the button (without not pressing it)
        if (dashCounter >= dashCooldown)
        {
            dashButtonPressed = Input.GetAxisRaw("ControllerRightTrigger");
            if (dashButtonPressed == 0)
            {
                dashReady = true;
            }
        }
        
        /*  Not Implemented yet
        //Pre Jump Input
        if (Input.GetButtonDown("Jump") && (!grounded || airborn) )
        {
            preJumpInput = true;
        }*/

        


        //Input for Jumping while on wall
        if (Input.GetButtonDown("Jump") && (touchWallLeft || touchWallRight) &&!weirdGroundWallJump)
        {
            airborn = true;
            grounded = false;
            
            if (touchWallLeft) wallJump = 1;
            else wallJump = 2;

            currentJumpDuration = 0;
            minimumJump = 0;
            jumpHeight = 115;
            

        }
        //Input for Jumping while grounded
        else if (Input.GetButtonDown("Jump") && grounded && rigidBody.velocity.y == 0)
        {
            animator.SetBool("JumpingUp", true);
            
            airborn = true;
            grounded = false;

            currentJumpDuration = 0;
            minimumJump = 0;
            jumpHeight = 115;
        }

        //Input for releasing the Jump Button
        if (Input.GetButtonUp("Jump") && airborn)
        {
            stopYAcceleration();
            currentJumpDuration = jumpDuration;
        }
        //Input for the down Button in the air
        if ((Input.GetButton("Down") || (vertialMovement < 0) && ( airborn || ( touchWallLeft || touchWallRight)) ))
        {
            downMovement = true;
        }
    }

    /// <summary>
    /// //////////////////////////////////////////////
    /// </summary>

    private void FixedUpdate()
    {
        //Clamp gravityDownFallSpeed
        ClampGravity();
        //Counts airtime
        if (airborn) currentJumpDuration++;
        //Counts intervall between dashes
        dashCounter++;

        //Movement
        controller.Move(horizontalMovement, false, false);
        //Dash
        Dash();
        //Jump
        Jump();
        //DownMovement in air
        DownMovement();
        //Stopp's jump in air
        StopJump();
        //TouchingWall Action
        TouchingWall();
        //Counts preJumpTiming

        //ANIMATION PART
        if (grounded)
        {
            animator.SetBool("JumpingDown", false);
        }

        if (rigidBody.position.x != lastPositionOnWall.x )
        {
            
            touchWallLeft = false;
            touchWallRight = false;
            animator.SetBool("Walled", false);

            //animator.SetBool("JumpingDown", false);
        }

        if(wallJump != 0)
        {
            airborn = true;
            grounded = false;
        }
    }

    //Stops the jump after a certain time
    public void StopJump()
    {
        //Stops air acceleration after jumpDuratio is ovestepped
        if (currentJumpDuration >= jumpDuration)
        {
            stopYAcceleration();
            if(!touchWallLeft || touchWallRight)
            {
            animator.SetBool("JumpingDown", true);
            animator.SetBool("JumpingUp", false);
            }
        }
    }

    
    //DASH Movement regulates the dash
    public void Dash()
    {
        //Active when PLayer pressed right button and is allowed to dash
        if (dashButtonPressed == 1 && dashReady)
        {
            //Stops dash when it should be over and when already dashed once in air
            if (dashCounter >= dashCooldown && dashOnlyOnceInAir)
            {

                if (airborn)
                {
                    dashOnlyOnceInAir = false;
                    rigidBody.velocity = Vector2.zero;
                }
                dashReady = false;
                dashCounter = 0;
                rigidBody.AddForce(new Vector2(lastHorizontMovement * (dashForce - (System.Math.Abs(horizontalMovement) * dashMultiply)), 0));
            }
        }
    }

    //Makes the player jump the minimum JumpHeight
    public void Jump()
    {
        jumpHeight = 115;
        float temp = 0;
        if (minimumJump < minimumJumpHeight)
        {
            downMovement = false;
            if (wallJump == 1)
            {
                temp = distanceWallJumpX;
                jumpHeight = wallJumpHeight;
                animator.SetBool("JumpingUp", true);
            }
            else if (wallJump == 2)
            {
                temp = -distanceWallJumpX;
                jumpHeight = wallJumpHeight;
                animator.SetBool("JumpingUp", true);
            }
            rigidBody.AddForce(new Vector2(temp, jumpHeight));
            minimumJump += 0.1f;
        }

        if (!grounded && currentJumpDuration < jumpDuration && minimumJump >= minimumJumpHeight)
        {
            rigidBody.AddForce(new Vector2(temp, jumpHeight));
            minimumJump++;
            jumpHeight -= jumpHeightRecument;
        }
    }

    //When Player presses down to move down in Air
    public void DownMovement()
    {
        if (downMovement && airborn)
        {
            rigidBody.AddForce(new Vector2(0, -downMovementForce));
            animator.SetBool("JumpingUp", false);
            animator.SetBool("JumpingDown", true);
        }
    }
    public void OnLanding()
    {
        if (!grounded )
        {
            animator.SetBool("JumpingDown", false);
            animator.SetBool("Walled", false);
            weirdGroundWallJump = false;
            grounded = true;
            airborn = false;
            downMovement = false;
            dashOnlyOnceInAir = true;
            if (touchWallLeft) transform.position = new Vector2(rigidBody.position.x + moveFromWallAway, rigidBody.position.y);
            else if (touchWallRight) transform.position = new Vector2(rigidBody.position.x - moveFromWallAway, rigidBody.position.y);
            currentJumpDuration = 0;                //GERADE HINZUGEFÜGT
        }
        wallJump = 0;
    }
    public void OnWallTouchLeftEvent()
    {
        
        if (weirdGroundWallJump && horizontalMovement == -1 && airborn)
        {
            animator.SetBool("JumpingDown", false);
            //animator.SetBool("ClimbingLeft", true);
            animator.SetBool("Walled", true);
            touchWallLeft = true;
            lastPositionOnWall = rigidBody.position;
            airborn = false;
            dashOnlyOnceInAir = true;
            weirdGroundWallJump = false;
            currentJumpDuration = jumpDuration;
            minimumJump = minimumJumpHeight;
            rigidBody.velocity = Vector2.zero;
           
        }
        else if (grounded)
        {
            weirdGroundWallJump = true;
        }else if (!grounded && !weirdGroundWallJump)
        {
            
            animator.SetBool("JumpingDown", false);
            animator.SetBool("Walled", true);

            touchWallLeft = true;
            lastPositionOnWall = rigidBody.position;
            airborn = false;
            dashOnlyOnceInAir = true;
            weirdGroundWallJump = false;
            
        }
    }


    public void OnWallTouchRightEvent()
    {
        
        if (weirdGroundWallJump && horizontalMovement == 1 && airborn)
        {
            animator.SetBool("JumpingDown", false);
          //  animator.SetBool("ClimbingRight", true);
            animator.SetBool("Walled", true);
            touchWallRight = true;
            lastPositionOnWall = rigidBody.position;
            airborn = false;
            dashOnlyOnceInAir = true;
            weirdGroundWallJump = false;
            currentJumpDuration = jumpDuration;
            minimumJump = minimumJumpHeight;
            rigidBody.velocity = Vector2.zero;
        }
        else if (grounded)
        {
            weirdGroundWallJump = true;
        }
        else if (!grounded && !weirdGroundWallJump)
        {
            animator.SetBool("JumpingDown", false);
           // animator.SetBool("ClimbingRight", true);
            animator.SetBool("Walled", true);
            touchWallRight = true;
            lastPositionOnWall = rigidBody.position;
            airborn = false;
            dashOnlyOnceInAir = true;
            weirdGroundWallJump = false;
        
        }
    }   

 
    //Stops Y Movement 
    public void stopYAcceleration()
    {
        if (minimumJump > minimumJumpHeight)
            rigidBody.AddForce(new Vector2(0, -downMovementForce));

    }

    //What happens when the wall is touched
    public void TouchingWall()
    {
        {
            if (downMovement && !grounded &&(touchWallLeft || touchWallRight) )
            {
                rigidBody.velocity = new Vector2(0, -downMovementForce/2);
            }
            else if (touchWallRight && horizontalMovement != -1)
            {

                if (!doOnlyOnceTouchingWall)
                {
                    rigidBody.velocity = Vector2.zero;
                    doOnlyOnceTouchingWall = true;
                }
                rigidBody.velocity = new Vector2(0, 0);
            }
            else if (touchWallLeft && horizontalMovement != 1)
            {
                if (!doOnlyOnceTouchingWall)
                {
                    rigidBody.velocity = Vector2.zero;
                    doOnlyOnceTouchingWall = true;
                }
                rigidBody.velocity = new Vector2(0, 0);
            }
        }
    }

    public void HitTargetInAir()
    {
        rigidBody.velocity = Vector2.zero;
        rigidBody.AddForce(new Vector2(0, pogoHeight));
    }

    private void ClampGravity()
    {
        if(rigidBody.velocity.y <= maxDownSpeed)
        {
            Vector2 temp = new Vector2(horizontalMovement *10 , maxDownSpeed);
            rigidBody.velocity = temp;
        }
    }
}
