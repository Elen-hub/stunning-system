using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Newtonsoft.Json;

public class SceneSaveEditor : EditorWindow
{
    [MenuItem("Tools/Build/SceneDataBuild")]
    public static void SceneDataBuild()
    {
        List<ChunkRootData> sceneDataArr;

        string path = Application.dataPath + "/Resources/Database/SceneData";
        if(!Directory.Exists(path))
            return;

        string[] directories = Directory.GetDirectories(path);
        int cullingLength = (Application.dataPath + "/Resources/").Length;
        sceneDataArr = new List<ChunkRootData>(directories.Length);
        if (directories != null && directories.Length > 0)
        {
            for (int i = 0; i < directories.Length; ++i)
            {
                string[] positionSplit = directories[i].Split("SceneData")[1].Split('(')[1].Split(')')[0].Split(',');
                sceneDataArr.Add(new ChunkRootData()
                {
                    Position = new Vector2Int(int.Parse(positionSplit[0]), int.Parse(positionSplit[1])),
                    RootPath = directories[i].Substring(cullingLength, directories[i].Length - cullingLength).Replace("\\", "/")
                });
            }
            string json = JsonConvert.SerializeObject(sceneDataArr, Formatting.Indented);
            FileStream fs = new FileStream(path + "/SceneBuildData.json", FileMode.Create);
            byte[] byteArr = System.Text.Encoding.UTF8.GetBytes(json);
            fs.Write(byteArr, 0, byteArr.Length);
            fs.Close();
        }
    }
}
