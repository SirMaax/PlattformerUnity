using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    public Rigidbody2D rigidBody;
    public Animator animator;

    [SerializeField] float runSpeed;                        //Controls RunSpeed for player
    [SerializeField] float jumpHeight;                //Controls jumpforce
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


    private float lastHorizontMovement = 0f;                //Important for knowing which direction the next dash is gonna go, when not moving

    private bool downMovement = false;                      //True = Player is pressing down , False not
    private bool dashReady = true;                          //True -> PLayer can dash
    private float dashCounter = 25f;                        //Counts intervalls between dashes
    private bool dashOnlyOnceInAir = true;                  //Allows the player to only dash once in air

    public bool grounded = true;                            //True when grounded in air false
    public bool airborn = false;                            //When jumping player is airborn
    private float minimumJump = 0f;                         //Counter for minimumJump
    public float currentJumpDuration = 0f;                  //Length of current Jump
    public float jumpDuration = 5f;                         //The length of the maximum jump
    [SerializeField] float maxDownSpeed;                    //How fast the player moves down in air when pressing odwn
    private bool DoOnlyOnce = false;                        //Used for jumpig       -> Should rename that :D
    private float realJumpHeight;

    public bool touchWallLeft = false;                      //IF player is touching wall from the left
    public bool touchWallRight = false;                     //IF player is touching wall from the right
    public float wallJump = 0f;                             //0 = No touch , 1 = left Wall ,2 = right wall
    public Vector2 lastPositionOnWall;                      //Holds the position where a wall was touched last
    public bool groundWallJump = false;                     //WHen standing next to a wall and jumping = true
    [SerializeField] float wallSlideSpeed;                  //How fast the player slides down the wall
    public bool canConnectToWAll = true;                    //When false the player can not conect to a wall
    private bool slideDownWall = false;                     //If the player wants to slide down the wall

    private float lastYPos;
    private bool wallTouchMethodExecuted = false;
    private float coyoteWallTime = 0f;                       //Counts down CooyteWallTime
    private float coyoteWallStartTime = 2f;                  //How long the player has time to press the button after leaving the wall and still beeing able to jump
    private float lastWallTouched = 0f;                      //1 == left wall ,  2 == right wall

    public bool doubleJumpedAlready = false;               //Used for switching jump Animation 
    [SerializeField] float doubleJumpIncreasment;           //Regulates ´how high the player can jump with double Jums
    public bool fallingFromPlattform = false;

    PlayerControls controls;
    Vector2 move;

    private void Awake()
    {
        controls = new PlayerControls();

        controls.GamePlay.Move.performed += temp => move = temp.ReadValue<Vector2>();
        controls.GamePlay.Move.canceled += temp => move = Vector2.zero;

        controls.GamePlay.Dodge.performed += temp => Dash();

        controls.GamePlay.Jump.performed += temp => PreJumpCheck();
        controls.GamePlay.Jump.canceled += temp => { StopYAcceleration();   currentJumpDuration = jumpDuration; };

    }
        
    private void GetUseInput()
    {
        //Left Right Movement
        Vector2 MovForce = move * runSpeed * Time.deltaTime;
    }
    private void OnEnable()
    {
        controls.GamePlay.Enable();
    }
    private void OnDisable()
    {
        controls.GamePlay.Disable();
    }

    private void Start()
    {
        realJumpHeight = jumpHeight;
    }
    
    private void PreJumpCheck()
    {
        //Checking if Player is touching Wall
        if (((touchWallLeft || touchWallRight) || coyoteWallTime != 0))
        {
            
            if (touchWallLeft)wallJump = 1;
            else wallJump = 2;

            ResetJumpVar();
        }
        //Used for double
        else if (!doubleJumpedAlready && !grounded  && (airborn || fallingFromPlattform) && !touchWallLeft && !touchWallRight)
        {
            doubleJumpedAlready = true;
            minimumJump = minimumJumpHeight;
            currentJumpDuration = 0;
            DoOnlyOnce = false;
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);

            animator.SetBool("JumpingUp", true);
            animator.SetBool("JumpingDown", false);
        }
        //Normal grounded Jump
        else if (grounded && rigidBody.velocity.y == 0) ResetJumpVar();  
    }
    
    void Update()
    {
        GetUseInput();

        //Tells in which direction the player is facing
        if (move.x < 0) lastHorizontMovement = -1;
        if (move.x > 0) lastHorizontMovement = 1;


        //animator.SetFloat("Speed", Mathf.Abs(horizontalMovement));

        //Disables consecuent dashing while pressing the button (without not pressing it)
        if (dashCounter >= dashCooldown)
        {
            dashReady = true;
        }
        /*
        //Input for Jumping while on wall
        if (Input.GetButtonDown("Jump") && ((touchWallLeft || touchWallRight) || coyoteWallTime != 0))
        {
            if (touchWallLeft)
            {
                wallJump = 1;
            }
            else
            {
                wallJump = 2;
            }
            ResetJumpVar();
        }
        //Double Jump
        else if (!doubleJumpedAlready && !grounded && Input.GetButtonDown("Jump") && (airborn || fallingFromPlattform) && !touchWallLeft && !touchWallRight)
        {
            doubleJumpedAlready = true;
            minimumJump = minimumJumpHeight;
            currentJumpDuration = 0;
            DoOnlyOnce = false;
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);

            animator.SetBool("JumpingUp", true);
            animator.SetBool("JumpingDown", false);

        }*/
        /*
        //Input for Jumping while grounded
        else if (Input.GetButtonDown("Jump") && grounded && rigidBody.velocity.y == 0)
        {
            ResetJumpVar();
        }

        //Input for releasing the Jump Button
        if (Input.GetButtonUp("Jump") && airborn)
        {
            StopYAcceleration();
            currentJumpDuration = jumpDuration;
        }*/
        //Input for the down Button in the air
        if ((Input.GetButton("Down") || (move.y < 0) && (airborn || (touchWallLeft || touchWallRight))))
        {
            downMovement = true;
        }
        //Order is important 
        if (!wallTouchMethodExecuted && (touchWallRight || touchWallLeft))
        {
            touchWallLeft = false;
            touchWallRight = false;
            animator.SetBool("JumpingDown", true);
            animator.SetBool("Walled", false);
        }
    }

    /// <summary>
    /// //////////////////////////////////////////////
    /// </summary>

    private void FixedUpdate()
    {
        wallTouchMethodExecuted = false;
        //Count down coyoteWallTime

        if (coyoteWallTime != 0) coyoteWallTime--;

        //Checks if the player is Airborn
        CheckIfAirborn();
        //Clamp gravityDownFallSpeed
        ClampGravity();
        //Counts airtime
        //Counts intervall between dashes
        dashCounter++;
        //Slide down Wall
        if (wallJump == 0 && (touchWallLeft || touchWallRight))
            rigidBody.velocity = new Vector2(0, wallSlideSpeed);
        //Movement
        controller.Move(move.x, false, false);

        //Jump
        Jump();
        //DownMovement in air
        DownMovement();
        //Stopp's jump in air
        JumpStopVelocity();
        //TouchingWall Action
        //TouchingWall();

        //ANIMATION PART




        if (wallJump != 0)
        {
            airborn = true;
            grounded = false;
        }
        lastYPos = rigidBody.position.y;
    }

    //Stops the jump after a certain time
    public void JumpStopVelocity()
    {
        //Stops air acceleration after jumpDuratio is ovestepped
        if (currentJumpDuration >= jumpDuration && !DoOnlyOnce && !touchWallLeft && !touchWallRight)
        {
            DoOnlyOnce = true;
            StopYAcceleration();
            wallJump = 0;

            animator.SetBool("JumpingDown", true);
            animator.SetBool("JumpingUp", false);
        }
    }


    //DASH Movement regulates the dash
    public void Dash()
    {
        //Active when PLayer pressed right button and is allowed to dash
        if (dashReady)
        {
            //Stops dash when it should be over and when already dashed once in air
            if (dashCounter >= dashCooldown && dashOnlyOnceInAir)
            {
                //Animation
                animator.SetBool("Walled", false);

                //Var that are reset upon dashing
                ResetDashVar();

                //Apply force
                rigidBody.AddForce(new Vector2(lastHorizontMovement * (dashForce - (System.Math.Abs(move.x) * dashMultiply)), 0));
            }
        }
    }



    //Makes the player jump the minimum JumpHeight
    public void Jump()
    {
        if (fallingFromPlattform) return;
            
        jumpHeight = realJumpHeight;
        float temp = 0;
        if (minimumJump < minimumJumpHeight)
        {
            downMovement = false;
            if (wallJump == 1 || lastWallTouched  == 1)
            {
                temp = distanceWallJumpX;
                jumpHeight = wallJumpHeight;
                animator.SetBool("JumpingUp", true);
            }
            else if (wallJump == 2 || lastWallTouched == 2)
            {
                temp = -distanceWallJumpX;
                jumpHeight = wallJumpHeight;
                animator.SetBool("JumpingUp", true);
            }
            rigidBody.AddForce(new Vector2(temp, jumpHeight));
            minimumJump += 0.1f;

            if (minimumJump == minimumJumpHeight && wallJump != 0)
            {
                canConnectToWAll = true;
            }
        }

        if (!grounded && currentJumpDuration < jumpDuration && minimumJump >= minimumJumpHeight)
        {
            currentJumpDuration++;
            if (wallJump != 0 || coyoteWallTime != 0)
            {
                jumpHeight = wallJumpHeight;
                animator.SetBool("Walled", false);
            }
            if (doubleJumpedAlready)
            {
                jumpHeight *= doubleJumpIncreasment;
            }
            rigidBody.AddForce(new Vector2(temp, jumpHeight));
            minimumJump++;
            //jumpHeight -= jumpHeightRecument;
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
        if (!grounded)
        {
            animator.SetBool("JumpingDown", false);
            animator.SetBool("Walled", false);
            grounded = true;
            airborn = false;
            downMovement = false;
            dashOnlyOnceInAir = true;
            if (touchWallLeft) transform.position = new Vector2(rigidBody.position.x + moveFromWallAway, rigidBody.position.y);
            else if (touchWallRight) transform.position = new Vector2(rigidBody.position.x - moveFromWallAway, rigidBody.position.y);
            currentJumpDuration = 0;                
            canConnectToWAll = true;
            DoOnlyOnce = false;
            lastWallTouched = 0f;
            touchWallLeft = false;
            touchWallRight = false;
            fallingFromPlattform = false;
            doubleJumpedAlready = false;
        }
        wallJump = 0;
    }

    //Stops Y Movement 
    public void StopYAcceleration()
    {
        if (minimumJump > minimumJumpHeight)
            rigidBody.AddForce(new Vector2(0, -downMovementForce));
    }

    //Pogo in air
    public void HitTargetInAir()
    {
        rigidBody.velocity = Vector2.zero;
        rigidBody.AddForce(new Vector2(0, pogoHeight));
    }

    private void ClampGravity()
    {
        if (rigidBody.velocity.y <= maxDownSpeed)
        {
            Vector2 temp = new Vector2(move.x * 10, maxDownSpeed);
            rigidBody.velocity = temp;
        }
    }

    //Is triggered when the wall is right from the player
    public void OnWallTouchRightEvent()
    {
        wallTouchMethodExecuted = true;
        //Connects to wall when jumping next to a wall and pressing the button in the right direction
        if (groundWallJump && move.x > 0)
        {
            groundWallJump = false;
        }
        //Used for detecting when standing next to a wall
        if (grounded && !fallingFromPlattform)
        {
            groundWallJump = true;
            touchWallRight = false;
            return;
        }
        if (downMovement)
        {
            slideDownWall = true;
            touchWallRight = false;
            //If slide Down Wall was used before and the player is not pressing down right now. It is set to false
            if (move.y == 0 && slideDownWall)
            {
                canConnectToWAll = true;
                slideDownWall = false;
                downMovement = false;
                OnWallTouchRightEvent();
            }
            return;
        }
        //Connects the player to the wall
        if (!groundWallJump)
        {
            lastWallTouched = 2;
            if (move.x >= 0 && !touchWallRight && canConnectToWAll && !downMovement && (currentJumpDuration > 3 || !groundWallJump))
            {
                touchWallRight = true;
                ResetWallVar();
            }
            else if (move.x < 0) // LEAVING WALL
            {
                LeavingWallVar();
            }
        }
    }
    //Is triggered when the wall is left from the player
    public void OnWallTouchLeftEvent()
    {
        wallTouchMethodExecuted = true;
        //Connects to wall when jumping next to a wall and pressing the button in the right direction
        if (groundWallJump && move.x < 0)
        {
            groundWallJump = false;
        }
        //Used for detecting when standing next to a wall
        if (grounded && !fallingFromPlattform)
        {
            groundWallJump = true;
            touchWallLeft = false;
            return;
        }


        //Used for sliding down the wall
        if (downMovement)
        {
            slideDownWall = true;
            touchWallLeft = false;
            //If slide Down Wall was used before and the player is not pressing down right now. It is set to false
            if (move.y == 0 && slideDownWall)
            {
                canConnectToWAll = true;
                slideDownWall = false;
                downMovement = false;
                OnWallTouchLeftEvent();
            }
            return;
        }
        //Connects the player to the wall
        if (!groundWallJump)
        {
            lastWallTouched = 1;
            if (move.x <= 0 && !touchWallLeft && canConnectToWAll && (currentJumpDuration > 3 || !groundWallJump))
            {
                touchWallLeft = true;
                ResetWallVar();
            }
            else if (move.x > 0) // LEAVING WALL
            {
                LeavingWallVar();
            }
        }
    }

    //Used before jumping
    private void ResetJumpVar()
    {
        animator.SetBool("JumpingUp", true);
        DoOnlyOnce = false;
        touchWallRight = false;
        touchWallLeft = false;
        //doubleJumpedAlready = false;
        fallingFromPlattform = false;

        airborn = true;
        grounded = false;
        
        currentJumpDuration = 0;
        minimumJump = 0;
        jumpHeight = realJumpHeight;

    }
    //USED for when touching wall
    private void ResetWallVar()
    {
        currentJumpDuration = jumpDuration;
        minimumJump = minimumJumpHeight;
        wallJump = 0;
        DoOnlyOnce = false;
        canConnectToWAll = false;
        dashOnlyOnceInAir = true;
        doubleJumpedAlready = false;
        fallingFromPlattform = false;
        animator.SetBool("Walled", true);
        animator.SetBool("JumpingDown", false);
        animator.SetBool("JumpingUp", false);
    }

    //Leaving wall
    private void LeavingWallVar()
    {
        touchWallLeft = false;
        touchWallRight = false;
        animator.SetBool("Walled", false);
        animator.SetBool("JumpingDown", true);

        //Resetting WallJump for new jump
        wallJump = 0;
        canConnectToWAll = true;
        coyoteWallTime = coyoteWallStartTime;
    }

    private void ResetDashVar()
    {
        if (airborn)
        {
            dashOnlyOnceInAir = false;
            rigidBody.velocity = Vector2.zero;
        }
        groundWallJump = false;
        dashReady = false;
        dashCounter = 0;
    }

    
    private void CheckIfAirborn()
    {
        float temp = rigidBody.position.y;

        if (lastYPos > temp)
        {
            animator.SetBool("JumpingDown", true);
            if (grounded) fallingFromPlattform = true;
        }
        else if (grounded)
        {
            animator.SetBool("JumpingDown", false);
            if (lastYPos == temp) fallingFromPlattform = false;
        }
    }


}
