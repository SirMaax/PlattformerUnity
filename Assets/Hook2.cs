using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook2 : MonoBehaviour

{
    public HookShooter hookPlayer;

    private bool alreadyConntected = false;
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (alreadyConntected) return;
        hookPlayer.setHookAtTarget();
        alreadyConntected = true;
    }
}
