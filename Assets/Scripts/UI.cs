using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.Mathematics;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// handles all the UI functions
public class UI : MonoBehaviour
{
    [Header("UI")]
    public GameObject fadeGameObject;
    public GameObject continueButton;
    public GameObject newGameButton;
    public GameObject homeScene;
    public GameObject newGameScene;
    public GameObject gameScene;
    public GameObject sudokuGrid;
    public GameObject heartContainer; // container with 3 hearts above the sudoku board
    public GameObject endGameContainer; // container that displays when the game ends
    public GameObject completedGameContainer; // container that displays when the user completes a sudoku grid
    public TMP_Text timer; // text that displays the time in the game
    public TMP_Text score; // text that displays score
    public GameObject[] difficultyButtons; // the easy, medium, etc buttons when you're making a new game
    public List<GameObject> numberButtons; // the buttons that let you change the number to place
    [Header("Other")]
    public AdsInitializer ads;
    public AdsBanner adsBanner;
    public Sudoku sudoku;
    public Main main;
    public bool isInGame;
    const float transitionTime = 0.1f;
    private Color numberButtonsStartingColor; // their color at the start of the game
    private void Start() {
        // show the home scene during the start of the game
        homeScene.SetActive(true);
        foreach (var element in new GameObject[]{newGameScene, gameScene, fadeGameObject}){
           element.SetActive(false);
        }

        numberButtonsStartingColor = difficultyButtons[0].GetComponent<Image>().color;

        // adding listeners to buttons
        newGameButton.GetComponent<Button>().onClick.AddListener(() => TransitionScene(homeScene, newGameScene));
        continueButton.GetComponent<Button>().onClick.AddListener(() => LoadPreviousGame());
        // create a new grid and transition to it
        int index = 0;
        foreach (var element in difficultyButtons){
            int index2 = index;
            element.GetComponent<Button>().onClick.AddListener(() => {
                TransitionScene(newGameScene, gameScene);
                StartGame(index2);
            });   
            index += 1;
        }

        InvokeRepeating("SaveGame", 2, 1);
        
    }

    private void SaveGame(){
        if (isInGame){
            SaveData.SaveGrid(main.grid);
        }
    }

    private void Update() {
        if (isInGame){
            main.grid.time += Time.deltaTime;
            timer.text = FormatTime(main.grid.time);
            if (main.grid.full == Data.SerializeArray(main.grid.data)){ // player completed the board
                isInGame = false;
                completedGameContainer.transform.GetChild(1).GetComponent<TMP_Text>().text = $"You finished in \n{FormatTime(main.grid.time)}!";
                Fade(0.5f, () => completedGameContainer.GetComponent<Modal>().Open(0.1f));
            }
        }
    }

    private void OnScoreChange(){
        score.text = $"Score: {main.grid.score}";
    }

    private void OnMistake(bool refresh=false){
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

    public void EndGame(){ // end OF game
        endGameContainer.transform.GetChild(2).GetComponent<AdsRewardedButton>().LoadAd();
        Fade(0.5f, () => endGameContainer.GetComponent<Modal>().Open(0.1f));
        // TransitionScene(gameScene, newGameScene);
    }

    public void StartGame(int difficulty, SudokuData data=null){
        OnMistake(true); // refresh the hearts
        if (data != null){
            main.grid = Data.dataToGrid(data);
        } else {
            main.grid = Data.GetSudokuGrid(main.difficulties[difficulty]);
        }
        
        main.grid.Draw(sudoku.gameObject, sudoku.textReference);
        main.grid.OnScoreChange(OnScoreChange);
        main.grid.OnMistake(() => OnMistake(false));
        SaveData.SaveGrid(main.grid);
        ChangeNumber(1, numberButtons[0]);
        isInGame = true;
        adsBanner.LoadBanner();
        // main.grid.data = Data.DecodeSudokuString(main.grid.full); // immediately finish the game
    }

    public void LeaveGame(){
        isInGame = false;
        if (endGameContainer.activeSelf){ // player made too many mistakes
            SaveData.WipeGridData();
            endGameContainer.GetComponent<Modal>().Close(0.1f);
            EndFade();
        }

        if (completedGameContainer.activeSelf){ // player completed the puzzle
            SaveData.WipeGridData();
            completedGameContainer.GetComponent<Modal>().Close(0.1f);
            EndFade();
        }

        TransitionScene(gameScene, homeScene);
    }

    public void ContinueGame(){ // when the player runs out of lives and decides to continue
        EndFade();
        endGameContainer.GetComponent<Modal>().Close(() => {
            main.grid.mistakesLeft = 3;
            OnMistake(true);
        }, 0.1f);
    }

    public void LoadPreviousGame(){ // loads the game that was saved
        if (File.ReadAllText(GlobalConstants.dataPath) != ""){
            StartGame(0, SaveData.LoadGrid());
            TransitionScene(homeScene, gameScene); // game saved, load it
            // Wait(2, () => OnMistake(true));
            OnMistake(false);
        } else {
            TransitionScene(homeScene, newGameScene); // no game saved, create a new one
        }
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

    public void Fade(float percentage, Action onEnd){ // darkens the screen by a certain amount ranging from 0 to 1
        Image image = fadeGameObject.GetComponent<Image>();
        Color color = image.color; // original color

        fadeGameObject.SetActive(true);
        image.color = new Color(color.r, color.g, color.b, 0);
        LeanTween.value(0, percentage, 0.5f).setOnUpdate((float value) => {
            fadeGameObject.GetComponent<Image>().color = new Color(color.r, color.g, color.b, value);
        }).setOnComplete(onEnd);

    }

    public void Fade(float perecentage){
        Fade(perecentage, () => {});
    }

    public void EndFade(Action onEnd){
        Image image = fadeGameObject.GetComponent<Image>();
        LeanTween.value(0, image.color.a, 0.5f).setOnComplete(() => {
            fadeGameObject.SetActive(false);
            onEnd();
        });
    }

    public void EndFade(){
        EndFade(() => {});
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

    public void Wait(float time, Action action){ // waits a certain amount of time before executing an action
        LeanTween.value(0, 1, time).setOnComplete(action);
    }
}
