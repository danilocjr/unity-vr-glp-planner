using eWolf.PipeBuilder;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PipeBase))]
[CanEditMultipleObjects]
public class PipeBaseUI : Editor
{
    private PipeBase _pipeBase;
    private readonly Texture _textureLogo = null;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        /*if (_textureLogo == null)
            _textureLogo = (Texture)Resources.Load("PipeBuilder-Logo");

        GUILayout.Box(_textureLogo, GUILayout.Height(130), GUILayout.ExpandWidth(true));*/

        GUI.color = Color.green;
        if (GUILayout.Button("Build Mesh"))
        {
            _pipeBase.SetAllModifed();
            _pipeBase.BuildPipes();
            EditorUtility.SetDirty(target);
            return;
        }

        if (GUILayout.Button("Add pipe"))
        {
            GameObject obj = _pipeBase.AddPipes();
            PipeNode pn = obj.GetComponent<PipeNode>();
            Selection.objects = new GameObject[] { pn.ExtendPipe() };
            EditorUtility.SetDirty(target);
            return;
        }

        GUI.color = Color.yellow;

        if (GUILayout.Button("Clear Mesh"))
        {
            _pipeBase.ClearMesh();
            EditorUtility.SetDirty(target);
            return;
        }
    }

    private void OnEnable()
    {
        _pipeBase = (PipeBase)target;
    }
}
