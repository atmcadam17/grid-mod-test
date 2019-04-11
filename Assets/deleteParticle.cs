using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deleteParticle : MonoBehaviour
{
    private ParticleSystem psystem;
    
    void Start()
    {
        psystem = gameObject.GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (psystem.isStopped)
        {
            Destroy(gameObject);
        }
    }
}
