using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class IconPanelElements : MonoBehaviour
{

    public UIManager uiManager;

    public SessionManager sessionManager;
    public MarkerDisplacer markerDisplacer;

    public UsinaStorage usinaStorageToSet;

    public FSRUStorage fsruStorageToSet;

    public FSUStorage fsuStorageToSet;

    public FrotaStorage frotaStorageToSet;

    public TerminalRegasStorage termRegasStorageToSet;

    public Text nameText;
    public Transform dataParent;
    public Button deleteButton;
    public OnScreenKeyboard osk;

    public GameObject numberFieldPrefab;
    public GameObject textFieldPrefab;
    public GameObject numberFieldWithLockerPrefab;

    //INPUT FIELDS DA USINA:
    GameObject _potencia;
    GameObject _demandaDiaM3;
    public GameObject _eficiencia;
    public GameObject _despacho;
    GameObject _opexFrota;
    GameObject _opexGasDist;
    GameObject _opexGasTrans;
    string oldUsinaName;

    GameObject _opex;
    GameObject _capex;

    //GameObject _valorMalha;

    //INPUT FIELDS DO FSRU 
    GameObject _gasFornecido; //FSU TAMBÉM
    GameObject _gasIntDiam;
    public GameObject _gasIntCusto;
    GameObject _opexGasInt;

    //INPUT FIELDS DO FSU
    GameObject _gasCrioDiam;
    public GameObject _gasCrioCusto;
    GameObject _opexGasCrio;

    //INPUT FIELDS DA FROTA
    public GameObject _capacidadeLiq;
    public bool isOpen;

    public bool isCapex;
    public GameObject paymentPanel;
    public Toggle capexToggle;
    public Toggle opexToggle;

    public GameObject usinasPanelBtn;
    public GameObject usinaTogglePrefab;
    public GameObject usinaButtonPrefab;
    public Transform usinaButtonsParent;
    public GameObject usinasPanel;


    public bool isFrota;
    public GameObject gasSourcePanel;
    public Toggle gasodutoToggle;
    public Toggle frotaToggle;

    public Transform errorLogParent;
    public GameObject errorPrefab;
    List<string> errors = new List<string>();
    List<string> eList = new List<string>();

    public GameObject renameBtn, saveNameBtn, nameField;

    private GameObject _gasIntComp;
    private GameObject _gasCrioComp;
    private GameObject _distanciaFrota;
    private GameObject _recargasFrota;
    private Frota frotaToSetDist;

    private void OnEnable()
    {
        usinasPanel.SetActive(false);
    }

    private void Update()
    {
        //SetEmptyInputFields();

        eList.Clear();

        if (fsruStorageToSet != null)
        {
            if (!string.IsNullOrEmpty(fsruStorageToSet.errorMessage))
            {
                string[] _errors = fsruStorageToSet.errorMessage.Split('*');


                foreach (string s in _errors)
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        eList.Add(s);
                        if (!errors.Contains(s))
                            errors.Add(s);
                    }
                }
            }
            else
            {
                eList.Clear();
                errors.Clear();
            }


            //errorLog.text = fsruStorageToSet.errorMessage;
            double capex;
            double opex;
            double gnlFornecido;
            if (string.IsNullOrEmpty(_capex.GetComponentInChildren<InputField>().text))
            {
                capex = 0;
            }
            else
            {
                capex = double.Parse(_capex.GetComponentInChildren<InputField>().text);
            }


            if (string.IsNullOrEmpty(_opex.GetComponentInChildren<InputField>().text))
            {
                opex = 0;
            }
            else
            {
                opex = double.Parse(_opex.GetComponentInChildren<InputField>().text);
            }


            if (string.IsNullOrEmpty(_gasFornecido.GetComponentInChildren<InputField>().text))
            {
                gnlFornecido = 0;
            }
            else
            {
                gnlFornecido = double.Parse(_gasFornecido.GetComponentInChildren<InputField>().text);
            }

            fsruStorageToSet.fsru.SetCusto("capex", capex, "dolar");


            fsruStorageToSet.fsru.SetCusto("opex", opex, "dolar");


            fsruStorageToSet.fsru.gnlFornecido = gnlFornecido;


            if (fsruStorageToSet.gasInt != null)
            {
                double gasIntCusto;
                double gasIntDiam;
                double gasIntComp;
                double opexGas = 0;
                if (string.IsNullOrEmpty(_gasIntCusto.GetComponentInChildren<InputField>().text))
                {
                    gasIntCusto = 0;
                }
                else
                {
                    gasIntCusto = double.Parse(_gasIntCusto.GetComponentInChildren<InputField>().text);
                }


                if (string.IsNullOrEmpty(_gasIntDiam.GetComponentInChildren<InputField>().text))
                {
                    gasIntDiam = 0;
                }
                else
                {
                    gasIntDiam = double.Parse(_gasIntDiam.GetComponentInChildren<InputField>().text);
                }

                if (string.IsNullOrEmpty(_gasIntComp.GetComponentInChildren<InputField>().text))
                {
                    gasIntComp = 0;
                }
                else
                {
                    gasIntComp = double.Parse(_gasIntComp.GetComponentInChildren<InputField>().text);
                }
                if (gasIntComp != 0)
                    fsruStorageToSet.gasInt.comprimento = gasIntComp;

                if (_opexGasInt.GetComponent<InputFieldBehaviour>().enableCalculation)
                {
                    fsruStorageToSet.gasIntUsingCalculation = true;
                    if (gasIntComp != 0)
                        fsruStorageToSet.gasInt.comprimento = gasIntComp;

                    fsruStorageToSet.gasInt.Set(gasIntCusto, gasIntDiam, sessionManager.session.premissas);
                    fsruStorageToSet.gasInt.Recalculate(sessionManager.session.premissas);

                    foreach (Custo c in fsruStorageToSet.gasInt.custos)
                    {
                        if (c.tipo == "opex")
                        {
                            opexGas = c.valor;
                        }
                    }

                    _opexGasInt.GetComponent<InputFieldBehaviour>().input.text = opexGas.ToString("F2");
                }
                else
                {
                    fsruStorageToSet.gasIntUsingCalculation = false;
                    if (string.IsNullOrWhiteSpace(_opexGasInt.GetComponent<InputFieldBehaviour>().input.text))
                    {
                        opexGas = 0;
                    }
                    else
                    {
                        opexGas = double.Parse(_opexGasInt.GetComponent<InputFieldBehaviour>().input.text);
                    }
                    if (opexGas != 0)
                    {
                        foreach (Custo c in fsruStorageToSet.gasInt.custos)
                        {
                            if (c.tipo == "opex")
                            {
                                c.valor = opexGas;
                            }
                        }
                    }
                }
            }


            if (isCapex)
                fsruStorageToSet.fsru.custoUsado = "capex";
            else
                fsruStorageToSet.fsru.custoUsado = "opex";
        }

        else if (usinaStorageToSet != null)
        {
            if (!string.IsNullOrEmpty(usinaStorageToSet.errorMessage))
            {
                string[] _errors = usinaStorageToSet.errorMessage.Split('*');
                foreach (string s in _errors)
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        eList.Add(s);
                        if (!errors.Contains(s))
                            errors.Add(s);
                    }
                }
            }
            else
            {
                eList.Clear();
                errors.Clear();
            }
            //errorLog.text = usinaStorageToSet.errorMessage;

            double potencia;
            double eficiencia;
            double despacho;
            double distancia;
            //double valorMalha;


            if (string.IsNullOrEmpty(_potencia.GetComponentInChildren<InputField>().text))
            {
                potencia = 0;
            }
            else
            {
                potencia = double.Parse(_potencia.GetComponentInChildren<InputField>().text);
            }


            if (string.IsNullOrEmpty(_eficiencia.GetComponentInChildren<InputField>().text))
            {
                eficiencia = 0;
            }
            else
            {
                eficiencia = double.Parse(_eficiencia.GetComponentInChildren<InputField>().text);
            }


            if (string.IsNullOrEmpty(_despacho.GetComponentInChildren<InputField>().text))
            {
                despacho = 0;
            }
            else
            {
                despacho = double.Parse(_despacho.GetComponentInChildren<InputField>().text);
            }

            //if (string.IsNullOrEmpty(_valorMalha.GetComponentInChildren<InputField>().text))
            //{
            //    valorMalha = sessionManager.GetMalhaValue(usinaStorageToSet.usina);
            //}
            //else
            //{
            //    valorMalha = double.Parse(_valorMalha.GetComponentInChildren<InputField>().text);
            //}

            _opexGasDist.GetComponent<InputFieldBehaviour>().input.text = usinaStorageToSet.gasDist.opex.valor.ToString("F2");


            //double custoDiario;
            //double opexMalha;

            //custoDiario = usinaStorageToSet.usina.valorMalha * usinaStorageToSet.usina.demandaDiaPci;
            //opexMalha = custoDiario * (365 * usinaStorageToSet.usina.despacho * 0.01f / sessionManager.session.premissas.cotacaoDolar);

            //_opexGasTrans.GetComponent<InputFieldBehaviour>().input.text = opexMalha.ToString("F2");

            if (_distanciaFrota != null)
            {
                if (string.IsNullOrEmpty(_distanciaFrota.GetComponentInChildren<InputField>().text))
                {
                    distancia = 0;
                }
                else
                {
                    distancia = double.Parse(_distanciaFrota.GetComponentInChildren<InputField>().text);
                }

                if (frotaToSetDist != null)
                {
                    if (distancia != 0)
                    {
                        frotaToSetDist.distancia = distancia;
                        frotaToSetDist.Recalculate(sessionManager.session.premissas, usinaStorageToSet.usina);
                    }

                    double opexFrota = 0;

                    foreach (Custo c in frotaToSetDist.custos)
                    {
                        if (c.tipo == "opex")
                            opexFrota = c.valor;
                    }

                    _opexFrota.GetComponent<InputFieldBehaviour>().input.text = opexFrota.ToString("F2");
                    _recargasFrota.GetComponent<InputFieldBehaviour>().input.text = frotaToSetDist.recargasDia.ToString("F2");

                    if (usinaStorageToSet.usina.fonteDoGas != "frota")
                    {
                        _opexFrota.GetComponent<InputFieldBehaviour>().BG.color = new Color(0.6f, 0.6f, 0.7f);
                        _opexGasDist.GetComponent<InputFieldBehaviour>().BG.color = new Color(1f, 1f, 1f);
                        _opexGasTrans.GetComponent<InputFieldBehaviour>().BG.color = new Color(1f, 1f, 1f);
                    }
                    else
                    {
                        _opexFrota.GetComponent<InputFieldBehaviour>().BG.color = new Color(1f, 1f, 1f);
                        _opexGasDist.GetComponent<InputFieldBehaviour>().BG.color = new Color(0.6f, 0.6f, 0.7f);
                        _opexGasTrans.GetComponent<InputFieldBehaviour>().BG.color = new Color(0.6f, 0.6f, 0.7f);
                    }
                }
            }
            else
            {
                _opexGasDist.GetComponent<InputFieldBehaviour>().BG.color = new Color(1f, 1f, 1f);
                _opexGasTrans.GetComponent<InputFieldBehaviour>().BG.color = new Color(1f, 1f, 1f);
            }


            usinaStorageToSet.usina.SetDemandas(potencia, eficiencia, sessionManager.session.gas, sessionManager.session.premissas, despacho);

            PontoDeEntrega pe = new PontoDeEntrega();
            foreach(PontoDeEntrega _pe in sessionManager.peFixos)
            {
                if (_pe.nome == usinaStorageToSet.usina.malha)
                    pe = _pe;
            }
            usinaStorageToSet.gasDist.Set(usinaStorageToSet.usina, pe, sessionManager.session.gas, sessionManager.session.premissas);

            //usinaStorageToSet.usina.valorMalha = valorMalha;

            _demandaDiaM3.GetComponent<InputFieldBehaviour>().input.text = usinaStorageToSet.usina.demandaDiaM3.ToString("F2");

            if (isFrota)
            {
                usinaStorageToSet.usina.fonteDoGas = "frota";
            }
            else
            {
                usinaStorageToSet.usina.fonteDoGas = "gasoduto";
            }


        }
        else if (fsuStorageToSet != null)
        {
            if (!string.IsNullOrEmpty(fsuStorageToSet.errorMessage))
            {
                string[] _errors = fsuStorageToSet.errorMessage.Split('*');


                foreach (string s in _errors)
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        eList.Add(s);
                        if (!errors.Contains(s))
                            errors.Add(s);
                    }
                }
            }
            else
            {
                eList.Clear();
                errors.Clear();
            }
            double capex;
            double opex;
            double gnlFornecido;
            if (string.IsNullOrEmpty(_capex.GetComponentInChildren<InputField>().text))
            {
                capex = 0;
            }
            else
            {
                capex = double.Parse(_capex.GetComponentInChildren<InputField>().text);
            }
            if (string.IsNullOrEmpty(_opex.GetComponentInChildren<InputField>().text))
            {
                opex = 0;
            }
            else
            {
                opex = double.Parse(_opex.GetComponentInChildren<InputField>().text);
            }
            if (string.IsNullOrEmpty(_gasFornecido.GetComponentInChildren<InputField>().text))
            {
                gnlFornecido = 0;
            }
            else
            {
                gnlFornecido = double.Parse(_gasFornecido.GetComponentInChildren<InputField>().text);
            }
            fsuStorageToSet.fsu.SetCusto("capex", capex, "dolar");
            fsuStorageToSet.fsu.SetCusto("opex", opex, "dolar");
            fsuStorageToSet.fsu.gnlFornecido = gnlFornecido;

            if (fsuStorageToSet.gasCrio != null)
            {
                double gasCrioCusto;
                double gasCrioDiam;
                double gasCrioComp;
                double opexGas = 0;
                if (string.IsNullOrEmpty(_gasCrioCusto.GetComponentInChildren<InputField>().text))
                {
                    gasCrioCusto = 0;
                }
                else
                {
                    gasCrioCusto = double.Parse(_gasCrioCusto.GetComponentInChildren<InputField>().text);
                }
                if (string.IsNullOrEmpty(_gasCrioDiam.GetComponentInChildren<InputField>().text))
                {
                    gasCrioDiam = 0;
                }
                else
                {
                    gasCrioDiam = double.Parse(_gasCrioDiam.GetComponentInChildren<InputField>().text);
                }
                if (string.IsNullOrEmpty(_gasCrioComp.GetComponentInChildren<InputField>().text))
                {
                    gasCrioComp = 0;
                }
                else
                {
                    gasCrioComp = double.Parse(_gasCrioComp.GetComponentInChildren<InputField>().text);
                }

                if (_opexGasCrio.GetComponent<InputFieldBehaviour>().enableCalculation)
                {
                    fsuStorageToSet.gasCrioUsingCalculation = true;
                    if (gasCrioComp != 0)
                        fsuStorageToSet.gasCrio.comprimento = gasCrioComp;
                    fsuStorageToSet.gasCrio.Set(gasCrioCusto, gasCrioDiam, sessionManager.session.premissas);
                    fsuStorageToSet.gasCrio.Recalculate(sessionManager.session.premissas);
                    foreach (Custo c in fsuStorageToSet.gasCrio.custos)
                    {
                        if (c.tipo == "opex")
                        {
                            opexGas = c.valor;
                        }
                    }
                    _opexGasCrio.GetComponent<InputFieldBehaviour>().input.text = opexGas.ToString("F2");
                }
                else
                {
                    fsuStorageToSet.gasCrioUsingCalculation = false;
                    if (string.IsNullOrWhiteSpace(_opexGasCrio.GetComponent<InputFieldBehaviour>().input.text))
                    {
                        opexGas = 0;
                    }
                    else
                    {
                        opexGas = double.Parse(_opexGasCrio.GetComponent<InputFieldBehaviour>().input.text);
                    }
                    if (opexGas != 0)
                    {
                        foreach (Custo c in fsuStorageToSet.gasCrio.custos)
                        {
                            if (c.tipo == "opex")
                            {
                                c.valor = opexGas;
                            }
                        }
                    }
                }
            }
            if (isCapex)
                fsuStorageToSet.fsu.custoUsado = "capex";
            else
                fsuStorageToSet.fsu.custoUsado = "opex";

        }
        else if (frotaStorageToSet != null)
        {
            double capacidadeLiq;
            if (string.IsNullOrEmpty(_capacidadeLiq.GetComponentInChildren<InputField>().text))
                capacidadeLiq = 0;
            else
                capacidadeLiq = double.Parse(_capacidadeLiq.GetComponentInChildren<InputField>().text);

            frotaStorageToSet.capacidadeLiq = capacidadeLiq;

            if (!string.IsNullOrEmpty(frotaStorageToSet.errorMessage))
            {
                string[] _errors = frotaStorageToSet.errorMessage.Split('*');


                foreach (string s in _errors)
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        eList.Add(s);
                        if (!errors.Contains(s))
                            errors.Add(s);
                    }
                }
            }
            else
            {
                eList.Clear();
                errors.Clear();
            }
            //errorLog.text = frotaStorageToSet.errorMessage;

            foreach (Frota f in frotaStorageToSet.frotas)
            {
                Usina usina = new Usina();
                foreach (OnlineMapsMarker3D m in markerDisplacer.markers)
                {
                    if (m.instance.GetComponent<TerminalRegasStorage>() != null)
                    {
                        if (m.instance.GetComponent<TerminalRegasStorage>().terminal.nome == f.terminalRegasDestino)
                        {
                            foreach (Usina u in sessionManager.session.usinasColocadas)
                            {
                                if (u.nome == m.instance.GetComponent<TerminalRegasStorage>().terminal.usina)
                                {
                                    usina = u;
                                }
                            }
                        }
                    }
                }
                f.Set(f.distancia, sessionManager.session.premissas.frete, frotaStorageToSet.capacidadeLiq,
                    sessionManager.session.gas, sessionManager.session.premissas, usina);
                f.Recalculate(sessionManager.session.premissas, usina);
            }

            //frotaStorageToSet.frota.SetCusto(frotaStorageToSet.frota.custos[0].recorrencia, double.Parse(_custo.GetComponentInChildren<InputField>().text), frotaStorageToSet.frota.custos[0].moeda);


        }
        else if (termRegasStorageToSet != null)
        {


            if (!string.IsNullOrEmpty(termRegasStorageToSet.errorMessage))
            {
                string[] _errors = termRegasStorageToSet.errorMessage.Split('*');


                foreach (string s in _errors)
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        eList.Add(s);
                        if (!errors.Contains(s))
                            errors.Add(s);
                    }
                }
            }
            else
            {
                eList.Clear();
                errors.Clear();
            }


            double capex;
            double opex;
            if (string.IsNullOrEmpty(_capex.GetComponentInChildren<InputField>().text))
            {
                capex = 0;
            }
            else
            {
                capex = double.Parse(_capex.GetComponentInChildren<InputField>().text);
            }


            if (string.IsNullOrEmpty(_opex.GetComponentInChildren<InputField>().text))
            {
                opex = 0;
            }
            else
            {
                opex = double.Parse(_opex.GetComponentInChildren<InputField>().text);
            }


            termRegasStorageToSet.terminal.SetCusto("capex", capex, "dolar");
            termRegasStorageToSet.terminal.SetCusto("opex", opex, "dolar");
            if (termRegasStorageToSet.gasInt != null)
            {
                if (!string.IsNullOrEmpty(termRegasStorageToSet.gasInt.nome))
                {
                    double gasIntCusto;
                    double gasIntDiam;
                    double gasIntComp;
                    double opexGas = 0;
                    if (string.IsNullOrEmpty(_gasIntCusto.GetComponentInChildren<InputField>().text))
                    {
                        gasIntCusto = 0;
                    }
                    else
                    {
                        gasIntCusto = double.Parse(_gasIntCusto.GetComponentInChildren<InputField>().text);
                    }

                    if (string.IsNullOrEmpty(_gasIntDiam.GetComponentInChildren<InputField>().text))
                    {
                        gasIntDiam = 0;
                    }
                    else
                    {
                        gasIntDiam = double.Parse(_gasIntDiam.GetComponentInChildren<InputField>().text);
                    }

                    if (string.IsNullOrEmpty(_gasIntComp.GetComponentInChildren<InputField>().text))
                    {
                        gasIntComp = 0;
                    }
                    else
                    {
                        gasIntComp = double.Parse(_gasIntComp.GetComponentInChildren<InputField>().text);
                    }

                    if (_opexGasInt.GetComponent<InputFieldBehaviour>().enableCalculation)
                    {
                        termRegasStorageToSet.gasIntUsingCalculation = true;
                        if (gasIntComp != 0)
                            termRegasStorageToSet.gasInt.comprimento = gasIntComp;

                        termRegasStorageToSet.gasInt.Set(gasIntCusto, gasIntDiam, sessionManager.session.premissas);
                        termRegasStorageToSet.gasInt.Recalculate(sessionManager.session.premissas);

                        foreach (Custo c in termRegasStorageToSet.gasInt.custos)
                        {
                            if (c.tipo == "opex")
                            {
                                opexGas = c.valor;
                            }
                        }

                        _opexGasInt.GetComponent<InputFieldBehaviour>().input.text = opexGas.ToString("F2");
                    }
                    else
                    {
                        termRegasStorageToSet.gasIntUsingCalculation = false;
                        if (string.IsNullOrWhiteSpace(_opexGasInt.GetComponent<InputFieldBehaviour>().input.text))
                        {
                            opexGas = 0;
                        }
                        else
                        {
                            opexGas = double.Parse(_opexGasInt.GetComponent<InputFieldBehaviour>().input.text);
                        }
                        if (opexGas != 0)
                        {
                            foreach (Custo c in termRegasStorageToSet.gasInt.custos)
                            {
                                if (c.tipo == "opex")
                                {
                                    c.valor = opexGas;
                                }
                            }
                        }
                    }
                }
            }


            if (isCapex)
                termRegasStorageToSet.terminal.custoUsado = "capex";
            else
                termRegasStorageToSet.terminal.custoUsado = "opex";
        }
        else
        {
            foreach (Transform t in errorLogParent.GetComponentInChildren<Transform>())
            {
                if (t != errorLogParent)
                    Destroy(t.gameObject);
            }
            eList.Clear();
            errors.Clear();
            return;
        }


        if (eList.Count > 0)
        {
            for (int x = errors.Count - 1; x >= 0; x--)
            {
                if (!eList.Contains(errors[x]))
                {
                    errors.RemoveAt(x);
                }
            }
            foreach (string s in eList)
            {
                if (!errors.Contains(s))
                    errors.Add(s);
            }

            Text[] texts = errorLogParent.GetComponentsInChildren<Text>();

            foreach (Text t in texts)
            {
                if (!errors.Contains(t.text))
                {
                    Destroy(t.transform.parent.gameObject);
                }
            }

            foreach (string e in errors)
            {

                if (texts.Length > 0)
                {
                    bool hasError = false;
                    foreach (Text t in texts)
                    {
                        if (t.text == e)
                        {
                            hasError = true;
                        }
                    }


                    if (!hasError)
                    {
                        GameObject errorInstance = Instantiate(errorPrefab, errorLogParent);
                        errorInstance.GetComponentInChildren<Text>().text = e;
                    }
                }
                else
                {
                    GameObject errorInstance = Instantiate(errorPrefab, errorLogParent);
                    errorInstance.GetComponentInChildren<Text>().text = e;
                }
            }
        }
        else
        {
            foreach (Transform t in errorLogParent.GetComponentInChildren<Transform>())
            {
                if (t != errorLogParent)
                    Destroy(t.gameObject);
            }


            errors.Clear();
        }

        sessionManager.UpdateSession();
    }


    public void ShowUsinaConfig(UsinaStorage storage)
    {
        DeleteAllInputFields();

        usinaStorageToSet = storage;

        nameText.text = usinaStorageToSet.usina.nome;

        _potencia = Instantiate(numberFieldPrefab, dataParent);
        InputFieldBehaviour potenciaInput = _potencia.GetComponent<InputFieldBehaviour>();
        potenciaInput.title.text = "Potência:";
        potenciaInput.input.text = usinaStorageToSet.usina.potencia.ToString("F2");
        potenciaInput.unit.text = "kW";
        potenciaInput.description.text = "Potência gerada por esta usina";
        AddOSKListenerToEventTrigger(_potencia.GetComponentInChildren<EventTrigger>(), _potencia.GetComponentInChildren<InputField>());

        _demandaDiaM3 = Instantiate(numberFieldPrefab, dataParent);
        InputFieldBehaviour demandaInput = _demandaDiaM3.GetComponent<InputFieldBehaviour>();
        demandaInput.title.text = "Demanda diária:";
        demandaInput.input.text = usinaStorageToSet.usina.demandaDiaM3.ToString("F2");
        demandaInput.unit.text = "m³/dia";
        demandaInput.description.text = "Demanda de GNL por dia";
        demandaInput.input.interactable = false;
        demandaInput.inputContent.color = Color.white;

        _eficiencia = Instantiate(numberFieldPrefab, dataParent);
        InputFieldBehaviour eficienciaInput = _eficiencia.GetComponent<InputFieldBehaviour>();
        eficienciaInput.title.text = "Eficiência:";
        eficienciaInput.input.text = usinaStorageToSet.usina.eficiencia.ToString("F2");
        eficienciaInput.unit.text = "%";
        eficienciaInput.description.text = "Taxa de eficiência desta usina";
        AddOSKListenerToEventTrigger(_eficiencia.GetComponentInChildren<EventTrigger>(), _eficiencia.GetComponentInChildren<InputField>());


        _despacho = Instantiate(numberFieldPrefab, dataParent);
        InputFieldBehaviour despachoInput = _despacho.GetComponent<InputFieldBehaviour>();
        despachoInput.title.text = "Despacho:";
        despachoInput.input.text = usinaStorageToSet.usina.despacho.ToString("F2");
        despachoInput.unit.text = "%";
        despachoInput.description.text = "Taxa de despacho desta usina";
        AddOSKListenerToEventTrigger(_despacho.GetComponentInChildren<EventTrigger>(), _despacho.GetComponentInChildren<InputField>());

        if (!string.IsNullOrEmpty(usinaStorageToSet.usina.malha))
        {
            if (usinaStorageToSet.usina.valorMalha == 0)
                usinaStorageToSet.usina.valorMalha = sessionManager.GetMalhaValue(usinaStorageToSet.usina);

            //_valorMalha = Instantiate(numberFieldPrefab, dataParent);
            //InputFieldBehaviour malhaInput = _valorMalha.GetComponent<InputFieldBehaviour>();
            //malhaInput.title.text = "Tarifa malha:";
            //malhaInput.input.text = usinaStorageToSet.usina.valorMalha.ToString("F2");
            //malhaInput.unit.text = "R$/MM Btu";
            //malhaInput.description.text = "Malha " + usinaStorageToSet.usina.malha;
            //AddOSKListenerToEventTrigger(_valorMalha.GetComponentInChildren<EventTrigger>(), _valorMalha.GetComponentInChildren<InputField>());

            _opexGasDist = Instantiate(numberFieldPrefab, dataParent);
            InputFieldBehaviour opexGasDistInput = _opexGasDist.GetComponent<InputFieldBehaviour>();
            opexGasDistInput.title.text = "OPEX Distribuição:";
            opexGasDistInput.input.text = usinaStorageToSet.gasDist.opex.valor.ToString("F2");
            opexGasDistInput.unit.text = "US$/ano";
            opexGasDistInput.description.text = "Custo anual do Gasoduto de Distribuição desta usina";
            opexGasDistInput.input.interactable = false;
            opexGasDistInput.inputContent.color = Color.white;

            double custoDiario;
            double opexMalha;

            custoDiario = usinaStorageToSet.usina.valorMalha * usinaStorageToSet.usina.demandaDiaPci;
            opexMalha = custoDiario * (365 * usinaStorageToSet.usina.despacho * 0.01f / sessionManager.session.premissas.cotacaoDolar);

            _opexGasTrans = Instantiate(numberFieldPrefab, dataParent);
            InputFieldBehaviour opexGasTransInput = _opexGasTrans.GetComponent<InputFieldBehaviour>();
            opexGasTransInput.title.text = "OPEX Malha:";
            opexGasTransInput.input.text = opexMalha.ToString("F2");
            opexGasTransInput.unit.text = "US$/ano";
            opexGasTransInput.description.text = "Custo anual da malha de gasodutos utilizada por esta usina";
            opexGasTransInput.input.interactable = false;
            opexGasTransInput.inputContent.color = Color.white;
        }


        if (usinaStorageToSet.frotaLine != null)
        {
            frotaToSetDist = null;
            foreach (Frota f in sessionManager.session.frotasColocadas)
            {
                if (f.terminalRegasDestino == usinaStorageToSet.termRegasMarker.instance.GetComponent<TerminalRegasStorage>().terminal.nome)
                {
                    frotaToSetDist = f;
                }
            }
            if (frotaToSetDist != null)
            {
                _distanciaFrota = Instantiate(numberFieldPrefab, dataParent);
                InputFieldBehaviour distInput = _distanciaFrota.GetComponent<InputFieldBehaviour>();
                distInput.title.text = "Comprimento da Rodovia:";
                distInput.input.text = frotaToSetDist.distancia.ToString("F2");
                distInput.unit.text = "km";
                distInput.description.text = "Distância da Usina até o Terminal de Carga";
                AddOSKListenerToEventTrigger(_distanciaFrota.GetComponentInChildren<EventTrigger>(), _distanciaFrota.GetComponentInChildren<InputField>());

                _recargasFrota = Instantiate(numberFieldPrefab, dataParent);
                InputFieldBehaviour recInput = _recargasFrota.GetComponent<InputFieldBehaviour>();
                _recargasFrota.GetComponent<InputFieldBehaviour>().title.text = "Recargas por dia:";
                recInput.input.text = frotaToSetDist.recargasDia.ToString("F2");
                recInput.unit.text = "";
                recInput.description.text = "Recargas necessárias para abastecer a usina";
                recInput.input.interactable = false;
                recInput.inputContent.color = Color.white;

                double opexFrota = 0;

                foreach (Custo c in frotaToSetDist.custos)
                {
                    if (c.tipo == "opex")
                        opexFrota = c.valor;
                }

                _opexFrota = Instantiate(numberFieldPrefab, dataParent);
                InputFieldBehaviour opexInput = _opexFrota.GetComponent<InputFieldBehaviour>();
                opexInput.title.text = "OPEX Frota:";
                opexInput.input.text = opexFrota.ToString("F2");
                opexInput.unit.text = "US$/ano";
                opexInput.description.text = "Custo anual da frota desta usina";
                opexInput.input.interactable = false;
                opexInput.inputContent.color = Color.white;
            }
        }

        if (usinaStorageToSet.usina.categoria == "Simulado")
        {
            _potencia.GetComponentInChildren<InputField>().enabled = true;
            _eficiencia.GetComponentInChildren<InputField>().enabled = true;
            _despacho.GetComponentInChildren<InputField>().enabled = true;
        }
        else
        {
            _potencia.GetComponentInChildren<InputField>().enabled = false;
            _eficiencia.GetComponentInChildren<InputField>().enabled = false;
            _despacho.GetComponentInChildren<InputField>().enabled = false;
        }


        bool showGasSources = false;

        foreach(OnlineMapsMarker3D m in markerDisplacer.markers)
        {
            if(m.instance.GetComponent<FSUStorage>()!=null)
            {
                showGasSources = true;
                break;
            }
        }
        //if (usinaStorageToSet.termRegasMarker != null)
        //{
        //    foreach (OnlineMapsMarker3D m in markerDisplacer.markers)
        //    {
        //        if (m.instance.GetComponent<FrotaStorage>() != null)
        //        {
        //            foreach (Frota f in m.instance.GetComponent<FrotaStorage>().frotas)
        //            {
        //                if (!string.IsNullOrEmpty(f.terminalRegasDestino))
        //                {
        //                    if (f.terminalRegasDestino == usinaStorageToSet.termRegasMarker.instance.GetComponent<TerminalRegasStorage>().terminal.nome)
        //                    {
        //                        showGasSources = true;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        
        gasSourcePanel.SetActive(showGasSources);


        renameBtn.SetActive(true);

        if (usinaStorageToSet.usina.fonteDoGas == "frota")
        {
            isFrota = true;
            frotaToggle.isOn = true;
        }
        else
        {
            isFrota = false;
            gasodutoToggle.isOn = true;
        }




        deleteButton.interactable = true;
        isOpen = true;


        paymentPanel.SetActive(false);


        usinasPanelBtn.SetActive(false);


        StartCoroutine(CheckUsinaStatus());
    }

    public void ShowFSRUConfig(FSRUStorage storage)
    {


        DeleteAllInputFields();


        fsruStorageToSet = storage;


        nameText.text = fsruStorageToSet.fsru.nome;


        Custo capex = new Custo();
        Custo opex = new Custo();
        foreach (Custo c in fsruStorageToSet.fsru.custos)
        {
            if (c.tipo == "capex")
                capex = c;
            else
                opex = c;
        }


        _opex = Instantiate(numberFieldPrefab, dataParent);
        InputFieldBehaviour opexInput = _opex.GetComponent<InputFieldBehaviour>();
        opexInput.title.text = "OPEX:";
        opexInput.input.text = opex.valor.ToString("F2");
        opexInput.unit.text = "US$/ano";
        opexInput.description.text = "Custo Anual para Operação e Manutenção";
        AddOSKListenerToEventTrigger(_opex.GetComponentInChildren<EventTrigger>(), _opex.GetComponent<InputFieldBehaviour>().input);


        _capex = Instantiate(numberFieldPrefab, dataParent);
        InputFieldBehaviour capexInput = _capex.GetComponent<InputFieldBehaviour>();
        capexInput.title.text = "CAPEX:";
        capexInput.input.text = capex.valor.ToString("F2");
        capexInput.unit.text = "US$";
        capexInput.description.text = "Custo para Aquisição";
        AddOSKListenerToEventTrigger(_capex.GetComponentInChildren<EventTrigger>(), _capex.GetComponent<InputFieldBehaviour>().input);


        _gasFornecido = Instantiate(numberFieldPrefab, dataParent);
        InputFieldBehaviour gasFornecidoInput = _gasFornecido.GetComponent<InputFieldBehaviour>();
        gasFornecidoInput.title.text = "Capacidade:";
        gasFornecidoInput.input.text = fsruStorageToSet.fsru.gnlFornecido.ToString("F2");
        gasFornecidoInput.unit.text = "m³";
        gasFornecidoInput.description.text = "Capacidade de Armazenamento de GN";
        AddOSKListenerToEventTrigger(_gasFornecido.GetComponentInChildren<EventTrigger>(), _gasFornecido.GetComponent<InputFieldBehaviour>().input);


        _gasIntCusto = Instantiate(numberFieldPrefab, dataParent);
        InputFieldBehaviour gasIntCustoInput = _gasIntCusto.GetComponent<InputFieldBehaviour>();
        gasIntCustoInput.title.text = "Custo Base:";
        gasIntCustoInput.input.text = fsruStorageToSet.gasInt.custoBase.ToString("F2");
        gasIntCustoInput.unit.text = "US$/(m.pol)";
        gasIntCustoInput.description.text = "Custo base do Gasoduto Integrador por metro e polegadas";
        AddOSKListenerToEventTrigger(_gasIntCusto.GetComponentInChildren<EventTrigger>(), _gasIntCusto.GetComponent<InputFieldBehaviour>().input);


        _gasIntDiam = Instantiate(numberFieldPrefab, dataParent);
        InputFieldBehaviour gasIntDiamInput = _gasIntDiam.GetComponent<InputFieldBehaviour>();
        gasIntDiamInput.title.text = "Diâmetro do gasoduto:";
        gasIntDiamInput.input.text = fsruStorageToSet.gasInt.diametro.ToString("F2");
        gasIntDiamInput.unit.text = "polegadas";
        gasIntDiamInput.description.text = "Diâmetro do Gasoduto Integrador";
        AddOSKListenerToEventTrigger(_gasIntDiam.GetComponentInChildren<EventTrigger>(), _gasIntDiam.GetComponent<InputFieldBehaviour>().input);

        _gasIntComp = Instantiate(numberFieldPrefab, dataParent);
        InputFieldBehaviour gasIntCompInput = _gasIntComp.GetComponent<InputFieldBehaviour>();
        gasIntCompInput.title.text = "Comprimento do gasoduto:";
        gasIntCompInput.input.text = fsruStorageToSet.gasInt.comprimento.ToString("F2");
        gasIntCompInput.unit.text = "km";
        gasIntCompInput.description.text = "Distância do FSRU ao Ponto de Entrega";
        AddOSKListenerToEventTrigger(_gasIntComp.GetComponentInChildren<EventTrigger>(), _gasIntComp.GetComponent<InputFieldBehaviour>().input);

        double opexGasInt = 0;

        foreach (Custo c in fsruStorageToSet.gasInt.custos)
        {
            if (c.tipo == "opex")
                opexGasInt = c.valor;
        }
        _opexGasInt = Instantiate(numberFieldWithLockerPrefab, dataParent);
        InputFieldBehaviour opexGasIntInput = _opexGasInt.GetComponent<InputFieldBehaviour>();
        opexGasIntInput.title.text = "OPEX Gasoduto:";
        opexGasIntInput.input.text = opexGasInt.ToString("F2");
        opexGasIntInput.unit.text = "US$/ano";
        opexGasIntInput.description.text = "Custo Anual do Gasoduto de Integração";
        AddOSKListenerToEventTrigger(_opexGasInt.GetComponentInChildren<EventTrigger>(), _opexGasInt.GetComponent<InputFieldBehaviour>().input);

        if (fsruStorageToSet.gasIntUsingCalculation)
        {
            _opexGasInt.GetComponent<InputFieldBehaviour>().CalculationToggle();
        }

        deleteButton.interactable = true;


        gasSourcePanel.SetActive(false);


        renameBtn.SetActive(false);
        paymentPanel.SetActive(true);


        isOpen = true;


        if (fsruStorageToSet.fsru.custoUsado == "capex")
        {
            isCapex = true;
            capexToggle.isOn = true;
        }
        else
        {
            isCapex = false;
            opexToggle.isOn = true;


        }
        usinasPanelBtn.SetActive(true);
        LoadUsinas();


        StartCoroutine(CheckFSRUStatus());


    }

    public void ShowFSUConfig(FSUStorage storage)
    {
        DeleteAllInputFields();

        fsuStorageToSet = storage;

        nameText.text = fsuStorageToSet.fsu.nome;

        Custo capex = new Custo();
        Custo opex = new Custo();

        foreach (Custo c in fsuStorageToSet.fsu.custos)
        {
            if (c.tipo == "capex")
                capex = c;
            else
                opex = c;
        }

        _opex = Instantiate(numberFieldPrefab, dataParent);
        InputFieldBehaviour opexInput = _opex.GetComponent<InputFieldBehaviour>();
        opexInput.title.text = "OPEX:";
        opexInput.input.text = opex.valor.ToString("F2");
        opexInput.unit.text = "US$/ano";
        opexInput.description.text = "Custo Anual para Operação e Manutenção";
        AddOSKListenerToEventTrigger(_opex.GetComponentInChildren<EventTrigger>(), _opex.GetComponent<InputFieldBehaviour>().input);


        _capex = Instantiate(numberFieldPrefab, dataParent);
        InputFieldBehaviour capexInput = _capex.GetComponent<InputFieldBehaviour>();
        capexInput.title.text = "CAPEX:";
        capexInput.input.text = capex.valor.ToString("F2");
        capexInput.unit.text = "US$";
        capexInput.description.text = "Custo para Aquisição";
        AddOSKListenerToEventTrigger(_capex.GetComponentInChildren<EventTrigger>(), _capex.GetComponent<InputFieldBehaviour>().input);


        _gasFornecido = Instantiate(numberFieldPrefab, dataParent);
        InputFieldBehaviour gasFornecido = _gasFornecido.GetComponent<InputFieldBehaviour>();
        gasFornecido.title.text = "Capacidade:";
        gasFornecido.input.text = fsuStorageToSet.fsu.gnlFornecido.ToString("F2");
        gasFornecido.unit.text = "m³";
        gasFornecido.description.text = "Capacidade de Armazenamento de GNL";
        AddOSKListenerToEventTrigger(_gasFornecido.GetComponentInChildren<EventTrigger>(), _gasFornecido.GetComponent<InputFieldBehaviour>().input);


        _gasCrioCusto = Instantiate(numberFieldPrefab, dataParent);
        InputFieldBehaviour gasCrioCustoInput = _gasCrioCusto.GetComponent<InputFieldBehaviour>();
        gasCrioCustoInput.title.text = "Custo Base:";
        gasCrioCustoInput.input.text = fsuStorageToSet.gasCrio.custoBase.ToString("F2");
        gasCrioCustoInput.unit.text = "US$/km";
        gasCrioCustoInput.description.text = "Custo base do Gasoduto Criogênico por km";
        AddOSKListenerToEventTrigger(_gasCrioCusto.GetComponentInChildren<EventTrigger>(), _gasCrioCusto.GetComponent<InputFieldBehaviour>().input);


        _gasCrioDiam = Instantiate(numberFieldPrefab, dataParent);
        InputFieldBehaviour gasCrioDiamInput = _gasCrioDiam.GetComponent<InputFieldBehaviour>();
        gasCrioDiamInput.title.text = "Diâmetro do gasoduto:";
        gasCrioDiamInput.input.text = fsuStorageToSet.gasCrio.diametro.ToString("F2");
        gasCrioDiamInput.unit.text = "polegadas";
        gasCrioDiamInput.description.text = "Diâmetro do Gasoduto Criogênico";
        AddOSKListenerToEventTrigger(_gasCrioDiam.GetComponentInChildren<EventTrigger>(), _gasCrioDiam.GetComponent<InputFieldBehaviour>().input);

        _gasCrioComp = Instantiate(numberFieldPrefab, dataParent);
        InputFieldBehaviour gasCrioCompInput = _gasCrioComp.GetComponent<InputFieldBehaviour>();
        gasCrioCompInput.title.text = "Comprimento do gasoduto:";
        gasCrioCompInput.input.text = fsuStorageToSet.gasCrio.comprimento.ToString("F2");
        gasCrioCompInput.unit.text = "km";
        gasCrioCompInput.description.text = "Distância do Terminal de Caminhões até o FSU";
        AddOSKListenerToEventTrigger(_gasCrioComp.GetComponentInChildren<EventTrigger>(), _gasCrioComp.GetComponent<InputFieldBehaviour>().input);

        double opexGas = 0;
        foreach (Custo c in fsuStorageToSet.gasCrio.custos)
        {
            if (c.tipo == "opex")
            {
                opexGas = c.valor;
            }
        }
        _opexGasCrio = Instantiate(numberFieldWithLockerPrefab, dataParent);
        InputFieldBehaviour opexGasCrioInput = _opexGasCrio.GetComponent<InputFieldBehaviour>();
        opexGasCrioInput.title.text = "OPEX Gasoduto:";
        opexGasCrioInput.input.text = opexGas.ToString("F2");
        opexGasCrioInput.unit.text = "US$/ano";
        opexGasCrioInput.description.text = "Custo Anual do Gasoduto Criogênico";

        opexGasCrioInput.input.interactable = !fsuStorageToSet.gasCrioUsingCalculation;

        AddOSKListenerToEventTrigger(_opexGasCrio.GetComponentInChildren<EventTrigger>(), _opexGasCrio.GetComponent<InputFieldBehaviour>().input);

        deleteButton.interactable = true;


        gasSourcePanel.SetActive(false);
        paymentPanel.SetActive(true);


        renameBtn.SetActive(false);


        if (fsuStorageToSet.fsu.custoUsado == "capex")
        {
            isCapex = true;
            capexToggle.isOn = true;
        }
        else
        {
            isCapex = false;
            opexToggle.isOn = true;
        }


        isOpen = true;


        usinasPanelBtn.SetActive(true);
        LoadUsinas();

        StartCoroutine(CheckFSUStatus());


    }

    public void ShowFrotaConfig(FrotaStorage storage)
    {
        DeleteAllInputFields();


        frotaStorageToSet = storage;


        nameText.text = "Terminal de Carga de " + frotaStorageToSet.frotas[0].terminalGnlOrigem;


        _capacidadeLiq = Instantiate(numberFieldPrefab, dataParent);
        InputFieldBehaviour capacidadeLiqInput = _capacidadeLiq.GetComponent<InputFieldBehaviour>();
        capacidadeLiqInput.title.text = "Capacidade dos caminhões:";
        capacidadeLiqInput.input.text = frotaStorageToSet.capacidadeLiq.ToString("F2");
        capacidadeLiqInput.unit.text = "m³";
        capacidadeLiqInput.description.text = "Volume de GNL transportado por caminhão";
        AddOSKListenerToEventTrigger(_capacidadeLiq.GetComponentInChildren<EventTrigger>(), _capacidadeLiq.GetComponentInChildren<InputField>());



        gasSourcePanel.SetActive(false);


        deleteButton.interactable = false;


        renameBtn.SetActive(false);


        isOpen = true;
        usinasPanelBtn.SetActive(false);
        paymentPanel.SetActive(false);


        StartCoroutine(CheckFrotaStatus());




    }

    public void ShowTermRegasConfig(TerminalRegasStorage storage)
    {
        DeleteAllInputFields();


        termRegasStorageToSet = storage;


        nameText.text = termRegasStorageToSet.terminal.nome;


        Custo capex = new Custo();
        Custo opex = new Custo();
        foreach (Custo c in termRegasStorageToSet.terminal.custos)
        {
            if (c.tipo == "capex")
                capex = c;
            else
                opex = c;
        }


        _opex = Instantiate(numberFieldPrefab, dataParent);
        InputFieldBehaviour opexInput = _opex.GetComponent<InputFieldBehaviour>();
        opexInput.title.text = "OPEX:";
        opexInput.input.text = opex.valor.ToString("F2");
        opexInput.unit.text = "US$/ano";
        opexInput.description.text = "Custo Anual para Operação e Manutenção";
        AddOSKListenerToEventTrigger(_opex.GetComponentInChildren<EventTrigger>(), _opex.GetComponent<InputFieldBehaviour>().input);


        _capex = Instantiate(numberFieldPrefab, dataParent);
        InputFieldBehaviour capexInput = _capex.GetComponent<InputFieldBehaviour>();
        capexInput.title.text = "CAPEX:";
        capexInput.input.text = capex.valor.ToString("F2");
        capexInput.unit.text = "US$";
        capexInput.description.text = "Custo para Aquisição";
        AddOSKListenerToEventTrigger(_capex.GetComponentInChildren<EventTrigger>(), _capex.GetComponent<InputFieldBehaviour>().input);


        if (!string.IsNullOrEmpty(termRegasStorageToSet.gasInt.nome))
        {
            _gasIntCusto = Instantiate(numberFieldPrefab, dataParent);
            InputFieldBehaviour gasIntCustoInput = _gasIntCusto.GetComponent<InputFieldBehaviour>();
            gasIntCustoInput.title.text = "Custo Base:";
            gasIntCustoInput.input.text = termRegasStorageToSet.gasInt.custoBase.ToString("F2");
            gasIntCustoInput.unit.text = "US$/(m.pol)";
            gasIntCustoInput.description.text = "Custo base do Gasoduto Integrador";
            AddOSKListenerToEventTrigger(_gasIntCusto.GetComponentInChildren<EventTrigger>(), _gasIntCusto.GetComponent<InputFieldBehaviour>().input);


            _gasIntDiam = Instantiate(numberFieldPrefab, dataParent);
            InputFieldBehaviour gasIntDiamInput = _gasIntDiam.GetComponent<InputFieldBehaviour>();
            gasIntDiamInput.title.text = "Diâmetro do gasoduto:";
            gasIntDiamInput.input.text = termRegasStorageToSet.gasInt.diametro.ToString("F2");
            gasIntDiamInput.unit.text = "polegadas";
            gasIntDiamInput.description.text = "Diâmetro do Gasoduto Integrador";
            AddOSKListenerToEventTrigger(_gasIntDiam.GetComponentInChildren<EventTrigger>(), _gasIntDiam.GetComponent<InputFieldBehaviour>().input);

            _gasIntComp = Instantiate(numberFieldPrefab, dataParent);
            InputFieldBehaviour gasIntCompInput = _gasIntComp.GetComponent<InputFieldBehaviour>();
            gasIntCompInput.title.text = "Comprimento do gasoduto:";
            gasIntCompInput.input.text = termRegasStorageToSet.gasInt.comprimento.ToString("F2");
            gasIntCompInput.unit.text = "km";
            gasIntCompInput.description.text = "Comprimento do Gasoduto Integrador";
            AddOSKListenerToEventTrigger(_gasIntComp.GetComponentInChildren<EventTrigger>(), _gasIntComp.GetComponent<InputFieldBehaviour>().input);

            double opexGasInt = 0;

            foreach (Custo c in termRegasStorageToSet.gasInt.custos)
            {
                if (c.tipo == "opex")
                    opexGasInt = c.valor;
            }

            _opexGasInt = Instantiate(numberFieldWithLockerPrefab, dataParent);
            InputFieldBehaviour opexGasIntInput = _opexGasInt.GetComponent<InputFieldBehaviour>();
            opexGasIntInput.title.text = "OPEX Gasoduto:";
            opexGasIntInput.input.text = opexGasInt.ToString("F2");
            opexGasIntInput.unit.text = "US$/ano";
            opexGasIntInput.description.text = "Custo Anual do Gasoduto de Integração";
            opexGasIntInput.input.interactable = !termRegasStorageToSet.gasIntUsingCalculation;

            if (termRegasStorageToSet.gasIntUsingCalculation)
            {
                _opexGasInt.GetComponent<InputFieldBehaviour>().CalculationToggle();
            }

            AddOSKListenerToEventTrigger(_opexGasInt.GetComponentInChildren<EventTrigger>(), _opexGasInt.GetComponent<InputFieldBehaviour>().input);
        }

        deleteButton.interactable = false;

        gasSourcePanel.SetActive(false);
        paymentPanel.SetActive(true);


        if (termRegasStorageToSet.terminal.custoUsado == "capex")
        {
            isCapex = true;
            capexToggle.isOn = true;
        }
        else
        {
            isCapex = false;
            opexToggle.isOn = true;
        }


        renameBtn.SetActive(false);


        isOpen = true;


        usinasPanelBtn.SetActive(false);


        StartCoroutine(CheckTermRegasStatus());
    }

    public void HideSelf()
    {
        DeleteAllInputFields();


        nameText.text = "";


        nameField.GetComponent<InputField>().text = "";
        nameField.SetActive(false);
        saveNameBtn.SetActive(false);

        osk.SetActive(false);
        isOpen = false;


        gameObject.SetActive(false);
    }

    public void SelectCapex(bool response)
    {
        isCapex = response;
    }

    public void SelectFrota(bool response)
    {
        isFrota = response;
    }

    void DeleteAllInputFields()
    {
        usinaStorageToSet = null;
        fsruStorageToSet = null;
        fsuStorageToSet = null;
        frotaStorageToSet = null;
        termRegasStorageToSet = null;


        if (_eficiencia != null)
        {
            GameObject temp = _eficiencia;
            _eficiencia = null;
            Destroy(temp);
        }
        if (_potencia != null)
        {
            GameObject temp = _potencia;
            _potencia = null;
            Destroy(temp);
        }
        if (_despacho != null)
        {
            GameObject temp = _despacho;
            _despacho = null;
            Destroy(temp);
        }
        if (_opex != null)
        {
            GameObject temp = _opex;
            _opex = null;
            Destroy(temp);
        }
        if (_capex != null)
        {
            GameObject temp = _capex;
            _capex = null;
            Destroy(temp);
        }
        if (_gasFornecido != null)
        {
            GameObject temp = _gasFornecido;
            _gasFornecido = null;
            Destroy(temp);
        }
        if (_gasIntCusto != null)
        {
            GameObject temp = _gasIntCusto;
            _gasIntCusto = null;
            Destroy(temp);
        }
        if (_gasIntDiam != null)
        {
            GameObject temp = _gasIntDiam;
            _gasIntDiam = null;
            Destroy(temp);
        }


        if (_gasIntComp != null)
        {
            GameObject temp = _gasIntComp;
            _gasIntComp = null;
            Destroy(temp);
        }


        if (_gasCrioCusto != null)
        {
            GameObject temp = _gasCrioCusto;
            _gasCrioCusto = null;
            Destroy(temp);
        }
        if (_gasCrioDiam != null)
        {
            GameObject temp = _gasCrioDiam;
            _gasCrioDiam = null;
            Destroy(temp);
        }


        if (_gasCrioComp != null)
        {
            GameObject temp = _gasCrioComp;
            _gasCrioComp = null;
            Destroy(temp);
        }

        if (_capacidadeLiq != null)
        {
            GameObject temp = _capacidadeLiq;
            _capacidadeLiq = null;
            Destroy(temp);
        }

        if (_distanciaFrota != null)
        {
            GameObject temp = _distanciaFrota;
            _distanciaFrota = null;
            Destroy(temp);
        }

        //if (_valorMalha != null)
        //{
        //    GameObject temp = _valorMalha;
        //    _valorMalha = null;
        //    Destroy(temp);
        //}

        if (_opexGasCrio != null)
        {
            GameObject temp = _opexGasCrio;
            _opexGasCrio = null;
            Destroy(temp);
        }

        if (_opexGasInt != null)
        {
            GameObject temp = _opexGasInt;
            _opexGasInt = null;
            Destroy(temp);
        }

        if (_opexGasDist != null)
        {
            GameObject temp = _opexGasDist;
            _opexGasDist = null;
            Destroy(temp);
        }

        if (_opexFrota != null)
        {
            GameObject temp = _opexFrota;
            _opexFrota = null;
            Destroy(temp);
        }

        if (_opexGasTrans != null)
        {
            GameObject temp = _opexGasTrans;
            _opexGasTrans = null;
            Destroy(temp);
        }

        if (_recargasFrota != null)
        {
            GameObject temp = _recargasFrota;
            _recargasFrota = null;
            Destroy(temp);
        }

        if (_demandaDiaM3 != null)
        {
            GameObject temp = _demandaDiaM3;
            _demandaDiaM3 = null;
            Destroy(temp);
        }
    }

    public void DeactivateUsinasPanel()
    {
        usinasPanel.SetActive(false);
    }

    public void LoadUsinas()
    {
        foreach (Transform t in usinaButtonsParent.GetComponentsInChildren<Transform>())
        {
            if (t != usinaButtonsParent)
                Destroy(t.gameObject);
        }

        if (fsuStorageToSet != null)
        {
            GameObject frotaObj = null;
            TextMesh msg = markerDisplacer.msg;
            FSUDestinationBehaviour fsuManager = markerDisplacer.FSUManager;

            foreach (OnlineMapsMarker3D m in markerDisplacer.markers)
            {
                if (m.instance.GetComponent<FrotaStorage>() != null)
                {
                    FrotaStorage frotaStorage = m.instance.GetComponent<FrotaStorage>();
                    foreach (Frota f in frotaStorage.frotas)
                    {
                        if (f.terminalGnlOrigem == fsuStorageToSet.fsu.terminalGnl)
                        {
                            frotaObj = m.instance;
                        }
                    }
                }
            }
            List<OnlineMapsMarker3D> usinasWithoutTerminal = new List<OnlineMapsMarker3D>();

            foreach (OnlineMapsMarker3D m in markerDisplacer.markers)
            {
                if (m.instance.GetComponent<UsinaStorage>() != null)
                {
                    UsinaStorage usinaStorage = m.instance.GetComponent<UsinaStorage>();

                    GameObject btnInstance = Instantiate(usinaTogglePrefab, usinaButtonsParent);
                    UsinaButtonBehaviour btnBehaviour = btnInstance.GetComponent<UsinaButtonBehaviour>();

                    if (frotaObj != null)
                    {
                        FrotaStorage frotaStorage = frotaObj.GetComponent<FrotaStorage>();
                        if (usinaStorage.termRegasMarker != null)
                        {
                            foreach (Frota f in frotaStorage.frotas) 
                            {
                                if (f.terminalRegasDestino == usinaStorage.termRegasMarker.instance.GetComponent<TerminalRegasStorage>().terminal.nome && usinaStorage.usina.fonteDoGas == "frota")
                                {
                                    btnInstance.GetComponent<Toggle>().isOn = true;
                                }
                            }
                        }
                        else
                        {
                            usinasWithoutTerminal.Add(m);
                        }
                        if (usinasWithoutTerminal.Count > 0)
                            fsuManager.fsuMarker = fsuStorageToSet.GetComponent<OnlineMapsMarker3DInstance>().marker as OnlineMapsMarker3D;

                        btnBehaviour.SetForFSU(m.instance, frotaObj, markerDisplacer.connCreator, fsuManager, usinasWithoutTerminal);
                    }
                }
            }
        }
        else if (fsruStorageToSet != null)
        {
            foreach (OnlineMapsMarker3D m in markerDisplacer.markers)
            {
                if (m.instance.GetComponent<UsinaStorage>() != null)
                {
                    UsinaStorage usinaStorage = m.instance.GetComponent<UsinaStorage>();
                    GameObject btnInstance = Instantiate(usinaButtonPrefab, usinaButtonsParent);
                    UsinaButtonBehaviour btnBehaviour = btnInstance.GetComponent<UsinaButtonBehaviour>();
                    btnBehaviour.SetForFSRU(m.instance);
                }
            }
        }

    }

    public void AllowEditUsinaName()
    {
        oldUsinaName = usinaStorageToSet.usina.nome;
        nameField.SetActive(true);
        nameField.GetComponent<InputField>().text = usinaStorageToSet.usina.nome;
        nameField.GetComponent<InputField>().Select();


        for (int x = 0; x < osk.panels.Length; x++)
            osk.panels[x].SetActive(x == 0);


        uiManager.SetCenterOSKPoint();


        osk.SetActiveFocus(nameField.GetComponent<InputField>());


        renameBtn.SetActive(false);
        saveNameBtn.SetActive(true);
    }

    public void SaveUsinaName()
    {
        if (nameField.activeSelf)
        {
            if (!string.IsNullOrEmpty(nameField.GetComponent<InputField>().text))
            {
                usinaStorageToSet.usina.nome = nameField.GetComponent<InputField>().text;
                nameText.text = usinaStorageToSet.usina.nome;
                saveNameBtn.SetActive(false);
                renameBtn.SetActive(true);
                nameField.SetActive(false);
            }
            foreach (TerminalRegas t in sessionManager.session.terminaisRegasColocados)
            {
                if (t.usina == oldUsinaName)
                {
                    t.usina = nameField.GetComponent<InputField>().text;
                }
            }
            foreach (GasodutoDistribuicao g in sessionManager.session.gasDistColocados)
            {
                if (g.usinaDestino == oldUsinaName)
                {
                    g.usinaDestino = nameField.GetComponent<InputField>().text;
                }
            }
        }
    }

    IEnumerator CheckUsinaStatus()
    {
        while (usinaStorageToSet != null)
        {
            usinaStorageToSet.SetState();
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator CheckFSUStatus()
    {
        while (fsuStorageToSet != null)
        {
            fsuStorageToSet.SetState();
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator CheckFSRUStatus()
    {
        while (fsruStorageToSet != null)
        {
            fsruStorageToSet.SetState();
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator CheckFrotaStatus()
    {
        while (frotaStorageToSet != null)
        {
            frotaStorageToSet.SetState();
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator CheckTermRegasStatus()
    {
        while (termRegasStorageToSet != null)
        {
            termRegasStorageToSet.SetState();
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void AddOSKListenerToEventTrigger(EventTrigger trigger, InputField field)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();

        entry.eventID = EventTriggerType.Select;
        entry.callback.AddListener((eventData) => { sessionManager.setupPanel.CheckContentType(field); });
        entry.callback.AddListener((eventData) => { uiManager.SetCenterOSKPoint(); });
        entry.callback.AddListener((eventData) => { osk.SetActiveFocus(field); });

        trigger.triggers.Add(entry);
    }


}



