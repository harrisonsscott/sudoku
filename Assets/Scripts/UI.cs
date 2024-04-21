using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

// handles all the UI functions
public class UI : MonoBehaviour
{
    [Header("UI")]
    public GameObject header; // CONTENT of the top of the game scene
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
    public GameObject backButton; // button in the top of the screen during that lets you go back
    public GameObject hintButton;
    public GameObject undoButton;
    public GameObject notesButton;
    public Texture2D border; // image with a shadow that makes a border
    public TMP_Text timer; // text that displays the time in the game
    public TMP_Text score; // text that displays score
    public GameObject[] difficultyButtons; // the easy, medium, etc buttons when you're making a new game
    public List<GameObject> numberButtons; // the buttons that let you change the number to place

    [Header("Other")]
    public TMP_FontAsset font; // applied to all text objects at runtime
    public AdsInitializer ads;
    public AdsInterstitial ads2;
    public AdsBanner adsBanner;
    public Sudoku sudoku;
    public Main main;
    public UserPref userPref;
    public bool isInGame;
    const float transitionTime = 0.1f;
    private Color numberButtonsStartingColor; // their color at the start of the game

    [Header("Theme")]
    public List<Theme> themes;
    public GameObject toggleThemeButton; // button in the top right corner that lets you swap themes
    private void Start() {
        userPref = SaveData.LoadPrefs() == null ? SaveData.LoadPrefs() : new UserPref();
        // set themes
        themes.Add(new("#ffffff", "#E0F2FE", "#ffffff", "#4c7c9c")); // light mode
        themes.Add(new("#151521", "#212234", "#151521", "#0D597A")); // dark mode
        themes.Add(new("FFF2B8", "F3DCA1", "FFF2B8", "#6ABC78")); // sandy mode
        ApplyTheme(true);

        // show the home scene during the start of the game
        header.transform.parent.gameObject.SetActive(false);
        homeScene.SetActive(true);
        foreach (var element in new GameObject[]{newGameScene, gameScene, fadeGameObject}){
           element.SetActive(false);
        }

        numberButtonsStartingColor = difficultyButtons[0].GetComponent<Image>().color;

        // adding listeners to buttons
        newGameButton.GetComponent<Button>().onClick.AddListener(() => TransitionScene(homeScene, newGameScene));
        continueButton.GetComponent<Button>().onClick.AddListener(() => LoadPreviousGame());
        // hintButton.GetComponent<Button>().onClick.AddListener(() => {
        //     // place a number on a random spot on the grid 
        //     for (int i = 0; i < 1000; i++){
        //         Vector2Int position = new(UnityEngine.Random.Range(0, 9), UnityEngine.Random.Range(0, 9));
        //         if (main.grid.data[position.x, position.y] == 0){
        //             main.grid.data[position.x, position.y] = main.grid.full[position.y * GlobalConstants.gridY + position.x] - '0';
        //             sudoku.Draw();
        //             break;
        //         }
        //     }
        // });
        hintButton.GetComponent<AdsRewardedButton>().LoadAd();
        hintButton.GetComponent<AdsRewardedButton>().reward = (() => {
            // place a number on a random spot on the grid 
            for (int i = 0; i < 1000; i++){
                Vector2Int pos = new(UnityEngine.Random.Range(0, 9), UnityEngine.Random.Range(0, 9));
                if (main.grid.data[pos.x, pos.y] == 0){
                    main.grid.data[pos.x, pos.y] = main.grid.full[pos.y * GlobalConstants.gridY + pos.x] - '0';
                    sudoku.DrawAll();
                    sudoku.Highlight(pos);
                    break;
                }
            }
        });

        undoButton.GetComponent<Button>().onClick.AddListener(() => {
            main.grid.data = History.PopMove();
            // main.grid.data = new int[9,9];
            sudoku.DrawAll();
        });

        toggleThemeButton.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => {
            userPref.themeIndex = (userPref.themeIndex + 1) % themes.Count;
            SaveData.Save(main.grid, userPref);
            ApplyTheme(true);
        });

        notesButton.GetComponent<Button>().onClick.AddListener(() => {
            main.grid.noteMode = !main.grid.noteMode;
            notesButton.transform.GetChild(2).GetComponent<TMP_Text>().text = main.grid.noteMode ? "ON" : "OFF";
        });
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

        index = 1;
        foreach(var element in numberButtons){
            int index2 = index;
            element.GetComponent<Button>().onClick.AddListener(() => {
                main.grid.Place(index2);
                // sudoku.DrawAll();
                sudoku.Draw(main.grid.position);
            });
            index += 1;
        }

        foreach (var element in FindObjectsByType<TMP_Text>(FindObjectsInactive.Include, FindObjectsSortMode.None)){
            element.font = font;
        }
        InvokeRepeating("SaveGame", 2, 1);
        
    }

    public void ApplyTheme(bool full = false){ // set full to true during a new scene, uses more memory
        Theme theme = themes[userPref.themeIndex];
        Camera.main.backgroundColor = theme.background.ToRGB();

        header.GetComponent<Image>().color = theme.background.ToRGB();

        // update text and button color
        foreach (var element in FindObjectsByType<TMP_Text>(FindObjectsInactive.Include, FindObjectsSortMode.None)){
            Transform parent = element.transform.parent;
            if (element.color != theme.text2.ToRGB())
                element.color = Data.Grayscale(Data.Invert(theme.background.ToRGB()));
            
            if (parent.gameObject.HasComponent<Button>() && full){
                Image image = parent.gameObject.GetComponent<Image>();
                float trans = image.color.a;
                image.color = parent.gameObject == sudokuGrid ? theme.sudokuGrid.ToRGB() : theme.button.ToRGB();
                image.color = new Color(image.color.r, image.color.g, image.color.b, trans); // keep transparency
            }
        }

        if (full){
            sudoku.DrawAll();
            foreach (var element in FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                // make text-less buttons that same color as the text
                if (element.transform.childCount > 0){
                    if (!element.transform.GetChild(0).gameObject.HasComponent<TMP_Text>()){
                        element.GetComponent<Image>().color = theme.text.ToRGB();
                    }
                }
            }

            foreach(var element in FindObjectsByType<RawImage>(FindObjectsInactive.Include, FindObjectsSortMode.None)){
                if (element.texture == border){
                    element.color = theme.background.ToRGB();
                }
            }

            foreach(var element in new GameObject[]{hintButton, undoButton, notesButton}){
                element.transform.GetChild(1).GetComponent<Image>().color = theme.text.ToRGB();
            }
        }

        endGameContainer.GetComponent<Modal>().ApplyTheme(theme);
        completedGameContainer.GetComponent<Modal>().ApplyTheme(theme);

        backButton.GetComponent<RawImage>().color = theme.text.ToRGB();
        // make the toggle theme button's color to be the next theme
        toggleThemeButton.GetComponent<Image>().color = themes[(userPref.themeIndex + 1) % themes.Count].background.ToRGB();
        sudoku.highlightMaterial.color = theme.button.ToRGB();
        sudoku.highlightMaterial2.color = theme.text2.ToRGB();
        // sudoku.highlightMaterial2.color = Color.Lerp(theme.button.ToRGB(), new Color(0.7f, 0.7f, 0.7f, 1), 0.9f);
    }

    private void SaveGame(){
        if (isInGame){
            SaveData.Save(main.grid, userPref);
        }
    }

    private void Update() {
        ApplyTheme(); // constantly update theme
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
        
        main.grid.DrawAll(sudoku.gameObject, sudoku.textReference);
        main.grid.OnScoreChange(OnScoreChange);
        main.grid.OnMistake(() => OnMistake(false));
        SaveData.Save(main.grid, userPref);
        // ChangeNumber(1, numberButtons[0]);
        isInGame = true;
        adsBanner.LoadBanner();
        // main.grid.data = Data.DecodeSudokuString(main.grid.full); // immediately finish the game
    }

    public void LeaveGame(){
        continueButton.GetComponent<Button>().interactable = true;
        isInGame = false;
        ads2.LoadAd();
        if (endGameContainer.activeSelf){ // player made too many mistakes
            SaveData.WipeGridData();
            endGameContainer.GetComponent<Modal>().Close(0.1f);
            continueButton.GetComponent<Button>().interactable = false; // no saved game
            EndFade();
        }

        if (completedGameContainer.activeSelf){ // player completed the puzzle
            SaveData.WipeGridData();
            completedGameContainer.GetComponent<Modal>().Close(0.1f);
            continueButton.GetComponent<Button>().interactable = false; // no saved game
            EndFade();
        }

        TransitionScene(gameScene, homeScene);
        ads2.ShowAd();
        adsBanner.HideBannerAd();
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
            SudokuData data = SaveData.LoadGrid();
            if (data.time < 0.5f){
                TransitionScene(homeScene, newGameScene); // the grid data is most likely empty, which would cause an error
                return;
            }
            StartGame(0, data);
            TransitionScene(homeScene, gameScene); // game saved, load it
            // Wait(2, () => OnMistake(true));
            OnMistake(false);
        } else {
            TransitionScene(homeScene, newGameScene); // no game saved, create a new one
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
            if (from == gameScene){
                header.transform.parent.gameObject.SetActive(false);
            }
        });

        to.SetActive(true);
        if (to == gameScene){
            header.transform.parent.gameObject.SetActive(true);
        }
        toRect.localPosition = new Vector3(toRect.rect.width, 0, 0);
        LeanTween.moveLocal(to, Vector3.zero, transitionTime);
        ApplyTheme(true);
    }

    public void Wait(float time, Action action){ // waits a certain amount of time before executing an action
        LeanTween.value(0, 1, time).setOnComplete(action);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(UI))]
public class UIEditor : Editor {
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Toggle Banner Ad")){
            FindAnyObjectByType<AdsBanner>().HideBannerAd();
        }
    }
}
#endif