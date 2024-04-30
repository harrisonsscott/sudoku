using UnityEngine;

public static class GlobalConstants {
    public static int gridX = 9;
    public static int gridY = 9;    
    public static int precision = 1; // how many decimal places percentages will round to
    public static string dataPath = Application.persistentDataPath + "/data.sudoku";
    public static bool darkMode = true;
    public static bool disableAds = false; // disables banner ads, for play store listing images
}