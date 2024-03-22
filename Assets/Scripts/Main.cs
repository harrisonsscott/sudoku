using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AdaptivePerformance;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    public TextAsset jsonFile;
    public SudokuGrid grid;

    private void Start() {
        string path = Application.dataPath + "/data.json";

        if (!File.Exists(path)){
            File.WriteAllText(path, jsonFile.text);
        }

        grid = Data.GetSudokuGrid(path);
        grid.Log();
        Debug.Log(grid.Place(0, 0, 2));
        grid.Log();
    }
}
