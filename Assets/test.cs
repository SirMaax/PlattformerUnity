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
        
        Debug.Log(move);
        Vector3 vec3 = new Vector3(0, 2, 0);
        Vector3 temp = new Vector3(move.x, move.y, 0);
        temp = vec3 + temp;
        Debug.DrawLine(vec3, temp);
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
