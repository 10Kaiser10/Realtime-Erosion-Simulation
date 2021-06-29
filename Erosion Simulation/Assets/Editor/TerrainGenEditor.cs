using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGenEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TerrainGenerator mc = (TerrainGenerator)target;

        if (DrawDefaultInspector())
        {
            mc.generate();
        }

        if (GUILayout.Button("Generate Terrain"))
        {
            mc.generate();
        }
    }
}
