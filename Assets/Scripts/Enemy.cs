using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public Rigidbody2D rigidBody;
    public PlayerMovement player;
    public PolygonCollider2D collider;

    private float health =30f;
    private bool died = false;                  //This Enemy can only die once

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
        if(!died)
        collider.enabled = false;
        died = true;
    }

    private void RunTowardsPlayer()
    {
        
    }
}
