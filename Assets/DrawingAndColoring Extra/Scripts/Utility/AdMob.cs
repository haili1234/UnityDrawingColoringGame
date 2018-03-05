using UnityEngine;
using System.Collections;
using System;
#if GOOGLE_MOBILE_ADS
using GoogleMobileAds.Api;
#endif

///Developed By Indie Studio
///https://www.assetstore.unity3d.com/en/#!/publisher/9268
///www.indiestd.com
///info@indiestd.com

namespace IndieStudio.DrawingAndColoring.Utility
{
	[DisallowMultipleComponent]
	public class AdMob : MonoBehaviour
	{
		#if GOOGLE_MOBILE_ADS
		private BannerView bannerView;
		private InterstitialAd interstitial;
		private RewardBasedVideoAd rewardBasedVideo;
		public string androidBannerAdUnitID;
		public string IOSBannerAdUnitID;
		public string androidInterstitialAdUnitID;
		public string IOSInterstitialAdUnitID;
		public string androidRewardBasedVideoAdUnitID;
		public string IOSRewardBasedVideoAdUnitID;
		private AdPosition currentBannerPostion;

		void Start ()
		{
		// Get singleton reward based video ad reference.
		this.rewardBasedVideo = RewardBasedVideoAd.Instance;

		this.rewardBasedVideo.OnAdLoaded += this.HandleRewardBasedVideoLoaded;
		this.rewardBasedVideo.OnAdFailedToLoad += this.HandleRewardBasedVideoFailedToLoad;
		this.rewardBasedVideo.OnAdOpening += this.HandleRewardBasedVideoOpened;
		this.rewardBasedVideo.OnAdStarted += this.HandleRewardBasedVideoStarted;
		this.rewardBasedVideo.OnAdRewarded += this.HandleRewardBasedVideoRewarded;
		this.rewardBasedVideo.OnAdClosed += this.HandleRewardBasedVideoClosed;
		this.rewardBasedVideo.OnAdLeavingApplication += this.HandleRewardBasedVideoLeftApplication;
		}

		public void RequestBanner (AdPosition adPostion)
		{
		if (adPostion == null) {
		return;
		}

		DestroyBanner ();

		#if UNITY_EDITOR
		string adUnitId = "unused";
		#elif UNITY_ANDROID
		string adUnitId = androidBannerAdUnitID;
		#elif UNITY_IPHONE
		string adUnitId = IOSBannerAdUnitID;
		#else
		string adUnitId = "unexpected_platform";
		#endif

		currentBannerPostion = adPostion;

		// Create a banner
		this.bannerView = new BannerView (adUnitId, AdSize.Banner, adPostion);

		// Register for ad events.
		this.bannerView.OnAdLoaded += this.HandleBannerLoaded;
		this.bannerView.OnAdFailedToLoad += this.HandleBannerFailedToLoad;
		this.bannerView.OnAdOpening += this.HandleBannerOpened;
		this.bannerView.OnAdClosed += this.HandleBannerClosed;
		this.bannerView.OnAdLeavingApplication += this.HandleBannerLeftApplication;

		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder ().Build ();

		// Load the banner with the request.
		this.bannerView.LoadAd (request);
		}

		public void RequestInterstitial ()
		{
		#if UNITY_EDITOR
		string adUnitId = "unused";
		#elif UNITY_ANDROID
		string adUnitId = androidInterstitialAdUnitID;
		#elif UNITY_IPHONE
		string adUnitId = IOSInterstitialAdUnitID;
		#else
		string adUnitId = "unexpected_platform";
		#endif

		// Initialize an InterstitialAd.
		this.interstitial = new InterstitialAd (adUnitId);

		// Register for ad events.
		this.interstitial.OnAdLoaded += this.HandleInterstitialLoaded;
		this.interstitial.OnAdFailedToLoad += this.HandleInterstitialFailedToLoad;
		this.interstitial.OnAdOpening += this.HandleInterstitialOpened;
		this.interstitial.OnAdClosed += this.HandleInterstitialClosed;
		this.interstitial.OnAdLeavingApplication += this.HandleInterstitialLeftApplication;

		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder ().Build ();

		// Load the interstitial with the request.
		this.interstitial.LoadAd (request);
		}

		public void RequestRewardBasedVideo ()
		{
		#if UNITY_EDITOR
		string adUnitId = "unused";
		#elif UNITY_ANDROID
		string adUnitId = androidRewardBasedVideoAdUnitID;
		#elif UNITY_IPHONE
		string adUnitId = IOSRewardBasedVideoAdUnitID;
		#else
		string adUnitId = "unexpected_platform";
		#endif

		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder ().Build ();

		this.rewardBasedVideo.LoadAd (request, adUnitId);
		}

		// Returns an ad request with custom ad targeting.
		private AdRequest createAdRequest ()
		{
		return new AdRequest.Builder ()
		.AddTestDevice (AdRequest.TestDeviceSimulator) // Simulator.
		.AddTestDevice ("DeviceID") // Your device ID.
		.Build ();
		}

		private void ShowBanner ()
		{
		if (this.bannerView == null) {
		return;
		}
		this.bannerView.Show ();
		}

		private void ShowInterstitial ()
		{
		if (this.interstitial == null) {
		return;
		}

		if (this.interstitial.IsLoaded ()) {
		this.interstitial.Show ();
		}
		}

		private void ShowRewardBasedVideo ()
		{
		if (this.rewardBasedVideo == null) {
		return;
		}

		if (rewardBasedVideo.IsLoaded ()) {
		this.rewardBasedVideo.Show ();
		}
		}

		public void DestroyBanner ()
		{
		if (this.bannerView == null) {
		return;
		}
		this.bannerView.Destroy ();
		}

		public void DestroyInterstitial ()
		{
		if (this.interstitial == null) {
		return;
		}
		this.interstitial.Destroy ();
		}

		#region Banner callback handlers

		public void HandleBannerLoaded (object sender, EventArgs args)
		{
		ShowBanner ();
		//MonoBehaviour.print ("HandleBannerLoaded event received");
		}

		public void HandleBannerFailedToLoad (object sender, AdFailedToLoadEventArgs args)
		{
		//MonoBehaviour.print ("HandleBannerFailedToLoad event received with message: " + args.Message);
		}

		public void HandleBannerOpened (object sender, EventArgs args)
		{
		//MonoBehaviour.print ("HandleBannerOpened event received");
		}

		public void HandleBannerClosed (object sender, EventArgs args)
		{
		//MonoBehaviour.print ("HandleBannerClosed event received");
		}

		public void HandleBannerLeftApplication (object sender, EventArgs args)
		{
		//MonoBehaviour.print ("HandleBannerLeftApplication event received");
		}

		#endregion

		#region Interstitial callback handlers

		public void HandleInterstitialLoaded (object sender, EventArgs args)
		{
		ShowInterstitial ();
		//MonoBehaviour.print ("HandleInterstitialLoaded event received");
		}

		public void HandleInterstitialFailedToLoad (object sender, AdFailedToLoadEventArgs args)
		{
		//MonoBehaviour.print ("HandleInterstitialFailedToLoad event received with message: " + args.Message);
		}

		public void HandleInterstitialOpened (object sender, EventArgs args)
		{
		//MonoBehaviour.print ("HandleInterstitialOpened event received");
		}

		public void HandleInterstitialClosed (object sender, EventArgs args)
		{
		//MonoBehaviour.print ("HandleInterstitialClosed event received");
		DestroyInterstitial ();
		}

		public void HandleInterstitialLeftApplication (object sender, EventArgs args)
		{
		//MonoBehaviour.print ("HandleInterstitialLeftApplication event received");
		}

		#endregion

		#region RewardBasedVideo callback handlers

		public void HandleRewardBasedVideoLoaded (object sender, EventArgs args)
		{
		ShowRewardBasedVideo ();
		//MonoBehaviour.print ("HandleRewardBasedVideoLoaded event received");
		}

		public void HandleRewardBasedVideoFailedToLoad (object sender, AdFailedToLoadEventArgs args)
		{
		//MonoBehaviour.print ("HandleRewardBasedVideoFailedToLoad event received with message: " + args.Message);
		}

		public void HandleRewardBasedVideoOpened (object sender, EventArgs args)
		{
		//MonoBehaviour.print ("HandleRewardBasedVideoOpened event received");
		}

		public void HandleRewardBasedVideoStarted (object sender, EventArgs args)
		{
		//MonoBehaviour.print ("HandleRewardBasedVideoStarted event received");
		}

		public void HandleRewardBasedVideoClosed (object sender, EventArgs args)
		{
		//MonoBehaviour.print ("HandleRewardBasedVideoClosed event received");
		}

		public void HandleRewardBasedVideoRewarded (object sender, Reward args)
		{
		string type = args.Type;
		double amount = args.Amount;
		//MonoBehaviour.print ("HandleRewardBasedVideoRewarded event received for " + amount.ToString () + " " + type);
		}

		public void HandleRewardBasedVideoLeftApplication (object sender, EventArgs args)
		{
		//MonoBehaviour.print ("HandleRewardBasedVideoLeftApplication event received");
		}

		#endregion

		#endif
		}
		}