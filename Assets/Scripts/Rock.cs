using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    public ParticleSystem death;
    private bool dieOnce = false;
    public SpriteRenderer render;
    public BoxCollider2D collider;
    public GameObject rock;

    // Start is called before the first frame update
    void Start()
    {
        
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(!dieOnce)
            death.Play();
            StartCoroutine(Die());
            collider.enabled = false;
            render.enabled = false;
            dieOnce = true;
        
    }

    private IEnumerator Die()
    {
        yield return new WaitForSeconds(1f);
        GameObject.Destroy(rock);
    }
}
