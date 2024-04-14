using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

// plays an ad when the button is clicked and gives a reward

public class AdsRewardedButton : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    private Button button;
    [SerializeField] string androidadUnitID = "Interstitial_Android";
    [SerializeField] string iosadUnitID = "Interstitial_iOS";
    string _adUnitID = null;
    public Action reward; // function that plays after the user watches the ad
    private float debounce; // prevents playing the reward function multiple times

    private void Start() {
        button = GetComponent<Button>();
    }
    private void Initialize() {
        #if UNITY_IOS
            _adUnitID = iosadUnitID;
        #elif UNITY_ANDROID
            _adUnitID = androidadUnitID;
        #elif UNITY_EDITOR
            _adUnitID = androidadUnitID;
        #endif

        button = GetComponent<Button>();
        // disable the button until the ads are loaded
        button.interactable = false;
    }

    private void Update() {
        debounce += Time.deltaTime;
    }

    public void LoadAd()
    {
        if (_adUnitID == null)
            Initialize();
        Debug.Log("Loading Ad: " + _adUnitID);
        Advertisement.Load(_adUnitID, this);
    }

     public void ShowAd()
    {
        // disable the button
        button.interactable = false;
        // show ad
        Advertisement.Show(_adUnitID, this);
    }

    public void OnUnityAdsAdLoaded(string adUnitID)
    {
        Debug.Log("Ad Loaded: " + adUnitID);
 
        if (adUnitID.Equals(_adUnitID))
        {
            // button.onClick.AddListener(ShowAd);
            /// re-enable the button
            button.interactable = true;
        }
    }

    public void OnUnityAdsShowComplete(string adUnitID, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitID.Equals(_adUnitID) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            Debug.Log("Unity Ads Rewarded Ad Completed");
            if (reward != null && debounce > 1){
                debounce = 0;
                reward();
                LoadAd();
            }
        }
    }
 
    public void OnUnityAdsFailedToLoad(string adUnitID, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit {adUnitID}: {error} - {message}");
        // Use the error details to determine whether to try to load another ad.
    }
 
    public void OnUnityAdsShowFailure(string adUnitID, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {adUnitID}: {error} - {message}");
        // Use the error details to determine whether to try to load another ad.
    }
 
    public void OnUnityAdsShowStart(string adUnitID) { }
    public void OnUnityAdsShowClick(string adUnitID) { }
 
    void OnDestroy()
    {
        // clean up listeners
        button.onClick.RemoveAllListeners();
    }
}

[CustomEditor(typeof(AdsRewardedButton))]
public class AdsRewardedButtonEditor : Editor {
    public override void OnInspectorGUI() {
        AdsRewardedButton ads = (AdsRewardedButton)target;

        base.OnInspectorGUI();

        if (GUILayout.Button("Load Ad")){
            ads.LoadAd();
        }

        if (GUILayout.Button("Show Ad")){
            ads.ShowAd();
        }
        
    }
}