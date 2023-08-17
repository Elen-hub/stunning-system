using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using Newtonsoft.Json;

[System.Serializable]
public class Preference
{
    public bool MuteMusic;
    public float MusicVolume;
    public eLanguage Language = eLanguage.Korean;
}
public class RuntimePreference : TSingletonMono<RuntimePreference>
{
    [SerializeField]
    Preference _preference;
    public static Preference Preference => Instance._preference;
    protected override void OnInitialize()
    {
        if (!Directory.Exists(ClientConst.CacheDirectoryPath))
            Directory.CreateDirectory(ClientConst.CacheDirectoryPath);

        if (!LoadPreference())
        {
            DefaultSetting();
            SavePreference();
        }

#if UNITY_EDITOR

#else

#endif
    }
    void DefaultSetting()
    {
        _preference = new Preference()
        {
            MuteMusic = false,
            MusicVolume = 1f,
        };
    }
    public bool LoadPreference()
    {
        if (!File.Exists(ClientConst.CacheDirectoryPath + "Preference.json"))
            return false;

        FileStream fs = new FileStream(ClientConst.CacheDirectoryPath + "Preference.json", FileMode.Open);
        byte[] byteArr = new byte[fs.Length];
        try {
            fs.Read(byteArr, 0, (int)fs.Length);
            fs.Close();
        }
        catch(IOException e) {
            Debug.Log(e);
            fs.Close();
            return false;
        }
        string str = null;
        try {
            str = Encoding.UTF8.GetString(byteArr);
        }
        catch {
            Debug.Log("Encoding Error");
            return false;
        }
        Preference preference = null;
        try {
            preference = JsonConvert.DeserializeObject<Preference>(str);
        }
        catch(JsonException e) {
            Debug.Log(e);
            return false;
        }
        _preference = preference;
        return true;
    }
    public void SavePreference()
    {
        FileStream fs = new FileStream(ClientConst.CacheDirectoryPath + "Preference.json", FileMode.Create);
        string json = JsonConvert.SerializeObject(_preference, Formatting.None);
        byte[] byteArray = Encoding.UTF8.GetBytes(json);
        fs.Write(byteArray, 0, byteArray.Length);
        fs.Close();
    }
}
