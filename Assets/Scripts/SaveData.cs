
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class UserData {
    public int[,] gridData;
    public string gridFull;
    public int score;
    public int mistakesLeft;

    public UserData(SudokuGrid grid){
        gridFull = grid.full;
        gridData = grid.data;
        score = grid.score;
        mistakesLeft = grid.mistakesLeft;
    }
}
public static class SaveData {
    // save data to persistent data path
    public static void SaveGrid(SudokuGrid grid){
        BinaryFormatter formatter = new();

        FileStream stream = new(GlobalConstants.persistentDataPath, FileMode.Create);
        UserData data = new(grid);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static SudokuGrid LoadGrid(){
        if (File.Exists(GlobalConstants.persistentDataPath)){
            BinaryFormatter formatter = new();
            FileStream stream = new(GlobalConstants.persistentDataPath, FileMode.Open);

            SudokuGrid data = formatter.Deserialize(stream) as SudokuGrid;
            stream.Close();
            return data;
        } else {
            Debug.Log("error fetching data!");
            return null;
        }
    }
}
