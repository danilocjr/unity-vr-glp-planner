using eWolf.PipeBuilder.Builders;
using eWolf.PipeBuilder.Data;
using eWolf.PipeBuilder.Helpers;
using eWolf.PipeBuilder.Sections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace eWolf.PipeBuilder
{
    public class PipeBase : MonoBehaviour
    {
        public Material Material;
        public PipeSettings PipeSettings = new PipeSettings();
        
        public GameObject AddPipes()
        {
            Vector3 pos = transform.position;
            GameObject newPipe = new GameObject();

            newPipe = CreatorHelper.CreatePipe(pos, transform);

            newPipe.name = NodeHelper.FindUniqueName(transform);
            return newPipe;
        }

        public void SetAllModifed()
        {
            ClearMesh();
        }

        public void BuildPipes()
        {
            List<SectionStraightBase> sectionBuilders = new List<SectionStraightBase>();

            MeshBuilder mb = new MeshBuilder();
            Transform[] transforms = gameObject.GetComponentsInChildren<Transform>();
            foreach (Transform tr in transforms)
            {
                PipeNode pipeNode = tr.GetComponent<PipeNode>();

                if (pipeNode != null)
                {
                    pipeNode.CreateSections(sectionBuilders, this);
                }
            }

            float uvPosition = 0;
            foreach (SectionStraightBase ssb in sectionBuilders)
            {
                uvPosition = ssb.Build(uvPosition);
            }
        }

        public void RemoveNode(PipeNode pipeToRemove)
        {
            PipeNode pnReplaceA = null;
            PipeNode pnReplaceB = null;

            Transform[] transforms = gameObject.GetComponentsInChildren<Transform>();
            foreach (Transform tr in transforms)
            {
                PipeNode pipeNode = tr.GetComponent<PipeNode>();
                if (pipeNode != null)
                {
                    if (pipeNode._pipes.Contains(pipeToRemove))
                    {
                        if (pnReplaceA == null)
                        {
                            pnReplaceA = pipeNode;
                        }
                        else
                        {
                            pnReplaceB = pipeNode;
                        }
                    }
                }
            }
            if (pnReplaceA != null)
                pnReplaceA.RemoveNodeFromList(pipeToRemove);

            if (pnReplaceB != null)
                pnReplaceB.RemoveNodeFromList(pipeToRemove);

            if (pnReplaceA != null && pnReplaceB != null)
            {
                pnReplaceA.AddPipe(pnReplaceB);
            }
        }

        public void InsertNode(List<PipeNode> nodes)
        {
            PipeNode pnReplaceA = nodes[0];
            PipeNode pnReplaceB = nodes[1];

            Vector3 posBase = transform.position;
            Vector3 pos = ((pnReplaceA.transform.position) + (pnReplaceB.transform.position)) / 2;

            pnReplaceA.RemoveNodeFromList(pnReplaceB);
            pnReplaceB.RemoveNodeFromList(pnReplaceA);

            PipeNode pn = PipeNode.CreatePipe(pos, gameObject.transform);

            pnReplaceA.AddPipe(pn);
            pnReplaceB.AddPipe(pn);
        }

        public void ClearMesh()
        {
            Transform[] transforms = gameObject.GetComponentsInChildren<Transform>();
            foreach (Transform tr in transforms)
            {
                PipeNode pipeNode = tr.GetComponent<PipeNode>();
                if (pipeNode != null)
                {
                    pipeNode.GetComponent<MeshFilter>().mesh = null;

                    pipeNode.GetComponent<MeshCollider>().sharedMesh = null;
                }
            }
        }
    }
}
