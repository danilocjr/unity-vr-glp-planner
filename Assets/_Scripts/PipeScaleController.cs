using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeScaleController : MonoBehaviour
{
    public GameObject originObj;
    public GameObject destinationObj;
    public bool canRender = true;
    Vector3 pos1;
    Vector3 pos2;

    Vector3 defaultOrientation = new Vector3(1, 0, 0);

    Vector3 center;
    float dist;
    Vector3 dirV;
    Vector3 rotAxisV;
    private void Update()
    {
        if (originObj != null && destinationObj != null)
        {
            if (originObj.activeSelf && destinationObj.activeSelf)
            {
                ScalePipe(originObj.transform.position, destinationObj.transform.position);
                if (canRender)
                foreach (MeshRenderer m in GetComponentsInChildren<MeshRenderer>())
                    m.enabled = true;
                else
                    foreach (MeshRenderer m in GetComponentsInChildren<MeshRenderer>())
                        m.enabled = false;
            }
            else
            {                
                    foreach (MeshRenderer m in GetComponentsInChildren<MeshRenderer>())
                        m.enabled = false;                              
            }
        }
        else
        {
            foreach (MeshRenderer m in GetComponentsInChildren<MeshRenderer>())
                m.enabled = false;
        }
    }

    void ScalePipe(Vector3 origin, Vector3 destination)
    {
        pos1 = origin;
        pos2 = destination;

        center = new Vector3((pos1.x + pos2.x) / 2, (pos1.y + pos2.y) / 2, (pos1.z + pos2.z) / 2);

        transform.position = center;

        dist = Vector3.Distance(pos1, pos2);

        transform.localScale = new Vector3(dist / 2, 0.5f, 0.5f);

        dirV = Vector3.Normalize(pos2 - pos1);

        rotAxisV = dirV + defaultOrientation;

        rotAxisV = Vector3.Normalize(rotAxisV);

        transform.rotation = new Quaternion(rotAxisV.x, rotAxisV.y, rotAxisV.z, 0);
    }
}
