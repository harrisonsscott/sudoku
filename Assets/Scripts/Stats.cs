using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class Stat { // stats for one single difficulty
    public int gamesPlayed;
    public int gamesWon;
    public string winRate = "0%"; // ratio of games played to games won as a percentage
    public int highScore;
    public int totalPoints;
    public string averageScore = "0%";

    public Stat(){
        gamesPlayed = 0;
        gamesWon = 0;
        totalPoints = 0;
        highScore = 0;
        Refresh();
    }

    public Stat(int gamesPlayed, int gamesWon, int highScore, int totalPoints){
        this.gamesPlayed = gamesPlayed;
        this.gamesWon = gamesWon;
        this.highScore = highScore;
        this.totalPoints = totalPoints;
        Refresh();
    }

    public void Refresh(){ // refreshes variables
        // display the win rate percentage up to 1 decimal place
        winRate = gamesPlayed == 0 ? "-" : 
            Mathf.Floor(gamesWon / (float)gamesPlayed * 100 * Mathf.Pow(10, GlobalConstants.precision)) / Mathf.Pow(10, GlobalConstants.precision) + "%";
        averageScore = gamesPlayed == 0 ? "-" : (totalPoints / gamesPlayed) + "";
            
    }
}

// place in the parent of the stat panels

public class Stats : MonoBehaviour
{
    [HideInInspector] public List<GameObject> panels;
    [HideInInspector] public List<TMP_Text> textList; // text for the panels

    [HideInInspector] public List<GameObject> headerDifficulty; // text in the header that show what difficulty you're looking at
    [HideInInspector] public List<TMP_Text> difficultyList; // text for each fo the objects in headerDifficulty
    public int currentDifficultyIndex;
    public Transform header;

    private int index;
    private UI ui;
    private Theme theme;

    void Start()
    {
        ui = FindAnyObjectByType<UI>();

        currentDifficultyIndex = 0;
        for (int i = 0; i < transform.childCount; i++){
            if (transform.GetChild(i).Find("Amount")){
                panels.Add(transform.GetChild(i).gameObject);
                textList.Add(transform.GetChild(i).Find("Amount").GetComponent<TMP_Text>());
            }
        }

        Transform headerDifficultyParent = header.GetChild(1).GetChild(1).GetChild(0);
        for (int i = 0; i < headerDifficultyParent.childCount; i++){
            headerDifficulty.Add(headerDifficultyParent.GetChild(i).gameObject);
            difficultyList.Add(headerDifficultyParent.GetChild(i).GetChild(0).GetComponent<TMP_Text>());
        }

        Refresh();
    }

    // updates all the stats
    public void Refresh(){
        Stat[] stats = SaveData.LoadStats();
        Theme theme = ui.themes[ui.userPref.themeIndex];
        
        index = 0;
        foreach (var property in typeof(Stat).GetFields()){
            object prop = property.GetValue(stats[currentDifficultyIndex]);

            textList[index].text = prop != null ? prop + "" : "-";
            index += 1;
        }

        foreach (TMP_Text text in difficultyList){
            text.color = ui.currentTheme.text.ToRGB();
        }

        foreach(Stat stat in stats){
            stat.Refresh();
        }

        difficultyList[currentDifficultyIndex].color = theme.text2.ToRGB();

        SaveData.SaveStats(stats);
    }

    private void Update() {
        // Refresh();
    }
}
