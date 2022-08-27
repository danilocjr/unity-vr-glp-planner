using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace eWolf.PipeBuilder.Helpers
{
    public static class NodeHelper
    {
        public static PipeBase GetPipeBase(Transform transform)
        {
            PipeBase pb = transform.GetComponent<PipeBase>();
            if (pb != null)
                return pb;

            for (int i = 0; i < 10; i++)
            {
                Transform parent = transform.transform.parent;
                if (parent == null)
                    return null;

                pb = parent.GetComponent<PipeBase>();
                if (pb != null)
                    return pb;

                transform = parent.transform;
            }

            return null;
        }

        public static void DrawAll(GameObject go)
        {
            PipeBase pb = GetPipeBase(go.transform);
            if (pb == null)
                return;

            Vector3 pos = new Vector3(0, 0, 0);
            Transform[] transforms = pb.gameObject.GetComponentsInChildren<Transform>();
            foreach (Transform tr in transforms)
            {
                PipeNode pipeNode = tr.GetComponent<PipeNode>();
                if (pipeNode != null)
                {
                    pipeNode.Draw();
                }
            }
        }

        public static string ConbindedNames(string nameA, string nameB)
        {
            List<string> names = new List<string>();
            names.Add(nameA);
            names.Add(nameB);
            return string.Join("-", names.OrderBy(x => x).ToArray());
        }

        public static string FindUniqueName(Transform parent)
        {
            Transform[] transforms = parent.gameObject.GetComponentsInChildren<Transform>();

            List<string> usedNames = new List<string>();
            foreach (Transform tr in transforms)
            {
                PipeNode pipeNode = tr.GetComponent<PipeNode>();
                if (pipeNode != null)
                {
                    usedNames.Add(pipeNode.name);
                }
            }

            for (int i = 1; i < 9000; i++)
            {
                string testName = "Pipe_" + i.ToString("0000");
                if (!usedNames.Contains(testName))
                {
                    return testName;
                }
            }
            return "RanoutOfNames";
        }
    }
}