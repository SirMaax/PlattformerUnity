using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private float horizontal = 0f;              //Keeps PlayerMovement horzontal from player
    private float vertical = 0f;
    private bool airborn = false;    // Update is called once per frame

    private float currentSwordAttackCounter = 0f;
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        airborn = player.airborn;
        currentSwordAttackCounter++;

        if (Input.GetButtonDown("Attack") && currentSwordAttackCounter >= swordAttackCooldown)
        {
            currentSwordAttackCounter = 0;
            if (airborn && vertical < 0)
            {
                animator.SetTrigger("AttackAirDown");
                swordChecker = swordPointDown;
            }
            else if(!airborn && vertical > 0)
            {
                animator.SetTrigger("AttackUpGrounded");
                swordChecker = swordPointUp;
            }
            else if (airborn && vertical > 0)
            {
                animator.SetTrigger("AttackAirUp");
                 swordChecker = swordPointUp;
            }
            else if (airborn)
            {
                animator.SetTrigger("AttackAirLR");
                swordChecker = swordPointNormal;
            }
            else
            {
                animator.SetTrigger("AttackLR");
                swordChecker = swordPointNormal;
            }
            Attacking();
        }    
    }

    private void Attacking()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(swordChecker.position, detectionCircle, enemieLayer);
        foreach(Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().HitBySword(horizontal * 500 + 50,vertical  * 500 + 50);
            
            if(airborn && vertical < 0) { 
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
