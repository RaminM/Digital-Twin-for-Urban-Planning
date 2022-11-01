using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

public class SaveManager : MonoBehaviour
{
    public SaveData activeSave;
    public void Save()
    {
        string dataPath = Application.persistentDataPath;
        var serializer = new XmlSerializer(typeof(SaveData));
        var stream = new FileStream(dataPath + "/" + activeSave.saveName + ".CG2", FileMode.Create);
        serializer.Serialize(stream, activeSave);
        stream.Close();
    }
    public void Load()
    {
        string dataPath = Application.persistentDataPath;
        if (File.Exists(dataPath + "/" + activeSave.saveName + ".CG2"))
        {
            var serializer = new XmlSerializer(typeof(SaveData));
            var stream = new FileStream(dataPath + "/" + activeSave.saveName + ".CG2", FileMode.Open);
            activeSave = serializer.Deserialize(stream) as SaveData;
            stream.Close();
        }
    }
}
[System.Serializable]
public class SaveData
{
    public string saveName = "save";
    public List<GameObject> spawnedLights;
}
