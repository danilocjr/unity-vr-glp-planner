using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
//SCRIPT UTILIZADO PARA GERAR AS LINHAS FIXAS NO MAPA (MALHAS)
public class LinesGenerator : MonoBehaviour
{
    public RasterizeLines rasterizer;
    // Start is called before the first frame update
    public GameObject pointMarkerPrefab;

    public OnlineMaps maps;
    public OnlineMapsMarker3DManager markerManager;
    public OnlineMapsTileSetControl tileSet;


    public GameObject gasLinePrefab;
    public GameObject eletricLinePrefab;

    public GameObject gasDottedLinePrefab;

    public TextAsset gasPipelineFile;
    public TextAsset transmissionLineFile;

    public TextAsset gasPipelineInfosFile;
    public TextAsset eletricLineInfosFile;

    string gasPipelineString;
    string transmissionLineString;
    string gasPipelineInfoString;
    string eletricLineInfoString;

    public Pipelines pipelines;
    public PipelineInfos pipelineInfos;

    public EletricLines eletricLines;
    public EletricLineInfos eletricLineInfos;

    public List<OnlineMapsMarker3D> gasMarkerPoints = new List<OnlineMapsMarker3D>();

    public bool canDrawPipes;

    public Toggle gasToggle;

    public Material dotMaterial;

    List<GasPath> gasodutosExistentes;
    List<GasPath> gasodutosEmEstudo;

    List<OnlineMapsDrawingLine> drawingLines;

    public Texture2D dotTex;

    public TextMesh debug;

    private void Start()
    {       
        
        rasterizer = GetComponent<RasterizeLines>();
        tileSet = maps.GetComponent<OnlineMapsTileSetControl>();
        gasPipelineString = gasPipelineFile.text;
        //transmissionLineString = transmissionLineFile.text;
        gasPipelineInfoString = gasPipelineInfosFile.text;
        //eletricLineInfoString = eletricLineInfosFile.text;

        pipelines = JsonUtility.FromJson<Pipelines>(gasPipelineString);
        //eletricLines = JsonUtility.FromJson<EletricLines>(transmissionLineString);
        pipelineInfos = JsonUtility.FromJson<PipelineInfos>(gasPipelineInfoString);
        //eletricLineInfos = JsonUtility.FromJson<EletricLineInfos>(eletricLineInfoString);



        canDrawPipes = false;

        gasodutosExistentes = new List<GasPath>();
        gasodutosEmEstudo = new List<GasPath>();
        drawingLines = new List<OnlineMapsDrawingLine>();
        //StartCoroutine(GenerateDetailedLines());

        gasToggle.interactable = false;


        GenerateDetailedLines();

        
    }


    void GenerateDetailedLines()
    {


        gasToggle.interactable = false;


        for (int x = 0; x < pipelines.paths.Count; x++)
        {
            if (pipelineInfos.pipelines[x].attributes.Categoria.Contains("Existente"))
            {
                gasodutosExistentes.Add(pipelines.paths[x]);
            }
            else
            {
                gasodutosEmEstudo.Add(pipelines.paths[x]);
            }
        }

        
        foreach (GasPath g1 in gasodutosExistentes)
        {
            List<Vector2> v2List = new List<Vector2>();
            int z = 0;
            foreach (GasPoint g2 in g1.path)
            {
                if (z == 0 || z % 500 == 0 || z == g1.path.Count - 1)
                {
                    Vector2 v2 = new Vector2(Convert.ToSingle(g2.lon), Convert.ToSingle(g2.lat));
                    v2List.Add(v2);
                }
            }
            OnlineMapsDrawingLine line = new OnlineMapsDrawingLine(v2List, Color.yellow, 0.025f);
            OnlineMapsDrawingElementManager.AddItem(line);
            drawingLines.Add(line);
            z++;

            line.visible = canDrawPipes;
        }


        
        foreach (GasPath g1 in gasodutosEmEstudo)
        {
            List<Vector2> v2List = new List<Vector2>();
            int y = 0;
            foreach (GasPoint g2 in g1.path)
            {
                if (y == 0 || y % 100 == 0 || y == g1.path.Count - 1)
                {
                    Vector2 v2 = new Vector2(Convert.ToSingle(g2.lon), Convert.ToSingle(g2.lat));
                    v2List.Add(v2);
                }
            }

            OnlineMapsDrawingLine line = new OnlineMapsDrawingLine(v2List, Color.green, 0.1f);
            line.texture = dotTex;
            OnlineMapsDrawingElement lineInstance = OnlineMapsDrawingElementManager.AddItem(line);
            line.visible = canDrawPipes;
            drawingLines.Add(line);

            y++;

        }

        gasToggle.interactable = true;



    }    


    public void GasToggle(bool response)
    {
        canDrawPipes = response;
        foreach (OnlineMapsDrawingLine line in drawingLines)
        {
            line.visible = canDrawPipes;
        }
    }


}

