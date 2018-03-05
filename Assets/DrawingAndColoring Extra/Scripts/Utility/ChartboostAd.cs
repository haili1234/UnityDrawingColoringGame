using System;
using UnityEngine;
#if CHARTBOOST_ADS
using ChartboostSDK;
#endif
using System.Collections.Generic;
using System.Collections;

#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0414 // variable assigned but not used.

namespace IndieStudio.DrawingAndColoring.Utility
{
	[DisallowMultipleComponent]
	public class ChartboostAd: MonoBehaviour
	{
		#if CHARTBOOST_ADS
		private CBInPlay inPlayAd;
		private List<string> delegateHistory;
		private bool ageGate = false;
		private bool autocache = true;
		private bool activeAgeGate = false;
		private bool showInterstitial = true;
		private bool showMoreApps = true;
		private bool showRewardedVideo = true;

		#if UNITY_IPHONE
		private CBStatusBarBehavior statusBar = CBStatusBarBehavior.Ignore;
		#endif

		void OnEnable() {
		SetupDelegates();
		}

		void Start(){
		delegateHistory = new List<string>();
		#if UNITY_IPHONE
		Chartboost.setShouldPauseClickForConfirmation(ageGate);
		Chartboost.setStatusBarBehavior(statusBar);
		#endif
		Chartboost.setAutoCacheAds(autocache);
		Chartboost.setMediation (CBMediation.AdMob, "1.0");

		AddLog("Is Initialized: " + Chartboost.isInitialized());

		// Create the Chartboost gameobject with the editor AppId and AppSignature
		Chartboost.Create();
		}

		void SetupDelegates()
		{
		// Listen to all impression-related events
		Chartboost.didInitialize += didInitialize;
		Chartboost.didFailToLoadInterstitial += didFailToLoadInterstitial;
		Chartboost.didDismissInterstitial += didDismissInterstitial;
		Chartboost.didCloseInterstitial += didCloseInterstitial;
		Chartboost.didClickInterstitial += didClickInterstitial;
		Chartboost.didCacheInterstitial += didCacheInterstitial;
		Chartboost.shouldDisplayInterstitial += shouldDisplayInterstitial;
		Chartboost.didDisplayInterstitial += didDisplayInterstitial;
		Chartboost.didFailToLoadMoreApps += didFailToLoadMoreApps;
		Chartboost.didDismissMoreApps += didDismissMoreApps;
		Chartboost.didCloseMoreApps += didCloseMoreApps;
		Chartboost.didClickMoreApps += didClickMoreApps;
		Chartboost.didCacheMoreApps += didCacheMoreApps;
		Chartboost.shouldDisplayMoreApps += shouldDisplayMoreApps;
		Chartboost.didDisplayMoreApps += didDisplayMoreApps;
		Chartboost.didFailToRecordClick += didFailToRecordClick;
		Chartboost.didFailToLoadRewardedVideo += didFailToLoadRewardedVideo;
		Chartboost.didDismissRewardedVideo += didDismissRewardedVideo;
		Chartboost.didCloseRewardedVideo += didCloseRewardedVideo;
		Chartboost.didClickRewardedVideo += didClickRewardedVideo;
		Chartboost.didCacheRewardedVideo += didCacheRewardedVideo;
		Chartboost.shouldDisplayRewardedVideo += shouldDisplayRewardedVideo;
		Chartboost.didCompleteRewardedVideo += didCompleteRewardedVideo;
		Chartboost.didDisplayRewardedVideo += didDisplayRewardedVideo;
		Chartboost.didCacheInPlay += didCacheInPlay;
		Chartboost.didFailToLoadInPlay += didFailToLoadInPlay;
		Chartboost.didPauseClickForConfirmation += didPauseClickForConfirmation;
		Chartboost.willDisplayVideo += willDisplayVideo;
		#if UNITY_IPHONE
		Chartboost.didCompleteAppStoreSheetFlow += didCompleteAppStoreSheetFlow;
		#endif
		}

		void AddLog(string text)
		{
		Debug.Log(text);
		delegateHistory.Insert(0, text + "\n");
		int count = delegateHistory.Count;
		if( count > 20 )
		{
		delegateHistory.RemoveRange(20, count-20);
		}
		}

		public bool HasInterstitial(){
		return  Chartboost.hasInterstitial(CBLocation.Default);
		}

		public bool HasMoreApps(){
		return Chartboost.hasMoreApps(CBLocation.Default);;
		}

		public bool HasRewardedVideo(){
		return Chartboost.hasRewardedVideo(CBLocation.Default);;
		}

		public bool HasInPlay (){
		return Chartboost.hasInPlay(CBLocation.Default);
		}

		public void CacheInterstitial(){
		Chartboost.cacheInterstitial(CBLocation.Default);
		}

		public void ShowInterstitial(){
		Chartboost.showInterstitial(CBLocation.Default);
		}

		public void CacheMoreApps(){
		Chartboost.cacheMoreApps(CBLocation.Default);
		}

		public void ShowMoreApps(){
		Chartboost.showMoreApps(CBLocation.Default);
		}

		public void CacheRewardedVideo(){
		Chartboost.cacheRewardedVideo(CBLocation.Default);
		}

		public void  ShowRewardedVideo(){
		Chartboost.showRewardedVideo(CBLocation.Default);
		}

		public void CacheInPlayAd(){
		Chartboost.cacheInPlay(CBLocation.Default);
		}


		public void ShowInPlayAd(){
		inPlayAd = Chartboost.getInPlay(CBLocation.Default);
		if(inPlayAd != null) {
		inPlayAd.show();
		//inPlayAd.appName
		//if(GUILayout.Button(inPlayAd.appIcon, GUILayout.Width(ELEMENT_WIDTH))) {
		//	inPlayAd.click();
		//}
		}
		}

		public void SendPIAMainLevelEvent(){
		Chartboost.trackLevelInfo("Test Data", CBLevelType.HIGHEST_LEVEL_REACHED, 1, "Test Send mail level Information");
		}

		public void SendPIASubLevelEvent(){
		Chartboost.trackLevelInfo("Test Data", CBLevelType.HIGHEST_LEVEL_REACHED, 1, 2, "Test Send sub level Information");
		}

		void OnDisable() {
		// Remove event handlers
		Chartboost.didInitialize -= didInitialize;
		Chartboost.didFailToLoadInterstitial -= didFailToLoadInterstitial;
		Chartboost.didDismissInterstitial -= didDismissInterstitial;
		Chartboost.didCloseInterstitial -= didCloseInterstitial;
		Chartboost.didClickInterstitial -= didClickInterstitial;
		Chartboost.didCacheInterstitial -= didCacheInterstitial;
		Chartboost.shouldDisplayInterstitial -= shouldDisplayInterstitial;
		Chartboost.didDisplayInterstitial -= didDisplayInterstitial;
		Chartboost.didFailToLoadMoreApps -= didFailToLoadMoreApps;
		Chartboost.didDismissMoreApps -= didDismissMoreApps;
		Chartboost.didCloseMoreApps -= didCloseMoreApps;
		Chartboost.didClickMoreApps -= didClickMoreApps;
		Chartboost.didCacheMoreApps -= didCacheMoreApps;
		Chartboost.shouldDisplayMoreApps -= shouldDisplayMoreApps;
		Chartboost.didDisplayMoreApps -= didDisplayMoreApps;
		Chartboost.didFailToRecordClick -= didFailToRecordClick;
		Chartboost.didFailToLoadRewardedVideo -= didFailToLoadRewardedVideo;
		Chartboost.didDismissRewardedVideo -= didDismissRewardedVideo;
		Chartboost.didCloseRewardedVideo -= didCloseRewardedVideo;
		Chartboost.didClickRewardedVideo -= didClickRewardedVideo;
		Chartboost.didCacheRewardedVideo -= didCacheRewardedVideo;
		Chartboost.shouldDisplayRewardedVideo -= shouldDisplayRewardedVideo;
		Chartboost.didCompleteRewardedVideo -= didCompleteRewardedVideo;
		Chartboost.didDisplayRewardedVideo -= didDisplayRewardedVideo;
		Chartboost.didCacheInPlay -= didCacheInPlay;
		Chartboost.didFailToLoadInPlay -= didFailToLoadInPlay;
		Chartboost.didPauseClickForConfirmation -= didPauseClickForConfirmation;
		Chartboost.willDisplayVideo -= willDisplayVideo;
		#if UNITY_IPHONE
		Chartboost.didCompleteAppStoreSheetFlow -= didCompleteAppStoreSheetFlow;
		#endif
		}


		void didInitialize(bool status) {
		AddLog(string.Format("didInitialize: {0}", status));
		}

		void didFailToLoadInterstitial(CBLocation location, CBImpressionError error) {
		AddLog(string.Format("didFailToLoadInterstitial: {0} at location {1}", error, location));
		}

		void didDismissInterstitial(CBLocation location) {
		AddLog("didDismissInterstitial: " + location);
		}

		void didCloseInterstitial(CBLocation location) {
		AddLog("didCloseInterstitial: " + location);
		}

		void didClickInterstitial(CBLocation location) {
		AddLog("didClickInterstitial: " + location);
		}

		void didCacheInterstitial(CBLocation location) {
		AddLog("didCacheInterstitial: " + location);
		}

		bool shouldDisplayInterstitial(CBLocation location) {
		// return true if you want to allow the interstitial to be displayed
		AddLog("shouldDisplayInterstitial @" + location + " : " + showInterstitial);
		return showInterstitial;
		}

		void didDisplayInterstitial(CBLocation location){
		AddLog("didDisplayInterstitial: " + location);
		}

		void didFailToLoadMoreApps(CBLocation location, CBImpressionError error) {
		AddLog(string.Format("didFailToLoadMoreApps: {0} at location: {1}", error, location));
		}

		void didDismissMoreApps(CBLocation location) {
		AddLog(string.Format("didDismissMoreApps at location: {0}", location));
		}

		void didCloseMoreApps(CBLocation location) {
		AddLog(string.Format("didCloseMoreApps at location: {0}", location));
		}

		void didClickMoreApps(CBLocation location) {
		AddLog(string.Format("didClickMoreApps at location: {0}", location));
		}

		void didCacheMoreApps(CBLocation location) {
		AddLog(string.Format("didCacheMoreApps at location: {0}", location));
		}

		bool shouldDisplayMoreApps(CBLocation location) {
		AddLog(string.Format("shouldDisplayMoreApps at location: {0}: {1}", location, showMoreApps));
		return showMoreApps;
		}

		void didDisplayMoreApps(CBLocation location){
		AddLog("didDisplayMoreApps: " + location);
		}

		void didFailToRecordClick(CBLocation location, CBClickError error) {
		AddLog(string.Format("didFailToRecordClick: {0} at location: {1}", error, location));
		}

		void didFailToLoadRewardedVideo(CBLocation location, CBImpressionError error) {
		AddLog(string.Format("didFailToLoadRewardedVideo: {0} at location {1}", error, location));
		}

		void didDismissRewardedVideo(CBLocation location) {
		AddLog("didDismissRewardedVideo: " + location);
		}

		void didCloseRewardedVideo(CBLocation location) {
		AddLog("didCloseRewardedVideo: " + location);
		}

		void didClickRewardedVideo(CBLocation location) {
		AddLog("didClickRewardedVideo: " + location);
		}

		void didCacheRewardedVideo(CBLocation location) {
		AddLog("didCacheRewardedVideo: " + location);
		}

		bool shouldDisplayRewardedVideo(CBLocation location) {
		AddLog("shouldDisplayRewardedVideo @" + location + " : " + showRewardedVideo);
		return showRewardedVideo;
		}

		void didCompleteRewardedVideo(CBLocation location, int reward) {
		AddLog(string.Format("didCompleteRewardedVideo: reward {0} at location {1}", reward, location));
		}

		void didDisplayRewardedVideo(CBLocation location){
		AddLog("didDisplayRewardedVideo: " + location);
		}

		void didCacheInPlay(CBLocation location) {
		AddLog("didCacheInPlay called: "+location);
		}

		void didFailToLoadInPlay(CBLocation location, CBImpressionError error) {
		AddLog(string.Format("didFailToLoadInPlay: {0} at location: {1}", error, location));
		}

		void didPauseClickForConfirmation() {
		#if UNITY_IPHONE
		AddLog("didPauseClickForConfirmation called");
		activeAgeGate = true;
		#endif
		}

		void willDisplayVideo(CBLocation location) {
		AddLog("willDisplayVideo: " + location);
		}

		#if UNITY_IPHONE
		void didCompleteAppStoreSheetFlow() {
		AddLog("didCompleteAppStoreSheetFlow");
		}

		void TrackIAP() {
		// The iOS receipt data from Unibill is already base64 encoded.
		// Others store kit plugins may be different.
		// This is a sample sandbox receipt.
		string sampleReceipt = @"ewoJInNpZ25hdHVyZSIgPSAiQXBNVUJDODZBbHpOaWtWNVl0clpBTWlKUWJLOEVk
		ZVhrNjNrV0JBWHpsQzhkWEd1anE0N1puSVlLb0ZFMW9OL0ZTOGNYbEZmcDlZWHQ5
		aU1CZEwyNTBsUlJtaU5HYnloaXRyeVlWQVFvcmkzMlc5YVIwVDhML2FZVkJkZlcr
		T3kvUXlQWkVtb05LeGhudDJXTlNVRG9VaFo4Wis0cFA3MHBlNWtVUWxiZElWaEFB
		QURWekNDQTFNd2dnSTdvQU1DQVFJQ0NHVVVrVTNaV0FTMU1BMEdDU3FHU0liM0RR
		RUJCUVVBTUg4eEN6QUpCZ05WQkFZVEFsVlRNUk13RVFZRFZRUUtEQXBCY0hCc1pT
		QkpibU11TVNZd0pBWURWUVFMREIxQmNIQnNaU0JEWlhKMGFXWnBZMkYwYVc5dUlF
		RjFkR2h2Y21sMGVURXpNREVHQTFVRUF3d3FRWEJ3YkdVZ2FWUjFibVZ6SUZOMGIz
		SmxJRU5sY25ScFptbGpZWFJwYjI0Z1FYVjBhRzl5YVhSNU1CNFhEVEE1TURZeE5U
		SXlNRFUxTmxvWERURTBNRFl4TkRJeU1EVTFObG93WkRFak1DRUdBMVVFQXd3YVVI
		VnlZMmhoYzJWU1pXTmxhWEIwUTJWeWRHbG1hV05oZEdVeEd6QVpCZ05WQkFzTUVr
		RndjR3hsSUdsVWRXNWxjeUJUZEc5eVpURVRNQkVHQTFVRUNnd0tRWEJ3YkdVZ1NX
		NWpMakVMTUFrR0ExVUVCaE1DVlZNd2daOHdEUVlKS29aSWh2Y05BUUVCQlFBRGdZ
		MEFNSUdKQW9HQkFNclJqRjJjdDRJclNkaVRDaGFJMGc4cHd2L2NtSHM4cC9Sd1Yv
		cnQvOTFYS1ZoTmw0WElCaW1LalFRTmZnSHNEczZ5anUrK0RyS0pFN3VLc3BoTWRk
		S1lmRkU1ckdYc0FkQkVqQndSSXhleFRldngzSExFRkdBdDFtb0t4NTA5ZGh4dGlJ
		ZERnSnYyWWFWczQ5QjB1SnZOZHk2U01xTk5MSHNETHpEUzlvWkhBZ01CQUFHamNq
		QndNQXdHQTFVZEV3RUIvd1FDTUFBd0h3WURWUjBqQkJnd0ZvQVVOaDNvNHAyQzBn
		RVl0VEpyRHRkREM1RllRem93RGdZRFZSMFBBUUgvQkFRREFnZUFNQjBHQTFVZERn
		UVdCQlNwZzRQeUdVakZQaEpYQ0JUTXphTittVjhrOVRBUUJnb3Foa2lHOTJOa0Jn
		VUJCQUlGQURBTkJna3Foa2lHOXcwQkFRVUZBQU9DQVFFQUVhU2JQanRtTjRDL0lC
		M1FFcEszMlJ4YWNDRFhkVlhBZVZSZVM1RmFaeGMrdDg4cFFQOTNCaUF4dmRXLzNl
		VFNNR1k1RmJlQVlMM2V0cVA1Z204d3JGb2pYMGlreVZSU3RRKy9BUTBLRWp0cUIw
		N2tMczlRVWU4Y3pSOFVHZmRNMUV1bVYvVWd2RGQ0TndOWXhMUU1nNFdUUWZna1FR
		Vnk4R1had1ZIZ2JFL1VDNlk3MDUzcEdYQms1MU5QTTN3b3hoZDNnU1JMdlhqK2xv
		SHNTdGNURXFlOXBCRHBtRzUrc2s0dHcrR0szR01lRU41LytlMVFUOW5wL0tsMW5q
		K2FCdzdDMHhzeTBiRm5hQWQxY1NTNnhkb3J5L0NVdk02Z3RLc21uT09kcVRlc2Jw
		MGJzOHNuNldxczBDOWRnY3hSSHVPTVoydG04bnBMVW03YXJnT1N6UT09IjsKCSJw
		dXJjaGFzZS1pbmZvIiA9ICJld29KSW05eWFXZHBibUZzTFhCMWNtTm9ZWE5sTFdS
		aGRHVXRjSE4wSWlBOUlDSXlNREV5TFRBMExUTXdJREE0T2pBMU9qVTFJRUZ0WlhK
		cFkyRXZURzl6WDBGdVoyVnNaWE1pT3dvSkltOXlhV2RwYm1Gc0xYUnlZVzV6WVdO
		MGFXOXVMV2xrSWlBOUlDSXhNREF3TURBd01EUTJNVGM0T0RFM0lqc0tDU0ppZG5K
		eklpQTlJQ0l5TURFeU1EUXlOeUk3Q2draWRISmhibk5oWTNScGIyNHRhV1FpSUQw
		Z0lqRXdNREF3TURBd05EWXhOemc0TVRjaU93b0pJbkYxWVc1MGFYUjVJaUE5SUNJ
		eElqc0tDU0p2Y21sbmFXNWhiQzF3ZFhKamFHRnpaUzFrWVhSbExXMXpJaUE5SUNJ
		eE16TTFOems0TXpVMU9EWTRJanNLQ1NKd2NtOWtkV04wTFdsa0lpQTlJQ0pqYjIw
		dWJXbHVaRzF2WW1Gd2NDNWtiM2R1Ykc5aFpDSTdDZ2tpYVhSbGJTMXBaQ0lnUFNB
		aU5USXhNVEk1T0RFeUlqc0tDU0ppYVdRaUlEMGdJbU52YlM1dGFXNWtiVzlpWVhC
		d0xrMXBibVJOYjJJaU93b0pJbkIxY21Ob1lYTmxMV1JoZEdVdGJYTWlJRDBnSWpF
		ek16VTNPVGd6TlRVNE5qZ2lPd29KSW5CMWNtTm9ZWE5sTFdSaGRHVWlJRDBnSWpJ
		d01USXRNRFF0TXpBZ01UVTZNRFU2TlRVZ1JYUmpMMGROVkNJN0Nna2ljSFZ5WTJo
		aGMyVXRaR0YwWlMxd2MzUWlJRDBnSWpJd01USXRNRFF0TXpBZ01EZzZNRFU2TlRV
		Z1FXMWxjbWxqWVM5TWIzTmZRVzVuWld4bGN5STdDZ2tpYjNKcFoybHVZV3d0Y0hW
		eVkyaGhjMlV0WkdGMFpTSWdQU0FpTWpBeE1pMHdOQzB6TUNBeE5Ub3dOVG8xTlNC
		RmRHTXZSMDFVSWpzS2ZRPT0iOwoJImVudmlyb25tZW50IiA9ICJTYW5kYm94IjsK
		CSJwb2QiID0gIjEwMCI7Cgkic2lnbmluZy1zdGF0dXMiID0gIjAiOwp9";

		// Demonstrate Base64 encoding. Not necessary for the data above
		// If the receipt was not base64 encoded, send encodedText not sampleReceipt 
		//byte[] bytesToEncode = Encoding.UTF8.GetBytes(sampleReceipt);
		//string encodedText = Convert.ToBase64String(bytesToEncode);

		// Send the receipt for track an In App Purchase Event
		Chartboost.trackInAppAppleStorePurchaseEvent(sampleReceipt,
		"sample product title", "sample product description", "1.99", "USD", "sample product identifier" );
		//byte[] decodedText = Convert.FromBase64String(sampleReceipt);
		//Debug.Log("Decoded: " + System.Text.Encoding.UTF8.GetString(decodedText));
		//Debug.Log("Encoded: " + encodedText);
		}
		#elif UNITY_ANDROID
		void TrackIAP() {
		Debug.Log("TrackIAP");
		// title, description, price, currency, productID, purchaseData, purchaseSignature
		// This data should be sent after handling the results from the google store.
		// This is fake data and doesn't represent a real or even imaginary purchase
		Chartboost.trackInAppGooglePlayPurchaseEvent("SampleItem", "TestPurchase", "0.99", "USD", "ProductID", "PurchaseData", "PurchaseSignature");

		// If you are using the Amazon store...		
		//Chartboost.trackInAppAmazonStorePurchaseEvent("SampleItem", "TestPurchase", "0.99", "ProductID", "UserId", "PurchaseToken");
		}
		#else
		void TrackIAP() {
		Debug.Log("TrackIAP on unsupported platform");
		}
		#endif

		#endif
	}
}