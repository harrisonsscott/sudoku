using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Unity.Profiling;

public class Sudoku : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    [Range(1, 9)]
    public int number;
    public Main main;
    public GameObject textReference;
    public Material highlightMaterial; // a special shader so the transparency doesn't overlap and look weird
    public Material highlightMaterial2; // secondary highlight material

    private GameObject highlightX;
    private GameObject highlightY;
    private GameObject highlightZ; // highlights the current section
    private GameObject highlightW; // highlight the exact location more

    private void Start() {
        highlightX = new GameObject("highlightX");

        Image image = highlightX.AddComponent<Image>();
        image.color = new Color(1, 1, 1, 0.5f);
        image.raycastTarget = false;
        image.material = highlightMaterial;

        highlightX.transform.parent = FindAnyObjectByType<Sudoku>().transform.parent;
        highlightX.transform.localScale = new Vector3(1,1,1);
        highlightX.AddComponent<RectTransform>();
        
        Button button = highlightX.AddComponent<Button>(); // not interacble, for themes
        button.interactable = false;
        button.enabled = false;

        GameObject child = Instantiate(textReference); // not used, for themes
        child.transform.parent = highlightX.transform;
        child.GetComponent<TMP_Text>().text = "";

        highlightY = Instantiate(highlightX);
        highlightY.transform.parent = highlightX.transform.parent;
        highlightY.transform.localScale = new Vector3(1,1,1);
        highlightY.name = "highlightY";

        highlightZ = Instantiate(highlightX);
        highlightZ.transform.parent = highlightX.transform.parent;
        highlightZ.transform.localScale = new Vector3(1,1,1);
        highlightZ.name = "highlightZ";

        highlightW = Instantiate(highlightX);
        highlightW.transform.parent = highlightX.transform.parent;
        highlightW.transform.localScale = new Vector3(1,1,1);
        highlightW.name = "highlightW";
        highlightW.GetComponent<Image>().material = highlightMaterial2;
        highlightW.transform.SetSiblingIndex(highlightX.transform.GetSiblingIndex());
    }

    public void DrawAll(){
        main.grid.DrawAll(gameObject, textReference); // draw the grid data onto the image
    }

    public void Draw(Vector2Int pos){
        main.grid.Draw(gameObject, pos);
    }

    public void Draw(Vector2 pos){
        main.grid.Draw(gameObject, new Vector2Int((int)pos.x, (int)pos.y));
    }

    public void Highlight(Vector2 pos){
        main.grid.Highlight(gameObject, pos, highlightX, highlightY, highlightZ, highlightW);
    }

    public void OnPointerClick(PointerEventData eventData){
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Vector2 clickPosition = eventData.position;
            Vector2 localPosition;
            RectTransform rect = GetComponent<RectTransform>();

            RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, clickPosition, eventData.pressEventCamera, out localPosition);
            localPosition -= new Vector2(-rect.sizeDelta.x / 2, rect.sizeDelta.y / 2);

            Vector2 gridSize = new Vector2(rect.sizeDelta.x/9, rect.sizeDelta.y/9);
            Vector2 gridPosition = new Vector2((int)(localPosition.x/gridSize.x),   Math.Abs((int)(localPosition.y/gridSize.y)));

            // main.grid.Place(gridPosition, number);
            main.grid.position = gridPosition;
            // DrawAll();
            main.grid.Highlight(gameObject, gridPosition, highlightX, highlightY, highlightZ, highlightW);
        }
    }

}
