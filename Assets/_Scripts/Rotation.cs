using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(Vector3.up, Time.deltaTime * 100f);
        transform.Rotate(Vector3.left, Time.deltaTime * 50f);

    }
}
