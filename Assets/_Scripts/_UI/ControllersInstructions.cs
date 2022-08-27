using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus;
public class ControllersInstructions : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] instructions;
    public GameObject instructionBtnA;
    public GameObject instructionBtnB;
    bool show = false;
    bool previousShow = false;
    void Start()
    {
        foreach (GameObject i in instructions)
        {
            i.SetActive(show);
        }
        instructionBtnB.SetActive(show);
        instructionBtnA.SetActive(!show);
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.A))
        {
            show = !show;
        }

        if (previousShow!=show)
        {
            foreach(GameObject i in instructions)
            {
                i.SetActive(show);                
            }
            instructionBtnB.SetActive(show);
            instructionBtnA.SetActive(!show);
        }
        previousShow = show;
        
    }
}