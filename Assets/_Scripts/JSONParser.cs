using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JSONParser : MonoBehaviour
{
    // Start is called before the first frame update
    List<string> pathStrings;

    string pipelinesJSON;
    bool error;
    public string fileName;
    public string newFileName;
    public int minimunValidCharNumber;
    public bool getOnlyCoordinates;
    //Pipelines pipelines;

    EletricLines eletricLines;
    void Start()
    {
        pipelinesJSON = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, fileName + ".json"));

        pathStrings = new List<string>();

        //pipelines = new Pipelines();

        eletricLines = new EletricLines();

        if (getOnlyCoordinates)
            StartCoroutine(ParseJSON());

        UsinasJSON ujson = JsonUtility.FromJson<UsinasJSON>(pipelinesJSON);
        UsinasJSON newUJson = new UsinasJSON();
        newUJson.features = new List<UsinaJSON>();
        foreach(UsinaJSON u in ujson.features)
        {
            if (u.attributes.COMBUST == "Gás Natural")
            {
                newUJson.features.Add(u);
            }
        }

        string newUJSONStr = JsonUtility.ToJson(newUJson);
        File.WriteAllText( Path.Combine(Application.streamingAssetsPath, newFileName + ".json"), newUJSONStr);
    }

    IEnumerator ParseJSON()
    {
        while (pipelinesJSON.Length > minimunValidCharNumber)
        {
            int initIndex = pipelinesJSON.IndexOf("     [\n      [");
            int finalIndex = pipelinesJSON.IndexOf("]\n    ]");
            print(pipelinesJSON.Substring(initIndex,50));

            string temp = pipelinesJSON.Substring(initIndex, finalIndex - initIndex) + "]";

            temp = temp.Replace(" ", "");
            temp = temp.Replace("\n", "");
            temp = temp.Replace("],", "]_");
            temp = temp.Replace(" + ", "");
            temp = temp.Remove(0, 1);
            temp = temp.Remove(temp.Length - 1, 1);
            pathStrings.Add(temp);
            pipelinesJSON = pipelinesJSON.Remove(0, finalIndex + 1);
            yield return null;
        }

        //pipelines.paths = new List<GasPath>();
        eletricLines.paths = new List<EletricPath>();

        for (int x = 0; x < pathStrings.Count; x++)
        {
            pathStrings[x] = pathStrings[x].Replace("[", "X:");
            pathStrings[x] = pathStrings[x].Replace(",", ",Y:");
            pathStrings[x] = pathStrings[x].Replace("]", "");

            //GasPath gPath = new GasPath();
            EletricPath ePath = new EletricPath();

            //gPath.path = new List<GasPoint>();
            ePath.path = new List<EletricPoint>();

            while (pathStrings[x].Length > 0)
            {
                int xIndex = pathStrings[x].IndexOf("X:");
                int yIndex = pathStrings[x].IndexOf("Y:");
                int pointEnd;
                if (pathStrings[x].Contains("_"))
                {
                    pointEnd = pathStrings[x].IndexOf("_") - 1;
                }
                else
                {
                    pointEnd = pathStrings[x].Length - 1;
                }

                string xStr = pathStrings[x].Substring(xIndex + 2, yIndex - 3 - xIndex);
                string yStr = pathStrings[x].Substring(yIndex + 2, pointEnd - 1 - yIndex);


                if (pathStrings[x].Contains("_"))
                    pathStrings[x] = pathStrings[x].Remove(xIndex, 2 + xStr.Length + 3 + yStr.Length + 1);
                else
                    pathStrings[x] = pathStrings[x].Remove(xIndex, 2 + xStr.Length + 3 + yStr.Length);

                double pointX = 0;
                double pointY = 0;
                try
                {
                    pointX = float.Parse(xStr);
                }
                catch
                {
                    print("ERRO COM A STRING DE X: \n\n + " + xStr);
                    xStr = xStr.Replace("X:", "");
                    xStr = xStr.Replace(": ", "");
                    print("NOVA STRING: \n\n: " + xStr);
                    pointX = double.Parse(xStr);
                    print(pointX);
                    error = true;
                }

                try
                {
                    pointY = float.Parse(yStr);
                }
                catch
                {
                    print("ERRO COM A STRING DE X: \n\n + " + yStr);
                    yStr = yStr.Replace(" + Y:", "");
                    yStr = yStr.Replace(": ", "");
                    print("NOVA STRING: \n\n: " + yStr);
                    pointY = double.Parse(yStr);
                    error = true;
                }

                //GasPoint gPoint = new GasPoint();
                EletricPoint ePoint = new EletricPoint();

                //gPoint.lon = pointX;
                //gPoint.lat = pointY;
                //gPath.path.Add(gPoint);
                ePoint.lon = pointX;
                ePoint.lat = pointY;
                ePath.path.Add(ePoint);

                yield return null;
            }

            //pipelines.paths.add(gPath);
            eletricLines.paths.Add(ePath);

            print(eletricLines.paths.Count);

            string newJson = JsonUtility.ToJson(eletricLines);

            File.WriteAllText(Path.Combine(Application.streamingAssetsPath, newFileName + ".json"), newJson);

            yield return null;
        }
        print("SAIU DO LOOP");
        //string newJson = JsonUtility.ToJson(pathPoints);
        //File.WriteAllText(Path.Combine(Application.streamingAssetsPath, "Gasodutos.json"), newJson);
        print("JSON CREATED");
    }

    // Update is called once per frame
    void Update()
    {
        if (error)
            StopCoroutine(ParseJSON());
    }
}

#region GAS_PIPELINE

[Serializable]
public class PipelineInfos
{
    public PipelineInfo[] pipelines;
}
[Serializable]
public class PipelineInfo
{
    public PipelineAttributes attributes;   
}

[Serializable]
public class PipelineAttributes
{
    public string Nome_Dut_1;
    public string Categoria;
    public string Diam_Pol_x;
    public string CLASSIFIC;
    public string TIPO_USO;
    public float COMPRIM_KM;
    public string MUNIC_ORIG;
    public string MUNIC_DEST;
    public string Fontes;
    public string P_Max_Op;
    public int OBJECTID_1;
}

[Serializable]
public class Pipelines
{
    public List<GasPath> paths;
}

[Serializable]
public class GasPath
{
    public List<GasPoint> path;
}

[Serializable]
public class GasPoint
{
    public double lon;
    public double lat;
}
#endregion

#region ELETRIC_LINES

[Serializable]
public class EletricLineInfos
{
    public EletricLineInfo[] features;
}

[Serializable]
public class EletricLineInfo
{
    public EletricLineAttributes attributes;
}

[Serializable]
public class EletricLineAttributes
{
    public string Nome;
    public string Tensao;
    public int n_circ;
    public float extensao;
    public int OBJECTID;
}

[Serializable]
public class EletricLines
{
    public List<EletricPath> paths;
}

[Serializable]
public class EletricPath
{
    public List<EletricPoint> path;
}

[Serializable]
public class EletricPoint
{
    public double lon;
    public double lat;
}

#endregion

[Serializable]
public class UsinasJSON
{
    public List<UsinaJSON> features;
}

[Serializable]
public class UsinaJSON
{
    public UJSONAttributes attributes;
    public UJSONGeometry geometry;
}

[Serializable]
public class UJSONAttributes
{
    public string Nome;
    public string UF1;
    public string COMBUST;
    public double P_OUT_KW;
    public double INIC_OPER;
    public string ESTAGIO;
    public float OBJECTID;
    public string CEG;
}

[Serializable]
public class UJSONGeometry
{
    public double x;
    public double y;
}