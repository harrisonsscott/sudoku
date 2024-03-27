using System;
using UnityEngine;

public class Modal : MonoBehaviour {
    [SerializeField] Vector2 originalSize;
    [SerializeField] RectTransform rect;

    private void Awake() {
        rect = GetComponent<RectTransform>();
        originalSize = rect.sizeDelta;
    }

    public void Close(Action onFinished, float time = 0.2f){
        // disable children
        for (int i = 0; i < gameObject.transform.childCount; i++){   
            gameObject.transform.GetChild(i).gameObject.SetActive(false);
        }
        
        // scale height to 0
        LeanTween.size(rect, new Vector2(originalSize.x, 0), time).setOnComplete(() => {
            gameObject.SetActive(false);
            onFinished();
        });
    }

    public void Close(float time = 0.2f){
        Close(() => {}, time);
    }

    public void Open(Action onFinished, float time = 0.2f){
        gameObject.SetActive(true);
        GetComponent<RectTransform>().sizeDelta = new Vector2(originalSize.x, 0);

        // disable children
        for (int i = 0; i < gameObject.transform.childCount; i++){   
            gameObject.transform.GetChild(i).gameObject.SetActive(false);
        }

        // scale to original size
        LeanTween.size(rect, originalSize, time).setOnComplete(() => {
            // re-enable children
            for (int i = 0; i < gameObject.transform.childCount; i++){   
                gameObject.transform.GetChild(i).gameObject.SetActive(true);
            }
            onFinished();
        });
    }

    public void Open(float time = 0.2f){
        Open(() => {}, time);
    }
}