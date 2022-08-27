using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//SCRIPT UTILIZADO PARA CONECTAR OS OBJETOS COLOCADOS NO MAPA
public class ConnectionCreator : MonoBehaviour
{
    int gasIntIndex;
    int gasDistIndex;
    int gasCrioIndex;

    public TextMesh debug;
    public SessionManager sessionManager;
    public MarkerDisplacer markerDisplacer;
    public GasClasses gasClasses;
    public OnlineMaps maps;
    OnlineMapsMarker3DManager markerManager;

    public GameObject goOrigin;
    public GameObject goDestination;

    public List<GasodutoDistribuicao> gasDistList;
    public List<GasodutoIntegrador> gasIntList;

    public Material gasDistMat;
    public Material gasIntMat;
    public Material gasCrioMat;
    public Material frotaMat;
    public Texture2D dotFrotaLineMat;

    public GameObject originMarkPrefab;
    GameObject originMarkInstance;

    public GameObject gasIntPrefab;
    public GameObject gasDistPrefab;
    public GameObject gasCrioPrefab;

    public GameObject frotaLineStartPointPrefab;


    public void SetOrigin(GameObject _origin)
    {
        goOrigin = _origin;
        originMarkInstance = Instantiate(originMarkPrefab, goOrigin.transform);
    }

    // Conexões com DUTOVIARIAS

    public void ConectarFSRUComPEProximo(GameObject fsruObj, bool newFsru = false, GameObject peObj = null, GasodutoIntegrador loadedGasInt = null)
    {
        if (fsruObj.GetComponent<FSRUStorage>() != null)
        {
            FSRU _fsru = fsruObj.GetComponent<FSRUStorage>().fsru;
            Vector2 fsruCoord = new Vector2(Convert.ToSingle(_fsru.coordenadas.lon), Convert.ToSingle(_fsru.coordenadas.lat));
            PontoDeEntrega peEscolhido = new PontoDeEntrega();
            GameObject destination = new GameObject();
            if (peObj == null)
            {
                List<PontoDeEntrega> pes = sessionManager.peFixos;
                foreach (PontoDeEntrega p in sessionManager.session.pontosDeEntrega)
                    pes.Add(p);
                for (int x = 0; x < pes.Count; x++)
                {
                    for (int y = pes.Count - 1; y >= x; y--)
                    {
                        Vector2 pe1Coord = new Vector2(Convert.ToSingle(pes[x].coordenadas.lon), Convert.ToSingle(pes[x].coordenadas.lat));
                        Vector2 pe2Coord = new Vector2(Convert.ToSingle(pes[y].coordenadas.lon), Convert.ToSingle(pes[y].coordenadas.lat));
                        if (OnlineMapsUtils.DistanceBetweenPointsD(fsruCoord, pe1Coord) > OnlineMapsUtils.DistanceBetweenPointsD(fsruCoord, pe2Coord))
                        {
                            PontoDeEntrega temp = pes[x];
                            pes[x] = pes[y];
                            pes[y] = temp;
                        }
                    }
                }
                foreach (OnlineMapsMarker3D m in markerDisplacer.markers)
                {
                    if (m.transform.GetComponent<PEStorage>() != null)
                    {
                        if (m.transform.GetComponent<PEStorage>().pe != null)
                        {
                            if (m.transform.GetComponent<PEStorage>().pe == pes[0])
                            {
                                peEscolhido = m.transform.GetComponent<PEStorage>().pe;
                                destination = m.transform.gameObject;
                            }
                        }
                    }
                }

                foreach (OnlineMapsMarker3D m in gasClasses.cg2DIcons)
                {
                    if (m.transform.GetComponent<PEStorage>() != null)
                    {
                        if (m.transform.GetComponent<PEStorage>().pe != null)
                        {
                            if (m.transform.GetComponent<PEStorage>().pe == pes[0])
                            {
                                peEscolhido = m.transform.GetComponent<PEStorage>().pe;
                                destination = m.transform.gameObject;
                            }
                        }
                    }
                }
                Vector2 nextPeCoord = new Vector2(Convert.ToSingle(peEscolhido.coordenadas.lon), Convert.ToSingle(peEscolhido.coordenadas.lat));
                GasodutoIntegrador gasInt = new GasodutoIntegrador("Gasoduto Integrador " + gasIntIndex, sessionManager.session.presets.gasIntPreset.custoBase,
                    OnlineMapsUtils.DistanceBetweenPointsD(fsruCoord, nextPeCoord), sessionManager.session.presets.gasIntPreset.diametro, sessionManager.session.premissas);
                gasInt.fsruOrigem = fsruObj.GetComponent<FSRUStorage>().fsru.nome;
                gasInt.peDestino = peEscolhido.nome;
                fsruObj.GetComponent<FSRUStorage>().gasInt = gasInt;
                sessionManager.session.gasIntColocados.Add(gasInt);
                if (newFsru)
                    gasIntIndex++;
                foreach (GasodutoIntegrador g in sessionManager.session.gasIntColocados)
                {
                    if (("Gasoduto Integrador " + gasIntIndex.ToString()) == g.nome)
                    {
                        int newIndex = int.Parse(g.nome.Replace("Gasoduto Integrador ", "")) + 1;
                        gasIntIndex = newIndex;
                    }
                }
            }
            else
            {
                destination = peObj;
                fsruObj.GetComponent<FSRUStorage>().gasInt = loadedGasInt;
                peObj.GetComponent<PEStorage>().gasIntList.Add(loadedGasInt);
            }


            GameObject pipe = Instantiate(gasIntPrefab/*, maps.transform*/);
            pipe.GetComponent<PipeScaleController>().originObj = fsruObj;
            pipe.GetComponent<PipeScaleController>().destinationObj = destination;

            List<Vector2> points = new List<Vector2>();

            points.Add((fsruObj.GetComponent<OnlineMapsMarker3DInstance>().marker as OnlineMapsMarker3D).position);
            points.Add((destination.GetComponent<OnlineMapsMarker3DInstance>().marker as OnlineMapsMarker3D).position);

            OnlineMapsDrawingLine line = new OnlineMapsDrawingLine(points, new Color(1, 0.6f, 0), 0.01f);
            OnlineMapsDrawingElementManager.AddItem(line);

            fsruObj.GetComponent<FSRUStorage>().gasIntPipe = pipe;
            fsruObj.GetComponent<FSRUStorage>().gasIntLine = line;
        }
    }

    public void ConectarUsinaComPEProximo(GameObject usinaObj, bool newUsina = false, GameObject peObj = null, GasodutoDistribuicao loadedGasDist = null)
    {
        if (usinaObj.GetComponent<UsinaStorage>() != null)
        {
            UsinaStorage usinaStorage = usinaObj.GetComponent<UsinaStorage>();
            Usina _usina = usinaStorage.usina;
            GameObject destination = new GameObject();
            Vector2 usinaCoord = new Vector2(Convert.ToSingle(_usina.coordenadas.lon), Convert.ToSingle(_usina.coordenadas.lat));
            if (peObj == null)
            {
                List<PontoDeEntrega> pes = sessionManager.peFixos;
                foreach (PontoDeEntrega p in sessionManager.session.pontosDeEntrega)
                    pes.Add(p);
                for (int x = 0; x < pes.Count; x++)
                {
                    for (int y = pes.Count - 1; y >= x; y--)
                    {
                        Vector2 pe1Coord = new Vector2(Convert.ToSingle(pes[x].coordenadas.lon), Convert.ToSingle(pes[x].coordenadas.lat));
                        Vector2 pe2Coord = new Vector2(Convert.ToSingle(pes[y].coordenadas.lon), Convert.ToSingle(pes[y].coordenadas.lat));
                        if (OnlineMapsUtils.DistanceBetweenPointsD(usinaCoord, pe1Coord) > OnlineMapsUtils.DistanceBetweenPointsD(usinaCoord, pe2Coord))
                        {
                            PontoDeEntrega temp = pes[x];
                            pes[x] = pes[y];
                            pes[y] = temp;
                        }
                    }
                }
                foreach (OnlineMapsMarker3D m in markerDisplacer.allInstantiatedMarkers)
                {
                    if (m.transform.GetComponent<PEStorage>() != null)
                    {
                        if (m.transform.GetComponent<PEStorage>().pe != null)
                        {
                            if (m.transform.GetComponent<PEStorage>().pe == pes[0])
                            {
                                destination = m.transform.gameObject;
                            }
                        }
                    }
                }


                GasodutoDistribuicao gasDist = new GasodutoDistribuicao("Gasoduto Distribuidor " + gasDistIndex, _usina, pes[0], sessionManager.session.gas, sessionManager.session.premissas)
                {
                    peOrigem = pes[0].nome
                };
                sessionManager.session.gasDistColocados.Add(gasDist);
                usinaStorage.gasDist = gasDist;
                usinaStorage.usina.malha = pes[0].GasodutoTransporte;
                usinaStorage.usina.pontoDeEntrega = pes[0].nome;
                if (newUsina)
                    gasDistIndex++;
                foreach (GasodutoDistribuicao g in sessionManager.session.gasDistColocados)
                {
                    if (("Gasoduto Distribuidor " + gasDistIndex.ToString()) == g.nome)
                    {
                        int newIndex = int.Parse(g.nome.Replace("Gasoduto Distribuidor ", "")) + 1;
                        gasDistIndex = newIndex;
                    }
                }
                usinaStorage.usina.fonteDoGas = "gasoduto"; //Comment
                usinaStorage.usina.valorMalha = pes[0].valorMalha;
            }
            else
            {
                destination = peObj;
                usinaStorage.gasDist = loadedGasDist;
                peObj.GetComponent<PEStorage>().gasDistList.Add(loadedGasDist);
            }
            GameObject pipe = Instantiate(gasDistPrefab/*, maps.transform*/);
            pipe.GetComponent<PipeScaleController>().originObj = usinaObj;
            pipe.GetComponent<PipeScaleController>().destinationObj = destination;
            usinaObj.GetComponent<UsinaStorage>().gasDistPipe = pipe;
            List<Vector2> points = new List<Vector2>();
            points.Add((usinaObj.GetComponent<OnlineMapsMarker3DInstance>().marker as OnlineMapsMarker3D).position);
            points.Add((destination.GetComponent<OnlineMapsMarker3DInstance>().marker as OnlineMapsMarker3D).position);
            OnlineMapsDrawingLine line = new OnlineMapsDrawingLine(points, new Color(1, 0.847f, 0), 0.01f);
            OnlineMapsDrawingElementManager.AddItem(line);
            if (usinaObj.GetComponent<UsinaStorage>().gasDistLine != null)
            {
                OnlineMapsDrawingElementManager.RemoveItem(usinaStorage.gasDistLine);
                usinaStorage.gasDistLine = null;
            }
            usinaStorage.gasDistLine = line;
           
        }
    }

    public void ConectarTermRegasComPEProximo(GameObject termRegasObj, GameObject peObj = null, GasodutoIntegrador loadedGasInt = null)
    {
        Vector2 termRegasCoord = termRegasObj.GetComponent<OnlineMapsMarker3DInstance>().marker.position;
        GameObject destination = new GameObject();

        if (peObj == null)
        {
            List<PontoDeEntrega> pes = sessionManager.peFixos;

            foreach (PontoDeEntrega p in sessionManager.session.pontosDeEntrega)
                pes.Add(p);

            for (int x = 0; x < pes.Count; x++)
            {
                for (int y = pes.Count - 1; y >= x; y--)
                {
                    Vector2 pe1Coord = new Vector2(Convert.ToSingle(pes[x].coordenadas.lon), Convert.ToSingle(pes[x].coordenadas.lat));
                    Vector2 pe2Coord = new Vector2(Convert.ToSingle(pes[y].coordenadas.lon), Convert.ToSingle(pes[y].coordenadas.lat));
                    if (OnlineMapsUtils.DistanceBetweenPointsD(termRegasCoord, pe1Coord) > OnlineMapsUtils.DistanceBetweenPointsD(termRegasCoord, pe2Coord))
                    {
                        PontoDeEntrega temp = pes[x];
                        pes[x] = pes[y];
                        pes[y] = temp;
                    }
                }
            }


            foreach (OnlineMapsMarker3D m in markerDisplacer.allInstantiatedMarkers)
            {
                if (m.transform.GetComponent<PEStorage>() != null)
                {
                    if (m.transform.GetComponent<PEStorage>().pe != null)
                    {
                        if (m.transform.GetComponent<PEStorage>().pe == pes[0])
                        {
                            destination = m.transform.gameObject;
                        }
                    }
                }
            }

            Vector2 gnlCoord = new Vector2();
            foreach (OnlineMapsMarker3D m in markerDisplacer.allInstantiatedMarkers)
            {
                if (m.instance.GetComponent<GNLStorage>() != null)
                {
                    TerminalGNL g = m.instance.GetComponent<GNLStorage>().gnl;
                    if (g.nome == termRegasObj.GetComponent<TerminalRegasStorage>().terminal.terminalGnl)
                    {
                        gnlCoord = m.position;
                    }
                }
            }


            Vector2 nextPeCoord = new Vector2(Convert.ToSingle(pes[0].coordenadas.lon), Convert.ToSingle(pes[0].coordenadas.lat));

            GasodutoIntegrador gasInt = new GasodutoIntegrador("Gasoduto Integrador " + gasIntIndex, sessionManager.session.presets.gasIntPreset.custoBase,
                    OnlineMapsUtils.DistanceBetweenPointsD(gnlCoord, nextPeCoord), sessionManager.session.presets.gasIntPreset.diametro, sessionManager.session.premissas);

            gasInt.terminalRegasOrigem = termRegasObj.GetComponent<TerminalRegasStorage>().terminal.nome;
            gasInt.peDestino = pes[0].nome;

            termRegasObj.GetComponent<TerminalRegasStorage>().gasInt = gasInt;

            sessionManager.session.gasIntColocados.Add(gasInt);

            gasIntIndex++;

            foreach (GasodutoIntegrador g in sessionManager.session.gasIntColocados)
            {
                if (("Gasoduto Integrador " + gasIntIndex.ToString()) == g.nome)
                {
                    int newIndex = int.Parse(g.nome.Replace("Gasoduto Integrador ", "")) + 1;
                    gasIntIndex = newIndex;
                }
            }
        }
        else
        {
            destination = peObj;
            termRegasObj.GetComponent<TerminalRegasStorage>().gasInt = loadedGasInt;
            peObj.GetComponent<PEStorage>().gasIntList.Add(loadedGasInt);
        }


        GameObject pipe = Instantiate(gasIntPrefab/*, maps.transform*/);
        pipe.GetComponent<PipeScaleController>().originObj = termRegasObj;
        pipe.GetComponent<PipeScaleController>().destinationObj = destination;
        termRegasObj.GetComponent<TerminalRegasStorage>().gasIntPipe = pipe;

        List<Vector2> points = new List<Vector2>();

        points.Add((termRegasObj.GetComponent<OnlineMapsMarker3DInstance>().marker as OnlineMapsMarker3D).position);
        points.Add((destination.GetComponent<OnlineMapsMarker3DInstance>().marker as OnlineMapsMarker3D).position);

        OnlineMapsDrawingLine line = new OnlineMapsDrawingLine(points, new Color(1, 0.5f, 0.5f), 0.01f);
        OnlineMapsDrawingElementManager.AddItem(line);
        termRegasObj.GetComponent<TerminalRegasStorage>().gasIntLine = line;

    }

    // Conexões com RODOVIARIAS

    public void ConectarFrotaComTermRegas(GameObject frotaObj, GameObject termRegasObj, bool isLoading = false, Frota f = null)
    {
        List<Vector2> points = new List<Vector2>
        {
            (frotaObj.GetComponent<OnlineMapsMarker3DInstance>().marker as OnlineMapsMarker3D).position,
            (termRegasObj.GetComponent<OnlineMapsMarker3DInstance>().marker as OnlineMapsMarker3D).position
        };

        OnlineMapsDrawingLine line = new OnlineMapsDrawingLine(points, new Color(0.5f, 0.5f, 0.5f), 0.025f);
        OnlineMapsDrawingLine line2 = new OnlineMapsDrawingLine(points, new Color(1f, 1f, 1f), 0.008f);
        line2.texture = dotFrotaLineMat;

        OnlineMapsDrawingElementManager.AddItem(line);
        OnlineMapsDrawingElementManager.AddItem(line2);

        TerminalRegas termRegasDestino = termRegasObj.GetComponent<TerminalRegasStorage>().terminal;
        if (!isLoading)
        {
            Vector2 termGnlCoord = new Vector2();
            Vector2 usinaCoord = new Vector2();
            Usina usina_destination = new Usina();

            OnlineMapsMarker3D frotaMarker = frotaObj.GetComponent<OnlineMapsMarker3DInstance>().marker as OnlineMapsMarker3D;

            Frota frota = new Frota("Frota " + markerDisplacer.frotaIndex);

            frota.coordenadasMarker = new Coordenadas()
            {
                lon = frotaMarker.position.x,
                lat = frotaMarker.position.y
            };
            frota.fretePorKm = sessionManager.session.premissas.frete;

            frotaMarker.transform.GetComponent<FrotaStorage>().frotas.Add(frota);

            frota.terminalRegasDestino = termRegasDestino.nome;

            markerDisplacer.frotaIndex++;

            foreach (OnlineMapsMarker3D m in markerDisplacer.allInstantiatedMarkers)
            {
                if (m.instance.GetComponent<GNLStorage>() != null)
                {
                    if (m.instance.GetComponent<GNLStorage>().frotaMarker == (frotaObj.GetComponent<OnlineMapsMarker3DInstance>().marker as OnlineMapsMarker3D))
                    {
                        frota.terminalGnlOrigem = m.instance.GetComponent<GNLStorage>().gnl.nome;
                        termGnlCoord = m.position;
                    }
                }
                else if (m.instance.GetComponent<UsinaStorage>() != null)
                {
                    if (m.instance.GetComponent<UsinaStorage>().termRegasMarker == termRegasObj.GetComponent<OnlineMapsMarker3DInstance>().marker as OnlineMapsMarker3D)
                    {
                        usinaCoord = m.position;
                        usina_destination = m.instance.GetComponent<UsinaStorage>().usina;
                        m.instance.GetComponent<UsinaStorage>().frotaLine = line;
                        m.instance.GetComponent<UsinaStorage>().frotaLine2 = line2;
                        m.instance.GetComponent<UsinaStorage>().usina.fonteDoGas = "frota";

                        m.instance.GetComponent<UsinaStorage>().SetState();
                    }

                }
            }

            frota.Set(OnlineMapsUtils.DistanceBetweenPointsD(termGnlCoord, usinaCoord), sessionManager.session.presets.frotaPreset.fretePorKm,
                sessionManager.session.presets.frotaPreset.capacidadeLiquido, sessionManager.session.gas, sessionManager.session.premissas, usina_destination);

            frotaObj.GetComponent<FrotaStorage>().frotaLines.Add(frota.nome, line);
            frotaObj.GetComponent<FrotaStorage>().frotaLines2.Add(frota.nome, line2);

            sessionManager.session.frotasColocadas.Add(frota);
        }
        else
        {
            if (f != null)
            {
                OnlineMapsMarker3D frotaMarker = frotaObj.GetComponent<OnlineMapsMarker3DInstance>().marker as OnlineMapsMarker3D;

                frotaMarker.instance.GetComponent<FrotaStorage>().frotaLines.Add(f.nome, line);
                frotaMarker.instance.GetComponent<FrotaStorage>().frotaLines2.Add(f.nome, line2);

                foreach (OnlineMapsMarker3D m in markerDisplacer.markers)
                {
                    if (m.instance.GetComponent<UsinaStorage>() != null)
                    {
                        if (m.instance.GetComponent<UsinaStorage>().termRegasMarker == (termRegasObj.GetComponent<OnlineMapsMarker3DInstance>().marker as OnlineMapsMarker3D))
                        {
                            m.instance.GetComponent<UsinaStorage>().frotaLine = line;
                            m.instance.GetComponent<UsinaStorage>().frotaLine2 = line2;

                            m.instance.GetComponent<UsinaStorage>().SetState();

                        }
                    }
                }
            }

        }
    }

    public void ConectarFSUComTerminais(GameObject fsuObj, GameObject terminalObj, bool isLoading = false, GasodutoCriogenico loadedGasCrio = null)
    {
        GameObject pipe = Instantiate(gasCrioPrefab);
        pipe.GetComponent<PipeScaleController>().originObj = fsuObj;
        pipe.GetComponent<PipeScaleController>().destinationObj = terminalObj;

        fsuObj.GetComponent<FSUStorage>().gasCrioPipe = pipe;
        Vector2 fsuCoord = fsuObj.GetComponent<OnlineMapsMarker3DInstance>().marker.position;
        Vector2 termCoord = new Vector2();
        TerminalGNL gnl = null;
        foreach (OnlineMapsMarker3D m in markerDisplacer.allInstantiatedMarkers)
        {
            if (m.instance.GetComponent<GNLStorage>() != null)
            {
                gnl = m.instance.GetComponent<GNLStorage>().gnl;

                if (terminalObj.GetComponent<TerminalRegasStorage>() != null)
                {
                    if (terminalObj.GetComponent<TerminalRegasStorage>().terminal.terminalGnl == gnl.nome)
                    {
                        if (loadedGasCrio != null)
                        {
                            terminalObj.GetComponent<TerminalRegasStorage>().gasCrio = loadedGasCrio;
                            fsuObj.GetComponent<FSUStorage>().gasCrio = loadedGasCrio;
                        }
                        termCoord = m.position;
                        break;
                    }
                }
                else if (terminalObj.GetComponent<FrotaStorage>() != null)
                {
                    if (terminalObj.GetComponent<FrotaStorage>().frotas.Count > 0)
                    {
                        if (terminalObj.GetComponent<FrotaStorage>().frotas[0].terminalGnlOrigem == gnl.nome)
                        {
                            if (loadedGasCrio != null)
                            {
                                terminalObj.GetComponent<FrotaStorage>().gasCrio = loadedGasCrio;
                                fsuObj.GetComponent<FSUStorage>().gasCrio = loadedGasCrio;
                            }
                            termCoord = m.position;
                            break;
                        }
                    }
                }
            }
        }

        if (!isLoading)
        {
            foreach (GasodutoCriogenico g in sessionManager.session.gasCrioColocados)
            {
                if (("Gasoduto Criogênico " + gasCrioIndex.ToString()) == g.nome)
                {
                    int newIndex = int.Parse(g.nome.Replace("Gasoduto Criogênico ", "")) + 1;
                    gasCrioIndex = newIndex;
                }
            }
            GasodutoCriogenico gasCrio = new GasodutoCriogenico("Gasoduto Criogênico " + gasCrioIndex, sessionManager.session.presets.gasCrioPreset.custoBase,
                OnlineMapsUtils.DistanceBetweenPointsD(fsuCoord, termCoord), sessionManager.session.presets.gasCrioPreset.diametro, sessionManager.session.premissas);
            gasCrio.fsuOrigem = fsuObj.GetComponent<FSUStorage>().fsu.nome;
            foreach(Custo c in gasCrio.custos)
            {
                if (c.tipo == "opex")
                {
                    c.valor = 3000000;
                }
            }
            if (terminalObj.GetComponent<FrotaStorage>() != null)
            {
                terminalObj.GetComponent<FrotaStorage>().gasCrio = gasCrio;
                if (gnl != null)
                {
                    gasCrio.terminalCargaDestino = "Terminal de carga do " + gnl.nome;
                }

            }
            else
            {
                terminalObj.GetComponent<TerminalRegasStorage>().gasCrio = gasCrio;
                gasCrio.terminalRegasDestino = terminalObj.GetComponent<TerminalRegasStorage>().terminal.nome;
            }
            fsuObj.GetComponent<FSUStorage>().gasCrio = gasCrio;
            sessionManager.session.gasCrioColocados.Add(gasCrio);
            gasCrioIndex++;
        }


    }


}
