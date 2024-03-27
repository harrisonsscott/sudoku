using System;
using UnityEngine;
using UnityEngine.Advertisements;

[Serializable]
public class Credentials {
    public string androidGameID;
    public string iosGameID;
}

public class AdsInitializer : MonoBehaviour, IUnityAdsInitializationListener {
    public TextAsset credentialsJson; // not included in the project
    [SerializeField]
    bool testingMode = true;
    Credentials credentials;
    private string selectedID;
    

    private void Awake() {
        credentials = JsonUtility.FromJson<Credentials>(credentialsJson.text);
    }

    private void InitializeAds(){
        #if UNITY_IOS
            selectedID = credentials.iosGameID;
        #elif UNITY_ANDROID
            selectedID = credentials.androidGameID;
        #elif UNITY_EDITOR
            selectedID = credentials.androidGameID;
        #endif

        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(selectedID, testingMode, this);
        }
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
    }
 
    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error} - {message}");
    }
}