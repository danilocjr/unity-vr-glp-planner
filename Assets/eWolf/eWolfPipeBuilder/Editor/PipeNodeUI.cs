using eWolf.PipeBuilder;
using eWolf.PipeBuilder.Helpers;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(PipeNode))]
[CanEditMultipleObjects]
public class PipeNodeUI : Editor
{
    private PipeNode _pipeNode;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUI.color = Color.green;
        if (GUILayout.Button("Build Mesh"))
        {
            PipeBase pb = NodeHelper.GetPipeBase(_pipeNode.transform);
            pb.BuildPipes();
            SetSceneDirty();
            return;
        }

        if (Selection.gameObjects.Length == 1)
        {
            GUI.color = Color.yellow;
            if (_pipeNode.CanExtendPipes())
            {
                if (GUILayout.Button("Extend Pipe"))
                {
                    Selection.objects = new GameObject[] { _pipeNode.ExtendPipe() };
                    SetSceneDirty();
                    return;
                }
            }

            GUI.color = Color.red;
            if (GUILayout.Button("Delete Node"))
            {
                PipeBase pb = NodeHelper.GetPipeBase(_pipeNode.transform);
                pb.RemoveNode(_pipeNode);

                DestroyImmediate(_pipeNode.gameObject);

                pb.SetAllModifed();
                pb.BuildPipes();
                SetSceneDirty();
                EditorGUIUtility.ExitGUI();
            }
        }

        if (Selection.gameObjects.Length == 2)
        {
            GUI.color = Color.yellow;
            if (GUILayout.Button("Insert Node Between"))
            {
                PipeBase pb = NodeHelper.GetPipeBase(_pipeNode.transform);
                pb.ClearMesh();
                List<PipeNode> nodes = new List<PipeNode>();
                foreach (GameObject o in Selection.gameObjects)
                {
                    var rnn = o.GetComponent<PipeNode>();
                    if (rnn != null)
                        nodes.Add(rnn);
                }
                if (nodes.Count == 2)
                {
                    pb.InsertNode(nodes);
                    pb.BuildPipes();
                    SetSceneDirty();
                    EditorGUIUtility.ExitGUI();
                }
            }
        }
    }

    private void OnEnable()
    {
        _pipeNode = (PipeNode)target;
    }

    private void SetSceneDirty()
    {
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }
}
