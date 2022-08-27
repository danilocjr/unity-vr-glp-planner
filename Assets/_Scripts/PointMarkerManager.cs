using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointMarkerManager : MonoBehaviour
{
    //public Vector3[] lastPositions = new Vector3[5];
    //public bool isFacility;
    //public bool onTable;
    //public OnlineMapsMarker3D lastPosMarker = null;
    //int index;


    //private void OnEnable()
    //{
        //if (lastPosMarker != null)
        //{
        //    OnlineMapsMarker3DManager.instance.Remove(lastPosMarker);
        //}

        //index = 0;
    //}

    //private void Update()
    //{
        //if (!onTable)
        //{
        //    lastPositions[index] = transform.position;
        //    index++;
        //    if (index > 4)
        //        index = 0;
        //}
    //}

    //private void OnDisable()
    //{
        //GameObject go = new GameObject();

        //if (go.transform == null)
        //{            
        //    go.AddComponent<Transform>();
        //}

        //go.AddComponent<PointMarkerManager>();
        //Vector2 coords = OnlineMapsTileSetControl.instance.GetCoordsByWorldPosition(lastPositions[0]);
        //lastPosMarker = OnlineMapsMarker3DManager.instance.Create(coords.x, coords.y, go);
    //}
}
