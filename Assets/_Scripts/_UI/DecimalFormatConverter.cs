using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
public class DecimalFormatConverter : MonoBehaviour
{
    InputField input;
    private void Start()
    {
        if (GetComponent<InputField>() != null)
        {
            input = GetComponent<InputField>();
            input.onValueChanged.AddListener(ConvertToPtBr);
            input.text = String.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:0.####}", input.text);
        }

    }

    public void ConvertToPtBr(string _text)
    {
        input.text = String.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:0.####}", _text);
    }


}
