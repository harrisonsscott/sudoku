using System.IO;
using UnityEngine;

public class Main : MonoBehaviour
{
    public TextAsset[] difficulties; // list of json files with sudoku grids, ex: easy.json, hard.jsonZ
    public SudokuGrid grid;
    public string path;

    private void Start() {
        grid = SaveData.LoadGrid();
        if (grid == null){
            grid = Data.GetSudokuGrid(difficulties[0]);
        }
        grid.Log();
    }

}
