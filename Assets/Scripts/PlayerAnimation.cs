using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator animator;

    // Update is called once per frame
    void Update()
    {
    }

    private void PlayerHit()
    {
        animator.SetTrigger("PlayerHit");
    }
}
