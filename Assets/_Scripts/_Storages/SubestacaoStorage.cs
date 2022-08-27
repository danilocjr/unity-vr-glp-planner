using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubestacaoStorage : MonoBehaviour
{
    public SubestacaoEnergia subestacao;

    private void Start()
    {
        foreach (ParticleSystem p in GetComponentsInChildren<ParticleSystem>())
            p.enableEmission = false;
    }

    
}
