#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WorldMap))]
public class EditorConfig : Editor
{
    public override void OnInspectorGUI()
    {
        WorldMap worldMap = (WorldMap)target;

        if(DrawDefaultInspector())
        {
            if (worldMap.AutoUpdate)
            {
                worldMap.RenderWorldCombined();
            }
        }

        if (GUILayout.Button("GenerateMap"))
        {
            //worldMap.RenderWorld();    
        }
    }
}
#endif