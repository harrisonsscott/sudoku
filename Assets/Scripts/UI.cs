using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// handles all the UI functions
public class UI : MonoBehaviour
{
    [Header("UI")]
    public GameObject newGameButton;
    public GameObject homeScene;
    public GameObject newGameScene;
    public GameObject gameScene;
    public GameObject sudokuGrid;
    public GameObject heartContainer; // container with 3 hearts above the sudoku board
    public TMP_Text timer; // text that displays the time in the game
    public TMP_Text score; // text that displays score
    public List<GameObject> difficultyButtons; // the easy, medium, etc buttons when you're making a new game
    public List<GameObject> numberButtons; // the buttons that let you change the number to place
    [Header("Other")]
    public Sudoku sudoku;
    public Main main;
    public float timeSinceStart; // time since a new game has started
    public bool isInGame;
    const float transitionTime = 0.1f;
    private Color numberButtonsStartingColor; // their color at the start of the game
    private void Start() {
        // show the home scene during the start of the game
        homeScene.SetActive(true);
        foreach (var element in new GameObject[]{newGameScene, gameScene}){
           element.SetActive(false);
        }

        numberButtonsStartingColor = difficultyButtons[0].GetComponent<Image>().color;

        // adding listeners to buttons
        newGameButton.GetComponent<Button>().onClick.AddListener(() => TransitionScene(homeScene, newGameScene));
        // create a new grid and transition to that grid during a new game
        foreach (var element in difficultyButtons){
            element.GetComponent<Button>().onClick.AddListener(() => {
                TransitionScene(newGameScene, gameScene);
                StartGame();
            });
        }
    }

    private void Update() {
        if (isInGame){
            timeSinceStart += Time.deltaTime;
            timer.text = FormatTime(timeSinceStart);
        }
    }

    private void OnScoreChange(){
        score.text = $"Score: {main.grid.score}";
    }

    private void OnMistake(bool refresh){
        for (int i = 0; i < heartContainer.transform.childCount; i++){
            RawImage image = heartContainer.transform.GetChild(i).GetComponent<RawImage>();

            if (i < main.grid.mistakesLeft || refresh){
                image.color = Color.red;
            } else {
                image.color = Color.gray;
            }
        }
        if (main.grid.mistakesLeft == 0 && !refresh){
            EndGame();
        }
    }

    public void EndGame(){
        OnMistake(true); // refresh the hearts
        TransitionScene(gameScene, newGameScene);
    }

    public void StartGame(){
        main.grid = Data.GetSudokuGrid(main.jsonFile);
        main.grid.Draw(sudoku.gameObject, sudoku.textReference);
        main.grid.OnScoreChange(OnScoreChange);
        main.grid.OnMistake(() => OnMistake(false));
        ChangeNumber(1, numberButtons[0]);
        timeSinceStart = 0;
        isInGame = true;
    }

    public void ChangeNumber(int num){ // called by the buttons that let you change the number to place
        sudoku.number = num;
        foreach (var element in numberButtons){
            if (element == EventSystem.current.currentSelectedGameObject){
                element.GetComponent<Image>().color = numberButtonsStartingColor - new Color(0.1f, 0.1f, 0.1f, 0);
            } else {
                element.GetComponent<Image>().color = numberButtonsStartingColor;
            }
        }
    }

    public void ChangeNumber(int num, GameObject go){
        sudoku.number = num;
        foreach (var element in numberButtons){
            if (element == go){
                element.GetComponent<Image>().color = numberButtonsStartingColor - new Color(0.1f, 0.1f, 0.1f, 0);
            } else {
                element.GetComponent<Image>().color = numberButtonsStartingColor;
            }
        }
    }

    public string FormatTime(int timeInSeconds){
        string seconds = "0" + timeInSeconds % 60;
        string minutes = "0" + timeInSeconds / 60;

        string secondsFormatted = $"0{seconds}".Substring(seconds.Length-1); // add a zero when the time is under 10
        string minutesFormatted = $"0{minutes}".Substring(minutes.Length-1);

        return $"{minutesFormatted}:{secondsFormatted}";
    }

    public string FormatTime(float timeInSeconds){
        return FormatTime((int)timeInSeconds);
    }

    public void TransitionScene(GameObject from, GameObject to){ // moves one scene off the screen and moves another to it
        RectTransform fromRect = from.GetComponent<RectTransform>();
        RectTransform toRect = to.GetComponent<RectTransform>();

        LeanTween.moveLocal(from, new Vector3(-fromRect.rect.width, 0, 0), transitionTime).setOnComplete(() => {
            from.SetActive(false);
        });

        to.SetActive(true);
        toRect.localPosition = new Vector3(toRect.rect.width, 0, 0);
        LeanTween.moveLocal(to, Vector3.zero, transitionTime);
    }
}
