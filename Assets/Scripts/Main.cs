using System.IO;
using UnityEngine;

public class Main : MonoBehaviour
{
    public TextAsset jsonFile;
    public SudokuGrid grid;
    public string path;
    private float index;    

    private void Start() {
        index = 0;

        grid = Data.GetSudokuGrid(jsonFile);
        grid.Log();
    }
}
