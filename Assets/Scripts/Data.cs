using System;
using TMPro;
using UnityEngine;

// color scheme for the ui
[System.Serializable]
public class Theme {
    // use hex strings for colors
    public string background;
    public string button;
    public string sudokuGrid; // automatically set to button
    public string text; // set automatically
    public string text2; // secondary text color, set automatically

    public Theme(string background, string button){
        this.background = background;
        this.button = button;
        this.sudokuGrid = button;
        this.text = Data.Grayscale(Data.Invert(background)).ToHex();
        this.text2 = this.text;
    }

    public Theme(string background, string button, string sudokuGrid){
        this.background = background;
        this.button = button;
        this.sudokuGrid = sudokuGrid;
        this.text = Data.Grayscale(Data.Invert(background)).ToHex();
        this.text2 = this.text;
    }

    public Theme(string background, string button, string sudokuGrid, string text2){
        this.background = background;
        this.button = button;
        this.sudokuGrid = sudokuGrid;
        this.text = Data.Grayscale(Data.Invert(background)).ToHex();
        this.text2 = text2;
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
    public string notes;
    public string data;
    public int mistakesLeft;
    public float time; // in seconds

    public SudokuData(){}

    public SudokuData(string partial, string full){
        this.partial = partial;
        this.full = full;
    }

    public SudokuData(string partial, string full, int[,] data, bool[,,] noteData){
        this.partial = partial;
        this.full = full;
        this.dataArray = data;
        this.data = Data.SerializeArray(data);
        this.notes = Data.SerializeNotes(noteData);
    }
}

[System.Serializable]
public class SudokuGrid : MonoBehaviour {
    public string partial; // encoded partially completed sudoku grid for reference
    public string full; // encoded completed sudoku grid for reference
    public int[,] data = new int[GlobalConstants.gridX, GlobalConstants.gridY];
    public bool[,,] noteData = new bool[GlobalConstants.gridX, GlobalConstants.gridY, 9]; // each place has 9 bools representing notes
    public int mistakesLeft = 3;
    public int score;
    public int combo; // amount of times the user has correctly placed a number in a row, gives more score
    public int difficulty;
    public float time; // time since the game started
    public Vector2 position; // part that the user has currently selected
    public bool noteMode;

    private Action onScoreChange; // action that is called when the score is changed
    private Action onMistake; // action thats called when the player misplaces a number

    public SudokuGrid(){
        History.ResetMoves();
    }

    public SudokuGrid(string partial, string full){
        this.partial = partial;
        this.full = full;

        History.ResetMoves();

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
    3 - Valid (Note)
    */
    public int Place(int num){
        int x = (int)position.x;
        int y = (int)position.y;

        if (noteMode){
            noteData[x, y, num - 1] = !noteData[x, y, num - 1];
            return 3;
        }

        if (data[x, y] != 0){
            return 1;
        }
        int fullReference = full[y*GlobalConstants.gridY+x] - '0';

        History.PushMove(this);
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

    public void Draw(GameObject image, GameObject textReference, Vector2Int pos){ // DrawAll for one text object
        GameObject textGO = image.transform.GetChild(pos.x * 9 + pos.y).gameObject;
        TMP_Text text = textGO.GetComponent<TMP_Text>();

        textGO.SetActive(true);
        if (data[pos.x, pos.y] == 0){
            // DrawNotes(image, textReference, pos);
            DrawAll(image, textReference);
            text.text = "";
            return;
        }
        for (int i = 0; i < textGO.transform.childCount; i++){
            Destroy(textGO.transform.GetChild(i).gameObject);
        }
        text.text = data[pos.x, pos.y] + "";
    }  

    public void Draw(GameObject image, GameObject textReference, int x, int y){
        Draw(image, textReference, new Vector2Int(x, y));
    }

    public void DrawNotes(GameObject image, GameObject textReference, Vector2Int position){
        for (int z = 0; z < 9; z++){
            if (noteData[position.x, position.y, z]){
                GameObject noteGO = Instantiate(textReference);
                TMP_Text noteText = noteGO.GetComponent<TMP_Text>();

                        // set note position
                Vector2 pos = new Vector2(z % 3, z / 3) - new Vector2(1, 1);
                pos *= noteGO.GetComponent<RectTransform>().sizeDelta / 3f;
                pos.y *= -1;

                noteText.text = z + 1 + "";
                noteText.fontSize = textReference.GetComponent<TMP_Text>().fontSize / 3f;

                noteGO.transform.SetParent(image.transform.GetChild(0).transform);
                // noteGO.transform.SetParent(image.transform);        
                
                noteGO.GetComponent<RectTransform>().sizeDelta /= 3f;
                noteGO.GetComponent<RectTransform>().localPosition = pos;
            }
        }
    }

    public void DrawAll(GameObject image, GameObject textReference){ // draws the grid onto an image, DON'T CALL OFTEN (MEMORY INTENSIVE)
        // clear the previous text
        for (int i = 0; i < image.transform.childCount; i++){
            Destroy(image.transform.GetChild(i).gameObject);
        }

        RectTransform rect = image.GetComponent<RectTransform>();

        UI ui = FindAnyObjectByType<UI>();
        UserPref pref = SaveData.LoadPrefs();

        Vector2 gridSize = new Vector2(rect.sizeDelta.x/GlobalConstants.gridX, rect.sizeDelta.y/GlobalConstants.gridY);
        textReference.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width/9, Screen.width/9);

        for (int x = 0; x < GlobalConstants.gridX; x++){
            for (int y = 0;  y < GlobalConstants.gridY; y++){
                GameObject textGO = Instantiate(textReference);
                TMP_Text text = textGO.GetComponent<TMP_Text>();

                // numbers on the sudoku grid that the player placed will be different than the ones pre-generated
                if (partial.Substring(y * GlobalConstants.gridX + x, 1).ToInt() == 0)
                    text.color = ui.themes[ui.userPref.themeIndex].text2.ToRGB();

                textGO.transform.SetParent(image.transform);

                textGO.GetComponent<RectTransform>().localPosition = new Vector2(gridSize.x*x, -gridSize.y*y) - new Vector2(rect.sizeDelta.x/2, -rect.sizeDelta.y/2) + new Vector2(gridSize.x, -gridSize.y)/2;
                text.text = data[x,y] != 0 ? data[x, y] + "" : ""; // set text to grid data
                textGO.SetActive(true);

                for (int z = 0; z < 9; z++){
                    if (noteData[x, y, z] && data[x, y] == 0){
                        GameObject noteGO = Instantiate(textReference);
                        TMP_Text noteText = noteGO.GetComponent<TMP_Text>();

                        // set note position
                        Vector2 pos = new Vector2(z % 3, z / 3) - new Vector2(1, 1);
                        pos *= noteGO.GetComponent<RectTransform>().sizeDelta / 3f;
                        pos.y *= -1;

                        noteText.text = z + 1 + "";
                        noteText.fontSize = text.fontSize / 3f;

                        noteGO.transform.SetParent(textGO.transform);
                        
                        noteGO.GetComponent<RectTransform>().sizeDelta /= 3f;
                        noteGO.GetComponent<RectTransform>().localPosition = pos;
                    }
                }
            }
        }
    }

    // highs a part of the sudoku grid when the user clicks on it
    public void Highlight(GameObject image, Vector2 pos, GameObject selectPanelX, GameObject selectPanelY, GameObject selectPanelZ, GameObject selectPanelW){
        RectTransform rect = image.GetComponent<RectTransform>();
        selectPanelX.GetComponent<RectTransform>().sizeDelta = new Vector2(rect.sizeDelta.x, rect.sizeDelta.y/GlobalConstants.gridY);
        selectPanelY.GetComponent<RectTransform>().sizeDelta = new Vector2(rect.sizeDelta.x/GlobalConstants.gridX, rect.sizeDelta.y);
        selectPanelZ.GetComponent<RectTransform>().sizeDelta = new Vector2(rect.sizeDelta.y/GlobalConstants.gridX*3, rect.sizeDelta.x/GlobalConstants.gridY*3);
        selectPanelW.GetComponent<RectTransform>().sizeDelta = new Vector2(rect.sizeDelta.x/GlobalConstants.gridX, rect.sizeDelta.y/GlobalConstants.gridY);

        selectPanelX.transform.localPosition = new Vector2(rect.localPosition.x, -(pos.y - (GlobalConstants.gridY/2)) * rect.sizeDelta.y/GlobalConstants.gridY + rect.localPosition.y);
        selectPanelY.transform.localPosition = new Vector2((pos.x - (GlobalConstants.gridX/2))* rect.sizeDelta.x / GlobalConstants.gridX + rect.localPosition.x, rect.localPosition.y);

        selectPanelZ.transform.localPosition = new Vector2(
            ((int)(pos.x/3) - 1) % 3 * rect.sizeDelta.x/GlobalConstants.gridX * 3 + rect.localPosition.x, -(((int)(pos.y/3) - 1) % 3) * rect.sizeDelta.y/GlobalConstants.gridY * 3 + rect.localPosition.y
        );

        selectPanelW.transform.localPosition = new Vector2(
            selectPanelY.transform.localPosition.x,
            selectPanelX.transform.localPosition.y
        );
            
    }
}

public static class Data {
    public static SudokuGrid GetSudokuGrid(TextAsset jsonFile){ // gets a random encoded sudoku puzzle from the JSON 
        string json = jsonFile.text;
        PuzzleData data = JsonUtility.FromJson<PuzzleData>(json);
        int index = UnityEngine.Random.Range(0, data.puzzles.Length-1);

        SudokuGrid grid = new SudokuGrid
        {
            partial = data.puzzles[index].partial,
            full = data.puzzles[index].full
        };

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
        SudokuGrid grid = new SudokuGrid
        {
            partial = data.partial,
            full = data.full,
            data = DecodeSudokuString(data.data),
            noteData = DecodeNoteString(data.notes),
            mistakesLeft = data.mistakesLeft,
            time = data.time
        };

        return grid;
    }

    private static bool[,,] DecodeNoteString(string notes)
    {
        bool[,,] data = new bool[GlobalConstants.gridX, GlobalConstants.gridY, 9];

        for (int x = 0; x < GlobalConstants.gridX; x++){
            for (int y = 0; y < GlobalConstants.gridY; y++){
                for (int z = 0; z < GlobalConstants.gridY; z++){
                    data[x, y, z] = notes[x * 81 + y * 9 + z] == '1' ? true : false;
                }
            }
        } 

        return data;
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

    public static string SerializeNotes(bool[,,] notes){ // converts a 3d boolean array into a string for storing [true, false] -> 10
        string str = "";

        for (int x = 0; x < GlobalConstants.gridX; x++){
            for (int y = 0; y < GlobalConstants.gridY; y++){
                for (int z = 0; z < GlobalConstants.gridY; z++){
                    str += notes[x, y, z] == true ? "1" : "0";
                }
            }
        }

        return str;
    }

    public static Color Invert(Color color){ // flips the colors
        return new Color(1 - color.r, 1 - color.g, 1 - color.b);
    }

    public static Color Invert(string hex){
        return Invert(hex.ToRGB());
    }

    public static Color Grayscale(Color color){ // converts a color into black and white
        float avg = (color.r + color.g + color.b)/3f;

        return new Color(avg, avg, avg);
    }

    public static Color Grayscale(String hex){
        return Grayscale(hex.ToRGB());
    }
}