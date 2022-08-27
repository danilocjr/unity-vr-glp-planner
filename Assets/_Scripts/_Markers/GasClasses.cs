using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.EventSystems;

//SCRIPT USADO PARA GERAR OS OBJETOS FIXOS NO MAPA
// E PARA TRANSFORMAR OS ÍCONES DE 2D PARA 3D E VICE-VERSA

//O NOME GASCLASSES FOI DADO PORQUE LÁ EMBAIXO FICAM AS CLASSES UTILIZADAS PARA
// DESERIALIZAR OS JSONS DADOS
public class GasClasses : MonoBehaviour
{
    // Start is called before the first frame update

    public ConnectionCreator connCreator;
    public SessionManager sessionManager;
    OnlineMapsTileSetControl tileset;

    public string cityGatesJSON;
    public string gnlsJSON;
    public string termosJSON;
    public TextAsset subestacoesFile;

    public PontosDeEntrega cgs;
    public TerminaisGNLJson gnls;
    public UsinasJSON termos;
    public Pipelines pipelines;
    public SubestacoesJSON subestacoes;

    //Fixed 3D Marker Icons
    //public GameObject fixedPePrefab;
    //public GameObject fixedGnlPrefab_existente;
    //public GameObject fixedGnlPrefab_previsto;
    //public GameObject fixedGnlPrefab_estudo;
    //public GameObject fixedUsinaPrefab;
    //public GameObject fixedSubEstPrefab;

    //Fixed 2D Marker Icons
    public GameObject peIconPrefab2DFixed;
    public GameObject gnlIconPrefab2D_existente_Fixed;
    public GameObject gnlIconPrefab2D_previsto_Fixed;
    public GameObject gnlIconPrefab2D_estudo_Fixed;
    public GameObject termoIconPrefab2DFixed;
    public GameObject subEstIconPrefab2DFixed;

    //User 2D Marker Icons
    public GameObject FSU2DPrefab;
    public GameObject usinaIconPrefab2DUser;
    public GameObject subEst2DPrefab;
    public GameObject pe2DPrefab;
    public GameObject terminalRegas2DPrefab;
    public GameObject FSRU2DPrefab;
    public GameObject frota2DPrefab;

    //User 3D Marker Icons
    public GameObject FSU3DPrefab;
    public GameObject usina3DPrefab;
    public GameObject subEst3DPrefab;
    public GameObject pe3DPrefab;
    public GameObject terminalRegas3DPrefab;
    public GameObject FSRU3DPrefab;
    public GameObject frota3DPrefab;
    public GameObject gnl3dPrefab;

    public List<OnlineMapsMarker3D> cg2DIcons;
    public List<OnlineMapsMarker3D> gnl2DIcons;
    public List<OnlineMapsMarker3D> usina2DIcons;
    public List<OnlineMapsMarker3D> subEst2DIcons;
    public float iconsScale = 0.01f;


    public OnlineMaps maps;
    public OnlineMapsMarker3DManager markerManager;
    public MarkerDisplacer markerDisplacer;
    public MapController mapController;


    //public List<OnlineMapsMarker3D> fixedCityGatesMarkers;
    //public List<OnlineMapsMarker3D> fixedGNLMarkers;
    //public List<OnlineMapsMarker3D> fixedUsinaMarkers;
    //public List<OnlineMapsMarker3D> fixedSubEstMarkers;

    //public List<OnlineMapsMarker3D> userMarkers2DIcons;

    //public List<FSRU> fsrusFixos = new List<FSRU>();
    //public List<PontoDeEntrega> pontosDeEntregaFixos = new List<PontoDeEntrega>();
    //public List<Usina> usinasFixas = new List<Usina>();

    public TextMesh msg;


    void Awake()
    {
        tileset = markerManager.GetComponent<OnlineMapsTileSetControl>();

        cgs = JsonUtility.FromJson<PontosDeEntrega>(cityGatesJSON);
        foreach (PontoDeEntrega pe in cgs.pontosDeEntrega)
        {
            //    PontoDeEntrega pe = new PontoDeEntrega();
            //    pe.nome = cg.attributes.Nome;
            //    pe.categoria = cg.attributes.Classifica;
            //    pe.coordenadas.lat = cg.geometry.y;
            //    pe.coordenadas.lon = cg.geometry.x;
            //    if (!sessionManager.peFixos.Contains(pe))
            sessionManager.peFixos.Add(pe);
        }
        PontosDeEntrega pes = cgs;
        //pes.pontosDeEntrega = new List<PontoDeEntrega>();
        //pes.pontosDeEntrega = sessionManager.peFixos;

        gnls = JsonUtility.FromJson<TerminaisGNLJson>(gnlsJSON);
        foreach (TerminalGNLJson g in gnls.terminais_gnl)
        {
            TerminalGNL gnl = new TerminalGNL(g.attributes.Nome, g.attributes.Classifica, g.geometry.x, g.geometry.y);
            if (!sessionManager.terminaisGNLFixos.Contains(gnl))
                sessionManager.terminaisGNLFixos.Add(gnl);

            if (gnl.nome.Contains("Suape"))
            {
                maps.SetPosition(gnl.coordenadas.lon, gnl.coordenadas.lat);
            }
        }

        termos = JsonUtility.FromJson<UsinasJSON>(termosJSON);
        foreach (UsinaJSON termo in termos.features)
        {
            Usina u = new Usina(termo.attributes.Nome)
            {
                potencia = Convert.ToSingle(termo.attributes.P_OUT_KW),
                tipoDeGas = termo.attributes.COMBUST
            };
            u.coordenadas.lat = termo.geometry.y;
            u.coordenadas.lon = termo.geometry.x;

            if (!sessionManager.usinasFixas.Contains(u))
                sessionManager.usinasFixas.Add(u);
        }

        subestacoes = JsonUtility.FromJson<SubestacoesJSON>(subestacoesFile.text);
        foreach (SubestacaoJSON s in subestacoes.subestacoes)
        {
            SubestacaoEnergia sub = new SubestacaoEnergia()
            {
                nome = s.attributes.Nome,
                tensao = s.attributes.Tensao,
                categoria = s.attributes.Situacao,
                coordenadas = new Coordenadas()
                {
                    lat = s.geometry.y,
                    lon = s.geometry.x
                }
            };
            if (!sessionManager.subestacoesFixas.Contains(sub))
            {
                sessionManager.subestacoesFixas.Add(sub);
            }
        }

        cg2DIcons = new List<OnlineMapsMarker3D>();
        gnl2DIcons = new List<OnlineMapsMarker3D>();
        //userMarkers2DIcons = new List<OnlineMapsMarker3D>();
        usina2DIcons = new List<OnlineMapsMarker3D>();
        subEst2DIcons = new List<OnlineMapsMarker3D>();

        Generate2DIcons();

       
        
    }

   

    //public void ChangeAllTo2D()
    //{
    //    Generate2DIcons();

    //    EraseCityGates();
    //    EraseGNL();
    //    EraseTermo();
    //    EraseSubEst();

    //    ChangeAllUserMarkersTo2D();
    //}

    //public void ChangeAllTo3D()
    //{ 
    //    Remove2DIcons();

    //    GenerateCityGates();
    //    GenerateGNL();
    //    GenerateTermo();
    //    GenerateSubEst();

    //    ChangeAllUserMarkersTo3D();
    //}

    public void Generate2DIcons()
    {

        foreach (Usina u in sessionManager.usinasFixas)
        {
            OnlineMapsMarker3D marker = markerManager.Create(u.coordenadas.lon, u.coordenadas.lat, termoIconPrefab2DFixed);
            marker.transform.GetComponent<UsinaStorage>().usina = u;
            marker.scale = iconsScale;
            usina2DIcons.Add(marker);
            markerDisplacer.allInstantiatedMarkers.Add(marker);
        }
        foreach (PontoDeEntrega pe in sessionManager.peFixos)
        {
            OnlineMapsMarker3D marker = markerManager.Create(pe.coordenadas.lon, pe.coordenadas.lat, peIconPrefab2DFixed);
            marker.transform.GetComponent<PEStorage>().pe = pe;
            marker.scale = iconsScale;
            cg2DIcons.Add(marker);
            markerDisplacer.allInstantiatedMarkers.Add(marker);
        }
        foreach (TerminalGNL gnl in sessionManager.terminaisGNLFixos)
        {
            GameObject prefab = new GameObject();
            switch (gnl.categoria)
            {
                case "Existente":
                    prefab = gnlIconPrefab2D_existente_Fixed;
                    break;
                case "Em Estudo":
                    prefab = gnlIconPrefab2D_estudo_Fixed;
                    break;
                case "Previsto":
                    prefab = gnlIconPrefab2D_previsto_Fixed;
                    break;
            }
            OnlineMapsMarker3D marker = markerManager.Create(gnl.coordenadas.lon, gnl.coordenadas.lat, prefab);
            marker.transform.GetComponent<GNLStorage>().gnl = gnl;

            marker.scale = iconsScale;

            gnl2DIcons.Add(marker);

            markerDisplacer.allInstantiatedMarkers.Add(marker);

        }

        foreach (SubestacaoEnergia s in sessionManager.subestacoesFixas)
        {
            OnlineMapsMarker3D marker = markerManager.Create(s.coordenadas.lon, s.coordenadas.lat, subEstIconPrefab2DFixed);
            marker.transform.GetComponent<SubestacaoStorage>().subestacao = s;
            marker.scale = iconsScale;
            subEst2DIcons.Add(marker);
            markerDisplacer.allInstantiatedMarkers.Add(marker);
        }

        SubEstToggle(false);
        PEToggle(true);
        UsinaToggle(true);
        GNLToggle(true);
    }

    #region 2D_3D_TOGGLE_COMMENTED
    //void Remove2DIcons()
    //{
    //    foreach (OnlineMapsMarker3D m in cg2DIcons)
    //    {
    //        markerManager.Remove(m);
    //    }
    //    cg2DIcons.Clear();

    //    foreach (OnlineMapsMarker3D m in gnl2DIcons)
    //    {
    //        markerManager.Remove(m);
    //    }
    //    gnl2DIcons.Clear();

    //    foreach (OnlineMapsMarker3D m in usina2DIcons)
    //    {
    //        markerManager.Remove(m);
    //    }
    //    usina2DIcons.Clear();

    //    foreach (OnlineMapsMarker3D m in subEst2DIcons)
    //    {
    //        markerManager.Remove(m);
    //    }
    //    subEst2DIcons.Clear();
    //}

    //void ChangeAllUserMarkersTo2D()
    //{
    //    List<OnlineMapsMarker3D> tempList = new List<OnlineMapsMarker3D>();
    //    foreach (OnlineMapsMarker3D marker in markerDisplacer.markers)
    //    {
    //        tempList.Add(marker);
    //    }

    //    sessionManager.session.gasDistColocados.Clear();
    //    sessionManager.session.gasIntColocados.Clear();

    //    markerDisplacer.markers.Clear();

    //    foreach (OnlineMapsMarker3D m in tempList)
    //    {
    //        OnlineMapsMarker3D marker2d = new OnlineMapsMarker3D();
    //        if (m.transform.GetComponent<UsinaStorage>() != null)
    //        {
    //            marker2d = markerManager.Create(m.position.x, m.position.y, termoIconPrefab2DUser);

    //            marker2d.transform.GetComponent<UsinaStorage>().usina = m.transform.GetComponent<UsinaStorage>().usina;

    //            marker2d.transform.GetComponent<UsinaStorage>().gasDist = m.transform.GetComponent<UsinaStorage>().gasDist;

    //            connCreator.ConectarUsinaComPEProximo(marker2d.transform.gameObject);
    //        }
    //        else if (m.transform.GetComponent<FSRUStorage>() != null)
    //        {
    //            marker2d = markerManager.Create(m.position.x, m.position.y, FSRU2DPrefab);

    //            marker2d.transform.GetComponent<FSRUStorage>().fsru = m.transform.GetComponent<FSRUStorage>().fsru;

    //            marker2d.transform.GetComponent<FSRUStorage>().gasInt = m.transform.GetComponent<FSRUStorage>().gasInt;

    //            connCreator.ConectarFSRUComPEProximo(marker2d.transform.gameObject);

    //        }
    //        else if (m.transform.GetComponent<FSUStorage>() != null)
    //        {
    //            marker2d = markerManager.Create(m.position.x, m.position.y, FSU2DPrefab);
    //            marker2d.transform.GetComponent<FSUStorage>().fsu = m.transform.GetComponent<FSUStorage>().fsu;

    //        }
    //        else if (m.transform.GetComponent<TerminalStorage>() != null)
    //        {
    //            marker2d = markerManager.Create(m.position.x, m.position.y, terminalRegas2DPrefab);
    //            if (m.transform.GetComponent<TerminalStorage>().terminal != null)
    //                marker2d.transform.GetComponent<TerminalStorage>().terminal = m.transform.GetComponent<TerminalStorage>().terminal;
    //        }
    //        else if (m.transform.GetComponent<PEStorage>() != null)
    //        {
    //            marker2d = markerManager.Create(m.position.x, m.position.y, pe2DPrefab);
    //            marker2d.transform.GetComponent<PEStorage>().pe = m.transform.GetComponent<PEStorage>().pe;
    //        }
    //        else if (m.transform.GetComponent<SubestacaoStorage>() != null)
    //        {
    //            marker2d = markerManager.Create(m.position.x, m.position.y, subEst2DPrefab);
    //            if (m.transform.GetComponent<SubestacaoStorage>().subestacao != null)
    //                marker2d.transform.GetComponent<SubestacaoStorage>().subestacao = m.transform.GetComponent<SubestacaoStorage>().subestacao;
    //        }
    //        else if (m.transform.GetComponent<FrotaStorage>()!=null)
    //        {
    //            marker2d = markerManager.Create(m.position.x, m.position.y, frota2DPrefab);
    //            if (m.transform.GetComponent<FrotaStorage>().frota != null)
    //            {
    //                marker2d.transform.GetComponent<FrotaStorage>().frota = m.transform.GetComponent<FrotaStorage>().frota;
    //                marker2d.transform.GetComponent<FrotaStorage>().terminalDestino = m.transform.GetComponent<FrotaStorage>().terminalDestino;
    //            }
    //            OnlineMapsMarker3D termDestMarker = new OnlineMapsMarker3D();
    //            foreach(OnlineMapsMarker3D _m in userMarkers2DIcons)
    //            {
    //                if (_m.transform.GetComponent<TerminalStorage>() != null)
    //                    if (_m.transform.GetComponent<TerminalStorage>().terminal == m.transform.GetComponent<FrotaStorage>().terminalDestino)
    //                        termDestMarker = _m;
    //            }

    //            foreach(OnlineMapsMarker3D _m in tempList)
    //            {
    //                if (_m.transform.GetComponent<TerminalStorage>() != null)
    //                    if (_m.transform.GetComponent<TerminalStorage>().terminal == m.transform.GetComponent<FrotaStorage>().terminalDestino)
    //                        termDestMarker = _m;
    //            }

    //            connCreator.ConectarFrotaComTermRegas(marker2d.instance, termDestMarker.instance);
    //        }

    //        marker2d.scale = iconsScale;

    //        markerDisplacer.allInstantiatedMarkers.Remove(m);

    //        markerDisplacer.markers.Add(marker2d);

    //        markerDisplacer.allInstantiatedMarkers.Add(marker2d);

    //        userMarkers2DIcons.Add(marker2d);

    //        markerManager.Remove(m);


    //    }

    //}

    //void ChangeAllUserMarkersTo3D()
    //{
    //    List<OnlineMapsMarker3D> tempList = new List<OnlineMapsMarker3D>();
    //    foreach (OnlineMapsMarker3D marker in userMarkers2DIcons)
    //    {
    //        tempList.Add(marker);
    //    }
    //    userMarkers2DIcons.Clear();
    //    sessionManager.session.gasDistColocados.Clear();
    //    sessionManager.session.gasIntColocados.Clear();

    //    markerDisplacer.markers.Clear();
    //    foreach (OnlineMapsMarker3D m in tempList)
    //    {
    //        OnlineMapsMarker3D marker3d = new OnlineMapsMarker3D();
    //        if (m.transform.GetComponent<UsinaStorage>() != null)
    //        {
    //            marker3d = markerManager.Create(m.position.x, m.position.y, Termo3DPrefab);
    //            marker3d.transform.GetComponent<UsinaStorage>().usina = m.transform.GetComponent<UsinaStorage>().usina;
    //            marker3d.transform.GetComponent<UsinaStorage>().gasDist = m.transform.GetComponent<UsinaStorage>().gasDist;
    //            connCreator.ConectarUsinaComPEProximo(marker3d.transform.gameObject);

    //        }
    //        else if (m.transform.GetComponent<FSRUStorage>() != null)
    //        {
    //            marker3d = markerManager.Create(m.position.x, m.position.y, FSRU3DPrefab);
    //            marker3d.transform.GetComponent<FSRUStorage>().fsru = m.transform.GetComponent<FSRUStorage>().fsru;

    //            marker3d.transform.GetComponent<FSRUStorage>().gasInt = m.transform.GetComponent<FSRUStorage>().gasInt;
    //            connCreator.ConectarFSRUComPEProximo(marker3d.transform.gameObject);
    //        }
    //        else if (m.transform.GetComponent<FSUStorage>() != null)
    //        {
    //            marker3d = markerManager.Create(m.position.x, m.position.y, FSU3DPrefab);
    //        }
    //        else if (m.transform.GetComponent<TerminalStorage>() != null)
    //        {
    //            marker3d = markerManager.Create(m.position.x, m.position.y, terminalRegas3DPrefab);
    //        }
    //        else if (m.transform.GetComponent<PEStorage>() != null)
    //        {
    //            marker3d = markerManager.Create(m.position.x, m.position.y, pe3DPrefab);
    //        }
    //        else if (m.transform.GetComponent<SubestacaoStorage>() != null)
    //        {
    //            marker3d = markerManager.Create(m.position.x, m.position.y, subEst3DPrefab);
    //        }
    //        else if (m.transform.GetComponent<FrotaStorage>() != null)
    //        {
    //            marker3d = markerManager.Create(m.position.x, m.position.y, frota3DPrefab);
    //            marker3d.transform.GetComponent<FrotaStorage>().frota = m.transform.GetComponent<FrotaStorage>().frota;
    //        }
    //        marker3d.transform.gameObject.layer = 14;
    //        foreach (Transform t in marker3d.transform.gameObject.GetComponentInChildren<Transform>())
    //        {
    //            t.gameObject.layer = 14;
    //        }
    //        markerDisplacer.allInstantiatedMarkers.Remove(m);
    //        //marker3d.scale = markerDisplacer.markerScale;
    //        marker3d.transform.localRotation = Quaternion.Euler(270, 0, 180);
    //        markerDisplacer.allInstantiatedMarkers.Add(marker3d);
    //        markerDisplacer.markers.Add(marker3d);
    //        markerManager.Remove(m);
    //    }
    //}

    #endregion
    #region FIXED_3D_ICONS_COMMENTED
    //void GenerateCityGates()
    //{
    //    foreach (PontoDeEntrega pe in sessionManager.peFixos)
    //    {
    //        if ((pe.coordenadas.lon > maps.bounds.left && pe.coordenadas.lon < maps.bounds.right) && (pe.coordenadas.lat > maps.bounds.bottom && pe.coordenadas.lat < maps.bounds.top))
    //        {
    //            OnlineMapsMarker3D marker = markerManager.Create(pe.coordenadas.lon, pe.coordenadas.lat, fixedPePrefab);
    //            marker.transform.localRotation = Quaternion.Euler(270, 0, 180);
    //            //marker.scale = markerDisplacer.markerScale;
    //            marker.transform.GetComponent<PEStorage>().pe = pe;
    //            fixedCityGatesMarkers.Add(marker);
    //            markerDisplacer.allInstantiatedMarkers.Add(marker);
    //        }
    //    }
    //}

    //void EraseCityGates()
    //{
    //    foreach (OnlineMapsMarker3D m in fixedCityGatesMarkers)
    //    {
    //        markerDisplacer.allInstantiatedMarkers.Remove(m);
    //        markerManager.Remove(m);
    //    }
    //    fixedCityGatesMarkers.Clear();
    //}

    //void GenerateGNL()
    //{
    //    foreach (TerminalGNL gnl in sessionManager.terminaisGNLFixos)
    //    {
    //        if ((gnl.coordenadas.lon > maps.bounds.left && gnl.coordenadas.lon < maps.bounds.right) && (gnl.coordenadas.lat > maps.bounds.bottom && gnl.coordenadas.lat < maps.bounds.top))
    //        {
    //            OnlineMapsMarker3D marker = new OnlineMapsMarker3D();
    //            switch (gnl.categoria)
    //            {
    //                case "Existente":
    //                    marker = markerManager.Create(gnl.coordenadas.lon, gnl.coordenadas.lat, fixedGnlPrefab_existente);
    //                    break;
    //                case "Em Estudo":
    //                    marker = markerManager.Create(gnl.coordenadas.lon, gnl.coordenadas.lat, fixedGnlPrefab_estudo);
    //                    break;
    //                case "Previsto":
    //                    marker = markerManager.Create(gnl.coordenadas.lon, gnl.coordenadas.lat, fixedGnlPrefab_previsto);
    //                    break;
    //            }

    //            marker.transform.GetComponent<GNLStorage>().gnl = gnl;

    //            //marker.scale = markerDisplacer.markerScale;

    //            fixedGNLMarkers.Add(marker);

    //            markerDisplacer.allInstantiatedMarkers.Add(marker);


    //        }
    //    }

    //}

    //void EraseGNL()
    //{
    //    foreach (OnlineMapsMarker3D m in fixedGNLMarkers)
    //    {
    //        markerDisplacer.allInstantiatedMarkers.Remove(m);
    //        markerManager.Remove(m);
    //    }
    //    fixedGNLMarkers.Clear();
    //}

    //void GenerateTermo()
    //{
    //    foreach (Usina u in sessionManager.usinasFixas)
    //    {
    //        if ((u.coordenadas.lon > maps.bounds.left && u.coordenadas.lon < maps.bounds.right) && (u.coordenadas.lat > maps.bounds.bottom && u.coordenadas.lat < maps.bounds.top))
    //        {
    //            OnlineMapsMarker3D marker = markerManager.Create(u.coordenadas.lon, u.coordenadas.lat, Termo3DPrefab);
    //            marker.transform.localRotation = Quaternion.Euler(270, 0, 180);
    //           // marker.scale = markerDisplacer.markerScale;
    //            marker.transform.GetComponent<UsinaStorage>().usina = u;
    //            fixedUsinaMarkers.Add(marker);
    //            markerDisplacer.allInstantiatedMarkers.Add(marker);
    //            msg.text += "\n TRANSFORMOU USINA EM 3D";
    //        }
    //    }
    //}

    //void EraseTermo()
    //{
    //    foreach (OnlineMapsMarker3D m in fixedUsinaMarkers)
    //    {
    //        markerDisplacer.allInstantiatedMarkers.Remove(m);
    //        markerManager.Remove(m);
    //    }
    //    fixedUsinaMarkers.Clear();
    //}

    //void GenerateSubEst()
    //{
    //    foreach (SubestacaoEnergia s in sessionManager.subestacoesFixas)
    //    {
    //        if ((s.coordenadas.lon > maps.bounds.left && s.coordenadas.lon < maps.bounds.right) && (s.coordenadas.lat > maps.bounds.bottom && s.coordenadas.lat < maps.bounds.top))
    //        {
    //            OnlineMapsMarker3D marker = markerManager.Create(s.coordenadas.lon, s.coordenadas.lat, subEst3DPrefab);
    //            marker.transform.localRotation = Quaternion.Euler(270, 0, 180);
    //            //marker.scale = markerDisplacer.markerScale;
    //            marker.transform.GetComponent<SubestacaoStorage>().subestacao = s;
    //            fixedSubEstMarkers.Add(marker);
    //            markerDisplacer.allInstantiatedMarkers.Add(marker);
    //        }
    //    }
    //}

    //void EraseSubEst()
    //{
    //    foreach (OnlineMapsMarker3D m in fixedSubEstMarkers)
    //    {
    //        markerDisplacer.allInstantiatedMarkers.Remove(m);
    //        markerManager.Remove(m);
    //    }
    //    fixedSubEstMarkers.Clear();
    //}

    #endregion


    #region LOAD_MARKERS

    public void LoadUsinaMarkers(Usina u)
    {
        OnlineMapsMarker3D m = markerManager.Create(u.coordenadas.lon, u.coordenadas.lat, usina3DPrefab);
        m.instance.GetComponent<UsinaStorage>().usina = u;
        m.transform.localRotation = Quaternion.Euler(270, 0, 180);
        if (maps.floatZoom >= 10)
        {
            m.scale = markerDisplacer.markerScale;
        }
        else
        {
            m.instance.GetComponent<IconToggle>().ChangeTo2d();
            m.scale = 0.008f * maps.GetComponent<OnlineMapsMarker3DManager>().defaultScale;
        }
        markerDisplacer.allInstantiatedMarkers.Add(m);
        markerDisplacer.markers.Add(m);
        foreach (Transform t in m.instance.gameObject.GetComponentsInChildren<Transform>())
            t.gameObject.layer = 14;
    }
    public void LoadGNLMarkers(TerminalGNL g)
    {
        OnlineMapsMarker3D m = markerManager.Create(g.coordenadas.lon, g.coordenadas.lat, gnl3dPrefab);
        m.instance.GetComponent<GNLStorage>().gnl = g;
        m.transform.localRotation = Quaternion.Euler(270, 0, 180);
        if (maps.floatZoom >= 10)
        {
            m.scale = markerDisplacer.markerScale;
        }
        else
        {
            m.instance.GetComponent<IconToggle>().ChangeTo2d();
            m.scale = 0.008f * maps.GetComponent<OnlineMapsMarker3DManager>().defaultScale;
        }
        markerDisplacer.allInstantiatedMarkers.Add(m);
        markerDisplacer.markers.Add(m);
        foreach (Transform t in m.instance.gameObject.GetComponentsInChildren<Transform>())
            t.gameObject.layer = 14;
    }
    public void LoadFSUMarkers(FSU f)
    {
        OnlineMapsMarker3D m = markerManager.Create(f.coordenadas.lon, f.coordenadas.lat, FSU3DPrefab);
        m.instance.GetComponent<FSUStorage>().fsu = f;
        
        m.transform.localRotation = Quaternion.Euler(270, 0, 180);
        if (maps.floatZoom >= 10)
        {
            m.scale = markerDisplacer.markerScale;
        }
        else
        {
            m.instance.GetComponent<IconToggle>().ChangeTo2d();
            m.scale = 0.008f * maps.GetComponent<OnlineMapsMarker3DManager>().defaultScale;
        }
        foreach (OnlineMapsMarker3D marker in markerDisplacer.allInstantiatedMarkers)
        {
            if (marker.instance.GetComponent<GNLStorage>()!=null)
            {
                if (f.terminalGnl == marker.instance.GetComponent<GNLStorage>().gnl.nome)
                {
                    marker.instance.GetComponent<GNLStorage>().fsuMarker = m;
                }
            }
        }
        markerDisplacer.allInstantiatedMarkers.Add(m);
        markerDisplacer.markers.Add(m);
        foreach (Transform t in m.instance.gameObject.GetComponentsInChildren<Transform>())
            t.gameObject.layer = 14;

        m.instance.GetComponent<FSUStorage>().SetState();

    }

    public void LoadFSRUMarkers(FSRU f)
    {
        
        OnlineMapsMarker3D m = markerManager.Create(f.coordenadas.lon, f.coordenadas.lat, FSRU3DPrefab);
        m.instance.GetComponent<FSRUStorage>().fsru = f;
        m.transform.localRotation = Quaternion.Euler(270, 0, 180);
        if (maps.floatZoom >= 10)
        {
            m.scale = markerDisplacer.markerScale;
        }
        else
        {
            m.instance.GetComponent<IconToggle>().ChangeTo2d();
            m.scale = 0.008f * maps.GetComponent<OnlineMapsMarker3DManager>().defaultScale;
        }
        foreach (OnlineMapsMarker3D marker in markerDisplacer.allInstantiatedMarkers)
        {
            if (marker.instance.GetComponent<GNLStorage>() != null)
            {
                if (f.terminalGnl == marker.instance.GetComponent<GNLStorage>().gnl.nome)
                {
                    marker.instance.GetComponent<GNLStorage>().fsruMarker = m;
                }
            }
        }

        markerDisplacer.allInstantiatedMarkers.Add(m);
        markerDisplacer.markers.Add(m);
        foreach (Transform t in m.instance.gameObject.GetComponentsInChildren<Transform>())
            t.gameObject.layer = 14;

        m.instance.GetComponent<FSRUStorage>().SetState();

    }

    public void LoadTermRegasMarkers(TerminalRegas t)
    {

        OnlineMapsMarker3D m = markerManager.Create(t.coordenadasMarker.lon, t.coordenadasMarker.lat, terminalRegas3DPrefab);
        m.instance.GetComponent<TerminalRegasStorage>().terminal = t;
        m.transform.localRotation = Quaternion.Euler(270, 0, 180);
        if (maps.floatZoom >= 10)
        {
            m.scale = markerDisplacer.markerScale;
        }
        else
        {
            m.instance.GetComponent<IconToggle>().ChangeTo2d();
            m.scale = 0.008f * maps.GetComponent<OnlineMapsMarker3DManager>().defaultScale;
        }
        markerDisplacer.allInstantiatedMarkers.Add(m);
        markerDisplacer.markers.Add(m);
        foreach (Transform t2 in m.instance.gameObject.GetComponentsInChildren<Transform>())
            t2.gameObject.layer = 14;


        if (!string.IsNullOrEmpty(t.usina))
        {
            foreach (OnlineMapsMarker3D marker in markerDisplacer.markers)
            {
                if (marker.instance.GetComponent<UsinaStorage>() != null)
                {
                    if (marker.instance.GetComponent<UsinaStorage>().usina.nome == t.usina)
                    {
                        marker.instance.GetComponent<UsinaStorage>().termRegasMarker = m;
                    }
                }
            }
        }
        else if (!string.IsNullOrEmpty(t.terminalGnl))
        {
            foreach (OnlineMapsMarker3D marker in markerDisplacer.allInstantiatedMarkers)
            {
                if (marker.instance.GetComponent<GNLStorage>() != null)
                {
                    if (marker.instance.GetComponent<GNLStorage>().gnl.nome == t.terminalGnl)
                    {
                        marker.instance.GetComponent<GNLStorage>().termRegasMarker = m;
                    }
                }
            }
        }

        m.instance.GetComponent<TerminalRegasStorage>().SetState();

    }

    public void LoadSubestMarkers(SubestacaoEnergia s)
    {
        OnlineMapsMarker3D m = markerManager.Create(s.coordenadas.lon, s.coordenadas.lat, subEst3DPrefab);
        m.instance.GetComponent<SubestacaoStorage>().subestacao = s;
        m.transform.localRotation = Quaternion.Euler(270, 0, 180);
        if (maps.floatZoom >= 10)
        {
            m.scale = markerDisplacer.markerScale;
        }
        else
        {
            m.instance.GetComponent<IconToggle>().ChangeTo2d();
            m.scale = 0.008f * maps.GetComponent<OnlineMapsMarker3DManager>().defaultScale;
        }
        markerDisplacer.allInstantiatedMarkers.Add(m);
        markerDisplacer.markers.Add(m);
        foreach (Transform t in m.instance.gameObject.GetComponentsInChildren<Transform>())
            t.gameObject.layer = 14;
    }

    public void LoadPEMarkers(PontoDeEntrega p)
    {
        OnlineMapsMarker3D m = markerManager.Create(p.coordenadas.lon, p.coordenadas.lat, pe3DPrefab);
        m.instance.GetComponent<PEStorage>().pe = p;
        m.transform.localRotation = Quaternion.Euler(270, 0, 180);
        if (maps.floatZoom >= 10)
        {
            m.scale = markerDisplacer.markerScale;
        }
        else
        {
            m.instance.GetComponent<IconToggle>().ChangeTo2d();
            m.scale = 0.008f * maps.GetComponent<OnlineMapsMarker3DManager>().defaultScale;
        }
        markerDisplacer.allInstantiatedMarkers.Add(m);
        markerDisplacer.markers.Add(m);
        foreach (Transform t in m.instance.gameObject.GetComponentsInChildren<Transform>())
            t.gameObject.layer = 14;
    }

    public GameObject LoadFrotaMarkers(Frota f)
    {
        OnlineMapsMarker3D m = null;
        bool hasMarker = false;
        foreach (OnlineMapsMarker3D marker in markerDisplacer.markers)
        {
            if (marker.instance.GetComponent<FrotaStorage>() != null)
            {
                if (marker.position.x == f.coordenadasMarker.lon && marker.position.y == f.coordenadasMarker.lat)
                {
                    hasMarker = true;
                    m = marker;
                }
            }

        }
        if (!hasMarker)
        {
            m = markerManager.Create(f.coordenadasMarker.lon, f.coordenadasMarker.lat, frota3DPrefab);
            m.transform.localRotation = Quaternion.Euler(270, 0, 180);
            if (maps.floatZoom >= 10)
            {
                m.scale = markerDisplacer.markerScale;
            }
            else
            {
                m.instance.GetComponent<IconToggle>().ChangeTo2d();
                m.scale = 0.008f * maps.GetComponent<OnlineMapsMarker3DManager>().defaultScale;
            }
            markerDisplacer.allInstantiatedMarkers.Add(m);
            markerDisplacer.markers.Add(m);

            foreach (Transform t in m.instance.gameObject.GetComponentsInChildren<Transform>())
                t.gameObject.layer = 14;

            foreach (OnlineMapsMarker3D marker in markerDisplacer.allInstantiatedMarkers)
            {
                if (marker.instance.GetComponent<GNLStorage>() != null)
                {
                    if (marker.instance.GetComponent<GNLStorage>().gnl.nome == f.terminalGnlOrigem)
                    {
                        marker.instance.GetComponent<GNLStorage>().frotaMarker = m;
                    }
                }
            }
        }
        if (m != null)
        {
            m.instance.GetComponent<FrotaStorage>().frotas.Add(f);
            return m.instance;
        }
        else
        {
            return null;
        }
    }

    #endregion

    public void GNLToggle(bool response)
    {
        foreach (OnlineMapsMarker3D m in gnl2DIcons)
            m.enabled = response;
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void UsinaToggle(bool response)
    {
        foreach (OnlineMapsMarker3D m in usina2DIcons)
            m.enabled = response;
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void PEToggle(bool response)
    {
        foreach (OnlineMapsMarker3D m in cg2DIcons)
            m.enabled = response;
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void SubEstToggle(bool response)
    {
        foreach (OnlineMapsMarker3D m in subEst2DIcons)
            m.enabled = response;
        EventSystem.current.SetSelectedGameObject(null);
    }
}

#region CITYGATES

[Serializable]
public class CityGates
{
    public CityGate[] citygates;
}

[Serializable]
public class CityGate
{
    public CGAttributes attributes;
    public CGGeometry geometry;
}

[Serializable]
public class CGAttributes
{
    public string Nome;
    public string Estado;
    public string Municipio;
    public string Transporta;
    public string Classifica;
    public string Fonte;
    public int OBJECTID;
}

[Serializable]
public class CGGeometry
{
    public double x;
    public double y;
}

#endregion


#region TERMINAIS_GNL

[Serializable]
public class TerminaisGNLJson
{
    public TerminalGNLJson[] terminais_gnl;
}

[Serializable]
public class TerminalGNLJson
{
    public GNLAttributes attributes;
    public GNLGeometry geometry;
}

[Serializable]
public class GNLAttributes
{
    public string Nome;
    public string Municipio;
    public string UF;
    public string Proprietar;
    public string Classifica;
    public string Armz_m3;
    public int Cap_milm3d;
    public string Fonte;
    public int OBJECTID;
}

[Serializable]
public class GNLGeometry
{
    public double x;
    public double y;
}

#endregion

#region SUBESTACOES

[Serializable]
public class SubestacoesJSON
{
    public List<SubestacaoJSON> subestacoes;
}

[Serializable]
public class SubestacaoJSON
{
    public SubestacaoAttributes attributes;
    public SubestacaoGeometry geometry;
}

[Serializable]
public class SubestacaoAttributes
{
    public string Nome;
    public double Tensao;
    public string Situacao;
}

[Serializable]
public class SubestacaoGeometry
{
    public double x;
    public double y;
}

#endregion
