using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using eWolf.PipeBuilder;
using System;

public class PipeCreator : MonoBehaviour
{
    public Transform[] linePoints;
    Transform chosenOriginPoint;
    Transform chosenDestPoint;

    public List<PipeNode> pipeNodes = new List<PipeNode>();
    public List<Transform> pipePath = new List<Transform>();

    public GameObject pipeBasePrefab;

    public GameObject pipeBaseObj;
    PipeBase pipeBase;
    public GameObject destination = null;
    bool pipeCreated;
    bool hasDistance;


    private void Start()
    {
        hasDistance = false;
    }
    public void DeletePipe()
    {        
        pipeCreated = false;
        foreach (Transform child in pipeBase.transform)
        {
            Destroy(child.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (pipeBaseObj != null)
        {
            pipeBaseObj.transform.position = transform.position;
        }
        if (destination != null)
        {
           
            if (destination.activeSelf)
            {
                if (!hasDistance)
                {
                    pipeBaseObj = Instantiate(pipeBasePrefab);
                    pipeBaseObj.transform.position = transform.position;
                    pipeBase = pipeBaseObj.GetComponent<PipeBase>();
                    List<float> distances = new List<float>();
                    List<float> sortedDistances = new List<float>();
                    foreach (Transform t1 in linePoints)
                    {
                        foreach (Transform t2 in destination.GetComponentInChildren<PipeCreator>().linePoints)
                        {
                            float dist = Vector3.Distance(t1.position, t2.position);
                            distances.Add(dist);
                            sortedDistances.Add(dist);
                        }
                    }
                    for (int x = 0; x < sortedDistances.Count; x++)
                    {
                        for (int y = sortedDistances.Count - 1; y > x; y--)
                        {
                            if (sortedDistances[x] > sortedDistances[y])
                            {
                                float temp = sortedDistances[x];
                                sortedDistances[x] = sortedDistances[y];
                                sortedDistances[y] = temp;
                            }
                        }
                    }
                    foreach (float f in distances)
                    {
                        if (f == sortedDistances[0])
                        {
                            int index = distances.IndexOf(f);

                            int chosenOriginIndex = Convert.ToInt32(Math.Truncate((decimal)index / 8));
                            chosenOriginPoint = linePoints[chosenOriginIndex];

                            int chosenDestIndex = index - (chosenOriginIndex * 8);
                            chosenDestPoint = destination.GetComponentInChildren<PipeCreator>().linePoints[chosenDestIndex];
                        }
                    }
                    pipePath.Add(chosenOriginPoint);
                    Transform p2 = Instantiate(chosenOriginPoint.gameObject, transform).transform;
                    p2.localPosition = new Vector3(p2.localPosition.x, p2.localPosition.y, p2.localPosition.z + 0.01f);
                    pipePath.Add(p2);

                    Transform p3 = Instantiate(chosenDestPoint.gameObject, chosenDestPoint.transform.parent).transform;
                    p3.localPosition = new Vector3(p3.localPosition.x, p3.localPosition.y, p3.localPosition.z + 0.01f);

                    pipePath.Add(p3);

                    pipePath.Add(chosenDestPoint);
                    pipeBase.AddPipes();
                    bool first = true;
                    PipeNode pipe = new PipeNode();
                    foreach (Transform t in pipePath)
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
                        pipe.transform.position = t.position;
                        pipeNodes.Add(pipe);
                    }
                    pipeBase.BuildPipes();
                    hasDistance = true;
                }
                else if (pipePath.Count == 4)
                {
                    for (int x = 0; x < pipePath.Count; x++)
                    {
                        pipeNodes[x].transform.position = pipePath[x].position;
                        pipeBase.BuildPipes();
                    } 
                }
            }
        }
    }
}
