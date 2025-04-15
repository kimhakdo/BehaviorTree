using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class SceneChanger : Editor
{
    [MenuItem("Ironcow/Scene/Sample Scene &1")]
    public static void ChangeSampleScene()
    {
        EditorSceneManager.OpenScene(Application.dataPath + "/Scenes/SampleScene.unity");
    }

    [MenuItem("Ironcow/Scene/Intro Scene &1")]
    public static void ChangeIntroScene()
    {
        EditorSceneManager.OpenScene(Application.dataPath + "/Scenes/Intro.unity");
    }

    [MenuItem("Ironcow/Scene/Thumbnail Maker Scene &2")]
    public static void ChangeThumbnailMakerScene()
    {
        EditorSceneManager.OpenScene(Application.dataPath + "/Tools/ThumbnailMaker/Scenes/ThumbnailMaker.unity");
    }

    [MenuItem("Ironcow/Scene/DontDestroy Scene &3")]
    public static void ChangeDontDestroy()
    {
        EditorSceneManager.OpenScene(Application.dataPath + "/Scenes/DontDestroy.unity");
    }
}
