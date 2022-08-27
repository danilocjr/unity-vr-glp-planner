using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererDottedLine : MonoBehaviour
{
    private LineRenderer lR;
    private Renderer rend;
    // Use this for initialization
    void Start()
    {
        lR = GetComponent<LineRenderer>();
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        rend.material.mainTextureScale =
           new Vector3(Vector3.Distance(lR.GetPosition(0), lR.GetPosition(lR.positionCount - 1)) / lR.widthMultiplier, 1);


    }
}
