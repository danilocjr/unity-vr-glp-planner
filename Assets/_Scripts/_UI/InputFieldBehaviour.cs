using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InputFieldBehaviour : MonoBehaviour
{
    public Text title;
    public InputField input;
    public Text unit;
    public Text description;
    public Text placeholder;
    public Text inputContent;
    public Image BG;
    public bool enableCalculation = false;
    public Sprite lockSprite;
    public Sprite unlockSprite;
    public Image lockBtnImg;

    private void Awake()
    {
        enableCalculation = false;
    }

    public void CalculationToggle()
    {
        enableCalculation = !enableCalculation;
        if (enableCalculation)
        {
            lockBtnImg.sprite = lockSprite;
            input.interactable = false;
        }
        else
        {
            lockBtnImg.sprite = unlockSprite;
            input.interactable = true;
        }
    }
    
}
