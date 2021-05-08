using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSliding : MonoBehaviour
{
    [SerializeField] float SlidingSpeed;
    [SerializeField] float maxSlidingSpeed;
    public Transform firstPoint;
    public Transform secondPoint;
    public Rigidbody2D rb;


   
    private void FixedUpdate()
    {
        CheckPosition();
        
        rb.AddForce(new Vector2(SlidingSpeed * Time.deltaTime, 0));
        rb.velocity = new Vector2(Clamp(rb.velocity.x), 0);


    }
    private float Clamp(float x)
    {
        if (x >= maxSlidingSpeed) return maxSlidingSpeed;
        else return x;
    }

    private void CheckPosition()
    {
        if (rb.position.x >= secondPoint.position.x) StartCoroutine(SetNewPosition());
        
    }

    private IEnumerator SetNewPosition()
    {
        yield return new WaitForSeconds((float)0.1);
        Vector2 temp;
        temp.x = firstPoint.position.x;
        temp.y = Random.Range(firstPoint.position.y, 0);
        rb.position = temp;
        
    }
}
