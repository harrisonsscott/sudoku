using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AdaptivePerformance;
using UnityEngine.UI;

[System.Serializable]
public class PuzzleData {
    public string[] puzzles;
}

[System.Serializable]
public class SudokuGrid {
    public int[,] data = new int[GlobalConstants.gridX, GlobalConstants.gridY];
    public void Log(){
        string row = "";
        for (int y = 0; y < GlobalConstants.gridY; y++){
            for (int x = 0; x < GlobalConstants.gridX; x++){
                row += data[x, y] + ", ";
            }
            row += "\n";
        }
        Debug.Log(row);
    }
}

public class Main : MonoBehaviour
{
    public TextAsset jsonFile;

    private void Start() {
        string path = Application.dataPath + "/data.json";

        if (!File.Exists(path)){
            File.WriteAllText(path, jsonFile.text);
        }

        string sudokuString = Data.GetSudokuString(path);
        Debug.Log(sudokuString);
        Debug.Log(sudokuString[2]);
        SudokuGrid grid = Data.DecodeSudokuString(sudokuString);
        grid.Log();
    }
}
