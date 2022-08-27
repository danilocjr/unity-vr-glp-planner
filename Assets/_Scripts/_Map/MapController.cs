using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using OVR;
using System;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//SCRIPT UTILIZADO PARA MANIPULAR O MAPA
public class MapController : MonoBehaviour
{
    public TextMesh msg;

    OnlineMapsTileSetControl tileSet;
    public OnlineMaps maps;

    double lat;
    double lon;

    float curZoom;

    List<GameObject> controllers;

    //TO MOVE MAP
    Vector3 c0PrevPos;

    //TO ZOOM MAP
    Vector2 c0PrevPos2;
    Vector2 c1PrevPos2;


    public GameObject leftControllerHand;
    public GameObject rightControllerHand;

    public GameObject leftController;
    public GameObject rightController;

    public Camera centerCamera;

    HandPointer handPointer;

    Vector2 c0Pos;
    Vector2 c1Pos;

    float zoomMag;
    float initialZoomMag;
    float zoomFactor;
    public float minimumZoomMag;

    public float scaleCoof;

    bool zoomRelease;

    public float startZoom = 13;
    public float zoomOutLimit = 5.78f;
    public float zoomInLimit = 12.9f;

    MarkerDisplacer markerDisplacer;
    public float scaleFactor = 3;

    bool firstPress;

    public GasClasses gasClasses;
    public LinesGenerator linesGenerator;

    // public bool canControlMap = true;
    public GameObject lockMapBtn;
    public GameObject unlockMapBtn;
    //53.99999
    //1.40625
    public Animator zoomWarningAnim;

    bool isZooming;

    public HandPointer rightHandPointer;

    float stickX = 0;
    float stickY = 0;
    float zoomY = 0;
    void Start()
    {
        //while (maps == null)
        //    yield return null;
        //while (leftControllerHand == null)
        //    yield return null;
        //while (rightControllerHand == null)
        //    yield return null;

        markerDisplacer = GetComponent<MarkerDisplacer>();
        tileSet = maps.GetComponent<OnlineMapsTileSetControl>();
        handPointer = leftControllerHand.GetComponentInChildren<HandPointer>();
        //maps.SetPosition(-58, -10);
        //maps.SetPosition(-56.020857573999933, -15.678991052999947);
        //maps.floatZoom = zoomOutLimit;
        maps.GetPosition(out lon, out lat);
        curZoom = maps.floatZoom;
        controllers = new List<GameObject>();
        maps.floatZoom = startZoom;
        initialZoomMag = zoomMag = Convert.ToSingle(maps.bounds.top - maps.bounds.bottom);

        zoomFactor = 1;
        //maps.OnChangeZoom += ChangeMarkersScale;
        minimumZoomMag = 0.01098633f;

        scaleCoof = minimumZoomMag / zoomMag;

        markerDisplacer.markerScale = scaleCoof * maps.GetComponent<OnlineMapsMarker3DManager>().defaultScale;



    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.CompareTag("Controller"))
    //    {
    //        if (controllers.Count < 2 && !controllers.Contains(other.gameObject))
    //        {
    //            controllers.Add(other.gameObject);
    //        }
    //    }

    //    if (controllers.Count == 1)
    //    {
    //        c0PrevPos = controllers[0].transform.position;
    //    }
    //    if (controllers.Count == 2)
    //    {
    //        c0PrevPos2 = new Vector2(controllers[0].transform.position.x, controllers[0].transform.position.z);
    //        c1PrevPos2 = new Vector2(controllers[1].transform.position.x, controllers[1].transform.position.z);
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.gameObject.CompareTag("Controller"))
    //    {
    //        if (controllers.Contains(other.gameObject))
    //        {
    //            controllers.Remove(other.gameObject);
    //        }
    //    }
    //}


    // Update is called once per frame
    void FixedUpdate()
    {
        //if (leftHand.IsTracked || rightHand.IsTracked)
        //{
        //    foreach (SkinnedMeshRenderer m in leftControllerHand.GetComponentsInChildren<SkinnedMeshRenderer>())
        //        m.enabled = false;
        //    foreach (SkinnedMeshRenderer m in rightControllerHand.GetComponentsInChildren<SkinnedMeshRenderer>())
        //        m.enabled = false;
        //}
        //else
        //{
        //    foreach (SkinnedMeshRenderer m in leftControllerHand.GetComponentsInChildren<SkinnedMeshRenderer>())
        //        m.enabled = true;
        //    foreach (SkinnedMeshRenderer m in rightControllerHand.GetComponentsInChildren<SkinnedMeshRenderer>())
        //        m.enabled = true;
        //}

        //if (controllers != null)
        //{
        //    if (controllers.Count == 1)
        //    {
        //        if ((OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) ^ OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger)) ||
        //            rightHand.GetFingerIsPinching(OVRHand.HandFinger.Index) ^ leftHand.GetFingerIsPinching(OVRHand.HandFinger.Index))
        //        {
        //            if (!firstPress)
        //            {
        //                c0PrevPos = controllers[0].transform.position;
        //                firstPress = true;
        //            }
        //            else
        //            {
        //                firstPress = false;
        //            }
        //            GetControllerTransition();
        //        }
        //    }
        //    else if (controllers.Count == 2)
        //    {
        //        if ((OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) && OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger)) ||
        //            (rightHand.GetFingerIsPinching(OVRHand.HandFinger.Index) && leftHand.GetFingerIsPinching(OVRHand.HandFinger.Index)))
        //        {
        //            GameObject c0 = controllers[0];
        //            GameObject c1 = controllers[1];

        //            c0Pos = new Vector2(c0.transform.position.x, c0.transform.position.z);
        //            c1Pos = new Vector2(c1.transform.position.x, c1.transform.position.z);

        //            if (zoomRelease)
        //            {
        //                c0PrevPos2 = c0Pos;
        //                c1PrevPos2 = c1Pos;
        //                zoomRelease = false;
        //            }

        //            Vector2 diff = c0Pos - c1Pos;
        //            Vector2 prevDiff = c0PrevPos2 - c1PrevPos2;

        //            float mag = diff.magnitude;
        //            float prevMag = prevDiff.magnitude;

        //            float deltaMagnitudeDiff = prevMag - mag;

        //            ZoomMap(-deltaMagnitudeDiff);

        //            c0PrevPos2 = c0Pos;
        //            c1PrevPos2 = c1Pos;
        //        }
        //        else if ((OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) ^ OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger)) ||
        //            (rightHand.GetFingerIsPinching(OVRHand.HandFinger.Index) ^ leftHand.GetFingerIsPinching(OVRHand.HandFinger.Index)))
        //        {
        //            if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) || leftHand.GetFingerIsPinching(OVRHand.HandFinger.Index))
        //            {
        //                GetControllerTransition(leftController);
        //            }
        //            else
        //            {
        //                GetControllerTransition(rightController);
        //            }
        //        }
        //        else
        //        {
        //            zoomRelease = true;
        //        }
        //    }
        //}
        stickX = 0;
        stickY = 0;
        if (rightHandPointer.CheckStandardHit().collider)
        {
            if (rightHandPointer.CheckStandardHit().collider.gameObject.layer != 16)
            {
                stickX = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick).x;
                stickY = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick).y;
            }
        }
        else
        {
            stickX = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick).x;
            stickY = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick).y;
        }

        if (stickX != 0 || stickY != 0)
            MoveMap(stickX, stickY);

        zoomY = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick).y;


        if (zoomY != 0)
        {
            if (maps.floatZoom == zoomInLimit)
            {
                if (zoomY <= 0)
                {
                    ZoomMap(zoomY);
                }
            }
            else if (maps.floatZoom == zoomOutLimit)
            {
                if (zoomY >= 0)
                {
                    ZoomMap(zoomY);
                }
            }
            else if (maps.floatZoom < zoomInLimit && maps.floatZoom > zoomOutLimit)
            {
                ZoomMap(zoomY);
            }
        }
        markerDisplacer.canGrab = (maps.floatZoom >= zoomInLimit-1);
        zoomWarningAnim.SetBool("hide", maps.floatZoom >= zoomInLimit - 1);

    }

    void GetControllerTransition(GameObject controller = null)
    {
        GameObject c;
        if (controller == null)
        {
            c = controllers[0];
        }
        else
        {
            c = controller;
        }

        Vector3 diffPos = c.transform.position - c0PrevPos;
        if (Mathf.Abs(diffPos.x) < 0.1f && Mathf.Abs(diffPos.z) < 0.1f)
            MoveMap(-diffPos.x, -diffPos.z);

        c0PrevPos = c.transform.position;
    }


    void MoveMap(float x, float y)
    {
        lat += y / (2000 * zoomFactor);
        lon += x / (2000 * zoomFactor);

        if (lat > 0)
            lat = 0;
        if (lat < -40)
            lat = -40;
        if (lon < -60)
            lon = -60;
        if (lon > -34.83f)
            lon = -34.83f;



        maps.SetPosition(lon, lat);



    }

    void ZoomMap(float y)
    {
        //curZoom += y * 6f;
        //curZoom += y / 10;
        //if (curZoom >= zoomInLimit)
        //    curZoom = zoomInLimit;
        //if (curZoom <= zoomOutLimit)
        //    curZoom = zoomOutLimit;

        //zoomMag = Convert.ToSingle(maps.bounds.top - maps.bounds.bottom);

        //zoomFactor = initialZoomMag / zoomMag;

        // maps.floatZoom = curZoom;

        //ChangeMarkersScale();

        //if (maps.floatZoom <= 10)
        //{
        //    foreach (OnlineMapsMarker3D m in markerDisplacer.markers)
        //    {
        //        m.instance.GetComponent<IconToggle>().ChangeTo2d();                
        //    }
        //}
        //else
        //{
        //    foreach (OnlineMapsMarker3D m in markerDisplacer.markers)
        //    {
        //        m.instance.GetComponent<IconToggle>().ChangeTo3d();                
        //    }
        //}




        //PARA EDITOR -----------------------------
        //curZoom += y / 10;
        //if (curZoom >= zoomInLimit)
        //    curZoom = zoomInLimit;
        //if (curZoom <= zoomOutLimit)
        //    curZoom = zoomOutLimit;

        //zoomMag = Convert.ToSingle(maps.bounds.top - maps.bounds.bottom);

        //zoomFactor = initialZoomMag / zoomMag;

        //maps.floatZoom = curZoom;

        //ChangeMarkersScale();
        //-----------------------------------------


        Ray ray = new Ray(handPointer.transform.position, handPointer.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 10))
        {
            if (hit.collider)
            {
                Vector2 scrPoint = centerCamera.WorldToScreenPoint(hit.point);
                tileSet.ZoomOnPoint(y / 30, scrPoint);
                if (maps.floatZoom < zoomOutLimit)
                    maps.floatZoom = zoomOutLimit;
                if (maps.floatZoom > zoomInLimit)
                    maps.floatZoom = zoomInLimit;
                zoomMag = Convert.ToSingle(maps.bounds.top - maps.bounds.bottom);
                zoomFactor = initialZoomMag / zoomMag;
                
                ChangeMarkersScale();
            }
        }

        if (maps.floatZoom <= 13)
        {
            foreach (OnlineMapsMarker3D m in markerDisplacer.markers)
            {
                m.instance.GetComponent<IconToggle>().ChangeTo2d();
            }
        }
        else
        {
            foreach (OnlineMapsMarker3D m in markerDisplacer.markers)
            {
                m.instance.GetComponent<IconToggle>().ChangeTo3d();
            }
        }


        maps.GetPosition(out lon, out lat);
    }

    void ChangeMarkersScale()
    {
        scaleCoof = minimumZoomMag / zoomMag;

        markerDisplacer.markerScale = scaleCoof * maps.GetComponent<OnlineMapsMarker3DManager>().defaultScale;
        foreach (OnlineMapsMarker3D m in markerDisplacer.markers)
        {
            //if (!gasClasses.userMarkers2DIcons.Contains(m) && !gasClasses.gnl2DIcons.Contains(m) && !gasClasses.termo2DIcons.Contains(m) &&
            //!gasClasses.cg2DIcons.Contains(m) && !gasClasses.subEst2DIcons.Contains(m))
            if (!m.instance.GetComponent<IconToggle>().is2d)
            {
                m.scale = markerDisplacer.markerScale;
            }
            else
            {
                m.scale = 0.2f * maps.GetComponent<OnlineMapsMarker3DManager>().defaultScale;
            }
        }

    }

    public void ChangeMapToWorldImagery()
    {
        maps.mapType = "arcgis.worldimagery";

    }
    public void ChangeMapToStreetMap()
    {
        maps.mapType = "google.terrain";
    }
    public void ChangeMapToGrayStreetMap()
    {
        maps.mapType = "arcgis.worldgraycanvas";
    }

    //public void LockMap()
    //{
    //    canControlMap = false;
    //    gasClasses.ChangeAllTo3D();
    //    //linesGenerator.GenerateDetailedLines();
    //}

    //public void UnlockMap()
    //{
    //    canControlMap = true;
    //    gasClasses.ChangeAllTo2D();
    //    //linesGenerator.GenerateStandardLines();
    //    markerDisplacer.iconPanel.GetComponent<IconPanelElements>().HideSelf();
    //}
}
