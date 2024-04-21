using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// used for spacing ui objects
public class Spacer : MonoBehaviour
{

    public float ratio; // ratio of screen height

    void Update()
    {
        GetComponent<RectTransform>().sizeDelta = new Vector2(0, ratio * Screen.height);
    }
}
