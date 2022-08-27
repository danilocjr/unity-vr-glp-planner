using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//SCRIPT UTLIZADO PARA MANIPULAR OS RAYS QUE SAEM DAS MÃOS
public class HandPointer : MonoBehaviour
{
    // Start is called before the first frame update

    public float defaultLength;
    public GameObject pointerLight;
    LineRenderer lineRenderer;
    public GameObject markerSign;
    void Start()
    {
        markerSign.SetActive(false);
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLength();

    }

    private void UpdateLength()
    {
        lineRenderer.SetPosition(0, transform.position);
        Vector3 lineFadeEnd = lineRenderer.GetPosition(0) + (CalculateEnd() - lineRenderer.GetPosition(0)) / 2;
        lineRenderer.SetPosition(1, lineFadeEnd);
        pointerLight.transform.position = CalculateEnd();
        
    }

    public Vector3 CalculateEnd()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        Physics.Raycast(ray, out hit, defaultLength);

        Vector3 endPosition;

        if (hit.collider)
        {
            lineRenderer.enabled = true;
            endPosition = hit.point;

        }
        else
        {
            RaycastHit hit2;
            Physics.Raycast(ray, out hit2, defaultLength, 1 << 10);
            if (hit2.collider)
            {
                endPosition = hit2.point;
            }
            else
            {
                lineRenderer.enabled = false;
                endPosition = DefaultEnd(defaultLength);
            }
        }



        return endPosition;
    }

    public RaycastHit CheckStandardHit()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        Physics.Raycast(ray, out hit, defaultLength);
        return hit;
    }


    public RaycastHit GetMarkerDestination()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        Physics.Raycast(ray, out hit, defaultLength, 1 << 10);

        if (hit.collider)
        {
            markerSign.SetActive(true);
            markerSign.transform.position = new Vector3(hit.point.x, hit.point.y + 0.003f, hit.point.z + 0.001f);

        }
        else
        {
            markerSign.SetActive(false);
        }
        return hit;
    }

    public RaycastHit GetHitFromMap()
    {
        markerSign.SetActive(false);
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        Physics.Raycast(ray, out hit, defaultLength, 1 << 10);
        return hit;
    }

    public RaycastHit GetPlacedMarker()
    {
        RaycastHit hit = new RaycastHit();
        Ray ray = new Ray(transform.position, transform.forward);

        RaycastHit _hit;
        RaycastHit _hit2;

        if (Physics.Raycast(ray, out _hit, defaultLength, 1 << 14))
        {
            hit = _hit;
        }
        else if(Physics.Raycast(ray, out _hit2, defaultLength, 1 << 10))
        {
            if (_hit2.collider.gameObject.GetComponent<PEStorage>() != null)
                hit = _hit2;
        }

        return hit;
       

    }

    public RaycastHit GetAnyMarkerOnMap()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        Physics.Raycast(ray, out hit, defaultLength);
        return hit;
    }

    Vector3 DefaultEnd(float length)
    {
        return transform.position + (transform.forward * length);
    }


}
