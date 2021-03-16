using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    public Rigidbody2D rigidBody;

    [SerializeField]float runSpeed = 40f;                   //Controlls RunSpeed for player
    [SerializeField] float jumpHeight = 50f;                //Controls jumpforce
    [SerializeField] float jumpDuration = 5f;               //The length of the maximum jump
    [SerializeField] float downMovementForce = 5;           //The force of how fast the player is pulled towards earth while pressing down
    [SerializeField] float minimumJumpHeight = 0f;          //The minimum distance a player always jumps when pressing the jump button
    [SerializeField] float jumpHeightRecument = 0.1f;       //The longer a jump goes on the jumpHeight is reduced
    
    public float currentJumpDuration = 0f;                 //Length of current Jump
    private float lastJumpYPosition = 0;                    //The starting Y Position of last jump

    private float horizontalMovement = 0f;                  //Important for moving left and right take input
    private float vertialMovement = 0f;                     //Goes from 3 to -3 -> Only controls down movement right now
    private bool downMovement = false;                       //True = Player is pressing down , False not

    private bool grounded = true;                           //True when grounded in air false
    private bool airborn = false;                           //When jumping player is airborn
    private float minimumJump = 0f;                       //Counter for minimumJump


    // Update is called once per frame
    void Update()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal") * runSpeed;
        vertialMovement = Input.GetAxisRaw("Vertical") * runSpeed;



        if (Input.GetButtonDown("Jump") && grounded && rigidBody.velocity.y == 0 )
        {
            airborn = true;
            grounded = false;
            
            lastJumpYPosition = rigidBody.position.y;

            currentJumpDuration = 0;
            minimumJump = 0;
            jumpHeight = 115;

        }
        if (Input.GetButtonUp("Jump") && airborn )
        {
            Debug.Log("Stump stopped via not pressing");
            stopYAcceleration();
            currentJumpDuration = jumpDuration;
        }

        if ((Input.GetButton("Down") || (vertialMovement < 0) && airborn))
        {
            downMovement = true;
        }
    }
    private void FixedUpdate()
    {
        controller.Move(horizontalMovement, false, false);


        if (airborn) currentJumpDuration++;
       
        //Makes the player jump the minimum JumpHeight
        if(minimumJump < minimumJumpHeight)
        {
            rigidBody.AddForce(new Vector2(0, jumpHeight));
            minimumJump += 0.1f;
        }
        //Applies additional force while jumping. Is stopped when jumpDuration is overstepped
        if (!grounded && currentJumpDuration < jumpDuration && minimumJump >= minimumJumpHeight)
        {
            rigidBody.AddForce(new Vector2(0, jumpHeight));
            minimumJump++;
            jumpHeight -= jumpHeightRecument;
        } 

        //When Player presses down
        if (downMovement && airborn)
        {
            rigidBody.AddForce(new Vector2(0, -downMovementForce));
        }

        //Stops air acceleration after jumpDuratio is ovestepped
        if(currentJumpDuration >= jumpDuration)
        {
            stopYAcceleration();
        }
    }

    public void OnLanding()
    {
        if (!grounded && airborn)
        {
            grounded = true;
            airborn = false;
            downMovement = false;
           
        }
    }

   
    public void stopYAcceleration()
    {
        if(minimumJump > minimumJumpHeight)        
        rigidBody.AddForce(new Vector2(0, -downMovementForce));
        
    }
}
