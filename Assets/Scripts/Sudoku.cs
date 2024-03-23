using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Sudoku : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    [Range(1, 9)]
    public int number;
    public Main main;
    public GameObject textReference;
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

            main.grid.Place(gridPosition, number);
            main.grid.Draw(gameObject, textReference);
        }
    }

}
