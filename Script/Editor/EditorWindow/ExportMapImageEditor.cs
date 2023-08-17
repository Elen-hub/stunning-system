using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

public class ExportMapImageEditor
{
    static bool _waitCapture;
    static Camera _camera;
    static string _targetFilePath = "";

    public static void Export(Vector3 offset, string filePath)
    {
        if (File.Exists(filePath))
            File.Delete(filePath);

        _waitCapture = true;
        _camera = SettingCamera(offset, 512f, 512f);

         _targetFilePath = filePath;
        EditorApplication.update += CompleteCallback;
        StageCapture(_camera, filePath);
    }
    static void StageCapture(Camera camera, string filePath)
    {
        RenderTexture rt = new RenderTexture(1920, 1080, 24);
        camera.targetTexture = rt;
        camera.Render();
        RenderTexture.active = rt;

        float x = (1920 - 1080) / 2;
        Texture2D screenShot = new Texture2D(1080, 1080, TextureFormat.RGB24, false);
        screenShot.ReadPixels(new Rect(x, 0, 1080, 1080), 0, 0);
        screenShot.Apply();

        byte[] bytes = screenShot.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);

    }
    static Camera SettingCamera(Vector3 offset, float width, float height)
    {
        Camera camera = Object.Instantiate(PrefabUtility.LoadPrefabContents("Assets/ExportCapturingCamera.prefab")).GetComponent<Camera>();
        camera.rect = new Rect((1 - 1080f / 1920f) * 0.5f, 0, 1080f / 1920f, 1);

        float hypotenuse = Mathf.Sqrt(width * width + height * height * 0.5f * 0.5f);
        camera.transform.position = new Vector3(offset.x, offset.y, -10);
        camera.fieldOfView = Mathf.Acos(width / hypotenuse) * Mathf.Rad2Deg;
        return camera;
    }
    static void CompleteCallback()
    {
        if (_waitCapture)
        {
            if (File.Exists(_targetFilePath))
            {
                RenderTexture.active = null;
                Object.DestroyImmediate(_camera.gameObject);
                _waitCapture = false;
                EditorApplication.update -= CompleteCallback;
            }
        }
    }
}
