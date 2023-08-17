using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.IO;

public abstract class BaseStageEditor
{
    protected StageRuntimeEditor _runtimeEditor => StageEditor.RuntimeEditor;
    protected string _sceneName;
    protected bool _isChanged;
    protected bool _isDrawGrid;
    public BaseStageEditor(string sceneName)
    {
        _sceneName = sceneName;
    }
    public abstract bool StartEditor();
    public abstract void OnGUI();
    public abstract void OnChangedScene(Scene prev, Scene after);
    public abstract void Destroy();
    protected void OnDrawGrid()
    {

    }
}
