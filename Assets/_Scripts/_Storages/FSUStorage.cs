using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSUStorage : MonoBehaviour
{
    public FSU fsu;
    public GasodutoCriogenico gasCrio;
    public GameObject gasCrioPipe;

    SessionManager sessionManager;
    MarkerDisplacer markerDisplacer;
    GameObject errorIconInstance;
    string _errorMessage;
    public string errorMessage;
    bool _error;
    public bool error;

    public bool gasCrioUsingCalculation = false;

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
        {
            markerDisplacer = GameObject.Find("MapManager").GetComponent<MarkerDisplacer>();
        }
        if(sessionManager == null)
        {
            sessionManager = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        }
        if (markerDisplacer == null)
            return;
       
        _error = false;
        _errorMessage = "";

        if (fsu != null)
        {
            if (!string.IsNullOrEmpty(fsu.nome))
            {
                if (fsu.categoria == "Simulado")
                {
                    if (fsu.custos.Count <= 0)
                    {
                        _error = true;
                        _errorMessage += "* Sem custos inseridos";

                    }
                    else
                    {
                        foreach (Custo c in fsu.custos)
                        {
                            if (c.valor <= 0)
                            {
                                if (fsu.custoUsado == "capex" && c.tipo == "capex")
                                {
                                    _error = true;
                                    _errorMessage += "* CAPEX não especificado";
                                }
                                else if (fsu.custoUsado == "opex" && c.tipo == "opex")
                                {
                                    _error = true;
                                    _errorMessage += "* OPEX não especificado";
                                }

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
                                _errorMessage += "* Custo base do gasoduto não definido";
                            }

                            if (gasCrio.diametro <= 0)
                            {
                                _error = true;
                                _errorMessage += "* Diâmetro do gasoduto não definido";
                            }
                        }
                    }

                    if (fsu.gnlFornecido == 0)
                    {
                        _error = true;
                        _errorMessage += "* Quantidade de GNL fornecido não informado";
                    }
                    else if (sessionManager!=null)
                    {
                        if (sessionManager.faltaGas)
                        {
                            _error = true;
                            _errorMessage += "* Quantidade de GNL fornecido abaixo da demanda";
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
        }
        if (GetComponent<IconToggle>() != null)
            GetComponent<IconToggle>().hasError = error;

    }

}

