using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class test : MonoBehaviour
{

    PlayerControls playerControls;
    Vector2 move;
    float Force = 40;
    private void Awake()
    {

        playerControls = new PlayerControls();
        

        playerControls.GamePlay.Move.performed += temp => move = temp.ReadValue<Vector2>();
        playerControls.GamePlay.Move.canceled += temp => move = Vector2.zero;

    }
    private void Update()
    {
        Vector2 move2 = move * Force *Time.deltaTime;
        Debug.Log(move2);
    }
    private void OnEnable()
    {
        playerControls.GamePlay.Enable();
    }
    private void OnDisable()
    {
        playerControls.GamePlay.Disable();
    }
}
