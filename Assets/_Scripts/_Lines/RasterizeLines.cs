using System.Collections;
using System.IO;
using UnityEngine;

public class RasterizeLines : MonoBehaviour
{
    public int zoomFrom = 1;
    public int zoomTo = 10;

    private bool started = false;
    private IEnumerator routine;
    private Texture2D texture;
    private Color32[] colors;
    private OnlineMaps map;

    private void Finish()
    {
        Destroy(texture);
        texture = null;
        colors = null;
        map = null;

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }

    private void GetBounds(out double lx, out double ty, out double rx, out double by)
    {
        lx = double.MaxValue;
        ty = double.MinValue;
        rx = double.MinValue;
        by = double.MaxValue;

        foreach (var el in OnlineMapsDrawingElementManager.instance)
        {
            if (el is OnlineMapsDrawingRect)
            {
                OnlineMapsDrawingRect rect = el as OnlineMapsDrawingRect;
                UpdateBounds(ref lx, ref ty, ref rx, ref by, rect.x, rect.y);
                UpdateBounds(ref lx, ref ty, ref rx, ref by, rect.x + rect.width, rect.y + rect.height);
                continue;
            }

            IEnumerable points;
            if (el is OnlineMapsDrawingLine)
            {
                points = (el as OnlineMapsDrawingLine).points;
            }
            //else if (el is OnlineMapsDrawingPoly) points = (el as OnlineMapsDrawingPoly).points;
            else continue;

            
            int valueType = -1; // 0 - Vector2, 1 - float, 2 - double, 3 - OnlineMapsVector2d
            object v1 = null;
            int i = -1;
            double ppx = 0;

            foreach (object p in points)
            {
                i++;

                if (valueType == -1)
                {
                    if (p is Vector2) valueType = 0;
                    else if (p is float) valueType = 1;
                    else if (p is double) valueType = 2;
                    else if (p is OnlineMapsVector2d) valueType = 3;
                }
                object v2 = v1;
                v1 = p;

                double px = 0;
                double py = 0;

                if (valueType == 0)
                {
                    if (i == 0)
                    {
                        Vector2 p1 = (Vector2)v1;
                        ppx = p1.x;
                        UpdateBounds(ref lx, ref ty, ref rx, ref by, p1.x, p1.y);
                        continue;
                    }

                    Vector2 v = (Vector2)v1;
                    px = v.x;
                    py = v.y;
                }
                else if (valueType == 3)
                {
                    if (i == 0)
                    {
                        OnlineMapsVector2d p1 = (OnlineMapsVector2d)v1;
                        ppx = p1.x;
                        UpdateBounds(ref lx, ref ty, ref rx, ref by, p1.x, p1.y);
                        continue;
                    }

                    Vector2 v = (OnlineMapsVector2d)v1;
                    px = v.x;
                    py = v.y;
                }
                else if (i % 2 == 1)
                {
                    if (i == 1)
                    {
                        if (valueType == 1)
                        {
                            ppx = (float)v2;
                            UpdateBounds(ref lx, ref ty, ref rx, ref by, ppx, (float)v1);
                        }
                        else
                        {
                            ppx = (double)v2;
                            UpdateBounds(ref lx, ref ty, ref rx, ref by, ppx, (double)v1);
                        }
                        continue;
                    }

                    if (valueType == 1)
                    {
                        px = (float)v2;
                        py = (float)v1;
                    }
                    else
                    {
                        px = (double)v2;
                        py = (double)v1;
                    }
                }

                while (true)
                {
                    double ox = px - ppx;

                    if (ox > 180) px -= 360;
                    else if (ox < -180) px += 360;
                    else break;
                }

                UpdateBounds(ref lx, ref ty, ref rx, ref by, px, py);

                ppx = px;
            }
        }
    }

    private void OnGUI()
    {
        if (!started)
        {
            if (GUILayout.Button("Rasterize"))
            {
                routine = Rasterize();
                StartCoroutine(routine);
            }
        }
        else
        {
            if (GUILayout.Button("Cancel"))
            {
                StopCoroutine(routine);
                Finish();
            }
        }
    }

    public IEnumerator Rasterize()
    {
        if (OnlineMapsDrawingElementManager.CountItems == 0) yield break;
        print(OnlineMapsDrawingElementManager.CountItems);
        double lx, ty, rx, by;
        GetBounds(out lx, out ty, out rx, out by);

        map = OnlineMaps.instance;
        texture = new Texture2D(256, 256, TextureFormat.ARGB32, false);
        colors = new Color32[256 * 256];

        for (int z = zoomFrom; z <= zoomTo; z++)
        {
            int max = 1 << z;

            double tlx, tty, trx, tby;
            map.projection.CoordinatesToTile(lx, ty, z, out tlx, out tty);
            map.projection.CoordinatesToTile(rx, by, z, out trx, out tby);

            int ilx = (int)tlx;
            int ity = (int)tty;
            int irx = (int)trx;
            int iby = (int)tby;

            if (ilx > irx) irx += max;

            for (int x = ilx; x <= irx; x++)
            {
                int cx = x;
                if (cx >= max) cx -= max;

                for (int y = ity; y <= iby; y++)
                {
                    yield return RasterizeTile(z, cx, y);
                }
            }
        }

        Finish();
    }

    private IEnumerator RasterizeTile(int zoom, int x, int y)
    {
        yield return null;

        //Color32 empty = new Color32(255, 255, 255, 0);
        //for (int i = 0; i < 256 * 256; i++) colors[i] = empty;

        Vector2 bufferPos = map.buffer.topLeftPosition;
        double sx, sy;
        map.projection.TileToCoordinates(bufferPos.x, bufferPos.y, map.buffer.lastState.zoom, out sx, out sy);

        Vector2 bufferPosition = new Vector2(x, y);

        foreach (OnlineMapsDrawingLine el in map.control.drawingElementManager)
        {
            el.Draw(colors, bufferPosition, 256, 256, zoom);
        }
        bool hasColor = false;
        for (int i = 0; i < 256 * 256; i++)
        {
            if (colors[i].a != 0)
            {
               
                hasColor = true;
                break;
            }            
        }
        if (!hasColor)
        {
           Debug.Log("Ignore " + zoom + "/" + x + "/" + y);
            yield break;
        }

        texture.SetPixels32(colors);
        texture.Apply(false);

        byte[] bytes = texture.EncodeToPNG();

        string path = Application.dataPath + "/Resources/DrawingTiles/" + zoom + "/" + x + "/" + y + ".png";
        Debug.Log(path);
        FileInfo info = new FileInfo(path);
        if (!info.Directory.Exists) info.Directory.Create();

        File.WriteAllBytes(path, bytes);
    }

    private static void UpdateBounds(ref double lx, ref double ty, ref double rx, ref double by, double px, double py)
    {
        if (px < lx) lx = px;
        if (px > rx) rx = px;

        if (py > ty) ty = py;
        if (py < by) by = py;
    }
}