using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class scriptMain : MonoBehaviour {
	private string sStatus = "-";
	private LocationInfo liOld;
	private LocationInfo liNew;
	
	private double dLastTimestamp = 0;
	
	private Color cRed = new Color(1.000000000f,0.266666667f,0.180392157f, 1.0f);
	private Color cBlue = new Color(0.243137255f,0.745098039f,1.000000000f, 1.0f);
	private Color cGrey = new Color(0.635294118f,0.698039216f,0.749019608f, 1.0f);
	private Color cWhite = new Color(1.0f, 1.0f, 1.0f, 1.0f);
	
	private tk2dTextMesh tmStatus; //DEBUG
	private tk2dTextMesh tmMaxSpeed;
	private tk2dTextMesh tmAvgSpeed;
	private tk2dTextMesh tmCurSpeed;
	private tk2dTextMesh tmLapNumber;
	private tk2dTextMesh tmLapTime;
	private tk2dTextMesh tmGPS;
	private tk2dTextMesh tmBigInfo;
	private tk2dTextMesh tmBigInfoCaption;
	
	private tk2dButton btnStartStop;
	private tk2dSprite spriteStartStop;
	private tk2dButton btnSettings;
	private tk2dSprite spriteSettings;
	
	public enum GpsStates
    {
        Begin,
        RunningAccuracyOk,
		RunningAccuracyBad,
		Initializing,
		Faild,
		Stoped
    }
	
	private GpsStates gpsState = GpsStates.Begin;
	private GpsStates oldGpsState = GpsStates.Begin;
	
	private bool bUIEnabled = true; 
	
	private bool bRunning = false;
	
	private const double  dMS2KN = 1.97260274d;
	private double dLastLat = 0;
	private double dLastLon = 0;
	
	
	
	 
	// Start is called just before any of the
	// Update methods is called the first time.
	void Start () {
		// Grab all textmeshes we gonna change
    	tmStatus = GameObject.Find ("tmStatus").GetComponent<tk2dTextMesh>(); //DEBUG
		tmMaxSpeed = GameObject.Find ("tmMaxSpeed").GetComponent<tk2dTextMesh>();
		tmAvgSpeed = GameObject.Find ("tmAvgSpeed").GetComponent<tk2dTextMesh>();
		tmCurSpeed = GameObject.Find ("tmCurSpeed").GetComponent<tk2dTextMesh>();
		tmLapNumber = GameObject.Find ("tmLapNumber").GetComponent<tk2dTextMesh>();
		tmLapTime = GameObject.Find ("tmLapTime").GetComponent<tk2dTextMesh>();
		tmGPS = GameObject.Find ("tmGPS").GetComponent<tk2dTextMesh>();
		tmBigInfo = GameObject.Find ("tmBigInfo").GetComponent<tk2dTextMesh>();
		tmBigInfoCaption = GameObject.Find ("tmBigInfoCaption").GetComponent<tk2dTextMesh>();
		// And the start/stop and settings button
		spriteStartStop = GameObject.Find ("spriteStartStop").GetComponent<tk2dSprite>();
		btnStartStop = spriteStartStop.GetComponent<tk2dButton>();
		spriteSettings = GameObject.Find ("spriteSettings").GetComponent<tk2dSprite>();
		btnSettings = spriteSettings.GetComponent<tk2dButton>();
		
		//Start the location service
		Input.location.Start(1.0f, 1.0f);
		ResetGUI();
		EnableUI (false);
	}
	
	void ResetGUI(){
		tmMaxSpeed.text = "- kn";
		tmAvgSpeed.text = "- kn";
		tmCurSpeed.text = "- kn";
		tmLapNumber.text = "0";
		tmLapTime.text = "0:00.0";
		tmGPS.text = "-- m";
		tmBigInfoCaption.text = "STOP";
		tmBigInfo.text = ".";
		
		tmMaxSpeed.Commit ();
		tmAvgSpeed.Commit ();
		tmCurSpeed.Commit ();
		tmLapNumber.Commit ();
		tmLapTime.Commit ();
		tmGPS.Commit ();
		tmBigInfoCaption.Commit ();
		tmBigInfo.Commit ();
		
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}
	
	void EnableUI(bool bEnable) {
		if(bEnable != bUIEnabled ) {
			GameObject[] goEnableDisableTexts = GameObject.FindGameObjectsWithTag("EnableDisableText");
			//Enable/disable text color
			foreach (GameObject goEnableDisableText in goEnableDisableTexts) {
				tk2dTextMesh tmEnableDisableText = goEnableDisableText.GetComponent<tk2dTextMesh>();
				if(tmEnableDisableText != null) {
					//We have a textmesh with tag "EnableDisableText"
					if(bEnable == true) {
						tmEnableDisableText.color = cBlue;
					} else {
						tmEnableDisableText.color = cRed;
					}
					tmEnableDisableText.Commit ();
				}
			}
			//Enable/disable the start/stop button
			tk2dBaseSprite baseSprite = spriteStartStop.GetComponent<tk2dBaseSprite>();
			if(baseSprite) {
				if(bEnable) {
					spriteStartStop.spriteId = baseSprite.GetSpriteIdByName("Start_Normal");
					btnStartStop.enabled = true;
				} else {
					spriteStartStop.spriteId = baseSprite.GetSpriteIdByName("Start_Disabel");
					btnStartStop.enabled = false;
				}	
			}
		}
		//Updsate variable
		bUIEnabled = bEnable;
	}
	
	void UpdatePos() {
		LocationInfo liTmp = Input.location.lastData;
		if(bRunning == true) {
			double dSpeed = ((CalculateDistanceBetweenGPSCoordinates (dLastLon, dLastLat, (double)liTmp.longitude , (double)liTmp.latitude )
				/ Math.Abs(liTmp.timestamp - dLastTimestamp )) * dMS2KN);
			tmCurSpeed.text = Math.Round (dSpeed,1).ToString ("#0.0") + " kn";
		}
		
		//Save current pos as last pos
		dLastLat = (double)liTmp.latitude;
		dLastLon = (double)liTmp.longitude;
		dLastTimestamp = liTmp.timestamp;
		
		tmGPS.text = liTmp.horizontalAccuracy + " m";
		//Commit all text
		tmBigInfoCaption.Commit ();
		GameObject[] goEnableDisableTexts = GameObject.FindGameObjectsWithTag("EnableDisableText");
		foreach (GameObject goEnableDisableText in goEnableDisableTexts) {
			tk2dTextMesh tmEnableDisableText = goEnableDisableText.GetComponent<tk2dTextMesh>();
			if(tmEnableDisableText != null) {
				//We have a textmesh with tag "EnableDisableText"
				tmEnableDisableText.Commit ();
			}
		}	
	}
	
	// Update is called every frame, if the
	// MonoBehaviour is enabled.
	void Update () {
		//Make new state
		LocationInfo liTmp;
		if(Input.location.status == LocationServiceStatus.Running ) {
			//Okey, gps is running
			liTmp = Input.location.lastData;		
			//Okey, we got a new position from the gps
			if(liTmp.horizontalAccuracy < 50.0f) {
				//We do have a okey accuracy
				gpsState = GpsStates.RunningAccuracyOk;
			} else {
				//Too bad accuracy
				gpsState = GpsStates.RunningAccuracyBad;
			}
	
		} else if(Input.location.status == LocationServiceStatus.Failed ) {
			gpsState = GpsStates.Faild;
		} else if(Input.location.status == LocationServiceStatus.Initializing  ) {
			gpsState = GpsStates.Initializing;
		} else if(Input.location.status == LocationServiceStatus.Stopped   ) {
			gpsState = GpsStates.Stoped;
		} 
		
		//GPS FSM
		switch (gpsState) {
		case GpsStates.Begin:
			if(gpsState != oldGpsState ) {
				//Enter state
			}
		    break;
		case GpsStates.Faild:
			if(gpsState != oldGpsState ) {
				//Enter state
				tmStatus.text = "Faild to start GPS!";
				tmStatus.Commit ();
			}
			
		    break;
		case GpsStates.Initializing:
			if(gpsState != oldGpsState ) {
				//Enter state
				tmStatus.text = "GPS initilizing";
				tmStatus.Commit ();
			}
			
		    break;
		case GpsStates.RunningAccuracyBad:
			if(gpsState != oldGpsState ) {
				//Enter state
				tmStatus.text = "GPS accuracy too bad";
				tmStatus.Commit ();
			}
			
		    break;
		case GpsStates.RunningAccuracyOk:
			if(gpsState != oldGpsState ) {
				//Enter state
				tmStatus.text = "";
				tmStatus.Commit ();
			}
			if(Input.location.lastData.timestamp != dLastTimestamp ) {
				UpdatePos();
			}
		    break;
		case GpsStates.Stoped:
			if(gpsState != oldGpsState ) {
				//Enter state
				tmStatus.text = "GPS Stoped";
				tmStatus.Commit ();
			}
		    break;
		}
		//Save old state
		oldGpsState = gpsState;	
	}
	
	
	double dNowInEpoch() {
		TimeSpan span = DateTime.UtcNow.Subtract (new DateTime(1970,1,1,0,0,0));
		return (double)span.TotalSeconds;
	}
	
	void StartStopClicked(){
		
		if(bRunning == true) {
			//Okey, we have stoped, show play 
			btnStartStop.buttonDownSprite = "Start_Highlight";
			btnStartStop.buttonUpSprite = "Start_Normal";
			btnStartStop.buttonPressedSprite = "Start_Normal";
			bRunning = false;
		} else {
			btnStartStop.buttonDownSprite = "Stop_Highlight";
			btnStartStop.buttonUpSprite = "Stop_Normal";
			btnStartStop.buttonPressedSprite = "Stop_Normal";
			bRunning = true;
		}
		btnStartStop.UpdateSpriteIds ();
	}
	
	void SettingsClicked(){
		
	}
	
	public static double CalculateDistanceBetweenGPSCoordinates(double lon1, double lat1, double lon2, double lat2) {
		//Returns i meters
	    const double R = 6378137; 
	    const double degreesToRadians = Math.PI / 180; 
	
	    //convert from fractional degrees (GPS) to radians 
	    lon1 *= degreesToRadians; 
	    lat1 *= degreesToRadians; 
	    lon2 *= degreesToRadians; 
	    lat2 *= degreesToRadians; 
	
	    double dlon = lon2 - lon1; 
	    double dlat = lat2 - lat1; 
	    double a = Math.Pow(Math.Sin(dlat / 2), 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(dlon / 2), 2);
	    double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a)); 
	    double d = R * c; 
	
	    return d; 
	}
	
}

