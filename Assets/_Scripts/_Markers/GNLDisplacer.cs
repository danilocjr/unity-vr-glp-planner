using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GNLDisplacer : MonoBehaviour
{
    public OnlineMaps maps;
    public SessionManager sessionManager;
    public MarkerDisplacer markerDisplacer;

    GasClasses gasClasses;
    ConnectionCreator connCreator;
    FSUDestinationBehaviour fsuManager;
    OnlineMapsTileSetControl tileSet;
    OnlineMapsMarker3DManager markerManager;

    OnlineMapsMarker3D fsMarker;
    public GameObject gnlDisplacerPanel;
    private void Start()
    {
        gasClasses = sessionManager.GetComponent<GasClasses>();
        connCreator = sessionManager.GetComponent<ConnectionCreator>();
        tileSet = maps.GetComponent<OnlineMapsTileSetControl>();
        markerManager = maps.GetComponent<OnlineMapsMarker3DManager>();
        fsuManager = markerDisplacer.FSUManager;
        gnlDisplacerPanel.SetActive(false);
    }

    public void ShowGNLDisplacerPanel(OnlineMapsMarker3D _fsMarker)
    {
        fsMarker = _fsMarker;
        gnlDisplacerPanel.SetActive(true);
    }

    public void CancelGNLDisplacement()
    {
        markerDisplacer.markers.Remove(fsMarker);
        markerDisplacer.allInstantiatedMarkers.Remove(fsMarker);

        if (fsMarker.instance.GetComponent<FSUStorage>() != null)
        {
            sessionManager.session.fsuColocados.Remove(fsMarker.instance.GetComponent<FSUStorage>().fsu);

            //if (sessionManager.fsuUtilizados.Contains(fsMarker.instance.GetComponent<FSUStorage>().fsu))
            //    sessionManager.fsuUtilizados.Remove(fsMarker.instance.GetComponent<FSUStorage>().fsu);
        }
        else if (fsMarker.instance.GetComponent<FSRUStorage>() != null)
            sessionManager.session.fsruColocados.Remove(fsMarker.instance.GetComponent<FSRUStorage>().fsru);

        markerManager.Remove(fsMarker);

        gnlDisplacerPanel.SetActive(false);
        fsMarker = null;
    }

    public void PlaceGNL()
    {

        //maps.SetPosition(fsMarker.position.x, fsMarker.position.y);
        Vector2 gnlMarkerCoord = tileSet.GetCoordsByWorldPosition(fsMarker.instance.GetComponent<GNLPlacement>().gnlPoint.transform.position);

        OnlineMapsMarker3D m = markerManager.Create(gnlMarkerCoord.x, gnlMarkerCoord.y, gasClasses.gnl3dPrefab);

        m.transform.rotation = fsMarker.instance.GetComponent<GNLPlacement>().gnlPoint.transform.rotation;
        m.transform.position = fsMarker.instance.GetComponent<GNLPlacement>().gnlPoint.transform.position;


        m.scale = markerDisplacer.markerScale;

        TerminalGNL gnl = new TerminalGNL("Terminal GNL " + markerDisplacer.gnlIndex, "Simulado", m.position.x, m.position.y);
        m.instance.GetComponent<GNLStorage>().gnl = gnl;
        sessionManager.session.terminaisGNLColocados.Add(gnl);
        if (fsMarker.instance.GetComponent<FSUStorage>() != null)
        {
            m.instance.GetComponent<GNLStorage>().fsuMarker = fsMarker;
            fsMarker.instance.GetComponent<FSUStorage>().fsu.terminalGnl = gnl.nome;
            fsuManager.selectedTerminalGNLMarker = m;
            fsuManager.fsuMarker = fsMarker;
            gnlDisplacerPanel.SetActive(false);
            fsuManager.ShowFSUDestinationPanel();
        }
        else if (fsMarker.instance.GetComponent<FSRUStorage>() != null)
        {
            m.instance.GetComponent<GNLStorage>().fsruMarker = fsMarker;
            fsMarker.instance.GetComponent<FSRUStorage>().fsru.terminalGnl = gnl.nome;
            connCreator.ConectarFSRUComPEProximo(fsMarker.instance, true);
        }

        markerDisplacer.allInstantiatedMarkers.Add(m);
        markerDisplacer.markers.Add(m);
        markerDisplacer.gnlIndex++;

        gnlDisplacerPanel.SetActive(false);
    }
}
