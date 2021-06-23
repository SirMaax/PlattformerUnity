using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public CharacterController2D controller;
    public PlayerAttack plAttack;
    public HookShooter hook;
    public Rigidbody2D rigidBody;
    public Animator animator;
    PlayerControls controls;

    [Header("Important vars")]
    public bool grounded = true;                            //True when grounded in air false
    public bool airborn = false;                            //When jumping player is airborn

    [Header("Movement")]
    [SerializeField] float runSpeed;                        //Controls RunSpeed for player
    [SerializeField] float maxDownSpeed;                    //How fast the player moves down in air when pressing odwn
    [SerializeField] float downMovementForce;           //The force of how fast the player is pulled towards 
    [SerializeField] float maxDownSpeedWithPress;           //The force of how fast the player is pulled towards earth while pressing down
    private bool cancelMovement = false;                 //Used when aiming with hook that the player can not move while aiming
    private float lastHorizontMovement = 0f;                //Important for knowing which direction the next dash is gonna go, when not moving
    private float initalGravity;                            //The normal gravity is saved here to later restore a changed gravity amount;
    public Vector2 move;


    [Header("Dash")]
    [SerializeField] float dashForce;                 //Intial force behind a dash
    [SerializeField] float dashCooldown;               //How long the cooldown between dashes is
    [SerializeField] float dashMultiply;             //How far the player dashes
    private bool dashReady = true;                          //True -> PLayer can dash
    private bool dashOnlyOnceInAir = true;                  //Allows the player to only dash once in air
    private float dashCounter = 25f;                        //Counts intervalls between dashes

    [Header("JumpingMechanic")]
    [SerializeField] float jumpHeight;                //Controls jumpheight
    [SerializeField] float minimumJumpHeight;          //The minimum distance a player always jumps when pressing the jump button
    [SerializeField] float pogoHeight;                 //When hitting something with sword in the air, how far the player is send high;
    public bool fallingFromPlattform = false;
    private bool DoOnlyOnce = false;                        //Used for jumpig       -> Should rename that :D
    private float realJumpHeight;
    private float minimumJump = 0f;                         //Counter for minimumJump
    public float currentJumpDuration = 0f;                  //Length of current Jump
    public float jumpDuration = 5f;                         //The length of the maximum jump


    [Header("WallMecanicStuff")]
    [SerializeField] float downMovementForceWall;

    [SerializeField] float distanceWallJumpX;          //Distance the player will jump away from the wall
    [SerializeField] float moveFromWallAway;           //When sliding down the wall, player is set a few pixels away from the wall.    Maybe unnecessary ;D
    [SerializeField] float wallJumpHeight;             //Height player jumps with a wallJump
    [SerializeField] float wallSlideSpeed;                  //How fast the player slides down the wall
    private bool wallTouchMethodExecuted = false;
    private bool slideDownWall = false;                     //If the player wants to slide down the wall
    public bool touchWallLeft = false;                      //IF player is touching wall from the left
    public bool touchWallRight = false;                     //IF player is touching wall from the right
    public bool groundWallJump = false;                     //WHen standing next to a wall and jumping = true
    public bool canConnectToWAll = true;                    //When false the player can not conect to a wall
    private float lastWallTouched = 0f;                      //1 == left wall ,  2 == right wall
    public float wallJump = 0f;                             //0 = No touch , 1 = left Wall ,2 = right wall
    private float coyoteWallTime = 0f;                       //Counts down CooyteWallTime
    private float coyoteWallStartTime = 2f;                  //How long the player has time to press the button after leaving the wall and still beeing able to jump
    private float lastYPos;
    public Vector2 lastPositionOnWall;                      //Holds the position where a wall was touched last


    [Header("Double Jump")]
    [SerializeField] float doubleJumpIncreasment;           //Regulates ´how high the player can jump with double Jums
    public bool doubleJumpedAlready = false;               //Used for switching jump Animation 

    [Header("Hook")]
    public Transform hookTransform;
    [SerializeField] float hookCoolDownSeconds;
    [SerializeField] float hookForce;
    public bool CanShootHook = true;
    public bool hookPullActive = false;

    [Header("Unsure about")]
    private bool downMovement = false;                      //True = Player is pressing down , False not
    //[SerializeField] float jumpHeightRecument = 0.1f;       //The longer a jump goes on the jumpHeight is reduced

    [Header("new DAsh")]
    [SerializeField] float dashDuration;
    private bool canDash = true;
    private bool dashing = false;
    private void Awake()
    {
        controls = new PlayerControls();

        controls.GamePlay.Move.performed += temp => move = temp.ReadValue<Vector2>();
        controls.GamePlay.Move.canceled += temp => move = Vector2.zero;

        controls.GamePlay.Dodge.performed += temp => Dash2();

        controls.GamePlay.Jump.performed += temp => PreJumpCheck();
        controls.GamePlay.Jump.canceled += temp => { StopYAcceleration(); currentJumpDuration = jumpDuration; };

        controls.GamePlay.Attack.performed += temp => plAttack.Attack();


        controls.GamePlay.Hook.performed += temp => {
            if (CanShootHook)
            {
                hook.aimingActiveToggle();
                cancelMovement = true;
            }
        };
        controls.GamePlay.Hook.canceled += temp => {
            if (CanShootHook)
            {
                hook.shootHook();
                cancelMovement = false;
                CanShootHook = false;
                StartCoroutine(HookCooldown());
            }

        };
        controls.GamePlay.HookPull.performed += temp => {
            if (hook.hookAtTarget)
            {
                PullToHook2();
                hookPullActive = true;
            }
        };
        controls.GamePlay.HookPull.canceled += temp => {
            if (hook.hookAtTarget) hook.ResetHook();
        };

    }

    private IEnumerator HookCooldown()
    {
        if (CanShootHook) yield return null;
        yield return new WaitForSecondsRealtime(hookCoolDownSeconds);
        CanShootHook = true;
    }

    private void GetUseInput()
    {
        if (move.y < 0) downMovement = true;
        if (move.y >= 0) downMovement = false;
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
        initalGravity = rigidBody.gravityScale;
    }

    private void PreJumpCheck()
    {
        //Checking if Player is touching Wall
        if (((touchWallLeft || touchWallRight) || coyoteWallTime != 0))
        {

            if (touchWallLeft) wallJump = 1;
            else wallJump = 2;

            ResetJumpVar();
        }
        //Used for double
        else if (!doubleJumpedAlready && !grounded && (airborn || fallingFromPlattform) && !touchWallLeft && !touchWallRight)
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

        animator.SetFloat("Speed", Mathf.Abs(move.x));

        //Disables consecuent dashing while pressing the button (without not pressing it)
        if (dashCounter >= dashCooldown)
        {
            dashReady = true;
        }


        /*
        if ((Input.GetButton("Down") || (move.y < 0) && (airborn || (touchWallLeft || touchWallRight))))
        {
            downMovement = true;
        }

        */
        //Order is important 
        if (!wallTouchMethodExecuted && (touchWallRight || touchWallLeft))
        {
            touchWallLeft = false;
            touchWallRight = false;
            Debug.Log("set wrong");
            animator.SetBool("JumpingDown", true);
            animator.SetBool("Walled", false);
        }
    }

    /// <summary>
    /// //////////////////////////////////////////////
    /// </summary>
    /// 
    private void FixedUpdate()
    {
        //if (airborn && !cancelMovement) controller.Move2(move.x, false, false, hookPullActive);
        if (!touchWallLeft || !touchWallRight)
        {
            if (!cancelMovement) controller.Move2(move.x * runSpeed, false, false, hookPullActive);
            if (cancelMovement && grounded) controller.Move2(0, false, false, hookPullActive);
        }


        //wallTouchMethodExecuted = false;
        //Count down coyoteWallTime

        if (coyoteWallTime != 0) coyoteWallTime--;

        if ((touchWallLeft || touchWallRight) && downMovement) SlideDownWall();


        //Checks if the player is Airborn
        CheckIfAirborn();
        //Clamp gravityDownFallSpeed
        ClampGravity();
        //Counts intervall between dashes
        //dashCounter++;
        //Slide down Wall
        if (wallJump == 0 && (touchWallLeft || touchWallRight))
            rigidBody.velocity = new Vector2(0, wallSlideSpeed);

        //Jump
        Jump();
        //DownMovement in air
        DownMovement();
        //Stopp's jump in air
        JumpStopVelocity();





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


    //DEAD DASh Method
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
                //rigidBody.AddForce(new Vector2(lastHorizontMovement * (dashForce - (System.Math.Abs(move.x) * dashMultiply)), 0));
                rigidBody.AddForce(new Vector2(lastHorizontMovement * dashForce, 0));
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
            if (wallJump == 1 || lastWallTouched == 1)
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
        //a/asdfasdfasd
        if (move.y < 0 && airborn && !dashing && !touchWallRight && !touchWallLeft)
        {
            rigidBody.AddForce(new Vector2(0, -downMovementForce));
            animator.SetBool("JumpingUp", false);
            animator.SetBool("JumpingDown", true);
        }
    }
    public void OnLanding()
    {
        if (!grounded || hookPullActive)
        {
            hookPullActive = false;
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
            wallJump = 0;
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
        if (downMovement && rigidBody.velocity.y <= maxDownSpeedWithPress && !hookPullActive)
        {
            Vector2 temp = new Vector2(move.x * 10, maxDownSpeedWithPress);
            rigidBody.velocity = temp;
        }
        else if (!downMovement && rigidBody.velocity.y <= maxDownSpeed && !hookPullActive)
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
        if (grounded) return;
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
        if (grounded) return;
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
        hookPullActive = false;
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


    private void PullToHook2()
    {
        //rigidBody.gravityScale = 0;
        //rigidBody.velocity = Vector2.zero;]
        if (!hook.hookAtTarget) return;
        Vector2 direction = (Vector2)hookTransform.position - rigidBody.position;
        direction.Normalize();
        direction *= hookForce;
        rigidBody.AddForce(direction);
        //rigidBody.AddForceAtPosition((Vector2)hookTransform.position, rigidBody.position);
    }

    private void Dash2()
    {
        if (canDash)
        {
            hookPullActive = false;
            dashing = true;
            canDash = false;
            TogggleGravtiyToZero();
            cancelMovement = true;
            rigidBody.velocity = Vector2.zero;
            StartCoroutine(ApplyDashForce());
            StartCoroutine(DashCooldown2());
        }
    }
    private IEnumerator ApplyDashForce()
    {
        rigidBody.AddForce(new Vector2(lastHorizontMovement * dashForce, 0));
        yield return new WaitForSeconds(dashDuration);
        TogggleGravtiyToZero();
        cancelMovement = false;
        dashing = false;
    }
    private void TogggleGravtiyToZero()
    {
        if (rigidBody.gravityScale == 0) rigidBody.gravityScale = initalGravity;
        else rigidBody.gravityScale = 0;
    }
    private IEnumerator DashCooldown2()
    {
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;

    }
    //Used for sliding down the wall 
    private void SlideDownWall()
    {
        rigidBody.AddForce(new Vector2(0, downMovementForceWall));
    }
}

