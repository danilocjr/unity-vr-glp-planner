using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
public class FrotaStorage : MonoBehaviour
{

    public List<Frota> frotas = new List<Frota>();

    public GasodutoCriogenico gasCrio;
    public GameObject gasCrioPipe;

    public Dictionary<string, OnlineMapsDrawingLine> frotaLines = new Dictionary<string, OnlineMapsDrawingLine>();
    public Dictionary<string, OnlineMapsDrawingLine> frotaLines2 = new Dictionary<string, OnlineMapsDrawingLine>();


    public double capacidadeLiq;

    MarkerDisplacer markerDisplacer;
    GameObject errorIconInstance;
    IconPanelElements iconPanel;
    string _errorMessage;
    public string errorMessage;
    bool _error;
    public bool error;

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
            if (markerDisplacer != null)
            {
                iconPanel = markerDisplacer.iconPanel.GetComponent<IconPanelElements>();
                capacidadeLiq = markerDisplacer.sessionManager.session.presets.frotaPreset.capacidadeLiquido;
            }
               
        }

        if (markerDisplacer == null)
            return;

        

        _error = false;
        _errorMessage = "";

        if (frotas.Count > 0)
        {
            foreach (Frota f in frotas)
                f.fretePorKm = markerDisplacer.sessionManager.session.premissas.frete;
            if (capacidadeLiq <= 0)
            {
                _error = true;
                _errorMessage += "* Capacidade de GNL não definida";
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
        if (GetComponent<IconToggle>() != null)
            GetComponent<IconToggle>().hasError = error;
    }
}
