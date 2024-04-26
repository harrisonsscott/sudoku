using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class Stat { // stats for one single difficulty
    public int gamesPlayed;
    public int gamesWon;
    public string winRate; // ratio of games played to games won as a percentage

    public Stat(){
        gamesPlayed = 0;
        gamesWon = 0;
        winRate = "0%";
    }

    public Stat(int gamesPlayed, int gamesWon){
        this.gamesPlayed = gamesPlayed;
        this.gamesWon = gamesWon;
        Refresh();
    }

    public void Refresh(){ // refreshes variables
        this.winRate = gamesWon / (float)gamesPlayed * 100f + "%";
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

    private int index;
    private UI ui;
    private Theme theme;

    void Start()
    {
        ui = FindAnyObjectByType<UI>();

        currentDifficultyIndex = 0;
        for (int i = 0; i < transform.childCount; i++){
            panels.Add(transform.GetChild(i).gameObject);
            textList.Add(transform.GetChild(i).Find("Amount").GetComponent<TMP_Text>());
        }

        Debug.Log(transform.name);
        Transform headerDifficultyParent = transform.parent.parent.GetChild(1).GetChild(1).GetChild(0);
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
            textList[index].text = property.GetValue(stats[currentDifficultyIndex]) + "";
            index += 1;
        }

        foreach (TMP_Text text in difficultyList){
            text.color = ui.currentTheme.text.ToRGB();
        }

        difficultyList[currentDifficultyIndex].color = theme.text2.ToRGB();
    }

    private void Update() {
        Refresh();
    }
}
