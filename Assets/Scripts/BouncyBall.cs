using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BouncyBall : MonoBehaviour
{
    [SerializeField] float force;
    [SerializeField] float bossUpScaling;
    [SerializeField] bool collideWithOtherBalls;
    [SerializeField] int notCollidingWithOtherBalls;
    public Rigidbody2D rb;
    public Transform whichDirection;
    public float amoutOfJumpsTillDeath =100;
    public GameObject ball;
    
    public bool isBoss = true;
    public Sprite sprite;
    public SpriteRenderer renderer;
    private void Start()
    {
        float tempX = whichDirection.position.x - rb.position.x;
        float tempY = whichDirection.position.y - rb.position.y;

        Vector2 forceVec = new Vector2(tempX, tempY);
        forceVec.Normalize();
        forceVec*= force;
        rb.AddForce(forceVec);
        if (!collideWithOtherBalls) ball.layer = notCollidingWithOtherBalls;
        if(isBoss)
        {
            
            //renderer.sprite = sprite;                     //Insert Boss Sprite here
            Vector3 temp = ball.transform.localScale;
            temp.x *= bossUpScaling;
            temp.y *= bossUpScaling;
            ball.transform.localScale = temp;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        amoutOfJumpsTillDeath--;
    }
    private void Update()
    {
        if (amoutOfJumpsTillDeath <= 0) Destroy(ball);
    }
}
