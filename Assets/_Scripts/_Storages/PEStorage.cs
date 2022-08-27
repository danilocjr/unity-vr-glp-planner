using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PEStorage : MonoBehaviour
{
    public PontoDeEntrega pe;
    public List<GasodutoIntegrador> gasIntList;
    public List<GasodutoDistribuicao> gasDistList;

    private void Start()
    {
        foreach (ParticleSystem p in GetComponentsInChildren<ParticleSystem>())
            p.enableEmission = false;
    }
}
