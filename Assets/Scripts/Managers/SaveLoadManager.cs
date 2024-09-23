using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class SaveLoadManager : MonoBehaviour
{
    private string saveFilePath;

    private void Awake()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "savefile.json");
    }

    public void SaveCharacterData(CharacterData characterData)
    {
        string json = JsonConvert.SerializeObject(characterData, Formatting.Indented);
        File.WriteAllText(saveFilePath, json);
    }

    public CharacterData LoadCharacterData()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            return JsonConvert.DeserializeObject<CharacterData>(json);
        }

        return new CharacterData(); // Return a new CharacterData object if no save file exists
    }
}
