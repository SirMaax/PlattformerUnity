using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManagment : MonoBehaviour
{
    public Rigidbody2D rigidyBody;
    public BoxCollider2D boxCollider;
    public Animator animator;

    [SerializeField] float invincibilityTime;   //How long the player is invincible
    [SerializeField] float knockBackFromEnemies;
    [SerializeField] float knockBackFromEnemiesWhenUnderThem;

    public GameObject Health5;
    public GameObject Health4;
    public GameObject Health3;
    public GameObject Health2;
    public GameObject Health1;
    public GameObject Health0;
    private GameObject[] sprites;

    private GameObject currentHealthSprite;
    private float lastTimeHit = 0f;
    private float health = 5f;
    private List<int> layerList;                //The number stand for the layers the player will be able to recive dmg from
    private bool invincble = false;
    private Vector2 forceVec;
    public PlayerMovement player;
    private void Awake()
    {
        layerList = new List<int> { 8 };       //Defines from which layers the player can get hit
        sprites = new GameObject[] { Health0, Health1, Health2, Health3, Health4, Health5 };
        currentHealthSprite = Health5;
        Health5.GetComponent<SpriteRenderer>().enabled = true;
    }
    private void Update()
    {
        if (Input.GetButtonDown("HealingTest"))
        {
            Healing();
            UpdateHealth();
        }
    }

    private void FixedUpdate()
    {
        CountDownInvincibility();
        DisableJumpWhenHit();

    }


    private void OnCollisionEnter2D(Collision2D collision)
    {   
        if (invincble ||collision.gameObject == null) return;

        GameObject tempObject = collision.gameObject;
        if (layerList.Contains(tempObject.layer))
        {
            Knockback(tempObject);
            invincble = true;
            //Object is in the specific layer
            PlayerIsHit();
        }
    }

    //Subtracts 10 Health and disables Collision with more Enemies
    private void PlayerIsHit()
    {
        animator.SetTrigger("PlayerHit");

        health -= 1;
        lastTimeHit = Time.time;

        Physics2D.IgnoreLayerCollision(3, 8, true);
        UpdateHealth();
    }

    //After beeing hit enables hitboxes again with the Enemy Layer 
    private void CountDownInvincibility()
    {
        if ((Time.time - lastTimeHit) >= invincibilityTime)
        {
            Physics2D.IgnoreLayerCollision(3, 8, false);
            invincble = false;    
        }   
    }

    private void Knockback(GameObject tempObject)
    {
        Vector2 enemyVec = tempObject.transform.position;
        Vector2 playerVec = rigidyBody.transform.position;
         forceVec = playerVec - enemyVec;

        forceVec.Normalize();

        forceVec.x *= knockBackFromEnemies;
        forceVec.y *= knockBackFromEnemies;
        forceVec.y /= 2;

        if (rigidyBody.transform.position.y < tempObject.transform.position.y) forceVec.y += knockBackFromEnemiesWhenUnderThem;
        rigidyBody.velocity = Vector2.zero;

        rigidyBody.AddForce(forceVec);
    }


    private void DisableJumpWhenHit()
    {
        if(invincble)
        player.currentJumpDuration = player.jumpDuration;
    }

    //Disables old HealthSprite and enables new one;
    private void UpdateHealth()
    {
        if (health >= 0) { 
        currentHealthSprite.GetComponent<SpriteRenderer>().enabled = false;
        currentHealthSprite = sprites[(int)health];
        currentHealthSprite.GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    private void Healing()
    {
        if (health < 5) health++;
    }

    public void TakeDamage()
    {
        if (invincble) return;
        invincble = true;

        PlayerIsHit();
    }
}
