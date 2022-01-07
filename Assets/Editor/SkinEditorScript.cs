using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(SkinScriptableObject))]
public class SkinEditorScript : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var script = (SkinScriptableObject)target;

        if (GUILayout.Button("Set ID", GUILayout.Height(40)))
        {
            script.UpdateCounter();
        }


    }
}

[CustomEditor(typeof(VehiclesScriptableObject))]
public class VehicleEditorScript : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var script = (VehiclesScriptableObject)target;

        if (GUILayout.Button("Set ID", GUILayout.Height(40)))
        {
            script.UpdateCounter();
        }


    }
}
