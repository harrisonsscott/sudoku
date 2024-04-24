using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class Stat { // stats for one single difficulty
    public int gamesPlayed;
    public int gamesWon;
    public float winRate; // ratio of games played to games won


    public Stat(){
        gamesPlayed = 0;
        gamesWon = 0;
        winRate = 0;
    }

    public Stat(int gamesPlayed, int gamesWon){
        this.gamesPlayed = gamesPlayed;
        this.gamesWon = gamesWon;
        this.winRate = gamesWon / gamesWon;
    }
}

// place in the parent of the stat panels

public class Stats : MonoBehaviour
{
    [HideInInspector] public List<GameObject> panels;
    [HideInInspector] public List<TMP_Text> textList; // text for the panels
    public int currentDifficultyIndex;

    private int index;

    void Awake()
    {
        currentDifficultyIndex = 0;
        for (int i = 0; i < transform.childCount; i++){
            panels.Add(transform.GetChild(i).gameObject);
            textList.Add(transform.GetChild(i).Find("Amount").GetComponent<TMP_Text>());
        }
        Refresh();
    }

    // updates all the stats
    public void Refresh(){
        Stat[] stats = SaveData.LoadStats();
        
        index = 0;
        foreach (var property in typeof(Stat).GetFields()){
            textList[index].text = property.GetValue(stats[currentDifficultyIndex]) + "";
            index += 1;
        }
    }
}
