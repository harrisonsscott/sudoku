using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AdaptivePerformance;
using UnityEngine.UI;

// puzzle data is generated with python
[System.Serializable]
public class PuzzleData {
    public SudokuGrid[] puzzles;
}
[System.Serializable]
public class SudokuGrid {
    public string partial; // encoded partially completed sudoku grid for reference
    public string full; // encoded completed sudoku grid for reference
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

public static class Data {
    public static string GetSudokuString(string path){ // gets a random encoded sudoku puzzle from the JSON 
        string json = File.ReadAllText(path);
        PuzzleData data = JsonUtility.FromJson<PuzzleData>(json);
        Debug.Log(data.puzzles[0]);
        Debug.Log(json);
        int index = UnityEngine.Random.Range(0, data.puzzles.Length-1);

        return data.puzzles[0].full;
    }

    public static SudokuGrid DecodeSudokuString(string sudokuString){
        SudokuGrid grid = new SudokuGrid();

        for (int x = 0; x < GlobalConstants.gridX; x++){
            for (int y = 0; y < GlobalConstants.gridY; y++){
                grid.data[x,y] = (int)char.GetNumericValue(sudokuString[y * GlobalConstants.gridX + x]);
            }
        }

        return grid;
    }
}