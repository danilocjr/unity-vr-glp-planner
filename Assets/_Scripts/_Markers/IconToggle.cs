using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconToggle : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject icon3d;
    public GameObject icon2d;
    public bool is2d;
    public bool hasError;
    bool isPlaced;
    public GameObject[] partsToHide;
    private void Start()
    {
        ChangeTo3d();
        StartCoroutine(CheckIfPlaced());
    }

    public void ChangeTo3d()
    {
        icon3d.SetActive(true);
        icon2d.SetActive(false);
        is2d = false;
        if (hasError)
        {
            GetComponentInChildren<ErrorIconBehaviour>().Toggle3D();
        }
    }

    public void ChangeTo2d()
    {
        icon3d.SetActive(false);
        icon2d.SetActive(true);
        is2d = true;
        if (hasError)
        {
            GetComponentInChildren<ErrorIconBehaviour>().Toggle2D();
        }
    }

    IEnumerator CheckIfPlaced()
    {
        while (gameObject.layer != 14)
            yield return null;
        foreach (GameObject part in partsToHide)
            part.SetActive(false);

    }
}
