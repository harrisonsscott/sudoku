using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AdaptivePerformance;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    public TextAsset jsonFile;

    private void Start() {
        string path = Application.dataPath + "/data.json";

        // if (!File.Exists(path)){
            File.WriteAllText(path, jsonFile.text);
        // }

        string sudokuString = Data.GetSudokuString(path);
        SudokuGrid grid = Data.DecodeSudokuString(sudokuString);
        grid.Log();
    }
}
