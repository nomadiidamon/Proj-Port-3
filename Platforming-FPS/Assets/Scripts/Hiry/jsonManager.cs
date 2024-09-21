using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class jsonManager : MonoBehaviour
{
    private string filePath;

    private void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "gunList.json");
    }

    public void SaveGunList(List<gunStats> gunList)
    {
        string json = JsonUtility.ToJson(new Serialization<gunStats>(gunList));
        File.WriteAllText(filePath, json);
    }

    public List<gunStats> LoadGunList()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            Serialization<gunStats> deserialized = JsonUtility.FromJson<Serialization<gunStats>>(json);
            return deserialized.ToList();
        }
        return new List<gunStats>();
    }
}


[System.Serializable]
public class Serialization<T>
{
    public List<T> list;

    public Serialization(List<T> list)
    {
        this.list = list;
    }

    public List<T> ToList()
    {
        return list;
    }
}
