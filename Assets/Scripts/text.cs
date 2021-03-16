using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class text : MonoBehaviour
{
    public PlayerMovement player;
    public Text var;

    // Update is called once per frame
    void Update()
    {
        var.text = "CurrentJumpD " + player.currentJumpDuration.ToString();

    }
}
