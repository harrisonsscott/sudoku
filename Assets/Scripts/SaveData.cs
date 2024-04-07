using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveData {
    // save data to persistent data path
    public static void SaveGrid(SudokuGrid grid){

        SudokuData data = new SudokuData(grid.partial, grid.full, grid.data)
        {
            mistakesLeft = grid.mistakesLeft,
            time = grid.time
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GlobalConstants.dataPath, json);
    }

    public static SudokuData LoadGrid(){
        if (File.Exists(GlobalConstants.dataPath)){
            string json = File.ReadAllText(GlobalConstants.dataPath);
            return JsonUtility.FromJson<SudokuData>(json);
        } else {
            Debug.Log("error fetching data!");
            return null;
        }
    }

    // clear the sudoku grid data
    public static void WipeGridData(){
        Debug.Log("wiped data");
        File.WriteAllText(GlobalConstants.dataPath, "");
    }
}
