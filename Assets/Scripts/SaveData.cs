using UnityEngine;
using System.IO;

[System.Serializable]
public class UserData {
    public SudokuData gridData;
    public UserPref userPref;

    public UserData(SudokuData gridData){
        this.gridData = gridData;
        this.userPref = new UserPref();
    }

    public UserData(SudokuData gridData, UserPref userPref){
        this.gridData = gridData;
        this.userPref = userPref;
    }
}

[System.Serializable]
public class UserPref { // user preferences
    public int themeIndex;

    public UserPref(){
        themeIndex = 0;
    }
}

public static class SaveData {
    // save data to persistent data path
    public static void Save(SudokuGrid grid, UserPref userPref){

        SudokuData data = new SudokuData(grid.partial, grid.full, grid.data)
        {
            mistakesLeft = grid.mistakesLeft,
            time = grid.time
        };

        UserData userData = new(data, userPref);

        string json = JsonUtility.ToJson(userData, true);

        File.WriteAllText(GlobalConstants.dataPath, json);
    }

    public static SudokuData LoadGrid(){
        if (File.Exists(GlobalConstants.dataPath)){
            string json = File.ReadAllText(GlobalConstants.dataPath);
            UserData data = JsonUtility.FromJson<UserData>(json);
            return data.gridData;
        } else {
            Debug.Log("error fetching data!");
            Save(new SudokuGrid(), new UserPref());
            return null;
        }
    }

    public static UserPref LoadPrefs(){
        if (File.Exists(GlobalConstants.dataPath)){
            string json = File.ReadAllText(GlobalConstants.dataPath);
            UserData data = JsonUtility.FromJson<UserData>(json);
            return data.userPref;
        } else {
            Save(new SudokuGrid(), new UserPref());
            Debug.Log("error fetching data!");
            return null;
        }
    }

    // clear the sudoku grid data
    public static void WipeGridData(){
        Debug.Log("wiped data");
        UserPref userPref = LoadPrefs();
        File.WriteAllText(GlobalConstants.dataPath, "");
        Save(new SudokuGrid(), userPref);
    }
}
