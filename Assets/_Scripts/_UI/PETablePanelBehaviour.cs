using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class PETablePanelBehaviour : MonoBehaviour
{
    [SerializeField] SessionManager sessionManager;
    [Header("UI")]
    [SerializeField] Text title;
    private PontoDeEntrega selectedPe;

    [Header("Gasoduto de Transporte")]
    [SerializeField] InputField valorMalhaInput;

    [Header("De")]
    [SerializeField] List<Text> deFields;

    [Header("Até")]
    [SerializeField] List<InputField> ateFields;

    [Header("PRC")]
    [SerializeField] List<InputField> prcFields;

    [Header("PUC")]
    [SerializeField] List<InputField> pucFields;

    [Header("Keyboard")]
    [SerializeField] OnScreenKeyboard osk;



    public void SetTable(PontoDeEntrega pe)
    {
        selectedPe = pe;

        title.text = selectedPe.nome;

        valorMalhaInput.text = selectedPe.valorMalha.ToString();

        for (int x = 0; x < selectedPe.coluna_de.Length; x++)
        {
            selectedPe.coluna_de[x] = double.Parse(deFields[x].text.Replace(".", ""), CultureInfo.InvariantCulture);

            if (selectedPe.coluna_ate[x] <= 0)
            {
                if (x < selectedPe.coluna_de.Length - 1)
                {
                    ateFields[x].text = (double.Parse(deFields[x + 1].text.Replace(".", ""), CultureInfo.InvariantCulture) - 1).ToString();
                }
                else
                {
                    ateFields[x].text = 90000000.ToString();
                }
            }
            else
            {
                ateFields[x].text = selectedPe.coluna_ate[x].ToString();
            }


            prcFields[x].text = selectedPe.coluna_prc[x].ToString();
            pucFields[x].text = selectedPe.coluna_puc[x].ToString();

        }
    }

    public void HideSelf()
    {
        if (gameObject.activeSelf)
        {
            if (string.IsNullOrWhiteSpace(valorMalhaInput.text))
                valorMalhaInput.text = "3.88327";

            selectedPe.valorMalha = double.Parse(valorMalhaInput.text);

            for (int x = 0; x < deFields.Count; x++)
            {
                if (string.IsNullOrWhiteSpace(ateFields[x].text))
                {
                    if(x<deFields.Count-1)
                    {
                        ateFields[x].text = deFields[x + 1].text.Replace(".", "");
                    }
                    else
                    {
                        ateFields[x].text = 90000000.ToString();
                    }
                }

                if (string.IsNullOrWhiteSpace(prcFields[x].text))
                    prcFields[x].text = "0";

                if (string.IsNullOrWhiteSpace(pucFields[x].text))
                    pucFields[x].text = "0";

                selectedPe.coluna_ate[x] = double.Parse(ateFields[x].text);
                selectedPe.coluna_prc[x] = double.Parse(prcFields[x].text);
                selectedPe.coluna_puc[x] = double.Parse(pucFields[x].text);
            }

            sessionManager.UpdateSession();

            osk.SetActive(false);

            gameObject.SetActive(false);
        }
    }
}
