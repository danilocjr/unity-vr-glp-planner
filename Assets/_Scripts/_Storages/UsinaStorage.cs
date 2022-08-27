using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsinaStorage : MonoBehaviour
{
    public Usina usina;

    public GasodutoDistribuicao gasDist;
    public GameObject gasDistPipe;
    public OnlineMapsDrawingLine gasDistLine;
    public OnlineMapsDrawingElement frotaLine;
    public OnlineMapsDrawingElement frotaLine2;

    public OnlineMapsMarker3D termRegasMarker = null;
    public Transform termRegasPoint;

    MarkerDisplacer markerDisplacer;
    GameObject errorIconInstance;
    string _errorMessage;
    public string errorMessage;
    bool _error;
    public bool error;
    bool isFrota = false;

    SessionManager sessionManager;

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

        if (sessionManager == null)
            sessionManager = markerDisplacer.sessionManager;

        _error = false;
        _errorMessage = "";

        if (usina != null)
        {
            if (!string.IsNullOrEmpty(usina.nome))
            {
                if (usina.categoria == "Simulado")
                {
                    if (usina.potencia <= 0)
                    {
                        _error = true;
                        _errorMessage += "* Sem potência definida";
                    }

                    if (usina.eficiencia <= 0)
                    {
                        _error = true;
                        _errorMessage += "* Sem eficiência definida";
                    }

                    if (usina.despacho <= 0)
                    {
                        _error = true;
                        _errorMessage += "* Sem despacho definido";
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

                    errorMessage = "";
                    error = false;

                    foreach (ParticleSystem p in GetComponentsInChildren<ParticleSystem>())
                    {
                        if (!p.enableEmission)
                        {
                            p.enableEmission = true;
                        }

                    }
                }

                if (usina.fonteDoGas == "frota")
                {
                    if (gasDistPipe != null)
                        gasDistPipe.GetComponent<PipeScaleController>().canRender = false;

                    if (gasDistLine != null)
                    {
                        if (gasDistLine.visible)
                        {
                            gasDistLine.visible = false;
                        }
                    }

                    bool hasFrota = false;
                    List<OnlineMapsMarker3D> fsuMarkers = new List<OnlineMapsMarker3D>();
                    List<OnlineMapsMarker3D> frotaMarkers = new List<OnlineMapsMarker3D>();
                    foreach (OnlineMapsMarker3D m in markerDisplacer.markers)
                    {
                        if (m.instance.GetComponent<FrotaStorage>() != null)
                        {
                            if (termRegasMarker != null)
                            {
                                FrotaStorage frotaStorage = m.instance.GetComponent<FrotaStorage>();
                                foreach (Frota f in frotaStorage.frotas)
                                {
                                    if (f.terminalRegasDestino == termRegasMarker.instance.GetComponent<TerminalRegasStorage>().terminal.nome)
                                    {
                                        hasFrota = true;
                                        break;
                                    }
                                }
                            }
                            frotaMarkers.Add(m);

                        }
                        else if (m.instance.GetComponent<FSUStorage>() != null)
                        {
                            fsuMarkers.Add(m);
                        }
                    }

                    if (!hasFrota)
                    {
                        Vector2 usinaPos = new Vector2((float)usina.coordenadas.lat, (float)usina.coordenadas.lon);
                        if (frotaMarkers.Count > 0)
                        {
                            for (int x = 0; x < frotaMarkers.Count; x++)
                            {
                                for (int y = frotaMarkers.Count - 1; y >= x; y--)
                                {
                                    double distX = OnlineMapsUtils.DistanceBetweenPointsD(usinaPos, frotaMarkers[x].position);
                                    double distY = OnlineMapsUtils.DistanceBetweenPointsD(usinaPos, frotaMarkers[y].position);
                                    if (distX > distY)
                                    {
                                        OnlineMapsMarker3D temp = frotaMarkers[x];
                                        frotaMarkers[x] = frotaMarkers[y];
                                        frotaMarkers[y] = temp;
                                    }
                                }
                            }
                        }

                        GameObject frotaObj = frotaMarkers[0].instance;
                        OnlineMapsMarker3D fsuMarker = null;
                        foreach (OnlineMapsMarker3D m2 in fsuMarkers)
                        {
                            FSUStorage fsuStorage = m2.instance.GetComponent<FSUStorage>();
                            foreach (Frota f in frotaObj.GetComponent<FrotaStorage>().frotas)
                            {
                                if (f.terminalGnlOrigem == fsuStorage.fsu.terminalGnl)
                                {
                                    fsuMarker = m2;
                                    break;
                                }
                            }
                            if (fsuMarker != null)
                                break;
                        }

                        FSUDestinationBehaviour fsuManager = markerDisplacer.FSUManager;
                        fsuManager.fsuMarker = fsuMarker;
                        fsuManager.frotaObj = frotaObj;
                        fsuManager.BuildInUsina(GetComponent<OnlineMapsMarker3DInstance>().marker as OnlineMapsMarker3D);
                    }
                    if (frotaLine != null)
                    {
                        if (!frotaLine.visible)
                            frotaLine.visible = true;
                    }

                    if (frotaLine2 != null)
                    {
                        if (!frotaLine2.visible)
                            frotaLine2.visible = true;
                    }

                }
                else
                {

                    foreach (PontoDeEntrega pe in sessionManager.peFixos)
                    {
                        if (usina.malha == pe.nome)
                        {
                            usina.valorMalha = pe.valorMalha;
                        }
                    }
                    if (gasDistPipe != null)
                        gasDistPipe.GetComponent<PipeScaleController>().canRender = true;

                    if (gasDistLine != null)
                    {
                        if (!gasDistLine.visible)
                            gasDistLine.visible = true;
                    }

                    if (frotaLine != null)
                    {
                        if (frotaLine.visible)
                            frotaLine.visible = false;
                    }

                    if (frotaLine2 != null)
                    {
                        if (frotaLine2.visible)
                            frotaLine2.visible = false;
                    }


                }
            }
        }

        if (GetComponent<IconToggle>() != null)
            GetComponent<IconToggle>().hasError = error;

    }
}