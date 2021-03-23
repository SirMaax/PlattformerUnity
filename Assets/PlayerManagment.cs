using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManagment : MonoBehaviour
{
    public Rigidbody2D rigidyBody;
    public BoxCollider2D boxCollider;
    public Animator animator;

    [SerializeField] float invincibilityTime;   //How long the player is invincible

    private float lastTimeHit = 0f;
    private float health = 50f;
    private List<int> layerList;                //The number stand for the layers the player will be able to recive dmg from

    private void Awake()
    {
        layerList = new List<int> { 8 };       //Defines from which layers the player can get hit
    }

    private void FixedUpdate()
    {
        CountDownInvincibility();
    }

   
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == null) return;
       
        GameObject tempObject = collision.gameObject;
        if (layerList.Contains(tempObject.layer))
        {
            //Object is in the specific layer
            PlayerIsHit();
        }
    }

    //Subtracts 10 Health and disables Collision with more Enemies
    private void PlayerIsHit()
    {
        animator.SetTrigger("PlayerHit");

        health -= 10;
        lastTimeHit = Time.time;

        Physics2D.IgnoreLayerCollision(3, 8, true);
    }

    //After beeing hit enables hitboxes again with the Enemy Layer 
    private void CountDownInvincibility()
    {
        if ((Time.time - lastTimeHit) >= invincibilityTime)
        {
            Physics2D.IgnoreLayerCollision(3, 8, false);
            Debug.Log("Player can be hit again");
        }
    }
}
