using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public Rigidbody2D rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void HitBySword(float x, float y)

    {
        Debug.Log("Enemy hit");
        rigidBody.AddForce(new Vector2(x, y));
    }
}
