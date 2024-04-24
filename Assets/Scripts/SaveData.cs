using UnityEngine;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class UserData {
    public SudokuData gridData;
    public UserPref userPref;
    public Stat[] stats;

    public UserData(SudokuData gridData){
        this.gridData = gridData;
        this.userPref = new UserPref();
    }

    public UserData(SudokuData gridData, UserPref userPref){
        this.gridData = gridData;
        this.userPref = userPref;
        this.stats = new Stat[5];
    }

    public UserData(SudokuData gridData, UserPref userPref, Stat[] stats){
        this.gridData = gridData;
        this.userPref = userPref;
        this.stats = stats;
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
    public static void Save(SudokuGrid grid, UserPref userPref, Stat[] stats){

        SudokuData data = new SudokuData(grid.partial, grid.full, grid.data, grid.noteData)
        {
            mistakesLeft = grid.mistakesLeft,
            time = grid.time
        };

        UserData userData = new(data, userPref, stats);

        string json = JsonUtility.ToJson(userData, true);

        File.WriteAllText(GlobalConstants.dataPath, json);
    }

    public static void Save(SudokuGrid grid, UserPref userPref, List<Stat> stats){
        Save(grid, userPref, stats.ToArray());
    }

    public static SudokuData LoadGrid(){
        if (File.Exists(GlobalConstants.dataPath)){
            string json = File.ReadAllText(GlobalConstants.dataPath);
            UserData data = JsonUtility.FromJson<UserData>(json);
            return data.gridData;
        } else {
            Debug.Log("error fetching data!");
            Save(new SudokuGrid(), new UserPref(), new Stat[5]);
            return null;
        }
    }

    public static UserPref LoadPrefs(){
        if (File.Exists(GlobalConstants.dataPath)){
            string json = File.ReadAllText(GlobalConstants.dataPath);
            UserData data = JsonUtility.FromJson<UserData>(json);
            return data.userPref;
        } else {
            Save(new SudokuGrid(), new UserPref(), new Stat[5]);
            Debug.Log("error fetching data!");
            return null;
        }
    }

    public static Stat[] LoadStats(){
        if (File.Exists(GlobalConstants.dataPath)){
            string json = File.ReadAllText(GlobalConstants.dataPath);
            UserData data = JsonUtility.FromJson<UserData>(json);
            return data.stats;
        } else {
            Save(new SudokuGrid(), new UserPref(), new Stat[5]);
            Debug.Log("error fetching data!");
            return null;
        }
    }

    // clear the sudoku grid data
    public static void WipeGridData(){
        Debug.Log("wiped data");
        UserPref userPref = LoadPrefs();
        Stat[] stats = LoadStats();

        File.WriteAllText(GlobalConstants.dataPath, "");
        Save(new SudokuGrid(), userPref, stats);
    }
}
