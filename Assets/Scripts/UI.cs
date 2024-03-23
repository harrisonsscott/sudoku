using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// handles all the UI functions
public class UI : MonoBehaviour
{
    [Header("GameObjects")]
    public GameObject newGameButton;
    public GameObject homeScene;
    public GameObject newGameScene;
    public GameObject gameScene;
    public GameObject sudokuGrid;
    public List<GameObject> difficultyButtons; // the easy, medium, etc buttons when you're making a new game
    public List<GameObject> numberButtons; // the buttons that let you change the number to place
    [Header("Other")]
    public Sudoku sudoku;
    public Main main;
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
                main.grid = Data.GetSudokuGrid(main.jsonFile);
                main.grid.Draw(sudoku.gameObject, sudoku.textReference);
                numberButtons[0].GetComponent<Image>().color = numberButtonsStartingColor - new Color(0.1f, 0.1f, 0.1f, 0);
            });
        }
    }
    public void changeNumber(int num){ // called by the buttons that let you change the number to place
        sudoku.number = num;
        foreach (var element in numberButtons){
            if (element == EventSystem.current.currentSelectedGameObject){
                element.GetComponent<Image>().color = numberButtonsStartingColor - new Color(0.1f, 0.1f, 0.1f, 0);
            } else {
                element.GetComponent<Image>().color = numberButtonsStartingColor;
            }
        }
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
