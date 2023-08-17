using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using Unity.EditorCoroutines.Editor;

public struct SpreadSheedData
{
    public eTableName Name;
    public string Data;
}
public class SynchronizationSpreadSheetEditor : EditorWindow
{
    enum eState
    {
        None,
        Progress,
        Error,
    }

    static Dictionary<eTableName, string> _tablePath = new Dictionary<eTableName, string>() {
        { eTableName.CharacterTable, "0" },
        { eTableName.StatTable, "131375409" },
        { eTableName.StatusWeightTable, "1155866909" },
        { eTableName.ObjectTable, "743699977" },
        { eTableName.EnvironmentTable, "1429184104" },
        { eTableName.ItemTable, "1748252027" },
        { eTableName.LocalizingTable, "208755469" },
        { eTableName.RecipeTable, "608669484" },
        { eTableName.FoodTable, "898814572" },
        { eTableName.PatternTable, "858678798" },
        { eTableName.FSMConfigureTable, "329086332" },
        { eTableName.SkillTable, "1382904759" },
        { eTableName.RewardTable, "1979087947" },
        { eTableName.ItemStatTable, "1333179384" },
        { eTableName.MonsterTable, "573700195" },
        { eTableName.TileTable, "1967179883" },
        { eTableName.EffectTable, "565915944" },
        { eTableName.BuffTable, "1915959329"},
        { eTableName.CropTable, "824471153"},
    };
    static Queue<eTableName> _selectTableQueue = new Queue<eTableName>();
    static Queue<SpreadSheedData> _dataQueue;
    static string _spreadSheetID = "1wVYJjXnyt6bx3Xfvcm0WXk4BNPGuOEVv_D3egvxQQS8";
    static string _url = "https://docs.google.com/spreadsheets/d/" + _spreadSheetID + "/export?format=csv&id=" + _spreadSheetID + "&gid=";

    static SynchronizationSpreadSheetEditor _window;
    eTableName _tableFlag;

    static eState _currentState;
    [MenuItem("Tools/CSVSynchronization")]
    public static void SynchronizationSpreadSheet()
    {
        if (_window == null)
        {
            _window = CreateInstance<SynchronizationSpreadSheetEditor>();
            _window.titleContent = new GUIContent("SynchronizationSpreadSheed Editor");
            _window.minSize = new Vector2(300f, 80f);
            _window.maxSize = new Vector2(300f, 80f);
            _dataQueue = new Queue<SpreadSheedData>(_tablePath.Count);
        }

        switch (_currentState) {
            case eState.Error:
                Debug.LogError("Your download failed recently. Update editor status.");
                break;
            case eState.Progress:
                return;
        }

        _window.Show();
    }
    private void OnGUI()
    {
        _tableFlag = (eTableName)EditorGUI.EnumFlagsField(new Rect(25f, 0f, 250f, 20f), _tableFlag);
        if (GUI.Button(new Rect(25f, 30f, 250f, 20f), "Download spreadsheet"))
        {
            if (_tableFlag == 0)
                return;

            EditorCoroutineUtility.StartCoroutine(IERequestWeb(_tableFlag), _window);
        }
    }
    internal static IEnumerator IERequestWeb(eTableName tableName)
    {
        yield return null;

        if(_dataQueue.Count > 0)
            goto SaveCheck;

        foreach (var table in _tablePath)
        {
            if ((table.Key & tableName) != 0)
                _selectTableQueue.Enqueue(table.Key);
        }

        _currentState = eState.Progress;
        EditorUtility.DisplayProgressBar("SynchronizationSpreadSheet", "Download ready", 0f);
        Debug.Log("Download start csv data");


        while (_selectTableQueue.Count > 0)
        {
            eTableName dequeueName = _selectTableQueue.Dequeue();
            EditorUtility.DisplayProgressBar("SynchronizationSpreadSheet", $"Download {dequeueName}", (float)_dataQueue.Count / _tablePath.Count);
            Debug.Log($"Download Start {dequeueName}");
            using (UnityWebRequest webRequest = UnityWebRequest.Get(_url + _tablePath[dequeueName]))
            {
                yield return webRequest.SendWebRequest();
                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                        _currentState = eState.Error;
                        EditorUtility.ClearProgressBar();
                        Debug.LogError($"ConnectionError");
                        yield break;
                    case UnityWebRequest.Result.DataProcessingError:
                        _currentState = eState.Error;
                        EditorUtility.ClearProgressBar();
                        Debug.LogError($"DataProcessingError");
                        yield break;
                    case UnityWebRequest.Result.ProtocolError:
                        _currentState = eState.Error;
                        EditorUtility.ClearProgressBar();
                        Debug.LogError($"ProtocolError");
                        yield break;
                }
                Debug.Log($"Download completd {dequeueName} \n {webRequest.downloadHandler.text}");
                _dataQueue.Enqueue(new SpreadSheedData()
                {
                    Name = dequeueName,
                    Data = webRequest.downloadHandler.text,
                });
            }
            }
        EditorUtility.DisplayProgressBar("SynchronizationSpreadSheet", "Download Complete", 1f);

        SaveCheck:
        yield return null;
        if (EditorUtility.DisplayDialog("다운로드에 성공했습니다.", "데이터를 덮어쓰시겠습니까?", "Ok", "No"))
        {
            string dataPath = Application.dataPath + "/Resources/Database/Table/";
            if (!System.IO.Directory.Exists(dataPath))
                System.IO.Directory.CreateDirectory(dataPath);

            while (_dataQueue.Count > 0)
            {
                SpreadSheedData data = _dataQueue.Dequeue();
                System.IO.FileStream fs = new System.IO.FileStream(dataPath + data.Name.ToString() + ".csv", System.IO.FileMode.Create);
                byte[] byteArr = System.Text.Encoding.UTF8.GetBytes(data.Data);
                fs.Write(byteArr, 0, byteArr.Length);
                fs.Close();
            }
        }
        EditorUtility.ClearProgressBar();
        _window.Close();
        _window = null;
        _currentState = eState.None;
    }
    void OnDestroy()
    {
        EditorUtility.ClearProgressBar();
    }
}
