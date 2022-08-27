using eWolf.PipeBuilder;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatePipesFromScript : MonoBehaviour
{
    // Link to an pipe builder base so we will have all the setting we need.
    public PipeBase Pipe;

    private void Start()
    {
        CreateBasicPipeList();
    }
    //private void OnGUI()
    //{
    //    int y = 10;
    //    if (GUI.Button(new Rect(10, y, 250, 45), "Create basic pipes"))
    //    {
    //        CreateBasicPipe();
    //    }
    //    y += 50;
    //    if (GUI.Button(new Rect(10, y, 250, 45), "Clear Pipes"))
    //    {
    //        ClearAllPipes();
    //    }
    //    y += 50;
    //    if (GUI.Button(new Rect(10, y, 250, 45), "Create pipes from list"))
    //    {
    //        CreateBasicPipeList();
    //    }
    //}

    private void ClearAllPipes()
    {
        foreach (Transform child in Pipe.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    private void CreateBasicPipe()
    {
        GameObject go = Pipe.AddPipes();
        PipeNode pipeNode = go.GetComponent<PipeNode>();
        GameObject extendPipe = pipeNode.ExtendPipe();
        Pipe.BuildPipes();
    }

    private void CreateBasicPipeList()
    {
        List<Vector3> positions = new List<Vector3>();
        positions.Add(new Vector3(0, 0, 0));
        positions.Add(new Vector3(4, 0, 0));
        positions.Add(new Vector3(4, 0, 4));
        positions.Add(new Vector3(0, 0, 4));
        positions.Add(new Vector3(0, -4, 4));
        positions.Add(new Vector3(0, -4, 0));

        GameObject go = Pipe.AddPipes();
        bool first = true;
        PipeNode currentPipeNode = null;
        foreach (Vector3 pos in positions)
        {
            if (first)
            {
                currentPipeNode = Pipe.AddPipes().GetComponent<PipeNode>();
                first = false;
            }
            else
            {
                currentPipeNode = currentPipeNode.ExtendPipe().GetComponent<PipeNode>();
            }
            currentPipeNode.transform.position = pos;
        }
        Pipe.BuildPipes();
    }
}
