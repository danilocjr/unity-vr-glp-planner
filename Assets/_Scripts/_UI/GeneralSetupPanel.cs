using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
public class GeneralSetupPanel : MonoBehaviour
{
    // Start is called before the first frame update
    public UIManager uiManager;
    public SessionManager sessionManager;
    OnScreenKeyboard osk;
    public InputField pciInput, pcsInput, taxaCompInput, dolarInput, tmaInput, amortInput, freteInput;

    void Start()
    {
        osk = uiManager.oskParent.GetComponentInChildren<OnScreenKeyboard>();
        SetSessionPremiseFields();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDolarAndFreteCalculations();
        UpdateGNLCalculations();
    }

    private void UpdateGNLCalculations()
    {
        if (string.IsNullOrEmpty(pciInput.text) || string.IsNullOrEmpty(pcsInput.text) || string.IsNullOrEmpty(taxaCompInput.text))
            return;

        if (float.Parse(pciInput.text) != sessionManager.session.gas.pci ||
           float.Parse(pcsInput.text) != sessionManager.session.gas.pcs ||
           float.Parse(taxaCompInput.text) != sessionManager.session.gas.taxaCompressao)
        {
            sessionManager.session.gas.SetPciAndPcs(float.Parse(pciInput.text), float.Parse(pcsInput.text), float.Parse(taxaCompInput.text));
            sessionManager.UpdateSession();
        }
    }

    private void UpdateDolarAndFreteCalculations()
    {
        if (string.IsNullOrEmpty(dolarInput.text) || string.IsNullOrEmpty(freteInput.text))
            return;

      
        if (float.Parse(dolarInput.text) != sessionManager.session.premissas.cotacaoDolar ||
            float.Parse(tmaInput.text) != sessionManager.session.premissas.tma ||
            int.Parse(amortInput.text) != sessionManager.session.premissas.periodoAmortizacao ||
            float.Parse(freteInput.text) != sessionManager.session.premissas.frete)
        {
            sessionManager.session.premissas.Set(
                float.Parse(dolarInput.text),
                float.Parse(tmaInput.text),
                int.Parse(amortInput.text),
                float.Parse(freteInput.text));

            sessionManager.UpdateSession();
        }
    }

    public void SetSessionPremiseFields()
    {
        pciInput.text = sessionManager.session.gas.pci.ToString("F2"); ;
        pcsInput.text = sessionManager.session.gas.pcs.ToString("F2");
        taxaCompInput.text = sessionManager.session.gas.taxaCompressao.ToString("F2");
        dolarInput.text = sessionManager.session.premissas.cotacaoDolar.ToString("F2");
        tmaInput.text = sessionManager.session.premissas.tma.ToString("F2");
        amortInput.text = sessionManager.session.premissas.periodoAmortizacao.ToString();
        freteInput.text = sessionManager.session.premissas.frete.ToString("F2");
    }

    public void CheckContentType(InputField field)
    {
        if (field.contentType == InputField.ContentType.DecimalNumber)
        {
            for (int x = 0; x < osk.panels.Length; x++)
                osk.panels[x].SetActive(x == 1);
        }
        else
        {
            for (int x = 0; x < osk.panels.Length; x++)
                osk.panels[x].SetActive(x == 0);
        }

        field.Select();
    }
}
