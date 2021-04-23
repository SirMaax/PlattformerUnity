using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle_Soul : MonoBehaviour
{
    public ParticleSystem particels;

    public bool enableParticles = true;
    

    // Update is called once per frame
    void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);

        particels.enableEmission = enableParticles;
    }
    
    public void ToggleParticlesFly()
    {
        if (enableParticles) enableParticles = false;
        else enableParticles = true;
    }
}
