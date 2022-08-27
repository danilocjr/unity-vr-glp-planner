using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErrorIconBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    Camera cam;
    public GameObject icon3D;
    public GameObject icon2D;
    bool is3d;

    void OnEnable()
    {
        cam = Camera.main;
        Toggle3D();
    }

    // Update is called once per frame
    void Update()
    {
        if (is3d)
        {
            icon3D.transform.LookAt(cam.transform);
        }
    }

    public void Toggle3D()
    {
        icon3D.SetActive(true);
        icon2D.SetActive(false);
        is3d = true;
    }

    public void Toggle2D()
    {
        icon3D.SetActive(false);
        icon2D.SetActive(true);
        is3d = false;
    }
}
