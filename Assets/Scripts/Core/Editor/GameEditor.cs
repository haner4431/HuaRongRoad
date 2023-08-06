using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class CustomKeys : Editor
{
    [MenuItem("快捷命令/快速启动游戏 #a")]
    static void EasyStart()
    {
        EditorSceneManager.OpenScene("Assets/Launcher.unity");
        EditorApplication.EnterPlaymode();
    }
}
