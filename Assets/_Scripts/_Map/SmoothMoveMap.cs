using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothMoveMap : MonoBehaviour
{
    public float time = 3;
    public Vector2 toPos;

    private float angle;

    private Vector2 fromPosition;
    private bool isMovement;

    private double fromTileX, fromTileY, toTileX, toTileY;
    private int moveZoom;

    public bool moved;
    public GasClasses gasClasses;
    private void Update()
    {
        if (!isMovement) return;

        angle += Time.deltaTime / time;

        if (angle > 1)
        {
            isMovement = false;
            gasClasses.msg.text += "\n1";
            //gasClasses.ChangeAllTo3D();            
            gasClasses.msg.text += "\n2";
            moved = true;
            
            angle = 1;
        }

        double px = (toTileX - fromTileX) * angle + fromTileX;
        double py = (toTileY - fromTileY) * angle + fromTileY;
        OnlineMaps.instance.projection.TileToCoordinates(px, py, moveZoom, out px, out py);
        OnlineMaps.instance.SetPosition(px, py);

    }

    public void MoveMapToPoint(Vector2 toPosition)
    {
        gasClasses.msg.text += "\n0";
        moved = false;
        //gasClasses.ChangeAllTo2D();
        toPos = toPosition;
        fromPosition = OnlineMaps.instance.position;

        moveZoom = OnlineMaps.instance.zoom;
        OnlineMaps.instance.projection.CoordinatesToTile(fromPosition.x, fromPosition.y, moveZoom, out fromTileX, out fromTileY);
        OnlineMaps.instance.projection.CoordinatesToTile(toPosition.x, toPosition.y, moveZoom, out toTileX, out toTileY);

        if (OnlineMapsUtils.Magnitude(fromTileX, fromTileY, toTileX, toTileY) > 4)
        {
            angle = 0;

            isMovement = true;
        }
        else 
        {
            OnlineMaps.instance.position = toPosition;
            //gasClasses.ChangeAllTo3D();
        }
    }
}
