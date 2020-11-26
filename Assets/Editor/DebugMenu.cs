using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class  DebugMenu{

    [MenuItem("Debug/Print Global Position")]
    public static void PrintGlobalPosition()
    {
        if(Selection.activeGameObject != null)
        {
            Debug.Log(Selection.activeGameObject.name + " is at " + Selection.activeGameObject.transform.position);
        }
    }

 static Tool LastTool = Tool.None;

 [MenuItem("Debug/Hide")]
    public static void OnEnable()
    {
        LastTool = Tools.current;
        Tools.current = Tool.None;
    }

    public static void OnDisable()
    {
        Tools.current = LastTool;
    }

}
