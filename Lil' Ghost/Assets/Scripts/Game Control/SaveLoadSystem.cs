using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveLoadSystem 
{

    public static void SaveGameData(int slot)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        switch (slot)
        {
            case 1:
                {
                    string path = GameControl.savePath + "1.sav";
                    FileStream stream = File.Open(path, FileMode.OpenOrCreate);
                    SaveFile dataToSave = new SaveFile(GameControl.control.playerStats);
                    formatter.Serialize(stream, dataToSave);
                    stream.Close();
                    break;
                }
            case 2:
                {
                    string path = GameControl.savePath + "2.sav";
                    FileStream stream = new FileStream(path, FileMode.Create);
                    SaveFile dataToSave = new SaveFile(GameControl.control.playerStats);
                    formatter.Serialize(stream, dataToSave);
                    stream.Close();
                    break;
                }
            case 3:
                {
                    string path = GameControl.savePath + "3.sav";
                    FileStream stream = new FileStream(path, FileMode.Create);
                    SaveFile dataToSave = new SaveFile(GameControl.control.playerStats);
                    formatter.Serialize(stream, dataToSave);
                    stream.Close();
                    break;
                }
            case 4:
                {
                    string path = GameControl.savePath + "4.sav";
                    FileStream stream = new FileStream(path, FileMode.Create);
                    SaveFile dataToSave = new SaveFile(GameControl.control.playerStats);
                    formatter.Serialize(stream, dataToSave);
                    stream.Close();
                    break;
                }
        }

    }   

    public static SaveFile LoadGameData(int slot)
    {
        string pathToLoad = GameControl.savePath + slot + ".sav";
        if (File.Exists(pathToLoad))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(pathToLoad, FileMode.Open);
            SaveFile dataToLoad = formatter.Deserialize(stream) as SaveFile;
            stream.Close();
            return dataToLoad;
        }
        else
        {
            Debug.Log("NO Save for Slot " + slot);

            return null;
        }

    }

    public static void DeleteSaveFile(int slot)
    {
        string pathToDelete = GameControl.savePath + slot + ".sav";
        File.Delete(pathToDelete); 
    }

}
