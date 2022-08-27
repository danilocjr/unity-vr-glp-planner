using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;
using System.Globalization;

[Serializable]
public class Session
{
    public string nome;
    public Presets presets;
    public GasNatural gas;
    public Premissas premissas;

    public List<Usina> usinasColocadas;
    public List<FSRU> fsruColocados;
    public List<FSU> fsuColocados;
    public List<TerminalRegas> terminaisRegasColocados;
    public List<PontoDeEntrega> pontosDeEntrega;
    public List<SubestacaoEnergia> subestacoesColocadas;

    public List<TerminalGNL> terminaisGNLColocados;

    public List<GasodutoIntegrador> gasIntColocados;
    public List<GasodutoDistribuicao> gasDistColocados;
    public List<GasodutoCriogenico> gasCrioColocados;
    public List<Frota> frotasColocadas;


    public Session()
    {
        nome = "";
        presets = new Presets();

        gas = new GasNatural(presets.gasPreset.pci, presets.gasPreset.pcs, presets.gasPreset.taxaCompressao);

        premissas = new Premissas(presets.premissasPreset.cotacaoDolar, presets.premissasPreset.tma, presets.premissasPreset.periodoAmortizacao, presets.premissasPreset.frete);

        usinasColocadas = new List<Usina>();
        fsruColocados = new List<FSRU>();
        fsuColocados = new List<FSU>();
        terminaisRegasColocados = new List<TerminalRegas>();
        pontosDeEntrega = new List<PontoDeEntrega>();
        subestacoesColocadas = new List<SubestacaoEnergia>();

        terminaisGNLColocados = new List<TerminalGNL>();

        gasIntColocados = new List<GasodutoIntegrador>();
        gasDistColocados = new List<GasodutoDistribuicao>();
        gasCrioColocados = new List<GasodutoCriogenico>();
        frotasColocadas = new List<Frota>();

    }
}

[Serializable]
public class Presets
{
    public GasNatural gasPreset;
    public Premissas premissasPreset;

    public Usina usinaPreset;
    public FSRU fsruPreset;
    public FSU fsuPreset;
    public TerminalRegas terminalRegasPreset;
    public PontoDeEntrega pePreset;
    public SubestacaoEnergia subestacaoPreset;

    public TerminalGNL terminalGNLPreset;

    public GasodutoIntegrador gasIntPreset;
    public GasodutoDistribuicao gasDistPreset;
    public GasodutoCriogenico gasCrioPreset;
    public Frota frotaPreset;

    public Presets()
    {
        gasPreset = new GasNatural(8400, 9400, 600);
        premissasPreset = new Premissas(5.76f, 5, 15, 5.5f);
        usinaPreset = new Usina("Usina_Preset", 43000, 44, gasPreset, premissasPreset, 30);
        frotaPreset = new Frota("Frota_Preset", 88, 5.5f, 48, gasPreset, premissasPreset, usinaPreset);

        pePreset = new PontoDeEntrega();
        gasIntPreset = new GasodutoIntegrador("GasInt_Preset", 100, 12, 24, premissasPreset);
        gasDistPreset = new GasodutoDistribuicao("GasDist_Preset", usinaPreset, pePreset, gasPreset, premissasPreset);
        fsruPreset = new FSRU()
        {
            categoria = "Preset",
            nome = "FSRU_Preset"
        };
        fsuPreset = new FSU("FSU_Preset", "Preset");
        terminalRegasPreset = new TerminalRegas("TerminalRegas_Preset");
        terminalRegasPreset.usina = usinaPreset.nome;
        gasCrioPreset = new GasodutoCriogenico("GasCrio_Preset", 200, 4, 24, premissasPreset);

    }
}

public class SessionManager : MonoBehaviour
{
    public UIManager uiManager;
    public Session session;
    Session sessionToLoad;

    public MarkerDisplacer markerDisplacer;
    public OnlineMapsMarker3DManager markerManager;
    public GeneralSetupPanel setupPanel;
    GasClasses gasClasses;
    ConnectionCreator connCreator;

    public List<Usina> usinasFixas = new List<Usina>();
    public List<TerminalGNL> terminaisGNLFixos = new List<TerminalGNL>();
    public List<PontoDeEntrega> peFixos = new List<PontoDeEntrega>();
    public List<SubestacaoEnergia> subestacoesFixas = new List<SubestacaoEnergia>();



    double totalValue;

    public Text totalValueText;

    OnScreenKeyboard osk;
    public InputField sessionNameField;
    public GameObject sessionButtonPrefab;
    public Transform sessionButtonsParent;
    public Animator sessionNameFieldAnim;

    public Animator sessionRenameFieldAnim;
    public InputField sessionRenameField;


    public bool faltaGas;

    public PresetsManager presetsManager;

    public Animator saveOpenSessionBeforeClearAnim;
    public Animator saveOpenSessionBeforeLoadAnim;
    public Animator deleteOpenSessionAnim;

    public GameObject deleteBtn, renameBtn;

    public TextAsset simulacaoGasodutoFile, simulacaoFrotaFile;
    Session simulacaoGasoduto, simulacaoFrota;

    bool createNewSession;
    bool loadNewSession;

    public Text sessionName;

    private void Awake()
    {
        gasClasses = markerDisplacer.gasClasses;
        connCreator = markerDisplacer.connCreator;
        session = new Session();

        //sessionNameFieldAnim.SetBool("hide", true);
        //sessionRenameFieldAnim.SetBool("hide", true);
    }

    public double PMT(double loanAmount)
    {
        var n = session.premissas.periodoAmortizacao;
        var r = session.premissas.tma * 0.01f;
        var pmt = loanAmount * r / (1 - Math.Pow(1 + r, -n));
        return (double)pmt;
    }

    private void Start()
    {
        osk = uiManager.oskParent.GetComponentInChildren<OnScreenKeyboard>();
        deleteOpenSessionAnim.gameObject.SetActive(false);
        saveOpenSessionBeforeClearAnim.gameObject.SetActive(false);
        saveOpenSessionBeforeLoadAnim.gameObject.SetActive(false);
        sessionNameFieldAnim.gameObject.SetActive(false);
        sessionRenameFieldAnim.gameObject.SetActive(false);
        ShowSavedSessions();
    }

    public Button testButton;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            testButton.onClick.Invoke();
    }


    public void UpdateSession()
    {
        totalValue = 0;
        double _gasDemandado = 0;
        double _gasFornecido = 0;
        List<Frota> frotasUtilizadas = new List<Frota>();
        List<GasodutoDistribuicao> gasDistUtilizados = new List<GasodutoDistribuicao>();
        List<TerminalRegas> termRegasUtilizados = new List<TerminalRegas>();

        List<GasodutoCriogenico> gasCrioUtilizados = new List<GasodutoCriogenico>();
        List<FSU> fsuUtilizados = new List<FSU>();

        foreach (FSRU f in session.fsruColocados)
        {

            if (f.custoUsado.Contains("capex"))
            {
                foreach (Custo c in f.custos)
                {
                    if (c.tipo.Contains("capex"))
                    {
                        totalValue += PMT(c.valor);
                    }
                }

            }
            else
            {
                foreach (Custo c in f.custos)
                {
                    if (c.tipo.Contains("opex"))
                    {
                        totalValue += c.valor;
                    }
                }
            }
            _gasFornecido += f.gnlFornecido;
        }
        foreach (FSU f in session.fsuColocados)
        {
            if (!string.IsNullOrWhiteSpace(f.terminalRegasDestino))
            {
                if (!fsuUtilizados.Contains(f))
                {
                    fsuUtilizados.Add(f);
                }
            }

        }

        foreach (TerminalRegas t in session.terminaisRegasColocados)
        {
            if (!string.IsNullOrEmpty(t.terminalGnl))
            {
                if (!termRegasUtilizados.Contains(t))
                    termRegasUtilizados.Add(t);
            }
        }

        foreach (GasodutoIntegrador gas in session.gasIntColocados)
        {
            gas.Recalculate(session.premissas);
            foreach (Custo c in gas.custos)
            {
                if (c.tipo.Contains("opex"))
                {
                    totalValue += c.valor;
                }
            }

        }


        foreach (Usina u in session.usinasColocadas)
        {
            u.Recalculate(session.premissas, session.gas);
            _gasDemandado += u.demandaDiaM3;
            double custoDiario = 0;
            double opexMalha = 0;
            if (u.fonteDoGas == "gasoduto")
            {
                foreach (GasodutoDistribuicao gas in session.gasDistColocados)
                {
                    if (gas.usinaDestino == u.nome)
                    {
                        PontoDeEntrega pe = new PontoDeEntrega();
                        foreach (PontoDeEntrega _pe in peFixos)
                        {
                            if (_pe.nome == u.pontoDeEntrega)
                                pe = _pe;
                        }
                        u.valorMalha = pe.valorMalha;
                        gas.Recalculate(u, pe, session.premissas, session.gas);
                        gasDistUtilizados.Add(gas);
                    }
                }
                custoDiario = u.valorMalha * u.demandaDiaPci;
                opexMalha = custoDiario * (365 * u.despacho * 0.01f / session.premissas.cotacaoDolar);
            }
            else
            {
                foreach (TerminalRegas term in session.terminaisRegasColocados)
                {
                    if (term.usina == u.nome)
                    {
                        termRegasUtilizados.Add(term);
                        foreach (Frota f in session.frotasColocadas)
                        {
                            if (f.terminalRegasDestino == term.nome)
                            {
                                f.fretePorKm = session.premissas.frete;

                                f.Recalculate(session.premissas, u);
                                frotasUtilizadas.Add(f);

                                foreach (FSU fsu in session.fsuColocados)
                                {
                                    if (!string.IsNullOrEmpty(fsu.terminalCargaDestino))
                                    {
                                        if (fsu.terminalGnl == f.terminalGnlOrigem)
                                        {
                                            if (!fsuUtilizados.Contains(fsu))
                                            {
                                                fsuUtilizados.Add(fsu);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                opexMalha = 0;
            }

            totalValue += opexMalha;
        }

        foreach (TerminalRegas t in termRegasUtilizados)
        {
            Custo capex = new Custo();
            Custo opex = new Custo();
            foreach (Custo c in t.custos)
            {
                if (c.tipo.Contains("capex"))
                {
                    capex = c;
                }
                else
                {
                    opex = c;
                }
            }
            if (t.custoUsado.Contains("capex"))
            {
                if (capex.valor > 0)
                {
                    totalValue += PMT(capex.valor);
                }
            }
            else
            {
                totalValue += opex.valor;
            }
        }

        foreach (FSU f in fsuUtilizados)
        {
            Custo capex = new Custo();
            Custo opex = new Custo();
            foreach (Custo c in f.custos)
            {
                if (!string.IsNullOrEmpty(c.tipo))
                {
                    if (c.tipo.Contains("capex"))
                        capex = c;
                    else
                        opex = c;

                }
            }
            if (f.custoUsado.Contains("capex"))
            {
                if (capex.valor > 0)
                {
                    totalValue += PMT(capex.valor);
                }
            }
            else
            {
                if (opex.valor > 0)
                {
                    totalValue += opex.valor;
                }
            }

            foreach (GasodutoCriogenico g in session.gasCrioColocados)
            {
                if (g.fsuOrigem == f.nome)
                {
                    if (!gasCrioUtilizados.Contains(g))
                        gasCrioUtilizados.Add(g);
                }
            }
            _gasFornecido += f.gnlFornecido;
        }

        if (_gasDemandado > _gasFornecido)
        {
            faltaGas = true;
        }
        else
        {
            faltaGas = false;
        }
        foreach (GasodutoCriogenico gas in gasCrioUtilizados)
        {
            foreach (Custo c in gas.custos)
            {
                if (c.tipo.Contains("opex"))
                {
                    totalValue += c.valor;
                }
            }
        }

        foreach (Frota frota in frotasUtilizadas)
        {
            foreach (Custo c in frota.custos)
            {
                if (c.tipo.Contains("opex"))
                {
                    totalValue += c.valor;
                }
            }
        }

        foreach (GasodutoDistribuicao gas in gasDistUtilizados)
        {
            totalValue += gas.opex.valor;
        }

        totalValueText.text = String.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:N}", (double)totalValue);

        if (totalValueText.text == "NaN" || totalValueText.text == "Infinity")
        {
            totalValueText.text = "Faltando valores";
        }
        else if (totalValue == 0)
        {
            totalValueText.text = "0,00";
        }
    }

    public void ShowSavedSessions()
    {
        DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath);
        FileInfo[] files = dir.GetFiles();
        foreach (Button btn in sessionButtonsParent.GetComponentsInChildren<Button>())
            Destroy(btn.gameObject);
        bool hasSimGas = false;
        bool hasSimFro = false;
        foreach (FileInfo file in files)
        {
            if (file.Name.Contains(".json") && !file.Name.Contains(".meta"))
            {
                try
                {
                    string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, file.Name));
                    Session s = JsonUtility.FromJson<Session>(json);
                    if (!string.IsNullOrEmpty(s.nome))
                    {
                        GameObject buttonInstance = Instantiate(sessionButtonPrefab, sessionButtonsParent);
                        buttonInstance.GetComponentInChildren<Text>().text = s.nome;
                        buttonInstance.GetComponent<SessionStorage>().session = s;
                    }
                    if (file.Name.Contains(simulacaoGasodutoFile.name))
                        hasSimGas = true;
                    if (file.Name.Contains(simulacaoFrotaFile.name))
                        hasSimFro = true;
                }
                catch (Exception e)
                {

                    print(e.ToString());

                }
            }
        }

        if (!hasSimGas)
        {
            simulacaoGasoduto = JsonUtility.FromJson<Session>(simulacaoGasodutoFile.text);
            if (!string.IsNullOrEmpty(simulacaoGasoduto.nome))
            {
                GameObject buttonInstance = Instantiate(sessionButtonPrefab, sessionButtonsParent);
                buttonInstance.GetComponentInChildren<Text>().text = simulacaoGasoduto.nome;
                buttonInstance.GetComponent<SessionStorage>().session = simulacaoGasoduto;
                buttonInstance.GetComponent<SessionStorage>().isOnResources = true;
            }
        }

        if (!hasSimFro)
        {
            simulacaoFrota = JsonUtility.FromJson<Session>(simulacaoFrotaFile.text);
            if (!string.IsNullOrEmpty(simulacaoFrota.nome))
            {
                GameObject buttonInstance = Instantiate(sessionButtonPrefab, sessionButtonsParent);
                buttonInstance.GetComponentInChildren<Text>().text = simulacaoFrota.nome;
                buttonInstance.GetComponent<SessionStorage>().session = simulacaoFrota;
                buttonInstance.GetComponent<SessionStorage>().isOnResources = true;
            }
        }

        if (!string.IsNullOrEmpty(session.nome))
        {
            foreach (Button btn in sessionButtonsParent.GetComponentsInChildren<Button>())
            {
                btn.interactable = btn.GetComponent<SessionStorage>().session.nome != session.nome;
            }
            sessionName.text = session.nome;
        }
        else
        {
            sessionName.text = "Sessão não salva";
        }
    }

    public void LoadSession()
    {
        CreateNewSession();

        foreach (Button b in sessionButtonsParent.GetComponentsInChildren<Button>())
        {
            b.interactable = b.GetComponent<SessionStorage>().session != sessionToLoad;
            if (b.interactable)
            {
                deleteBtn.SetActive(!b.GetComponent<SessionStorage>().isOnResources);
                renameBtn.SetActive(!b.GetComponent<SessionStorage>().isOnResources);
            }
        }

        session = sessionToLoad;

        setupPanel.SetSessionPremiseFields();
        presetsManager.SetPresetsFields();

        markerDisplacer.markers.Clear();

        foreach (Usina u in session.usinasColocadas)
        {
            gasClasses.LoadUsinaMarkers(u);
        }

        foreach (TerminalGNL t in session.terminaisGNLColocados)
        {
            gasClasses.LoadGNLMarkers(t);
        }

        foreach (FSRU f in session.fsruColocados)
        {
            gasClasses.LoadFSRUMarkers(f);
        }

        foreach (TerminalRegas t in session.terminaisRegasColocados)
        {
            gasClasses.LoadTermRegasMarkers(t);
        }

        foreach (SubestacaoEnergia s in session.subestacoesColocadas)
        {
            gasClasses.LoadSubestMarkers(s);
        }

        //foreach (PontoDeEntrega p in session.pontosDeEntrega)
        //{
        //    gasClasses.LoadPEMarkers(p);
        //}

        foreach (PontoDeEntrega pe in peFixos)
        {
            foreach (PontoDeEntrega _pe in session.pontosDeEntrega)
            {
                if (pe.nome == _pe.nome)

                {
                    pe.valorMalha = _pe.valorMalha;

                    for (int x = 0; x < pe.coluna_de.Length; x++)
                    {
                        pe.coluna_de[x] = _pe.coluna_de[x];
                        pe.coluna_ate[x] = _pe.coluna_ate[x];
                        pe.coluna_prc[x] = _pe.coluna_prc[x];
                        pe.coluna_puc[x] = _pe.coluna_puc[x];
                    }
                }
            }
        }

        foreach (Frota f in session.frotasColocadas)
        {
            GameObject frotaObj = gasClasses.LoadFrotaMarkers(f);
            GameObject termRegasObj = new GameObject();

            if (frotaObj != null)
            {
                foreach (OnlineMapsMarker3D m in markerDisplacer.markers)
                {
                    if (m.instance.GetComponent<TerminalRegasStorage>() != null)
                    {
                        if (f.terminalRegasDestino == m.instance.GetComponent<TerminalRegasStorage>().terminal.nome)
                        {
                            termRegasObj = m.instance;
                            connCreator.ConectarFrotaComTermRegas(frotaObj, termRegasObj, true, f);
                        }
                    }

                }
            }
        }

        foreach (FSU f in session.fsuColocados)
        {
            gasClasses.LoadFSUMarkers(f);
        }

        foreach (GasodutoCriogenico g in session.gasCrioColocados)
        {
            GameObject fsuObj = new GameObject();
            GameObject terminalObj = new GameObject();
            foreach (OnlineMapsMarker3D m in markerDisplacer.markers)
            {
                if (m.instance.GetComponent<FSUStorage>() != null)
                {
                    if (m.instance.GetComponent<FSUStorage>().fsu.nome == g.fsuOrigem)
                    {
                        fsuObj = m.instance;
                    }
                }
            }
            if (!string.IsNullOrEmpty(g.terminalCargaDestino))
            {
                foreach (OnlineMapsMarker3D m in markerDisplacer.markers)
                {
                    if (m.instance.GetComponent<FrotaStorage>() != null)
                    {
                        if (m.instance.GetComponent<FrotaStorage>().frotas.Count > 0)
                        {
                            bool hasMarker = false;
                            foreach (Frota f in m.instance.GetComponent<FrotaStorage>().frotas)
                            {
                                if (g.terminalCargaDestino.Contains(f.terminalGnlOrigem) || (f.terminalGnlOrigem == g.terminalCargaDestino))
                                {
                                    hasMarker = true;
                                }
                            }
                            if (hasMarker)
                                terminalObj = m.instance;

                        }
                    }
                }
            }
            else if (!string.IsNullOrEmpty(g.terminalRegasDestino))
            {
                foreach (OnlineMapsMarker3D m in markerDisplacer.allInstantiatedMarkers)
                {
                    if (m.instance.GetComponent<TerminalRegasStorage>() != null)
                    {
                        if (m.instance.GetComponent<TerminalRegasStorage>().terminal.nome == g.terminalRegasDestino)
                        {
                            terminalObj = m.instance;
                        }
                    }
                }
            }

            connCreator.ConectarFSUComTerminais(fsuObj, terminalObj, true, g);
        }

        foreach (GasodutoIntegrador g in session.gasIntColocados)
        {
            GameObject origem = new GameObject();
            GameObject peObj = new GameObject();

            if (!string.IsNullOrEmpty(g.peDestino))
            {
                foreach (OnlineMapsMarker3D m in markerDisplacer.allInstantiatedMarkers)
                {
                    if (m.instance.GetComponent<PEStorage>() != null)
                    {
                        if (m.instance.GetComponent<PEStorage>().pe.nome == g.peDestino)
                        {
                            peObj = m.instance;
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(g.fsruOrigem))
            {
                foreach (OnlineMapsMarker3D m in markerDisplacer.allInstantiatedMarkers)
                {
                    if (m.instance.GetComponent<FSRUStorage>() != null)
                    {
                        if (m.instance.GetComponent<FSRUStorage>().fsru.nome == g.fsruOrigem)
                        {
                            origem = m.instance;
                        }
                    }
                }
                connCreator.ConectarFSRUComPEProximo(origem, false, peObj, g);
            }
            else if (!string.IsNullOrEmpty(g.terminalRegasOrigem))
            {
                foreach (OnlineMapsMarker3D m in markerDisplacer.allInstantiatedMarkers)
                {
                    if (m.instance.GetComponent<TerminalRegasStorage>() != null)
                    {
                        if (m.instance.GetComponent<TerminalRegasStorage>().terminal.nome == g.terminalRegasOrigem)
                        {
                            origem = m.instance;
                        }
                    }
                }
                connCreator.ConectarTermRegasComPEProximo(origem, peObj, g);
            }
        }

        // Inclusão do SetState para tirar line renderer
        foreach (GasodutoDistribuicao g in session.gasDistColocados)
        {
            GameObject usinaObj = new GameObject();
            GameObject peObj = new GameObject();

            foreach (OnlineMapsMarker3D m in markerDisplacer.allInstantiatedMarkers)
            {
                if (m.instance.GetComponent<UsinaStorage>() != null)
                {
                    if (m.instance.GetComponent<UsinaStorage>().usina.nome == g.usinaDestino)
                    {
                        usinaObj = m.instance;
                    }
                }
                else if (m.instance.GetComponent<PEStorage>() != null)
                {
                    if (m.instance.GetComponent<PEStorage>().pe.nome == g.peOrigem)
                    {
                        peObj = m.instance;
                    }
                }
            }

            connCreator.ConectarUsinaComPEProximo(usinaObj, false, peObj, g);

            //Debug.Log("Session Calls SetState UsinaStorage");
            usinaObj.GetComponent<UsinaStorage>().SetState();
        }

        UpdateSession();

        ShowSavedSessions();
    }

    public void ShowSessionFieldPanel()
    {
        sessionNameField.text = session.nome;
        sessionNameFieldAnim.gameObject.SetActive(true);
        //sessionNameFieldAnim.SetBool("hide", false);
    }

    public void ShowRenameSessionPanel()
    {
        sessionRenameField.text = ""; //session.nome;
        sessionRenameFieldAnim.gameObject.SetActive(true);
        //sessionRenameFieldAnim.SetBool("hide", false);
    }

    public void RenameSession()
    {
        if (sessionRenameFieldAnim.gameObject.activeSelf)
        {
            DeleteSessionFile();
            session.nome = sessionRenameField.text;
            string sessionJson = JsonUtility.ToJson(session);
            string path = Path.Combine(Application.persistentDataPath, sessionRenameField.text + ".json");
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            File.WriteAllText(path, sessionJson);

            ShowSavedSessions();
            sessionRenameFieldAnim.gameObject.SetActive(false);
            //sessionRenameFieldAnim.SetBool("hide", true);

            ShowSavedSessions();
        }

    }

    public void CheckIfWantToSaveChangesBeforeClear()
    {
        bool canSaveChanges = false;
        if (session.usinasColocadas.Count > 0 ||
            session.fsruColocados.Count > 0 ||
            session.fsuColocados.Count > 0 ||
            session.terminaisRegasColocados.Count > 0 ||
            session.frotasColocadas.Count > 0)
        {
            canSaveChanges = true;
        }
        if (!string.IsNullOrEmpty(session.nome) || canSaveChanges)
        {
            saveOpenSessionBeforeClearAnim.gameObject.SetActive(true);
            //saveOpenSessionAnim.SetBool("show", true);
        }
        else
        {
            CreateNewSession();
        }
    }

    public void CheckIfWantToSaveChangesBeforeLoad(GameObject buttonPressed)
    {
        foreach (Button b in sessionButtonsParent.GetComponentsInChildren<Button>())
        {
            b.interactable = b.gameObject != buttonPressed;
        }

        sessionToLoad = buttonPressed.GetComponent<SessionStorage>().session;

        //setupPanel.SetSessionPremiseFields();
        //presetsManager.SetPresetsFields();

        bool canSaveChanges = false;

        if (session.usinasColocadas.Count > 0 ||
            session.fsuColocados.Count > 0 ||
            session.fsuColocados.Count > 0 ||
            session.terminaisRegasColocados.Count > 0 ||
            session.frotasColocadas.Count > 0)
        {
            canSaveChanges = true;
        }
        if (!string.IsNullOrEmpty(session.nome) || canSaveChanges)
        {
            saveOpenSessionBeforeLoadAnim.gameObject.SetActive(true);
            //saveOpenSessionAnim.SetBool("show", true);
        }
        else
        {
            LoadSession();
        }
    }

    public void SaveChanges(bool isLoadingNewSession)
    {
        if (!string.IsNullOrEmpty(session.nome))
        {
            string sessionJson = JsonUtility.ToJson(session);
            string path = Path.Combine(Application.persistentDataPath, session.nome + ".json");

            if (File.Exists(path))
            {
                File.Delete(path);
            }
            File.WriteAllText(path, sessionJson);

            if (isLoadingNewSession)
                LoadSession();
            else
                CreateNewSession();
        }
        else
        {
            createNewSession = !isLoadingNewSession;
            loadNewSession = isLoadingNewSession;
            sessionNameFieldAnim.gameObject.SetActive(true);
        }

        saveOpenSessionBeforeClearAnim.gameObject.SetActive(false);
        saveOpenSessionBeforeLoadAnim.gameObject.SetActive(false);

        ShowSavedSessions();
    }

    public void DontSaveChanges(bool isLoadingNewSession)
    {
        if (isLoadingNewSession)
        {
            saveOpenSessionBeforeLoadAnim.gameObject.SetActive(false);
            LoadSession();
        }
        else
        {
            saveOpenSessionBeforeClearAnim.gameObject.SetActive(false);
            CreateNewSession();
        }

        ShowSavedSessions();
        //saveOpenSessionAnim.SetBool("show", false);

    }

    public void CancelSaveChanges()
    {
        saveOpenSessionBeforeClearAnim.gameObject.SetActive(false);
        saveOpenSessionBeforeLoadAnim.gameObject.SetActive(false);
        //saveOpenSessionAnim.SetBool("show", false);
    }

    public void CreateNewSession()
    {

        foreach (OnlineMapsMarker3D m in markerDisplacer.markers)
        {
            markerDisplacer.allInstantiatedMarkers.Remove(m);
            if (m.instance.GetComponent<TerminalRegasStorage>() != null)
            {
                if (m.instance.GetComponent<TerminalRegasStorage>().gasIntPipe != null)
                    Destroy(m.instance.GetComponent<TerminalRegasStorage>().gasIntPipe);
            }
            if (m.instance.GetComponent<FSUStorage>() != null)
            {
                if (m.instance.GetComponent<FSUStorage>().gasCrioPipe != null)
                    Destroy(m.instance.GetComponent<FSUStorage>().gasCrioPipe);
            }
            if (m.instance.GetComponent<FSRUStorage>() != null)
            {
                if (m.instance.GetComponent<FSRUStorage>().gasIntPipe != null)
                    Destroy(m.instance.GetComponent<FSRUStorage>().gasIntPipe);
            }
            if (m.instance.GetComponent<UsinaStorage>() != null)
            {
                if (m.instance.GetComponent<UsinaStorage>().gasDistPipe != null)
                    Destroy(m.instance.GetComponent<UsinaStorage>().gasDistPipe);
            }
            OnlineMapsDrawingElementManager.RemoveAllItems();
            markerManager.Remove(m);
        }
        markerDisplacer.markers.Clear();
        totalValue = 0;
        totalValueText.text = "";
        session = new Session();

        ShowSavedSessions();

    }

    public void SaveSession()
    {
        if (sessionNameFieldAnim.gameObject.activeSelf)
        {
            session.pontosDeEntrega.Clear();

            foreach (PontoDeEntrega pe in peFixos)
                session.pontosDeEntrega.Add(pe);

            if (!string.IsNullOrEmpty(sessionNameField.text) && !string.IsNullOrWhiteSpace(sessionNameField.text))
            {
                session.nome = sessionNameField.text;
                string sessionJson = JsonUtility.ToJson(session);
                string path = Path.Combine(Application.persistentDataPath, sessionNameField.text + ".json");

                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                File.WriteAllText(path, sessionJson);

                sessionNameFieldAnim.gameObject.SetActive(false);
                // sessionNameFieldAnim.SetBool("hide", true);
            }

            if (createNewSession)
            {
                CreateNewSession();
                createNewSession = false;
            }
            else if (loadNewSession)
            {
                LoadSession();
                loadNewSession = false;
            }
            else
            {
                ShowSavedSessions();
            }


            osk.SetActive(false);
        }
    }

    public void CheckIfWantToDelete()
    {
        deleteOpenSessionAnim.gameObject.SetActive(true);
        //deleteOpenSessionAnim.SetBool("show", true);

    }

    public void DeleteSession()
    {
        DeleteSessionFile();

        ShowSavedSessions();
        CreateNewSession();

        deleteOpenSessionAnim.gameObject.SetActive(false);
        //deleteOpenSessionAnim.SetBool("show", false);

        foreach (Button b in sessionButtonsParent.GetComponentsInChildren<Button>())
        {
            b.interactable = true;
        }
    }

    void DeleteSessionFile()
    {
        string path = Path.Combine(Application.persistentDataPath, session.nome + ".json");
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    public void CancelDeleteSession()
    {
        deleteOpenSessionAnim.gameObject.SetActive(false);
        //deleteOpenSessionAnim.SetBool("show", false);
    }


    public double GetMalhaValue(Usina u)
    {
        double tarifaMalha;

        if (u.malha.Contains("Nordeste") || u.malha.Contains("nordeste") || u.malha.Contains("NORDESTE"))
        {
            tarifaMalha = 3.88327f;
        }
        else if (u.malha.Contains(" Sul") || u.malha.Contains(" sul") || u.malha.Contains(" sul"))
        {
            tarifaMalha = 3.66930f;
        }
        else if (u.malha.Contains("Norte") || u.malha.Contains("norte") || u.malha.Contains("NORTE"))
        {
            tarifaMalha = 5.38660f;
        }
        else if (u.malha == "Pilar-Ipojuca")
        {
            tarifaMalha = 1.53300f;
        }
        else
        {
            tarifaMalha = 2.894434f;
        }

        return tarifaMalha;
    }
}
