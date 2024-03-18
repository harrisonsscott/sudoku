using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public static class Data {
    public static string GetSudokuString(string path){ // gets a random encoded sudoku puzzle from the JSON 
        string json = File.ReadAllText(path);
        PuzzleData data = JsonUtility.FromJson<PuzzleData>(json);

        int index = UnityEngine.Random.Range(0, data.puzzles.Length-1);

        return data.puzzles[index];
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