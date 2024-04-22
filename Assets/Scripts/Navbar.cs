using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

// place in the navigation bar content

public class Navbar : MonoBehaviour
{
    List<Button> buttons;
    public List<GameObject> scenes; // scenes that correspond to the buttons

    private UI ui;

    void Start()
    {
        ui = FindObjectOfType<UI>();

        buttons = new List<Button>();


        for (int i = 0; i < transform.childCount; i++){
            Debug.Log(transform.GetChild(i).GetChild(2).name);
            Button button = transform.GetChild(i).GetChild(2).GetComponent<Button>();
            if (i == 0)
                Select(button);
            buttons.Add(button);
            button.onClick.AddListener(() => {
                Select(button);

                // transition to another scene when clicked
                if (scenes[buttons.IndexOf(button)] != ui.currentScene){
                    ui.TransitionScene(ui.currentScene, scenes[buttons.IndexOf(button)]);
                }
            });
        }

    }

    public void Select(Button button){ // colors ones of the buttons when it's clicked
        // color all the other buttons gray
        foreach (Button element in buttons){
            element.transform.parent.GetChild(0).GetComponent<RawImage>().color = Color.gray;
            // parent.GetChild(1).GetComponent<TMP_Text>().color  = Color.gray;
        }

        // color the selected button
        Color color = ui.themes[ui.userPref.themeIndex].button.ToRGB();
        button.transform.parent.GetChild(0).GetComponent<RawImage>().color = color;
        // parent.GetChild(1).GetComponent<TMP_Text>().color = color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
