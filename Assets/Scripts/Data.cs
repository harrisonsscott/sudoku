using System;
using System.Globalization;
using TMPro;
using Unity.VisualScripting.Dependencies.Sqlite;
using Unity.VisualScripting.ReorderableList.Element_Adder_Menu;
using UnityEngine;

// color scheme for the ui
[System.Serializable]
public class Theme {
    // use hex strings for colors
    public string background;
    public string button;
    public string text; // set automatically

    public Theme(string background, string button){
        this.background = background;
        this.button = button;
        this.text = Data.Grayscale(Data.Invert(background)).ToHex();
    }

    public Theme(string background, string button, string text){
        this.background = background;
        this.button = button;
        this.text = text;
    }
}

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
    public float time; // in seconds

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
    public float time; // time since the game started

    private Action onScoreChange; // action that is called when the score is changed
    private Action onMistake; // action thats called when the player misplaces a number

    public SudokuGrid(){
        
    }
    public SudokuGrid(string partial, string full){
        this.partial = partial;
        this.full = full;
    }

    public void Log(bool logFull=false){ // logs either the current sudoku grid or the completed one
        string row = "";
        for (int y = 0; y < GlobalConstants.gridY; y++){
            for (int x = 0; x < GlobalConstants.gridX; x++){
                if (!logFull){
                    row += data[x, y] + ", ";
                } else {
                    row += Data.DecodeSudokuString(full)[x, y] + ", ";
                }
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
        textReference.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width/9, Screen.width/9);

        for (int x = 0; x < GlobalConstants.gridX; x++){
            for (int y = 0;  y < GlobalConstants.gridY; y++){
                GameObject textGO = Instantiate(textReference);
                TMP_Text text = textGO.GetComponent<TMP_Text>();

                textGO.transform.SetParent(image.transform);

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
        grid.time = data.time;
        
        return grid;
    }

    public static string SerializeArray(int[,] array){ // converts an array into a string for storing, ex: [1, 2, 5] -> "125"
        string str = "";

        for (int x = 0; x < GlobalConstants.gridX; x++){
            for (int y = 0; y < GlobalConstants.gridY; y++){
                str += array[y,x];
            }
        }
        
        return str;
    }

    public static Color HexToRGB(string hex){ // converts a hex code into an RGB value, ex "#ff0500" -> (255, 5 ,0)
        int shift = 0;

        if (hex.Substring(0, 1) == "#"){
            shift = 1; // ignore the hashtag if it exists
        }

        string r = hex.Substring(0 + shift, 2);
        string g = hex.Substring(2 + shift, 2);
        string b = hex.Substring(4 + shift, 2);
        
        return new Color(
            int.Parse(r, NumberStyles.HexNumber) / 255f,
            int.Parse(g, NumberStyles.HexNumber) / 255f,
            int.Parse(b, NumberStyles.HexNumber) / 255f);

    }

    public static string RGBToHex(Color rgb){
        return ColorUtility.ToHtmlStringRGBA(rgb);
    }

    public static Color Invert(Color color){ // flips the colors
        return new Color(1 - color.r, 1 - color.g, 1 - color.b);
    }

    public static Color Invert(string hex){
        return Invert(HexToRGB(hex));
    }

    public static Color Grayscale(Color color){ // converts a color into black and white
        float avg = (color.r + color.g + color.b)/3f;

        return new Color(avg, avg, avg);
    }

    public static Color Grayscale(String hex){
        return Grayscale(HexToRGB(hex));
    }

    public static bool HasComponent <T>(this GameObject obj) where T:Component{
        return obj.GetComponent<T>() != null;
    }

    public static string ToHex (this Color color){
        return RGBToHex(color);
    }

    public static Color ToRGB (this string str){
        return HexToRGB(str);
    }
}