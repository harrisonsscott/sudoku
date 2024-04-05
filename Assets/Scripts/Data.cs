using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.AdaptivePerformance;
using UnityEngine.AI;
using UnityEngine.UI;

// puzzle data is generated with python
[System.Serializable]
public class PuzzleData {
    public SudokuData[] puzzles;
}

// used for loading sudoku puzzles from json along with data saving
[System.Serializable]
public class SudokuData {
    public string partial;
    public string full;
    public int[,] dataArray;
    public string data;
    public int mistakesLeft;

    public SudokuData(){}

    public SudokuData(string partial, string full){
        this.partial = partial;
        this.full = full;
    }

    public SudokuData(string partial, string full, int[,] data){
        this.partial = partial;
        this.full = full;
        this.dataArray = data;
        this.data = Data.SerializeArray(data);
    }
}

[System.Serializable]
public class SudokuGrid : MonoBehaviour {
    public string partial; // encoded partially completed sudoku grid for reference
    public string full; // encoded completed sudoku grid for reference
    public int[,] data = new int[GlobalConstants.gridX, GlobalConstants.gridY];
    public int mistakesLeft = 3;
    public int score;
    public int combo; // amount of times the user has correctly placed a number in a row, gives more score

    private Action onScoreChange; // action that is called when the score is changed
    private Action onMistake; // action thats called when the player misplaces a number

    public SudokuGrid(){
        
    }
    public SudokuGrid(string partial, string full){
        this.partial = partial;
        this.full = full;
    }

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

    public void OnScoreChange(Action action){
        onScoreChange = action;
    }

    public void OnMistake(Action action){
        onMistake = action;
    }

    private void UpdateScore(bool correct){ // gives score if correct, deducts score if not
        if (correct){
            score += (int)(15 * (1 + combo * 0.5f));
            combo += 1;
        } else {
            combo = 0;
            score = Mathf.Max(0, score - 10);
        }
        if (onScoreChange != null)
            onScoreChange();
    }

    /*Places a number on the grid

    Return Types
    0 - Valid
    1 - Number already placed on that area
    2 - Incorrect
    */
    public int Place(int x, int y, int num){ 
        if (data[x, y] != 0){
            return 1;
        }
        int fullReference = full[y*GlobalConstants.gridY+x] - '0';

        if (num == fullReference){
            data[x, y] = num;
            UpdateScore(true);
            return 0;
        } else {
            mistakesLeft -= 1;
            onMistake();
            UpdateScore(false);
            return 2;
        }
    }

    public int Place(Vector2 pos, int num){
        return Place((int)pos.x, (int)pos.y, num);
    }

    public void Draw(GameObject image, GameObject textReference){ // draws the grid onto an image, DON'T CALL OFTEN (MEMORY INTENSIVE)
        // clear the previous text
        for (int i = 0; i < image.transform.childCount; i++){
            Destroy(image.transform.GetChild(i).gameObject);
        }

        RectTransform rect = image.GetComponent<RectTransform>();

        Vector2 gridSize = new Vector2(rect.sizeDelta.x/GlobalConstants.gridX, rect.sizeDelta.y/GlobalConstants.gridY);

        for (int x = 0; x < GlobalConstants.gridX; x++){
            for (int y = 0;  y < GlobalConstants.gridY; y++){
                GameObject textGO = Instantiate(textReference);
                TMP_Text text = textGO.GetComponent<TMP_Text>();

                textGO.transform.SetParent(image.transform);

                textGO.GetComponent<RectTransform>().sizeDelta = gridSize;
                textGO.GetComponent<RectTransform>().localPosition = new Vector2(gridSize.x*x, -gridSize.y*y) - new Vector2(rect.sizeDelta.x/2, -rect.sizeDelta.y/2) + new Vector2(gridSize.x, -gridSize.y)/2;
                // textGO.GetComponent<RectTransform>().localPosition = -(new Vector2(gridSize.x*x, -gridSize.y*y) - new Vector2(rect.sizeDelta.x/2, rect.sizeDelta.y/2) + gridSize/2);
                text.text = data[x,y] + "";
                if (data[x,y] == 0)
                    textGO.SetActive(false);
                else 
                    textGO.SetActive(true);
            }
        }
        
    }
}

public static class Data {
    public static SudokuGrid GetSudokuGrid(TextAsset jsonFile){ // gets a random encoded sudoku puzzle from the JSON 
        string json = jsonFile.text;
        PuzzleData data = JsonUtility.FromJson<PuzzleData>(json);
        int index = UnityEngine.Random.Range(0, data.puzzles.Length-1);

        SudokuGrid grid = new SudokuGrid();
        grid.partial = data.puzzles[index].partial;
        grid.full = data.puzzles[index].full;

        grid.data = DecodeSudokuString(grid.partial);

        return grid;
    }

    public static int[,] DecodeSudokuString(string sudokuString){
        int[,] data = new int[GlobalConstants.gridX, GlobalConstants.gridY];

        for (int x = 0; x < GlobalConstants.gridX; x++){
            for (int y = 0; y < GlobalConstants.gridY; y++){
                data[x,y] = (int)char.GetNumericValue(sudokuString[y * GlobalConstants.gridX + x]);
            }
        }

        return data;
    }

    public static SudokuGrid dataToGrid(SudokuData data){ // converts a SudokuData to SudokuGrid
        SudokuGrid grid = new SudokuGrid();
        grid.partial = data.partial;
        grid.full = data.full;
        grid.data = DecodeSudokuString(data.data);
        grid.mistakesLeft = data.mistakesLeft;

        return grid;
    }

    public static string SerializeArray(int[,] array){
        string str = "";

        for (int x = 0; x < GlobalConstants.gridX; x++){
            for (int y = 0; y < GlobalConstants.gridY; y++){
                str += array[y,x];
            }
        }
        
        return str;
    }
}