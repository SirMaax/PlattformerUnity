using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManagment : MonoBehaviour
{
    public Rigidbody2D rigidyBody;
    public BoxCollider2D boxCollider;
    public Animator animator;

    private float health = 50f;
    private List<int> layerList;                //The number stand for the layers the player will be able to recive dmg from
    private void Awake()
    {
        layerList = new List<int> { 8 ,};       //Defines from which layers the player can get hit
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
    }

   
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == null) return;

        GameObject tempObject = collision.gameObject;
        if (layerList.Contains(tempObject.layer))
        {
            //Object is in the specific layer
            PlayerIsHit();
            print(tempObject.name);
        }
        
     
    }
    private void PlayerIsHit()
    {
        animator.SetTrigger("PlayerHit");

        health -= 10;

            
    }
    
}
