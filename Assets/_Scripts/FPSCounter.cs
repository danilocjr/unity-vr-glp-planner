using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    public TextMesh textMesh;
    float fps;
    float frame;

    private void Start()
    {
        frame = 0;
        fps = 1 / Time.deltaTime;
        textMesh.text = Mathf.FloorToInt(fps).ToString();
    }
    private void Update()
    {
        frame++;
        if (frame>=40)
        {
            frame = 0;
            fps = 1 / Time.deltaTime;
            textMesh.text = Mathf.FloorToInt(fps).ToString();
        }
        
    }
}
