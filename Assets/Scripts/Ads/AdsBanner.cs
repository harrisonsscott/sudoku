using UnityEngine;
using UnityEngine.Advertisements;

public class AdsBanner : MonoBehaviour
{
    [SerializeField] BannerPosition _bannerPosition = BannerPosition.BOTTOM_CENTER;
 
    [SerializeField] string _androidAdUnitId = "Banner_Android";
    [SerializeField] string _iOSAdUnitId = "Banner_iOS";
    string id = null;
    
    void Start()
    {
        #if UNITY_IOS
                id = _iOSAdUnitId;
        #elif UNITY_ANDROID
                id = _androidAdUnitId;
        #elif UNITY_EDITOR
                id = _androidAdUnitId;
        #endif

        Advertisement.Banner.SetPosition(_bannerPosition);
    }
    public void LoadBanner()
    {
        BannerLoadOptions options = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        };
 
        Advertisement.Banner.Load(id, options);
    }

    public void ShowBannerAd()
    {
        if (GlobalConstants.disableAds) return;
        BannerOptions options = new BannerOptions
        {
            clickCallback = OnBannerClicked,
            hideCallback = OnBannerHidden,
            showCallback = OnBannerShown
        };
 
        Advertisement.Banner.Show(id, options);
    }
 
    public void HideBannerAd()
    {
        Advertisement.Banner.Hide();
    }

    void OnBannerError(string message)
    {
        Debug.Log($"Banner Error: {message}");
    }
 
    void OnBannerLoaded(){
        ShowBannerAd();
    }

    void OnBannerClicked() { }
    void OnBannerShown() { }
    void OnBannerHidden() { }
}