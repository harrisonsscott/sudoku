using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class UserData {
    public int[,] gridData;
    public string gridFull;

    public UserData(SudokuGrid grid){
        gridFull = grid.full;
        gridData = grid.data;
    }

    public UserData(){
        gridData = new int[2,2] { {1, 2}, {1, 2} };
        gridFull = "1201";
    }
}

public static class SaveData {
    // save data to persistent data path
    public static void SaveGrid(SudokuGrid grid){
        // BinaryFormatter formatter = new();

        // FileStream stream = new(GlobalConstants.dataPath, FileMode.Create);
        // UserData data = new();

        // formatter.Serialize(stream, data);
        // stream.Close();

        string json = JsonUtility.ToJson(grid, true);
        File.WriteAllText(GlobalConstants.dataPath, json);
    }

    public static SudokuData LoadGrid(){
        // if (File.Exists(GlobalConstants.dataPath)){
        //     BinaryFormatter formatter = new();
        //     FileStream stream = new FileStream(GlobalConstants.dataPath, FileMode.Open);

        //     UserData data = formatter.Deserialize(stream) as UserData;
        //     Debug.Log(formatter.Deserialize(stream));
        //     stream.Close();

        //     return data;
        // } else {
        //     
        // }

        if (File.Exists(GlobalConstants.dataPath)){
            SudokuData data = new();
            string json = File.ReadAllText(GlobalConstants.dataPath);
            return JsonUtility.FromJson<SudokuData>(json);
            // return JsonUtility.FromJsonOverwrite<SudokuGrid>(json, data);
        } else {
            Debug.Log("error fetching data!");
            
            return null;
        }
    }
}
