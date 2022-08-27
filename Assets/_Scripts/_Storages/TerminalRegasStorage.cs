using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerminalRegasStorage : MonoBehaviour
{
    public TerminalRegas terminal;

    public GasodutoCriogenico gasCrio;
    public GasodutoIntegrador gasInt;
    public GameObject gasIntPipe;
    public OnlineMapsDrawingLine gasIntLine;

    MarkerDisplacer markerDisplacer;
    GameObject errorIconInstance;
    string _errorMessage;
    public string errorMessage;
    bool _error;
    public bool error;

    public bool gasIntUsingCalculation = true;

    private void Start()
    {
        foreach (ParticleSystem p in GetComponentsInChildren<ParticleSystem>())
        {
            p.enableEmission = false;
        }
        SetState();
    }

    public void SetState()
    {
        if (markerDisplacer == null)
            markerDisplacer = GameObject.Find("MapManager").GetComponent<MarkerDisplacer>();

        if (markerDisplacer == null)
            return;
   
        _error = false;
        _errorMessage = "";

        if (terminal != null)
        {
            if (!string.IsNullOrEmpty(terminal.nome))
            {
                if (terminal.categoria == "Simulado")
                {
                    if (terminal.custos.Count <= 0)
                    {
                        _error = true;
                        _errorMessage += "* Sem custos inseridos";
                    }
                    else
                    {
                        foreach (Custo c in terminal.custos)
                        {
                            if (c.valor <= 0)
                            {
                                if (terminal.custoUsado == "capex" && c.tipo == "capex")
                                {
                                    _error = true;
                                    _errorMessage += "* CAPEX não especificado";
                                }
                                else if (terminal.custoUsado == "opex" && c.tipo == "opex")
                                {
                                    _error = true;
                                    _errorMessage += "* OPEX não especificado";
                                }
                            }
                        }
                    }                 

                    if (gasInt != null)
                    {
                        if (!string.IsNullOrEmpty(gasInt.nome))
                        {
                            if (gasInt.custoBase <= 0)
                            {
                                _error = true;
                                _errorMessage += "* Custo base do gasoduto integrador não definido";
                            }

                            if (gasInt.diametro <= 0)
                            {
                                _error = true;
                                _errorMessage += "* diâmetro do gasoduto integradornão definido";
                            }
                        }
                    }
                    if (gasCrio != null)
                    {
                        if (!string.IsNullOrEmpty(gasCrio.nome))
                        {
                            if (gasCrio.custoBase <= 0)
                            {
                                _error = true;
                                _errorMessage += "* Custo base do gasoduto criogênico não definido";
                            }

                            if (gasCrio.diametro <= 0)
                            {
                                _error = true;
                                _errorMessage += "* diâmetro do gasoduto criogênico não definido";
                            }
                        }
                    }

                }

                if (_error)
                {
                    if (errorIconInstance == null)
                    {
                        errorIconInstance = Instantiate(markerDisplacer.errorIcon, transform);
                        //errorIconInstance.transform.localPosition = new Vector3(0, 0, 0.04f);
                        //errorIconInstance.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    }
                    error = true;
                    errorMessage = _errorMessage;
                    foreach (ParticleSystem p in GetComponentsInChildren<ParticleSystem>())
                    {
                        if (p.enableEmission)
                            p.enableEmission = false;
                    }
                }
                else
                {
                    if (errorIconInstance != null)
                    {
                        GameObject temp = errorIconInstance;
                        errorIconInstance = null;
                        Destroy(temp);
                    }
                    error = false;
                    errorMessage = "";
                    foreach (ParticleSystem p in GetComponentsInChildren<ParticleSystem>())
                    {
                        if (!p.enableEmission)
                            p.enableEmission = true;
                    }
                }
            }
            //else
            //{
            //    if (GetComponent<OVRGrabbable>() != null)
            //    {
            //        if (GetComponent<OVRGrabbable>().isGrabbed)
            //        {
            //            foreach (ParticleSystem p in GetComponentsInChildren<ParticleSystem>())
            //            {
            //                p.enableEmission = true;
            //            }
            //        }
            //        else
            //        {
            //            foreach (ParticleSystem p in GetComponentsInChildren<ParticleSystem>())
            //            {
            //                p.enableEmission = false;
            //            }
            //        }
            //    }
            //}
        }
        else
        {
            //if (GetComponent<OVRGrabbable>() != null)
            //{
            //    if (GetComponent<OVRGrabbable>().isGrabbed)
            //    {
            //        foreach (ParticleSystem p in GetComponentsInChildren<ParticleSystem>())
            //        {
            //            p.enableEmission = true;
            //        }
            //    }
            //    else
            //    {
            //        foreach (ParticleSystem p in GetComponentsInChildren<ParticleSystem>())
            //        {
            //            p.enableEmission = false;
            //        }
            //    }
            //}

        }
        if (GetComponent<IconToggle>() != null)
            GetComponent<IconToggle>().hasError = error;

    }
}
