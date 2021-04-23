using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] float angle;
    private bool SwingRight = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TailSwing();
    }

    private void TailSwing()
    {
        var temp = transform.eulerAngles.z;
        if (temp <= 360 && temp >= 340) angle *= -1;
        if (temp >= 180 && temp <= 250) angle *= -1;
        Debug.Log(temp);

        Debug.Log(angle);
        transform.Rotate(0,0,angle);
    }
}
