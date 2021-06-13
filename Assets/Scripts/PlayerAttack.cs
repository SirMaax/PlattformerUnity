using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] float swordAttackCooldown;
    
    public Animator animator;
    public Transform swordChecker;
    public PlayerMovement player;

    public Transform swordPointNormal;
    public Transform swordPointUp;
    public Transform swordPointDown;
    public float detectionCircle = 0.5f;
    public LayerMask enemieLayer;


    private bool airborn = false;   

    private float currentSwordAttackCounter = 0f;

    Vector2 move;

 


    public void Attack()
    {
        if (currentSwordAttackCounter <= swordAttackCooldown) return;       
        
        if(airborn && move.y < 0)//Air attack down
        {
            animator.SetTrigger("AttackAirDown");
            swordChecker = swordPointDown;
        }
        else if (!airborn && move.y > 0)//Grounded Attack up
        {
            animator.SetTrigger("AttackUpGrounded");
             swordChecker = swordPointUp;
        }
        else if (airborn && move.y > 0)//Airborn attack up
        {
            animator.SetTrigger("AttackAirUp");
            swordChecker = swordPointUp;
        }
        else if (airborn)//Attack facing direction in air
        {
            animator.SetTrigger("AttackAirLR");
            swordChecker = swordPointNormal;
        }
        else            //Facing attack on grounded
        {
            animator.SetTrigger("AttackLR");
            swordChecker = swordPointNormal;
        }
        Attacking();
    }

    void Update()
    {
        airborn = player.airborn;
        move = player.move;
    }
    private void FixedUpdate()
    {
        currentSwordAttackCounter++;
    }

    private void Attacking()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(swordChecker.position, detectionCircle, enemieLayer);
        foreach(Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().HitBySword(move.x * 500 + 50,move.y  * 500 + 50);
            
            if(airborn && move.y < 0) { //No boost while jumping up !!! Is maybe not working
                Debug.Log("Shoul get boost");
                player.HitTargetInAir();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (swordPointNormal == null) return;
        //Gizmos.DrawWireSphere(swordPointDown.position, detectionCircle);
    }
   
}
