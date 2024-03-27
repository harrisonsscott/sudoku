using System.IO;
using UnityEngine;

public class Main : MonoBehaviour
{
    public TextAsset jsonFile;
    public SudokuGrid grid;
    public string path;

    private void Start() {

        grid = Data.GetSudokuGrid(jsonFile);
        grid.Log();
    }
}
