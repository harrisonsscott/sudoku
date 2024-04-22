using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

// place in the navigation bar content

public class Navbar : MonoBehaviour
{
    public List<Button> buttons; // set automatically
    public List<GameObject> scenes; // scenes that correspond to the buttons
    public Button currentButton; // button that is currently selected

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
        currentButton = button;
        Theme theme = ui.themes[ui.userPref.themeIndex];

        // color all the other buttons gray
        foreach (Button element in buttons){
            element.transform.parent.GetChild(0).GetComponent<RawImage>().color = theme.text.ToRGB();
            // parent.GetChild(1).GetComponent<TMP_Text>().color  = Color.gray;
        }

        // color the selected button
        Color color = theme.button.ToRGB();
        button.transform.parent.GetChild(0).GetComponent<RawImage>().color = theme.text2.ToRGB();
        // parent.GetChild(1).GetComponent<TMP_Text>().color = color;
    }

    // Update is called once per frame
    void Update()
    {
        Color color = ui.themes[ui.userPref.themeIndex].background.ToRGB();
        transform.GetComponent<Image>().color = color;
    }
}
