using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public Rigidbody2D rigidBody;
    public PlayerMovement player;
    public Collider2D collider;
    public EnemyAiGrounded enemyAiScript;
    
    private float health =30f;

    // Update is called once per frame
    void Update()
    {
        if(health == 0)
        {
            Die();
        }
    }

    public void HitBySword(float x, float y)
    {
        rigidBody.AddForce(new Vector2(x, y));
        health -= 10;
    }

    //This object dies
    private void Die()
    {
            enemyAiScript.enabled = false;
        rigidBody.velocity = Vector2.zero;  
            rigidBody.isKinematic = true;
            collider.enabled = false;
            this.enabled = false;
    }

}
