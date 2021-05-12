using System.Collections;
using UnityEngine;

public class BossSliding : MonoBehaviour
{
    public float maxSlidingSpeed;
    [SerializeField] float SlidingSpeed;

    public Transform firstPoint;
    public Transform secondPoint;
    public Rigidbody2D rb;

   
    private void FixedUpdate()
    {
        CheckPosition();

        rb.AddForce(new Vector2(SlidingSpeed , 0));
        Vector2 temp = rb.velocity;
        rb.velocity = new Vector2(Clamp(temp.x), temp.y);
        
    }
    private float Clamp(float x)
    {
        if (x >= maxSlidingSpeed) return maxSlidingSpeed;
        else return x;
    }

    private void CheckPosition()
    {
        if (rb.position.x >= secondPoint.position.x)
        {
            StartCoroutine(SetNewPosition());
        }
    }

    private IEnumerator SetNewPosition()
    {
        yield return new WaitForSeconds((float)0.1);
        Vector2 temp;
        temp.x = firstPoint.position.x;
        temp.y = Random.Range(firstPoint.position.y, 3);
        rb.position = temp;
    }
}
