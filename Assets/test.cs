using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class test : MonoBehaviour
{
    public Transform hookTransform;
    public float hookForce = 400;
    public Rigidbody2D rigidBody;
    public PlayerControls controls;
    bool hookPullActive = false;

    private void Awake()
    {

        controls = new PlayerControls();

        controls.GamePlay.HookPull.performed += temp => { hookPullActive = true; };
        controls.GamePlay.HookPull.canceled += temp => { hookPullActive = false; };
    }


    private void FixedUpdate()
    {
        if (hookPullActive) PullToHook();
    }
    
    private void PullToHook()
    {
        Vector2 direction = (Vector2)hookTransform.position - rigidBody.position;
        direction.Normalize();
        direction *= hookForce;
        rigidBody.AddForce(direction);
    }
}
