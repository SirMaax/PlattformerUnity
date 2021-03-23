using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator animator;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("TEST")) PlayerHit();
    }

    private void PlayerHit()
    {
        Debug.Log("Worked");
        animator.SetTrigger("PlayerHit");
    }
}
