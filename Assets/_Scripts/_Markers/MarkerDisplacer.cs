using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//SCRIPT UTILIZADO PARA COLOCAR ÍCONES NO MAPA E PARA MANIPULÁ-LOS
public class MarkerDisplacer : MonoBehaviour
{

    public bool canGrab = true;
    public SessionManager sessionManager;
    public GasClasses gasClasses;
    public TextMesh msg;
    public ConnectionCreator connCreator;
    public OnlineMaps maps;

    public List<OVRGrabbable> grabbables;
    public List<Vector3> grababblesInitialPos;
    Quaternion grabbablesInitRot;
    public OVRGrabbable grabbable;


    public OVRGrabber[] grabbers;
    public OVRGrabber grabber;


    OnlineMapsMarker3DManager markerManager;
    OnlineMapsTileSetControl tileset;

    public GameObject markerPoint;

    public int defaultZoom = 17;

    //Markers inseridos pelo usuário
    public List<OnlineMapsMarker3D> markers;

    //Todos os markers presentes no mapa
    public List<OnlineMapsMarker3D> allInstantiatedMarkers;


    public float markerScale;

    public MapController mapController;
    public GameObject markerMenu;
    GameObject _markerMenu;
    bool pointerOnPlacedMarker;

    [SerializeField]
    GameObject placedMarker;

    public FSUDestinationBehaviour FSUManager;
    public GNLDisplacer gnlManager;

    int usinaIndex;
    int fsruIndex;
    int fsuIndex;
    int peIndex;
    public int terminalIndex;
    public int frotaIndex;
    public int gnlIndex;


    public GameObject errorIcon;
    public GameObject iconPanel;

    [SerializeField] GameObject pePanel;
    private PETablePanelBehaviour peTableManager;

    public GameObject debugPrefab;
    public GameObject debugPrefab2;

    const string OnRaycastExitMessage = "OnRaycastExit";
    const string OnRaycastEnterMessage = "OnRaycastEnter";

    GameObject previousMarkerLeft;
    GameObject previousMarkerRight;

    IconPanelElements panelElements;
    void Start()
    {
        markerManager = maps.GetComponent<OnlineMapsMarker3DManager>();
        tileset = maps.GetComponent<OnlineMapsTileSetControl>();

        markers = new List<OnlineMapsMarker3D>();
        mapController = GetComponent<MapController>();

        grababblesInitialPos = new List<Vector3>();
        foreach (OVRGrabbable g in grabbables)
        {
            if (g != null)
                grababblesInitialPos.Add(g.transform.position);
        }
        grabbablesInitRot = grabbables[0].transform.rotation;

        iconPanel.SetActive(false);
        pePanel.SetActive(false);

        panelElements = iconPanel.GetComponent<IconPanelElements>();
        peTableManager = pePanel.GetComponent<PETablePanelBehaviour>();

    }
    void FixedUpdate()
    {
        foreach (OVRGrabbable g in grabbables)
        {
            g.gameObject.GetComponent<BoxCollider>().enabled = canGrab;
        }

        foreach (OVRGrabber _grabber in grabbers)
        {
            if (_grabber.grabbedObject != null)
            {
                grabber = _grabber;
                if (grabbable == null || grabbable != _grabber.grabbedObject)
                {
                    grabbable = _grabber.grabbedObject;
                    if (grabbable.GetComponent<OnlineMapsMarker3D>() == null)
                    {
                        foreach (OVRGrabbable g in grabbables)
                        {
                            g.enabled = (g == grabbable);
                        }
                    }
                }
            }
            else
            {
                if (grabbable != null)
                {
                    panelElements.HideSelf();
                    peTableManager.HideSelf();
                    if (grabber != null)
                    {
                        RaycastHit hit = grabber.GetComponentInChildren<HandPointer>().GetMarkerDestination();
                        if (hit.collider)
                        {
                            if (!grabbable.isGrabbed)
                            {

                                grabber.GetComponentInChildren<HandPointer>().markerSign.SetActive(false);

                                GameObject pointObj = Instantiate(markerPoint, maps.gameObject.transform);

                                pointObj.transform.position = hit.point;
                                pointObj.transform.localPosition = new Vector3(pointObj.transform.localPosition.x, 0, pointObj.transform.localPosition.z);

                                Vector2 coords = tileset.GetCoordsByWorldPosition(pointObj.transform.position);

                                OnlineMapsMarker3D marker = markerManager.Create(coords.x, coords.y, grabbable.gameObject);

                                if (marker.transform.GetComponent<OVRGrabbable>() != null)
                                    Destroy(marker.transform.GetComponent<OVRGrabbable>());

                                foreach (Transform t in marker.transform.gameObject.GetComponentsInChildren<Transform>())
                                    t.gameObject.layer = 14;

                                marker.GetPosition(out double lng, out double lat);

                                if (marker.transform.GetComponent<UsinaStorage>() != null)
                                {
                                    Usina usina = new Usina("Usina_" + usinaIndex, 50000,
                                        sessionManager.session.presets.usinaPreset.eficiencia, sessionManager.session.gas, sessionManager.session.premissas,
                                        sessionManager.session.presets.usinaPreset.despacho);

                                    usina.categoria = "Simulado";
                                    usina.fonteDoGas = "gasoduto";

                                    marker.transform.gameObject.name = usina.nome;

                                    usina.SetCoordenadas(Convert.ToSingle(lng), Convert.ToSingle(lat));

                                    UsinaStorage usinaStorage = marker.transform.GetComponent<UsinaStorage>();

                                    usinaStorage.usina = usina;

                                    usinaStorage.termRegasMarker = null;

                                    connCreator.ConectarUsinaComPEProximo(marker.transform.gameObject, true);

                                    sessionManager.session.usinasColocadas.Add(usina);
                                    usinaStorage.SetState();
                                    usinaIndex++;
                                }
                                else if (marker.transform.GetComponent<FSRUStorage>() != null)
                                {
                                    FSRU fsru = new FSRU();
                                    fsru.nome = "FSRU_" + fsruIndex;
                                    fsru.custoUsado = "opex";
                                    fsru.gnlFornecido = 315000;
                                    marker.transform.gameObject.name = fsru.nome;
                                    fsru.categoria = "Simulado";
                                    fsru.SetCusto("opex", 3000000, "dolar");
                                    fsru.SetCoordenadas(Convert.ToSingle(lng), Convert.ToSingle(lat));

                                    marker.transform.GetComponent<FSRUStorage>().fsru = fsru;
                                    sessionManager.session.fsruColocados.Add(fsru);
                                    fsruIndex++;

                                    OnlineMapsMarker3D gnlMarker = IsFSNearToGNLTerminal(marker);
                                    if (gnlMarker != null)
                                    {
                                        if (gnlMarker.instance.GetComponent<GNLStorage>().fsruMarker.instance == null)
                                        {
                                            GNLStorage gnlStorage = gnlMarker.instance.GetComponent<GNLStorage>();
                                            fsru.terminalGnl = gnlStorage.gnl.nome;
                                            gnlStorage.fsruMarker = marker;
                                            connCreator.ConectarFSRUComPEProximo(marker.instance, true);
                                        }
                                        else
                                        {
                                            sessionManager.session.fsruColocados.Remove(fsru);
                                            fsruIndex--;
                                            markerManager.Remove(marker);
                                        }
                                    }
                                    else
                                    {
                                        gnlManager.ShowGNLDisplacerPanel(marker);
                                    }
                                }
                                else if (marker.transform.GetComponent<FSUStorage>() != null)
                                {
                                    FSU fsu = new FSU("FSU_" + fsuIndex, "Simulado");
                                    fsu.SetCoordenadas(Convert.ToSingle(marker.position.x), marker.position.y);
                                    fsu.SetCusto("opex", 3000000, "dolar");

                                    fsu.custoUsado = "opex";
                                    marker.transform.GetComponent<FSUStorage>().fsu = fsu;
                                    sessionManager.session.fsuColocados.Add(fsu);
                                    fsuIndex++;

                                    OnlineMapsMarker3D gnlMarker = IsFSNearToGNLTerminal(marker);
                                    if (gnlMarker != null)
                                    {
                                        if (gnlMarker.instance.GetComponent<GNLStorage>().fsuMarker.instance == null)
                                        {
                                            gnlMarker.instance.GetComponent<GNLStorage>().fsuMarker = marker;
                                            fsu.terminalGnl = gnlMarker.instance.GetComponent<GNLStorage>().gnl.nome;
                                            canGrab = false;
                                            marker.transform.gameObject.name = fsu.nome;

                                            FSUManager.fsuMarker = marker;
                                            FSUManager.selectedTerminalGNLMarker = gnlMarker;
                                            FSUManager.canPlaceFSU = false;
                                            FSUManager.ShowFSUDestinationPanel();
                                        }
                                        else
                                        {
                                            sessionManager.session.fsuColocados.Remove(fsu);
                                            fsuIndex--;
                                            markerManager.Remove(marker);
                                        }
                                    }
                                    else
                                    {
                                        gnlManager.ShowGNLDisplacerPanel(marker);
                                    }

                                }
                                else if (marker.transform.GetComponent<TerminalRegasStorage>() != null)
                                {
                                    TerminalRegas terminal = new TerminalRegas("Terminal_" + terminalIndex);

                                    marker.transform.gameObject.name = terminal.nome;

                                    terminal.SetCoordenadas(Convert.ToSingle(lng), Convert.ToSingle(lat));

                                    terminal.categoria = "Simulado";

                                    sessionManager.session.terminaisRegasColocados.Add(terminal);

                                    marker.transform.GetComponent<TerminalRegasStorage>().terminal = terminal;

                                    terminalIndex++;
                                }
                                else if (marker.transform.GetComponent<PEStorage>() != null)
                                {
                                    PontoDeEntrega pe = new PontoDeEntrega();
                                    pe.nome = "PE_" + peIndex;
                                    marker.transform.gameObject.name = pe.nome;
                                    pe.categoria = "Simulado";
                                    pe.SetCoordenadas(Convert.ToSingle(lng), Convert.ToSingle(lat));
                                    sessionManager.session.pontosDeEntrega.Add(pe);
                                    marker.transform.GetComponent<PEStorage>().pe = pe;
                                    peIndex++;
                                }
                                if (marker != null)
                                {
                                    marker.transform.localRotation = Quaternion.Euler(270, 0, 180);
                                    marker.scale = markerScale;

                                    markers.Add(marker);
                                    allInstantiatedMarkers.Add(marker);
                                    Destroy(marker.transform.GetComponent<OVRGrabbable>());
                                    marker.instance.GetComponent<IconNamePanel>().isOnMap = true;
                                }
                                Destroy(pointObj);
                                grabbable.transform.position = grababblesInitialPos[grabbables.IndexOf(grabbable)];
                                grabbable.transform.rotation = grabbablesInitRot;

                                grabbable = null;
                                sessionManager.UpdateSession();

                            }
                        }
                        else
                        {
                            if (!grabbable.isGrabbed)
                            {
                                grabbable.transform.position = grababblesInitialPos[grabbables.IndexOf(grabbable)];
                                grabbable.transform.rotation = grabbablesInitRot;
                                grabbable = null;
                            }
                            grabber.GetComponentInChildren<HandPointer>().markerSign.SetActive(false);
                        }
                    }
                }
                else
                {

                    RaycastHit hit = _grabber.GetComponentInChildren<HandPointer>().GetPlacedMarker();

                    if (hit.collider)
                    {
                        if (_grabber == grabbers[0])
                        {
                            if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
                            {
                                placedMarker = hit.collider.gameObject;
                                ShowMarkerMenu();
                            }
                            else if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger))
                            {
                                panelElements.HideSelf();
                                peTableManager.HideSelf();
                                placedMarker = hit.collider.gameObject;
                                if (placedMarker.GetComponent<PEStorage>() == null)
                                {
                                    OnlineMapsMarker3D _placedMarker = placedMarker.GetComponent<OnlineMapsMarker3DInstance>().marker as OnlineMapsMarker3D;
                                    StopCoroutine(MoveMarker(_placedMarker, _grabber));
                                    StartCoroutine(MoveMarker(_placedMarker, _grabber));
                                }
                            }
                        }
                        else
                        {
                            if (OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
                            {
                                placedMarker = hit.collider.gameObject;
                                ShowMarkerMenu();
                            }
                            else if (OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger))
                            {
                                panelElements.HideSelf();
                                peTableManager.HideSelf();
                                placedMarker = hit.collider.gameObject;
                                if (placedMarker.GetComponent<PEStorage>() == null)
                                {
                                    OnlineMapsMarker3D _placedMarker = placedMarker.GetComponent<OnlineMapsMarker3DInstance>().marker as OnlineMapsMarker3D;
                                    StopCoroutine(MoveMarker(_placedMarker, _grabber));
                                    StartCoroutine(MoveMarker(_placedMarker, _grabber));
                                }
                            }
                        }
                    }
                    else
                    {
                        if (_grabber.GetComponentInChildren<HandPointer>().GetHitFromMap().collider)
                        {
                            if (_grabber == grabbers[0])
                            {
                                if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
                                {
                                    panelElements.HideSelf();
                                    peTableManager.HideSelf();
                                }
                            }
                            else
                            {
                                if (OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
                                {
                                    panelElements.HideSelf();
                                    peTableManager.HideSelf();
                                }
                            }
                        }
                    }

                    RaycastHit anyMarkerHit = _grabber.GetComponentInChildren<HandPointer>().GetAnyMarkerOnMap();
                    if (anyMarkerHit.collider)
                    {
                        GameObject current = anyMarkerHit.collider.gameObject;
                        if (_grabber == grabbers[0])
                        {
                            if (current.GetComponent<IconNamePanel>() != null)
                            {
                                if (current.GetComponent<IconNamePanel>().isOnMap)
                                {
                                    if (previousMarkerLeft != null)
                                    {
                                        if (previousMarkerLeft != current)
                                        {
                                            SendMessageTo(previousMarkerLeft, OnRaycastExitMessage);
                                            SendMessageTo(current, OnRaycastEnterMessage);
                                            previousMarkerLeft = current;
                                        }
                                    }
                                    else
                                    {
                                        SendMessageTo(current, OnRaycastEnterMessage);
                                        previousMarkerLeft = current;
                                    }
                                }
                                else
                                {
                                    if (previousMarkerLeft != null)
                                    {
                                        SendMessageTo(previousMarkerLeft, OnRaycastExitMessage);
                                        previousMarkerLeft = null;
                                    }
                                }
                            }
                            else
                            {
                                if (previousMarkerLeft != null)
                                {
                                    SendMessageTo(previousMarkerLeft, OnRaycastExitMessage);
                                    previousMarkerLeft = null;
                                }
                            }
                        }
                        else
                        {
                            if (current.GetComponent<IconNamePanel>() != null)
                            {
                                if (current.GetComponent<IconNamePanel>().isOnMap)
                                {
                                    if (previousMarkerRight != null)
                                    {
                                        if (previousMarkerRight != current)
                                        {
                                            SendMessageTo(previousMarkerRight, OnRaycastExitMessage);
                                            SendMessageTo(current, OnRaycastEnterMessage);
                                            previousMarkerRight = current;
                                        }
                                    }
                                    else
                                    {
                                        SendMessageTo(current, OnRaycastEnterMessage);
                                        previousMarkerRight = current;
                                    }
                                }
                                else
                                {
                                    if (previousMarkerRight != null)
                                    {
                                        SendMessageTo(previousMarkerRight, OnRaycastExitMessage);
                                        previousMarkerRight = null;
                                    }
                                }
                            }
                            else
                            {
                                if (previousMarkerRight != null)
                                {
                                    SendMessageTo(previousMarkerRight, OnRaycastExitMessage);
                                    previousMarkerRight = null;
                                }
                            }
                        }
                    }


                }
                if (_markerMenu != null)
                {
                    _markerMenu.transform.rotation = Quaternion.LookRotation(_markerMenu.transform.position - Camera.main.transform.position);
                }


            }
        }
    }
    public void ShowMarkerMenu()
    {
        panelElements.HideSelf();
        peTableManager.HideSelf();
        if (placedMarker.GetComponent<UsinaStorage>() != null)
        {
            iconPanel.SetActive(true);
            panelElements.ShowUsinaConfig(placedMarker.GetComponent<UsinaStorage>());
        }
        else if (placedMarker.GetComponent<FSRUStorage>() != null)
        {
            iconPanel.SetActive(true);
            panelElements.ShowFSRUConfig(placedMarker.GetComponent<FSRUStorage>());
        }
        else if (placedMarker.GetComponent<FSUStorage>() != null)
        {
            iconPanel.SetActive(true);
            panelElements.ShowFSUConfig(placedMarker.GetComponent<FSUStorage>());
        }
        else if (placedMarker.GetComponent<TerminalRegasStorage>() != null)
        {
            iconPanel.SetActive(true);
            panelElements.ShowTermRegasConfig(placedMarker.GetComponent<TerminalRegasStorage>());
        }
        else if (placedMarker.GetComponent<FrotaStorage>() != null)
        {
            iconPanel.SetActive(true);
            panelElements.ShowFrotaConfig(placedMarker.GetComponent<FrotaStorage>());
        }
        else if (placedMarker.GetComponent<PEStorage>() != null)
        {
            pePanel.SetActive(true);
            pePanel.GetComponent<PETablePanelBehaviour>().SetTable(placedMarker.GetComponent<PEStorage>().pe);
        }
    }



    public void HideMarkerMenu()
    {
        iconPanel.SetActive(false);
    }


    public void DeleteMarker()
    {
        if (placedMarker != null)
        {
            bool hasComponent = true;
            if (placedMarker.GetComponent<UsinaStorage>() != null)
            {
                UsinaStorage usinaStorage = placedMarker.GetComponent<UsinaStorage>();
                if (usinaStorage.gasDist != null)
                {
                    if (sessionManager.session.gasDistColocados.Contains(usinaStorage.gasDist))
                        sessionManager.session.gasDistColocados.Remove(usinaStorage.gasDist);
                }
                if (usinaStorage.gasDistPipe != null)
                {
                    GameObject temp = usinaStorage.gasDistPipe;
                    usinaStorage.gasDistPipe = null;
                    Destroy(temp);
                }
                if (usinaStorage.gasDistLine != null)
                {
                    OnlineMapsDrawingElementManager.RemoveItem(usinaStorage.gasDistLine);

                }
                if (usinaStorage.termRegasMarker != null)
                {
                    OnlineMapsMarker3D termRegasMarker = usinaStorage.termRegasMarker;
                    List<Frota> frotasToRemove = new List<Frota>();
                    List<OnlineMapsMarker3D> gnlMarkers = new List<OnlineMapsMarker3D>();
                    List<OnlineMapsMarker3D> frotaMarkersToRemove = new List<OnlineMapsMarker3D>();
                    List<OnlineMapsMarker3D> fsuMarkersToRemove = new List<OnlineMapsMarker3D>();
                    foreach (Frota f in sessionManager.session.frotasColocadas)
                    {
                        if (f.terminalRegasDestino == usinaStorage.termRegasMarker.instance.GetComponent<TerminalRegasStorage>().terminal.nome)
                        {
                            frotasToRemove.Add(f);
                        }
                    }
                    if (frotasToRemove.Count > 0)
                    {
                        foreach (OnlineMapsMarker3D m in markers)
                        {
                            if (m.instance.GetComponent<GNLStorage>() != null)
                            {
                                foreach (Frota f in frotasToRemove)
                                {
                                    if (f.terminalGnlOrigem == m.instance.GetComponent<GNLStorage>().gnl.nome)
                                    {
                                        gnlMarkers.Add(m);
                                    }
                                }
                            }
                        }
                        foreach (OnlineMapsMarker3D m in markers)
                        {
                            if (m.instance.GetComponent<FrotaStorage>() != null)
                            {
                                FrotaStorage frotaStorage = m.instance.GetComponent<FrotaStorage>();
                                for (int x = frotaStorage.frotas.Count - 1; x >= 0; x--)
                                {
                                    if (frotasToRemove.Contains(frotaStorage.frotas[x]))
                                    {
                                        OnlineMapsDrawingElementManager.RemoveItem(frotaStorage.frotaLines[frotaStorage.frotas[x].nome]);
                                        OnlineMapsDrawingElementManager.RemoveItem(frotaStorage.frotaLines2[frotaStorage.frotas[x].nome]);
                                        sessionManager.session.frotasColocadas.Remove(frotaStorage.frotas[x]);
                                        frotaStorage.frotas.RemoveAt(x);
                                    }
                                }
                            }
                        }

                    }
                    if (termRegasMarker.instance.GetComponent<TerminalRegasStorage>() != null)
                    {
                        sessionManager.session.terminaisRegasColocados.Remove(termRegasMarker.instance.GetComponent<TerminalRegasStorage>().terminal);
                        allInstantiatedMarkers.Remove(termRegasMarker);
                        markers.Remove(termRegasMarker);
                        markerManager.Remove(termRegasMarker);
                    }
                }
                sessionManager.session.usinasColocadas.Remove(usinaStorage.usina);
                panelElements.usinaStorageToSet = null;
            }
            else if (placedMarker.GetComponent<FSRUStorage>() != null)
            {
                FSRUStorage fsruStorage = placedMarker.GetComponent<FSRUStorage>();
                if (fsruStorage.gasInt != null)
                    if (sessionManager.session.gasIntColocados.Contains(fsruStorage.gasInt))
                        sessionManager.session.gasIntColocados.Remove(fsruStorage.gasInt);

                if (fsruStorage.gasIntPipe != null)
                {
                    GameObject temp = fsruStorage.gasIntPipe;
                    fsruStorage.gasIntPipe = null;
                    Destroy(temp);
                }

                if (fsruStorage.gasIntLine != null)
                {
                    OnlineMapsDrawingElementManager.RemoveItem(fsruStorage.gasIntLine);
                }

                OnlineMapsMarker3D terminalGnlToRemove = null;
                foreach (OnlineMapsMarker3D m in markers)
                {
                    if (m.instance.GetComponent<GNLStorage>() != null)
                    {
                        GNLStorage gnlStorage = m.instance.GetComponent<GNLStorage>();
                        if (gnlStorage.gnl.nome == fsruStorage.fsru.terminalGnl)
                        {
                            sessionManager.session.terminaisGNLColocados.Remove(gnlStorage.gnl);
                            terminalGnlToRemove = m;
                        }
                    }
                }
                if (terminalGnlToRemove != null)
                {
                    markers.Remove(terminalGnlToRemove);
                    allInstantiatedMarkers.Remove(terminalGnlToRemove);
                    markerManager.Remove(terminalGnlToRemove);
                }

                sessionManager.session.fsruColocados.Remove(fsruStorage.fsru);

                panelElements.fsruStorageToSet = null;
            }
            else if (placedMarker.GetComponent<FSUStorage>() != null)
            {
                FSUStorage fsuStorage = placedMarker.GetComponent<FSUStorage>();

                if (fsuStorage.gasCrio != null)
                {

                    if (sessionManager.session.gasCrioColocados.Contains(fsuStorage.gasCrio))
                    {
                        sessionManager.session.gasCrioColocados.Remove(fsuStorage.gasCrio);
                    }
                    fsuStorage.gasCrio = null;
                }
                if (fsuStorage.gasCrioPipe != null)
                {
                    GameObject temp = fsuStorage.gasCrioPipe;
                    fsuStorage.gasCrioPipe = null;
                    Destroy(temp);
                }



                GameObject terminalToRemove = null;
                GameObject gnlObj = new GameObject();
                foreach (OnlineMapsMarker3D m in allInstantiatedMarkers)
                {
                    if (m.instance.GetComponent<GNLStorage>() != null)
                    {
                        if (m.instance.GetComponent<GNLStorage>().fsuMarker == placedMarker.GetComponent<OnlineMapsMarker3DInstance>().marker as OnlineMapsMarker3D)
                        {
                            gnlObj = m.instance;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(placedMarker.GetComponent<FSUStorage>().fsu.terminalCargaDestino))
                {
                    terminalToRemove = gnlObj.GetComponent<GNLStorage>().frotaMarker.instance;
                }
                if (!string.IsNullOrEmpty(placedMarker.GetComponent<FSUStorage>().fsu.terminalRegasDestino))
                {
                    terminalToRemove = gnlObj.GetComponent<GNLStorage>().termRegasMarker.instance;
                }


                if (terminalToRemove != null)
                {
                    if (terminalToRemove.GetComponent<FrotaStorage>() != null)
                    {
                        FrotaStorage frotaStorage = terminalToRemove.GetComponent<FrotaStorage>();
                        foreach (Frota f in frotaStorage.frotas)
                        {
                            sessionManager.session.frotasColocadas.Remove(f);
                            if (frotaStorage.frotaLines.ContainsKey(f.nome))
                            {
                                OnlineMapsDrawingElementManager.RemoveItem(frotaStorage.frotaLines[f.nome]);
                            }
                            if (frotaStorage.frotaLines2.ContainsKey(f.nome))
                            {
                                OnlineMapsDrawingElementManager.RemoveItem(frotaStorage.frotaLines2[f.nome]);
                            }
                            foreach (OnlineMapsMarker3D m in markers)
                            {
                                if (m.instance.GetComponent<UsinaStorage>() != null)
                                {
                                    UsinaStorage usinaStorage = m.instance.GetComponent<UsinaStorage>();
                                    if (usinaStorage.termRegasMarker.instance.GetComponent<TerminalRegasStorage>().terminal.nome == f.terminalRegasDestino)
                                    {
                                        usinaStorage.usina.fonteDoGas = "gasoduto";
                                        usinaStorage.SetState();
                                    }
                                }
                            }
                        }
                    }
                    else if (terminalToRemove.GetComponent<TerminalRegasStorage>() != null)
                    {
                        TerminalRegasStorage termRegasStorage = terminalToRemove.GetComponent<TerminalRegasStorage>();
                        sessionManager.session.terminaisRegasColocados.Remove(termRegasStorage.terminal);
                        if (termRegasStorage.gasInt != null)
                        {
                            sessionManager.session.gasIntColocados.Remove(termRegasStorage.gasInt);
                        }
                        if (termRegasStorage.gasIntPipe != null)
                        {
                            GameObject temp = termRegasStorage.gasIntPipe;
                            termRegasStorage.gasInt = null;
                            Destroy(temp);
                        }
                        if (termRegasStorage.gasIntLine != null)
                        {
                            OnlineMapsDrawingElementManager.RemoveItem(termRegasStorage.gasIntLine);
                        }
                    }
                    allInstantiatedMarkers.Remove(terminalToRemove.GetComponent<OnlineMapsMarker3DInstance>().marker as OnlineMapsMarker3D);
                    markers.Remove(terminalToRemove.GetComponent<OnlineMapsMarker3DInstance>().marker as OnlineMapsMarker3D);
                    markerManager.Remove(terminalToRemove.GetComponent<OnlineMapsMarker3DInstance>().marker as OnlineMapsMarker3D);
                    panelElements.fsuStorageToSet = null;
                }

                OnlineMapsMarker3D terminalGnlToRemove = null;
                foreach (OnlineMapsMarker3D m in markers)
                {
                    if (m.instance.GetComponent<GNLStorage>() != null)
                    {
                        if (m.instance.GetComponent<GNLStorage>().gnl.nome == placedMarker.GetComponent<FSUStorage>().fsu.terminalGnl)
                        {
                            sessionManager.session.terminaisGNLColocados.Remove(m.instance.GetComponent<GNLStorage>().gnl);
                            terminalGnlToRemove = m;
                        }
                    }
                }
                if (terminalGnlToRemove != null)
                {
                    markers.Remove(terminalGnlToRemove);
                    allInstantiatedMarkers.Remove(terminalGnlToRemove);
                    markerManager.Remove(terminalGnlToRemove);
                }

                sessionManager.session.fsuColocados.Remove(placedMarker.GetComponent<FSUStorage>().fsu);

            }
            else if (placedMarker.GetComponent<PEStorage>() != null)
            {
                sessionManager.session.pontosDeEntrega.Remove(placedMarker.GetComponent<PEStorage>().pe);
            }
            else
            {
                hasComponent = false;
            }
            if (hasComponent)
            {
                allInstantiatedMarkers.Remove(placedMarker.GetComponent<OnlineMapsMarker3DInstance>().marker as OnlineMapsMarker3D);
                markers.Remove(placedMarker.GetComponent<OnlineMapsMarker3DInstance>().marker as OnlineMapsMarker3D);
                markerManager.Remove(placedMarker.GetComponent<OnlineMapsMarker3DInstance>().marker as OnlineMapsMarker3D);
                panelElements.HideSelf();
                peTableManager.HideSelf();
                sessionManager.UpdateSession();
            }
        }
    }

    public OnlineMapsMarker3D IsFSNearToGNLTerminal(OnlineMapsMarker3D fsUnit)
    {
        List<TerminalGNL> terminaisGNL = sessionManager.terminaisGNLFixos;
        List<TerminalGNL> terminaisProximos = new List<TerminalGNL>();


        foreach (TerminalGNL t in sessionManager.session.terminaisGNLColocados)
        {
            terminaisGNL.Add(t);
        }

        foreach (TerminalGNL t in terminaisGNL)
        {
            Vector2 tCoord = new Vector2((float)t.coordenadas.lon, (float)t.coordenadas.lat);
            if (OnlineMapsUtils.DistanceBetweenPointsD(tCoord, fsUnit.position) <= 4)
            {
                terminaisProximos.Add(t);
            }
        }

        if (terminaisProximos.Count > 0)
        {
            OnlineMapsMarker3D chosenMarker = new OnlineMapsMarker3D();
            for (int x = 0; x < terminaisProximos.Count; x++)
            {
                Vector2 t1Coord = new Vector2((float)terminaisProximos[x].coordenadas.lon, (float)terminaisProximos[x].coordenadas.lat);
                for (int y = terminaisProximos.Count - 1; y >= x; y--)
                {
                    Vector2 t2Coord = new Vector2((float)terminaisProximos[y].coordenadas.lon, (float)terminaisProximos[y].coordenadas.lat);
                    if (OnlineMapsUtils.DistanceBetweenPointsD(fsUnit.position, t1Coord) > OnlineMapsUtils.DistanceBetweenPointsD(fsUnit.position, t2Coord))
                    {
                        TerminalGNL temp = terminaisProximos[x];
                        terminaisProximos[x] = terminaisProximos[y];
                        terminaisProximos[y] = temp;
                    }
                }
            }

            foreach (OnlineMapsMarker3D m in allInstantiatedMarkers)
            {
                if (m.transform.GetComponent<GNLStorage>() != null)
                {
                    if (m.transform.GetComponent<GNLStorage>().gnl == terminaisProximos[0])
                    {
                        chosenMarker = m;
                        break;
                    }
                }
            }

            return chosenMarker;
        }
        else
        {
            return null;
        }
    }

    IEnumerator MoveMarker(OnlineMapsMarker3D marker, OVRGrabber grabber)
    {
        bool placed = false;
        bool isHolding;
        if (grabber == grabbers[0])
        {
            isHolding = OVRInput.Get(OVRInput.Button.PrimaryHandTrigger);
        }
        else
        {
            isHolding = OVRInput.Get(OVRInput.Button.SecondaryHandTrigger);
        }
        while (isHolding && !placed)
        {
            if (grabber == grabbers[0])
            {
                isHolding = OVRInput.Get(OVRInput.Button.PrimaryHandTrigger);
            }
            else
            {
                isHolding = OVRInput.Get(OVRInput.Button.SecondaryHandTrigger);
            }
            HandPointer hand = grabber.GetComponentInChildren<HandPointer>();
            RaycastHit mapHit = hand.GetHitFromMap();
            if (mapHit.collider)
            {
                if (marker.instance.GetComponent<UsinaStorage>() != null)
                {
                    Vector2 mapCoord = tileset.GetCoordsByWorldPosition(mapHit.point);
                    marker.SetPosition(mapCoord.x, mapCoord.y);
                    UsinaStorage storage = marker.instance.GetComponent<UsinaStorage>();
                    storage.usina.SetCoordenadas(marker.position.x, marker.position.y);
                    storage.gasDistLine.visible = false;
                    if (storage.termRegasMarker != null)
                    {
                        if (storage.termRegasMarker.instance != null)
                        {
                            if (storage.termRegasMarker.instance.GetComponent<TerminalRegasStorage>() != null)
                            {
                                Vector2 termCoord = tileset.GetCoordsByWorldPosition(storage.termRegasPoint.position);

                                storage.termRegasMarker.SetPosition(termCoord.x, termCoord.y);

                                storage.termRegasMarker.instance.GetComponent<TerminalRegasStorage>().terminal.SetCoordenadas(mapCoord.x, mapCoord.y);
                                storage.termRegasMarker.instance.GetComponent<TerminalRegasStorage>().terminal.SetMarkerCoords(termCoord.x, termCoord.y);
                            }
                        }
                    }
                }
                else if (marker.instance.GetComponent<FSUStorage>() != null)
                {
                    Vector2 mapCoord = tileset.GetCoordsByWorldPosition(mapHit.point);
                    marker.SetPosition(mapCoord.x, mapCoord.y);
                    FSUStorage storage = marker.instance.GetComponent<FSUStorage>();
                    storage.fsu.SetCoordenadas(mapCoord.x, mapCoord.y);

                }
                else if (marker.instance.GetComponent<FSRUStorage>() != null)
                {

                    Vector2 mapCoord = tileset.GetCoordsByWorldPosition(mapHit.point);
                    marker.SetPosition(mapCoord.x, mapCoord.y);
                    FSRUStorage storage = marker.instance.GetComponent<FSRUStorage>();
                    storage.fsru.SetCoordenadas(mapCoord.x, mapCoord.y);
                    if (storage.gasIntLine != null)
                        storage.gasIntLine.visible = false;
                }
                else if (marker.instance.GetComponent<FrotaStorage>() != null)
                {
                    Vector2 mapCoord = tileset.GetCoordsByWorldPosition(mapHit.point);
                    marker.SetPosition(mapCoord.x, mapCoord.y);
                    FrotaStorage storage = marker.instance.GetComponent<FrotaStorage>();
                    foreach (Frota f in storage.frotas)
                    {
                        f.coordenadasMarker = new Coordenadas()
                        {
                            lat = mapCoord.x,
                            lon = mapCoord.y
                        };
                    }
                }
            }
            if (grabber == grabbers[0])
            {
                if (OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger))
                {
                    placed = true;
                    yield return null;
                }
            }
            else
            {
                if (OVRInput.GetUp(OVRInput.Button.SecondaryHandTrigger))
                {
                    placed = true;
                    yield return null;
                }
            }

            yield return null;
        }
        RecalculateMarkerInfos(marker);
        sessionManager.UpdateSession();
    }

    void RecalculateMarkerInfos(OnlineMapsMarker3D marker)
    {

        if (marker.instance.GetComponent<UsinaStorage>() != null)
        {
            UsinaStorage storage = marker.instance.GetComponent<UsinaStorage>();

            OnlineMapsMarker3D peMarker = null;
            foreach (OnlineMapsMarker3D m in allInstantiatedMarkers)
            {
                if (m.instance.GetComponent<PEStorage>() != null)
                {
                    if (m.instance.GetComponent<PEStorage>().pe.nome == storage.gasDist.peOrigem)
                    {
                        peMarker = m;
                    }
                }
            }
            if (peMarker != null)
            {
                List<Vector2> points = new List<Vector2>
                {
                    peMarker.position,
                    marker.position
                };

                storage.gasDistLine.points = points;
                //storage.gasDistLine.visible = true;
            }

            if (storage.termRegasMarker != null)
            {
                foreach (OnlineMapsMarker3D m in markers)
                {
                    if (m.instance.GetComponent<FrotaStorage>() != null)
                    {
                        foreach (Frota f in m.instance.GetComponent<FrotaStorage>().frotas)
                        {
                            if (f.terminalRegasDestino == storage.termRegasMarker.instance.GetComponent<TerminalRegasStorage>().terminal.nome)
                            {
                                List<Vector2> points = new List<Vector2>
                                {
                                    m.position,
                                    storage.termRegasMarker.position
                                };

                                if (m.instance.GetComponent<FrotaStorage>().frotaLines.ContainsKey(f.nome))
                                {
                                    m.instance.GetComponent<FrotaStorage>().frotaLines[f.nome].points = points;
                                }

                                if (m.instance.GetComponent<FrotaStorage>().frotaLines2.ContainsKey(f.nome))
                                {
                                    m.instance.GetComponent<FrotaStorage>().frotaLines2[f.nome].points = points;
                                }

                                f.distancia = OnlineMapsUtils.DistanceBetweenPointsD(m.position, storage.gameObject.GetComponent<OnlineMapsMarker3DInstance>().marker.position);

                                f.Recalculate(sessionManager.session.premissas, storage.usina);
                            }
                        }
                    }
                }
            }

            storage.SetState();

        }
        else if (marker.instance.GetComponent<FSUStorage>() != null)
        {

            FSUStorage storage = marker.instance.GetComponent<FSUStorage>();

            double dist = storage.gasCrio.comprimento;

            foreach (OnlineMapsMarker3D m in allInstantiatedMarkers)
            {
                if (m.instance.GetComponent<GNLStorage>() != null)
                {
                    if (m.instance.GetComponent<GNLStorage>().fsuMarker == marker)
                    {
                        dist = OnlineMapsUtils.DistanceBetweenPointsD(marker.position, m.position);
                    }
                }
            }

            if (storage.gasCrio != null)
            {
                storage.gasCrio.comprimento = dist;
                storage.gasCrio.Recalculate(sessionManager.session.premissas);
            }
        }
        else if (marker.instance.GetComponent<FSRUStorage>() != null)
        {

            FSRUStorage storage = marker.instance.GetComponent<FSRUStorage>();

            if (storage.gasInt != null)
            {
                double dist = storage.gasInt.comprimento;
                foreach (OnlineMapsMarker3D m in allInstantiatedMarkers)
                {
                    if (m.instance.GetComponent<PEStorage>() != null)
                    {
                        if (m.instance.GetComponent<PEStorage>().pe.nome == storage.gasInt.peDestino)
                        {
                            dist = OnlineMapsUtils.DistanceBetweenPointsD(marker.position, m.position);
                            storage.gasInt.comprimento = dist;
                            storage.gasInt.Recalculate(sessionManager.session.premissas);

                            List<Vector2> points = new List<Vector2>
                            {
                                marker.position,
                                m.position
                            };

                            storage.gasIntLine.points = points;
                            storage.gasIntLine.visible = true;
                        }
                    }
                }
            }
        }
        else if (marker.instance.GetComponent<FrotaStorage>() != null)
        {
            FrotaStorage storage = marker.instance.GetComponent<FrotaStorage>();
            foreach (KeyValuePair<string, OnlineMapsDrawingLine> line in storage.frotaLines)
            {
                var _points = (List<Vector2>)line.Value.points;
                Vector2 destination = _points[1];
                List<Vector2> points = new List<Vector2>
                {
                    marker.position,
                    destination
                };
                line.Value.points = points;
            }
            foreach (KeyValuePair<string, OnlineMapsDrawingLine> line in storage.frotaLines2)
            {
                var _points = (List<Vector2>)line.Value.points;
                Vector2 destination = _points[1];
                List<Vector2> points = new List<Vector2>
                {
                    marker.position,
                    destination
                };
                line.Value.points = points;
            }
            foreach (Frota f in storage.frotas)
            {
                foreach (OnlineMapsMarker3D m in markers)
                {
                    if (m.instance.GetComponent<FSUStorage>() != null)
                    {
                        FSUStorage fsuStorage = m.instance.GetComponent<FSUStorage>();
                        if (fsuStorage.fsu.terminalGnl == f.terminalGnlOrigem)
                        {
                            fsuStorage.gasCrioPipe.GetComponent<PipeScaleController>().originObj = m.instance;
                            fsuStorage.gasCrioPipe.GetComponent<PipeScaleController>().destinationObj = marker.instance;
                        }
                    }
                }
            }
        }
    }

    void SendMessageTo(GameObject target, string message)
    {
        if (target)
            target.SendMessage(message, gameObject,
                    SendMessageOptions.DontRequireReceiver);
    }

}
