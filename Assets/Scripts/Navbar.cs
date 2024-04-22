using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// place in the navigation bar content

public class Navbar : MonoBehaviour
{
    List<Button> buttons;

    private UI ui;

    void Start()
    {
        ui = FindObjectOfType<UI>();

        buttons = new List<Button>();
        for (int i = 0; i < transform.childCount; i++){
            Debug.Log(transform.GetChild(i).GetChild(2).name);
            Button button = transform.GetChild(i).GetChild(2).GetComponent<Button>();
            buttons.Add(button);
            button.onClick.AddListener(() => {
                Select(button);
            });
        }

        Select(buttons[0]);
    }

    public void Select(Button button){ // colors ones of the buttons when it's clicked
        Transform parent;
        // color all the other buttons gray
        foreach (Button element in buttons){
            parent = element.transform.parent;
            parent.GetChild(0).GetComponent<RawImage>().color = Color.gray;
            // parent.GetChild(1).GetComponent<TMP_Text>().color  = Color.gray;
        }

        // color the selected button
        parent = button.transform.parent;
        Color color = ui.themes[ui.userPref.themeIndex].button.ToRGB();
        parent.GetChild(0).GetComponent<RawImage>().color = color;
        // parent.GetChild(1).GetComponent<TMP_Text>().color = color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
