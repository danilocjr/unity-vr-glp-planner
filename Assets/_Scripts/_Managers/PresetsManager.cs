using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
public class PresetsManager : MonoBehaviour
{
    public UIManager uiManager;
    OnScreenKeyboard osk;
    public SessionManager sessionManager;

    public InputField eficienciaInput, despachoInput, capacidadeInput, custoCrioInput, custoIntInput;

    public GameObject editButton, saveButton, CancelButton;

    public GameObject[] menuToggles;
    public Animator applyWindowAnim;

    public double eficienciaAtual, despachoAtual, capacidadeAtual, custoCrioAtual, custoIntAtual;

    double eficienciaAnterior, despachoAnterior, capacidadeAnterior, custoCrioAnterior, custoIntAnterior;

    public IconPanelElements iconPanel;

    void OnEnable()
    {
        osk = uiManager.oskParent.GetComponentInChildren<OnScreenKeyboard>();
        applyWindowAnim.gameObject.SetActive(false);
        SetPresetsFields();
    }

    public void SetPresetsFields()
    {
        eficienciaAtual = eficienciaAnterior = sessionManager.session.presets.usinaPreset.eficiencia;
        despachoAtual = despachoAnterior = sessionManager.session.presets.usinaPreset.despacho;
        capacidadeAtual = capacidadeAnterior = sessionManager.session.presets.frotaPreset.capacidadeLiquido;
        custoCrioAtual = custoCrioAnterior = sessionManager.session.presets.gasCrioPreset.custoBase;
        custoIntAtual = custoIntAnterior = sessionManager.session.presets.gasIntPreset.custoBase;

        eficienciaInput.text = eficienciaAtual.ToString();
        despachoInput.text = despachoAtual.ToString();
        capacidadeInput.text = capacidadeAtual.ToString();
        custoCrioInput.text = custoCrioAtual.ToString();
        custoIntInput.text = custoIntAtual.ToString();

        eficienciaInput.interactable = despachoInput.interactable = capacidadeInput.interactable = custoCrioInput.interactable = custoIntInput.interactable = false;

        DisableEdition();
    }
    public void ApplyToAll()
    {
        eficienciaAtual = float.Parse(eficienciaInput.text);
        despachoAtual = float.Parse(despachoInput.text);
        capacidadeAtual = float.Parse(capacidadeInput.text);
        custoCrioAtual = float.Parse(custoCrioInput.text);
        custoIntAtual = float.Parse(custoIntInput.text);

        Presets sessionPresets = sessionManager.session.presets;
        sessionPresets.usinaPreset.eficiencia = (float)eficienciaAtual;
        sessionPresets.usinaPreset.despacho = (float)despachoAtual;
        sessionPresets.frotaPreset.capacidadeLiquido = (float)capacidadeAtual;
        sessionPresets.gasCrioPreset.custoBase = custoCrioAtual;
        sessionPresets.gasIntPreset.custoBase = custoIntAtual;

        foreach (Usina u in sessionManager.session.usinasColocadas)
        {
            u.eficiencia = (float)eficienciaAtual;
            u.despacho = (float)despachoAtual;
        }

        foreach (Frota f in sessionManager.session.frotasColocadas)
            f.capacidadeLiquido = (float)capacidadeAtual;

        foreach (GasodutoCriogenico g in sessionManager.session.gasCrioColocados)
            g.custoBase = custoCrioAtual;

        foreach (GasodutoIntegrador g in sessionManager.session.gasIntColocados)
            g.custoBase = custoIntAtual;

        DisableEdition();

        saveButton.GetComponent<Button>().interactable = true;
        CancelButton.GetComponent<Button>().interactable = true;

        applyWindowAnim.gameObject.SetActive(false);
        //applyWindowAnim.SetBool("show", false);

        if (iconPanel.isOpen)
        {
            ChangeIconPanelInputFields(true);
        }

        
    }

    public void ApplyToNew()
    {
        eficienciaAtual = float.Parse(eficienciaInput.text);
        despachoAtual = float.Parse(despachoInput.text);
        capacidadeAtual = float.Parse(capacidadeInput.text);
        custoCrioAtual = float.Parse(custoCrioInput.text);
        custoIntAtual = float.Parse(custoIntInput.text);

        Presets sessionPresets = sessionManager.session.presets;
        sessionPresets.usinaPreset.eficiencia = (float)eficienciaAtual;
        sessionPresets.usinaPreset.despacho = (float)despachoAtual;
        sessionPresets.frotaPreset.capacidadeLiquido = (float)capacidadeAtual;
        sessionPresets.gasCrioPreset.custoBase = custoCrioAtual;
        sessionPresets.gasIntPreset.custoBase = custoIntAtual;

        DisableEdition();

        saveButton.GetComponent<Button>().interactable = true;
        CancelButton.GetComponent<Button>().interactable = true;

        applyWindowAnim.gameObject.SetActive(false);
        //applyWindowAnim.SetBool("show", false);       
    }

    public void ApplyToUnchanged()
    {
        eficienciaAtual = float.Parse(eficienciaInput.text);
        despachoAtual = float.Parse(despachoInput.text);
        capacidadeAtual = float.Parse(capacidadeInput.text);
        custoCrioAtual = float.Parse(custoCrioInput.text);
        custoIntAtual = float.Parse(custoIntInput.text);

        Presets sessionPresets = sessionManager.session.presets;
        sessionPresets.usinaPreset.eficiencia = (float)eficienciaAtual;
        sessionPresets.usinaPreset.despacho = (float)despachoAtual;
        sessionPresets.frotaPreset.capacidadeLiquido = (float)capacidadeAtual;
        sessionPresets.gasCrioPreset.custoBase = custoCrioAtual;
        sessionPresets.gasIntPreset.custoBase = custoIntAtual;

        foreach (Usina u in sessionManager.session.usinasColocadas)
        {
            if (u.eficiencia == eficienciaAnterior)
                u.eficiencia = (float)eficienciaAtual;

            if (u.despacho == despachoAnterior)
                u.despacho = (float)despachoAtual;
        }

        foreach (Frota f in sessionManager.session.frotasColocadas)
            if (f.capacidadeLiquido == capacidadeAnterior)
                f.capacidadeLiquido = (float)capacidadeAtual;

        foreach (GasodutoCriogenico g in sessionManager.session.gasCrioColocados)
            if (g.custoBase == custoCrioAnterior)
                g.custoBase = custoCrioAtual;

        foreach (GasodutoIntegrador g in sessionManager.session.gasIntColocados)
            if (g.custoBase == custoIntAnterior)
                g.custoBase = custoIntAtual;

        DisableEdition();

        saveButton.GetComponent<Button>().interactable = true;
        CancelButton.GetComponent<Button>().interactable = true;

        applyWindowAnim.gameObject.SetActive(false);
        //applyWindowAnim.SetBool("show", false);

        if (iconPanel.isOpen)
        {
            ChangeIconPanelInputFields(false);
        }
    }

    public void EditButton()
    {
        eficienciaInput.interactable = true;
        despachoInput.interactable = true;
        capacidadeInput.interactable = true;
        custoCrioInput.interactable = true;
        custoIntInput.interactable = true;

        eficienciaAnterior = eficienciaAtual;
        despachoAnterior = despachoAtual;
        capacidadeAnterior = capacidadeAtual;
        custoCrioAnterior = custoCrioAtual;
        custoIntAnterior = custoIntAtual;

        saveButton.SetActive(true);
        CancelButton.SetActive(true);
        editButton.SetActive(false);

    }

    public void ShowSaveOptions()
    {
        applyWindowAnim.gameObject.SetActive(true);
        //applyWindowAnim.SetBool("show", true);

        DisableEdition();
        saveButton.GetComponent<Button>().interactable = false;
        CancelButton.GetComponent<Button>().interactable = false;
    }

    public void CancelEdition()
    {
        applyWindowAnim.gameObject.SetActive(false);
        SetPresetsFields();
        DisableEdition();
        saveButton.GetComponent<Button>().interactable = true;
        CancelButton.GetComponent<Button>().interactable = true;

        eficienciaInput.text = eficienciaAnterior.ToString();
        despachoInput.text = despachoAnterior.ToString();
        capacidadeInput.text = capacidadeAnterior.ToString();
        custoCrioInput.text = custoCrioAnterior.ToString();
        custoIntInput.text = custoIntAnterior.ToString();

    }

    public void CancelSave()
    {
        applyWindowAnim.gameObject.SetActive(false);
        //applyWindowAnim.SetBool("show", false);

        saveButton.GetComponent<Button>().interactable = true;
        CancelButton.GetComponent<Button>().interactable = true;

        eficienciaInput.interactable = true;
        despachoInput.interactable = true;
        capacidadeInput.interactable = true;
        custoCrioInput.interactable = true;
        custoIntInput.interactable = true;
    }
    

    public void DisableEdition()
    {
        eficienciaInput.interactable = false;
        despachoInput.interactable = false;
        capacidadeInput.interactable = false;
        custoCrioInput.interactable = false;
        custoIntInput.interactable = false;

        editButton.SetActive(true);
        saveButton.SetActive(false);
        CancelButton.SetActive(false);
    }

    void ChangeIconPanelInputFields(bool applyToAll)
    {
        if (applyToAll)
        {
            if (iconPanel._eficiencia != null)
            {
                iconPanel._eficiencia.GetComponentInChildren<InputField>().text = eficienciaAtual.ToString();
            }
           
            if (iconPanel._despacho != null)
            {
                iconPanel._despacho.GetComponentInChildren<InputField>().text = despachoAtual.ToString();
            }
            
            if (iconPanel._gasIntCusto != null)
            {
                iconPanel._gasIntCusto.GetComponentInChildren<InputField>().text = custoIntAtual.ToString();
            }
           
            if (iconPanel._gasCrioCusto != null)
            {
                iconPanel._gasCrioCusto.GetComponentInChildren<InputField>().text = custoCrioAtual.ToString();
            }            
           
            if (iconPanel._capacidadeLiq != null)
            {
                iconPanel._capacidadeLiq.GetComponentInChildren<InputField>().text = capacidadeAtual.ToString();
            }
        }
        else
        {
            if (iconPanel._eficiencia != null)
            {
                if (iconPanel._eficiencia.GetComponentInChildren<InputField>().text == eficienciaAnterior.ToString())
                iconPanel._eficiencia.GetComponentInChildren<InputField>().text = eficienciaAtual.ToString();
            }

            if (iconPanel._despacho != null)
            {
                if (iconPanel._despacho.GetComponentInChildren<InputField>().text == despachoAnterior.ToString())
                    iconPanel._despacho.GetComponentInChildren<InputField>().text = despachoAtual.ToString();
            }

            if (iconPanel._gasIntCusto != null)
            {
                if (iconPanel._gasIntCusto.GetComponentInChildren<InputField>().text == custoIntAnterior.ToString())
                    iconPanel._gasIntCusto.GetComponentInChildren<InputField>().text = custoIntAtual.ToString();
            }

            if (iconPanel._gasCrioCusto != null)
            {
                if (iconPanel._gasCrioCusto.GetComponentInChildren<InputField>().text == custoCrioAnterior.ToString())
                    iconPanel._gasCrioCusto.GetComponentInChildren<InputField>().text = custoCrioAtual.ToString();
            }

            if (iconPanel._capacidadeLiq != null)
            {
                if (iconPanel._capacidadeLiq.GetComponentInChildren<InputField>().text == capacidadeAnterior.ToString())
                    iconPanel._capacidadeLiq.GetComponentInChildren<InputField>().text = capacidadeAtual.ToString();
            }
        }
    }
}
