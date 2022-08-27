using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using eWolf.PipeBuilder;

public class TestPipe : MonoBehaviour
{

    public PipeBase pipeBase;
    public GameObject[] nodeAnchors;
    public List<PipeNode> nodes;

    public TextMesh txt;

    // Start is called before the first frame update
    void Start()
    {
        pipeBase.AddPipes();
        bool first = true;
        PipeNode pipe = new PipeNode();
        foreach (GameObject go in nodeAnchors)
        {            
            if (first)
            {
                pipe = pipeBase.AddPipes().GetComponent<PipeNode>();
                first = false;
            }
            else
            {
                pipe = pipe.ExtendPipe().GetComponent<PipeNode>();
            }
            pipe.transform.position = go.transform.position;
            nodes.Add(pipe);
            print(pipe.gameObject.name);
        }
        pipeBase.BuildPipes();
    }

    // Update is called once per frame
    void Update()
    {
        for (int x = 0; x < nodeAnchors.Length; x++)
        {
            nodes[x].transform.position = nodeAnchors[x].transform.position;
            nodes[x].enabled = true;
            nodes[x].gameObject.SetActive(true);
            txt.text = nodes[x].transform.position.ToString();
            pipeBase.BuildPipes();
        }

        if (Input.GetKey(KeyCode.Space))
        {
            nodeAnchors[1].transform.position = new Vector3(nodeAnchors[1].transform.position.x + 0.01f, nodeAnchors[1].transform.position.y,
                nodeAnchors[1].transform.position.z);
        }
    }
}
