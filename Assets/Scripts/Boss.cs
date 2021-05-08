using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    [SerializeField] float RockDistance;
    [SerializeField] float swingLeftandRight;               //Wont work when angle is adjusted from 2,5
    [SerializeField] float angle;
    private float currentSwingCounter = 0;                  //Holds the already
    public GameObject Rock;
    // Start is called before the first frame update

    private void Awake()
    {
        StartCoroutine(CallRocks());
    }
    // Update is called once per frame
    void Update()
    {
        TailSwing();
    }
    private void FixedUpdate()
    {
        
        
    }

    private void TailSwing()
    {
        if (currentSwingCounter <= swingLeftandRight) { 
            var temp = transform.eulerAngles.z;
            if(temp >= 180 && temp <= 181)
            {
                currentSwingCounter++;
            }
            if (temp <= 360 && temp >= 340)
            {
                angle *= -1;
            }
            if (temp >= 180 && temp <= 250)
            {
                angle *= -1;
            }
                transform.Rotate(0, 0, angle);
        }
    }

    private IEnumerator CallRocks()
    {
        while (true)
        {
            
            yield return new WaitForSeconds(1f);
            RocksFalling();
        }
    }


    private void RocksFalling()
    {
        
        float[] list = new float[6];
        float x = -24;
        float x2 = -19;
        for(int i = 0; i <= 5; i++)
        {
            list[i] = Random.Range(x, x2);
            x += RockDistance;
            x2 += RockDistance;
        }

        foreach (float ele in list)
        {
            GameObject newRock = Instantiate(Rock);
            newRock.transform.position = new Vector2(ele, 4.5f);
        }
    }
}
    