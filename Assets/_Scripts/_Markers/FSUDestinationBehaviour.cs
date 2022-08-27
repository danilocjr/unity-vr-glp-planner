using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class FSUDestinationBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    public SessionManager sessionManager;
    public MarkerDisplacer markerDisplacer;
    ConnectionCreator connCreator;
    OnlineMapsMarker3DManager markerManager;
    OnlineMapsTileSetControl tileset;
    GasClasses gasClasses;
    public OnlineMaps maps;
    public GameObject usinasPanel;

    public GameObject fsuDestinationPanel;
    public GameObject buildInUsinaPanel;
    public GameObject usinaToggle;
    public List<Toggle> usinaToggles;
    public ToggleGroup usinaToggleGroup;
    public Button confirmUsinaButton;
    public Transform usinaToggleParent;
    public Button confirmUsinaBtn;
    string selectedUsinaNome;

    public GameObject noUsinaText;

    List<string> selectedUsinasNomes;
    public List<OnlineMapsMarker3D> selectedUsinaMarkers = new List<OnlineMapsMarker3D>();
    List<OnlineMapsMarker3D> usinasComTerminal = new List<OnlineMapsMarker3D>();

    public OnlineMapsMarker3D fsuMarker;
    public GameObject buildInTerminalGNLPanel;
    public OnlineMapsMarker3D selectedTerminalGNLMarker;
    public OnlineMapsMarker3D selectedUsinaMarker;
    OnlineMapsMarker3D frotaMarker;
    public GameObject frotaObj;
    public GameObject termRegasObj;



    public GameObject frotaPositioningPanel;
    public GameObject terminalPositioningPanel;
    

    SmoothMoveMap smoothMove;

    public GameObject termRegasPrefab;
    public GameObject frotaPrefab;

    public bool canPlaceFSU = false;

    bool builtInUsina;
    bool isPositioningTermGas;
    bool isPositioningFrota;

    void Start()
    {
        selectedUsinasNomes = new List<string>();
        selectedUsinaMarkers = new List<OnlineMapsMarker3D>();
        markerManager = maps.gameObject.GetComponent<OnlineMapsMarker3DManager>();
        tileset = maps.gameObject.GetComponent<OnlineMapsTileSetControl>();
        smoothMove = markerDisplacer.GetComponent<SmoothMoveMap>();
        gasClasses = markerDisplacer.gasClasses;
        connCreator = gasClasses.connCreator;
        fsuDestinationPanel.SetActive(false);
        usinasPanel.SetActive(false);
        buildInTerminalGNLPanel.SetActive(false);
        buildInUsinaPanel.SetActive(false);
        terminalPositioningPanel.SetActive(false);
    }

    public void ShowFSUDestinationPanel()
    {
        fsuDestinationPanel.SetActive(true);
        markerDisplacer.canGrab = false;
    }

    //TODO: CRIAR MÉTODO PARA CANCELAR O PROCESSO
    public void Cancel()
    {
        markerDisplacer.markers.Remove(fsuMarker);
        markerDisplacer.allInstantiatedMarkers.Remove(fsuMarker);
        sessionManager.session.fsuColocados.Remove(fsuMarker.instance.GetComponent<FSUStorage>().fsu);

        //if (sessionManager.fsuUtilizados.Contains(fsuMarker.instance.GetComponent<FSUStorage>().fsu))
        //    sessionManager.fsuUtilizados.Remove(fsuMarker.instance.GetComponent<FSUStorage>().fsu);

        markerManager.Remove(fsuMarker);
        fsuDestinationPanel.SetActive(false);
        usinasPanel.SetActive(false);
        buildInTerminalGNLPanel.SetActive(false);
        buildInUsinaPanel.SetActive(false);
        terminalPositioningPanel.SetActive(false);

        selectedTerminalGNLMarker = null;
        selectedUsinaMarker = null;
        builtInUsina = false;
        canPlaceFSU = false;

        isPositioningFrota = false;
        isPositioningTermGas = false;

        frotaObj = null;
        termRegasObj = null;

        smoothMove.MoveMapToPoint(fsuMarker.position);
    }

    #region RODOVIARIO
    public void OnClickRodoviario()
    {        
        fsuDestinationPanel.SetActive(false);
        frotaPositioningPanel.SetActive(true);
        isPositioningFrota = true;
        Vector2 terminalGnlCoords = selectedTerminalGNLMarker.position;
        //StartCoroutine(FrotaPositioning(terminalGnlCoords));
        Vector2 frotaCoords = tileset.GetCoordsByWorldPosition(selectedTerminalGNLMarker.transform.GetComponent<GNLStorage>().frotaPoint.transform.position);

        frotaMarker = markerManager.Create(frotaCoords.x, frotaCoords.y, frotaPrefab);

        frotaMarker.transform.localRotation = Quaternion.Euler(270, 0, 180);
        frotaMarker.scale = markerDisplacer.markerScale;

        
        foreach (Transform t in frotaMarker.transform.gameObject.GetComponentsInChildren<Transform>())
            t.gameObject.layer = 14;

        frotaObj = frotaMarker.instance;
        

        fsuMarker.instance.GetComponent<FSUStorage>().fsu.terminalCargaDestino = "Terminal de carga do "+ selectedTerminalGNLMarker.instance.GetComponent<GNLStorage>().gnl.nome;
        selectedTerminalGNLMarker.instance.GetComponent<GNLStorage>().frotaMarker = frotaMarker;

        //marker.scale = markerScale;
        
        markerDisplacer.markers.Add(frotaMarker);
        markerDisplacer.allInstantiatedMarkers.Add(frotaMarker);
        ShowUsinasPanel();
    }


    public void CancelRodoviario()
    {
        frotaPositioningPanel.SetActive(false);
        usinasPanel.SetActive(false);
        frotaObj = null;
        markerDisplacer.markers.Remove(frotaMarker);
        markerDisplacer.allInstantiatedMarkers.Remove(frotaMarker);
        markerManager.Remove(frotaMarker);
        fsuDestinationPanel.SetActive(true);
    }   

    public void ShowUsinasPanel()
    {        
        frotaPositioningPanel.SetActive(false);        
        foreach(Transform t in usinaToggleParent.GetComponentsInChildren<Transform>())
        {
            if (t != usinaToggleParent)
                Destroy(t.gameObject);
        }
        usinaToggles.Clear();
        usinasPanel.SetActive(true);
        if (sessionManager.session.usinasColocadas.Count > 0)
        {
            foreach (Usina u in sessionManager.session.usinasColocadas)
            {
                GameObject x = Instantiate(usinaToggle, usinaToggleParent);
                x.GetComponentInChildren<Text>().text = u.nome;
                usinaToggles.Add(x.GetComponent<Toggle>());
            }
            confirmUsinaBtn.interactable = true;
        }
        else
        {
            GameObject x = Instantiate(noUsinaText, usinaToggleParent);
            confirmUsinaBtn.interactable = false;
        }
        
    }

    public void GetSelectedUsina()
    {
        //selectedUsinaMarker = null;
        selectedUsinasNomes.Clear();
        selectedUsinaMarkers.Clear();
        foreach (Toggle t in usinaToggles)
        {            
            if (t.isOn)
            {
                selectedUsinasNomes.Add(t.GetComponentInChildren<Text>().text);
                //selectedUsinaNome = t.GetComponentInChildren<Text>().text;
                //break;
            }
        }

        if (selectedUsinasNomes.Count>0)
        {
            usinasPanel.SetActive(false);
            List<Usina> temp = sessionManager.session.usinasColocadas;
            
            foreach (Usina usina in temp)
            {
                if (selectedUsinasNomes.Contains(usina.nome))
                {
                    Vector2 usinaCoords = new Vector2((float)usina.coordenadas.lon, (float)usina.coordenadas.lat);
                    foreach (OnlineMapsMarker3D m in markerDisplacer.markers)
                    {
                        if (m.transform.GetComponent<UsinaStorage>() != null)
                        {
                            if (m.transform.GetComponent<UsinaStorage>().usina == usina)
                            {
                                selectedUsinaMarkers.Add(m);
                            }
                        }
                    }
                }
               
            }
            if (selectedUsinaMarkers.Count>0)
            {               
                List<string> terminalNames = new List<string>();
                usinasComTerminal = new List<OnlineMapsMarker3D>();
                foreach (TerminalRegas t in sessionManager.session.terminaisRegasColocados)
                {

                    foreach(OnlineMapsMarker3D u in selectedUsinaMarkers)
                    {
                        if (u.instance.GetComponent<UsinaStorage>().usina.nome == t.usina)
                        {
                            usinasComTerminal.Add(u);
                            terminalNames.Add(t.nome);
                        }
                    }                    
                }
                if (usinasComTerminal.Count==0)
                {
                    buildInUsinaPanel.SetActive(true);
                }
                else 
                {
                    foreach (OnlineMapsMarker3D m in markerDisplacer.markers)
                    {
                        if (m.instance.GetComponent<TerminalRegasStorage>() != null)
                        {
                            if (terminalNames.Contains(m.instance.GetComponent<TerminalRegasStorage>().terminal.nome))
                            {                             

                                OnlineMapsMarker3D termMarker = m;

                                connCreator.ConectarFrotaComTermRegas(frotaObj, m.instance);

                                connCreator.ConectarFSUComTerminais(fsuMarker.instance, frotaObj);
                                buildInUsinaPanel.SetActive(false);
                            }
                        }
                    }
                    if (usinasComTerminal.Count < selectedUsinasNomes.Count)
                        buildInUsinaPanel.SetActive(true);                    
                }
                
            }            
        }
       
    }

    public void BuildInUsina()
    {
        buildInUsinaPanel.SetActive(false);
        StartCoroutine(WaitToComeBackToFSU());
    }

    public void BuildInUsina(OnlineMapsMarker3D _usinaMarker = null)
    {
        markerDisplacer.msg.text = "Usina criada";
        if(_usinaMarker!=null)
        {
            markerDisplacer.msg.text += "\nUsina marker not null";
            selectedUsinaMarkers = new List<OnlineMapsMarker3D>();
            selectedUsinaMarkers.Add(_usinaMarker);
            markerDisplacer.msg.text += "\nUsina marker added to list";
        }
        StartCoroutine(WaitToComeBackToFSU(true));
       
    }

    IEnumerator WaitToComeBackToFSU(bool fsuCreated = false)
    {
        TextMesh msg = markerDisplacer.msg;

        foreach (OnlineMapsMarker3D m in selectedUsinaMarkers)
        {
            if(!fsuCreated)
            { 
                if (!usinasComTerminal.Contains(m))
                {
                    CreateTermRegas(m);
                }
            }
            else
            {
                CreateTermRegas(m);
            }
        }
        if (!fsuCreated)
        {
            maps.SetPosition(fsuMarker.position.x, fsuMarker.position.y);
            yield return new WaitForSeconds(0.5f);
            connCreator.ConectarFSUComTerminais(fsuMarker.instance, frotaObj);
            yield return new WaitForSeconds(0.5f);
        }
            sessionManager.UpdateSession();
            fsuMarker.instance.GetComponent<FSUStorage>().SetState();
            yield return null;
    }

    private void CreateTermRegas(OnlineMapsMarker3D m)
    {
        TextMesh msg = markerDisplacer.msg;
        if (!m.instance.activeSelf)
        {
            maps.SetPosition(m.position.x, m.position.y);
        }
        Vector2 termRegasCoords = tileset.GetCoordsByWorldPosition(m.transform.GetComponent<UsinaStorage>().termRegasPoint.transform.position);
        OnlineMapsMarker3D termRegasMarker = markerManager.Create(termRegasCoords.x, termRegasCoords.y, termRegasPrefab);

        foreach (Transform t in termRegasMarker.transform.gameObject.GetComponentsInChildren<Transform>())
            t.gameObject.layer = 14;
        TerminalRegas terminal = new TerminalRegas("Terminal de Regaseificacao_" + markerDisplacer.terminalIndex);
        markerDisplacer.terminalIndex++;
        terminal.SetCusto("opex", 3000000, "dolar");
        terminal.SetCoordenadas(m.position.x, m.position.y);
        terminal.SetMarkerCoords(termRegasMarker.position.x, termRegasMarker.position.y);
        terminal.usina = m.instance.GetComponent<UsinaStorage>().usina.nome;
        terminal.categoria = "Simulado";
        terminal.custoUsado = "opex";
        sessionManager.session.terminaisRegasColocados.Add(terminal);

        termRegasMarker.transform.GetComponent<TerminalRegasStorage>().terminal = terminal;
        m.instance.GetComponent<UsinaStorage>().termRegasMarker = termRegasMarker;
        termRegasMarker.transform.localRotation = Quaternion.Euler(270, 0, 180);
        termRegasMarker.scale = markerDisplacer.markerScale;
        markerDisplacer.markers.Add(termRegasMarker);
        markerDisplacer.allInstantiatedMarkers.Add(termRegasMarker);
        termRegasObj = termRegasMarker.instance;
        connCreator.ConectarFrotaComTermRegas(frotaObj, termRegasObj);
        fsuMarker.instance.GetComponent<FSUStorage>().fsu.gnlFornecido += m.instance.GetComponent<UsinaStorage>().usina.demandaDiaM3;
        markerDisplacer.canGrab = true;
    }
    #endregion

    #region DUTOVIARIO
    public void OnClickDutoviario()
    {
        fsuDestinationPanel.SetActive(false);
        bool hasTerminal = false;
        foreach(TerminalRegas t in sessionManager.session.terminaisRegasColocados)
        {
            if (t.terminalGnl == selectedTerminalGNLMarker.instance.GetComponent<GNLStorage>().gnl.nome)
            {
                fsuMarker.instance.GetComponent<FSUStorage>().fsu.terminalRegasDestino = t.nome;
                hasTerminal = true;
            }
        }
        
        if (!hasTerminal)
        {
            buildInTerminalGNLPanel.SetActive(true);
        }
        else
        {
            canPlaceFSU = true;
        }
    }

    public void CancelDutoviario()
    {
        buildInTerminalGNLPanel.SetActive(false);       
        fsuDestinationPanel.SetActive(true);
    }
    public void BuildInTerminalGNL()
    {
        
        //builtInUsina = false;
        terminalPositioningPanel.SetActive(true);
        
        //isPositioningTermGas = true;
        //StartCoroutine(TermRegasPositioning(terminalGnlCoords));

        
        Vector2 termRegasCoords = tileset.GetCoordsByWorldPosition(selectedTerminalGNLMarker.transform.GetComponent<GNLStorage>().termRegasPoint.transform.position);
        OnlineMapsMarker3D termRegasMarker = markerManager.Create(termRegasCoords.x, termRegasCoords.y, termRegasPrefab);

        TerminalRegas terminal = new TerminalRegas("Terminal de Regaseificacao_" + markerDisplacer.terminalIndex);

        termRegasObj = termRegasMarker.instance;

        termRegasMarker.transform.gameObject.name = terminal.nome;

        foreach (Transform t in termRegasMarker.transform.gameObject.GetComponentsInChildren<Transform>())
            t.gameObject.layer = 14;

        terminal.SetCoordenadas(selectedTerminalGNLMarker.position.x, selectedTerminalGNLMarker.position.y);
        terminal.SetMarkerCoords(termRegasMarker.position.x, termRegasMarker.position.y);


        terminal.categoria = "Simulado";
        terminal.SetCusto("opex", 3000000, "dolar");
        terminal.custoUsado = "opex";
        sessionManager.session.terminaisRegasColocados.Add(terminal);
        termRegasMarker.transform.GetComponent<TerminalRegasStorage>().terminal = terminal;
        termRegasMarker.transform.GetComponent<TerminalRegasStorage>().terminal.terminalGnl = selectedTerminalGNLMarker.transform.GetComponent<GNLStorage>().gnl.nome;

        termRegasMarker.scale = markerDisplacer.markerScale;
        termRegasMarker.transform.localRotation = Quaternion.Euler(270, 0, 180);

        selectedTerminalGNLMarker.instance.GetComponent<GNLStorage>().termRegasMarker = termRegasMarker;

        markerDisplacer.terminalIndex++;        

        fsuMarker.instance.GetComponent<FSUStorage>().fsu.terminalRegasDestino = terminal.nome;

        connCreator.ConectarTermRegasComPEProximo(termRegasObj);

        markerDisplacer.markers.Add(termRegasMarker);
        markerDisplacer.allInstantiatedMarkers.Add(termRegasMarker);

        connCreator.ConectarFSUComTerminais(fsuMarker.instance, termRegasMarker.instance);

        buildInTerminalGNLPanel.SetActive(false);
        markerDisplacer.canGrab = true;
        terminalPositioningPanel.SetActive(false);
        frotaObj = null;
        termRegasObj = null;
        fsuMarker.instance.GetComponent<FSUStorage>().fsu.gnlFornecido = 315000;
        sessionManager.UpdateSession();
        fsuMarker.instance.GetComponent<FSUStorage>().SetState();
    }
    #endregion


    #region TERMGAS_POS_COMMENTED
    //IEnumerator TermRegasPositioning(Vector2 centerPos)
    //{
    //    while (isPositioningTermGas)
    //    {
    //        RaycastHit hit = markerDisplacer.grabbers[1].GetComponentInChildren<HandPointer>().GetMarkerDestination();
    //        RaycastHit hit2 = markerDisplacer.grabbers[0].GetComponentInChildren<HandPointer>().GetMarkerDestination();
    //        Vector2 coords = new Vector2();
    //        bool canPlaceTerminal = false;
    //        if (hit.collider && hit2.collider)
    //        {
    //            if (OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
    //            {
    //                coords = tileset.GetCoordsByWorldPosition(hit.point);
    //                canPlaceTerminal = true;

    //            }
    //            else if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger))
    //            {
    //                coords = tileset.GetCoordsByWorldPosition(hit2.point);
    //                canPlaceTerminal = true;
    //            }
    //        }
    //        else if (hit.collider && !hit2.collider)
    //        {
    //            if (OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
    //            {
    //                coords = tileset.GetCoordsByWorldPosition(hit.point);
    //                canPlaceTerminal = true;
    //            }
    //        }
    //        else if (!hit.collider && hit2.collider)
    //        {
    //            if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger))
    //            {
    //                coords = tileset.GetCoordsByWorldPosition(hit2.point);
    //                canPlaceTerminal = true;
    //            }
    //        }

    //        if (canPlaceTerminal)
    //        {
    //            PutTermGas(centerPos, coords);
    //            if (builtInUsina)
    //            {
    //                //gasClasses.ChangeAllTo2D();
    //                maps.SetPosition(fsuCoord.x,fsuCoord.y);
    //                //gasClasses.ChangeAllTo3D();
    //            }
    //        }
    //        yield return null;
    //    }
    //    yield return null;
    //}

    //void PutTermGas(Vector2 centerPos, Vector2 coords)
    //{        
    //    if (OnlineMapsUtils.DistanceBetweenPointsD(coords, centerPos) <= 4)
    //    {
    //        isPositioningTermGas = false;
    //        OnlineMapsMarker3D marker = markerManager.Create((double)coords.x, (double)coords.y, termRegasPrefab);
    //        terminalPositioningPanel.SetActive(false);
    //        TerminalRegas terminal = new TerminalRegas("Terminal_" + markerDisplacer.terminalIndex);

    //        marker.transform.gameObject.name = terminal.nome;

    //        terminal.SetCoordenadas(marker.position.x, marker.position.y);
    //        terminal.SetProdutividade("bcu", 37, "ano");
    //        terminal.categoria = "Simulado";
    //        sessionManager.session.terminaisRegasColocados.Add(terminal);
    //        marker.transform.GetComponent<TerminalStorage>().terminal = terminal;
    //        markerDisplacer.terminalIndex++;

    //        marker.transform.localRotation = Quaternion.Euler(270, 0, 180);
    //        //marker.scale = markerScale;

    //        markerDisplacer.markers.Add(marker);
    //        markerDisplacer.allInstantiatedMarkers.Add(marker);

    //        foreach (Toggle t in usinaToggles)
    //            Destroy(t.gameObject);

    //        markerDisplacer.grabbers[1].GetComponentInChildren<HandPointer>().markerSign.SetActive(false);     

    //    }
    //}

    #endregion
}
