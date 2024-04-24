using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// hides the header of something when that somebody is disabled

public class Header : MonoBehaviour
{
    public GameObject matchVisibility;
    void Update()
    {
        if (matchVisibility.activeSelf == false){
            gameObject.SetActive(false);
        }
    }
}
