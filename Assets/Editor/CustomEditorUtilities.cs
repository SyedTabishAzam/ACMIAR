using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections;

public class CustomEditorUtilities
{

    Tool LastTool = Tool.None;

 	[MenuItem("Debug/Hide")]
    void OnEnable()
    {
        LastTool = Tools.current;
        Tools.current = Tool.None;
    }

    void OnDisable()
    {
        Tools.current = LastTool;
    }
}