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
    [SerializeField] float dashForce = 0f;
    [SerializeField] float dashCooldown = 0f;
    [SerializeField] float dashMultiply = 0.1f;

    private float currentJumpDuration = 0f;                 //Length of current Jump

    private float horizontalMovement = 0f;                  //Important for moving left and right take input
    private float lastHorizontMovement = 0f;
    private float vertialMovement = 1f;                     //Goes from 3 to -3 -> Only controls down movement right now

    private float dashButtonPressed = 0f;                   //0 When right should button is not pressed. 1 When pressed
    private bool downMovement = false;                      //True = Player is pressing down , False not
    private bool dashReady = true;                          //True -> PLayer can dash

    private bool grounded = true;                           //True when grounded in air false
    private bool airborn = false;                           //When jumping player is airborn
    private float minimumJump = 0f;                         //Counter for minimumJump

    public float dashCounter = 25f;                         //Counts intervalls between dashes
    private bool dashOnlyOnceInAir = true;                  //Allows the player to only dash once in air

    public bool touchWallLeft = false;                      //IF player is touching wall from the left
    public bool touchWallRight = false;                     //IF player is touching wall from the right
    [SerializeField] float distanceWallJumpX = 0f;          //Distance the player will jump away from the wall
    private bool doOnlyOnceTouchingWall = false;            //Used to stop Movement when hitting Wall, but only once per walltouch
    public Vector2 lastPositionOnWall;                      //Holds the position where a wall was touched last

    // Update is called once per frame
    void Update()
    {
        //Tells in which direction the player is facing
        if (horizontalMovement < 0) lastHorizontMovement = -1;
        if (horizontalMovement > 0) lastHorizontMovement = 1;

        horizontalMovement = Input.GetAxisRaw("Horizontal") * runSpeed;
        vertialMovement = Input.GetAxisRaw("Vertical") * runSpeed;

        //Disables consecuent dashing while pressing the button (without not pressing it)
        if (dashCounter >= dashCooldown)
        {
            dashButtonPressed = Input.GetAxisRaw("ControllerRightTrigger");
            if (dashButtonPressed == 0)
            {
                dashReady = true;
            }
        }

        //Input for Jumping while on wall
        if (Input.GetButtonDown("Jump") && (touchWallLeft || touchWallRight))
        {
            airborn = true;
            grounded = false;

            currentJumpDuration = 0;
            minimumJump = 0;
            jumpHeight = 115;
        }
        //Input for Jumping while grounded
        else if (Input.GetButtonDown("Jump") && grounded && rigidBody.velocity.y == 0)
        {
            airborn = true;
            grounded = false;

            currentJumpDuration = 0;
            minimumJump = 0;
            jumpHeight = 115;
        }

        //Input for releasing the Jump Button
        if (Input.GetButtonUp("Jump") && airborn)
        {
            Debug.Log("Stump stopped via not pressing");
            stopYAcceleration();
            currentJumpDuration = jumpDuration;
        }
        //Input for the down Button in the air
        if ((Input.GetButton("Down") || (vertialMovement < 0) && airborn))
        {
            downMovement = true;
        }
    }

    /// <summary>
    /// //////////////////////////////////////////////
    /// </summary>

    private void FixedUpdate()
    {
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
        //Stopes jump in air
        StopJump();
        //TouchingWall Action
        TouchingWall();
        
        
        if (rigidBody.position.x != lastPositionOnWall.x)
        {
            touchWallLeft = false;
            touchWallRight = false;
        }
    }

    //Stops the jump after a certain time
    public void StopJump()
    {
        //Stops air acceleration after jumpDuratio is ovestepped
        if (currentJumpDuration >= jumpDuration)
        {
            stopYAcceleration();
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
                if (airborn) dashOnlyOnceInAir = false;
                dashReady = false;
                dashCounter = 0;
                rigidBody.AddForce(new Vector2(lastHorizontMovement * (dashForce - (System.Math.Abs(horizontalMovement) * dashMultiply)), 0));
            }
        }
    }

    //Makes the player jump the minimum JumpHeight
    public void Jump()
    {
        float temp = 0;
        if (minimumJump < minimumJumpHeight)
        {

            if (touchWallLeft)
            {
                temp = distanceWallJumpX;
            }
            else if (touchWallRight)
            {
                temp = -distanceWallJumpX;
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
        }
    }
    public void OnLanding()
    {
        if (!grounded && airborn)
        {
            grounded = true;
            airborn = false;
            downMovement = false;
            dashOnlyOnceInAir = true;
        }
    }
    public void OnWallTouchLeftEvent()
    {
        if (!touchWallLeft)
        {
            touchWallLeft = true;
            lastPositionOnWall = rigidBody.position;
            grounded = true;
            airborn = false;
            downMovement = false;
            dashOnlyOnceInAir = true;
        }
    }


    public void OnWallTouchRightEvent()
    {
        if (!touchWallRight)
        {
            touchWallRight = true;
            lastPositionOnWall = rigidBody.position;
            grounded = true;
            airborn = false;
            downMovement = false;
            dashOnlyOnceInAir = true;
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
            if (touchWallLeft || touchWallRight)
            {

                if (!doOnlyOnceTouchingWall)
                {
                    rigidBody.velocity = Vector2.zero;
                    doOnlyOnceTouchingWall = true;
                }

                rigidBody.velocity = new Vector2(0, 0);
                //rigidBody.isKinematic = true;
            }
        }
    }

}
