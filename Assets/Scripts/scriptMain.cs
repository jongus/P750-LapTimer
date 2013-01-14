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
	
	private bool bUIEnabled = true; 
	
	 
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
		}
		//Updsate variable
		bUIEnabled = bEnable;
	}
	
	// Update is called every frame, if the
	// MonoBehaviour is enabled.
	void Update () {
		
		if(Input.location.status == LocationServiceStatus.Running ) {
			//Okey, gps is running
			LocationInfo liTmp = Input.location.lastData;
			if(liTmp.timestamp != dLastTimestamp ) {
				//Okey, we got a new position from the gps
				if(liTmp.horizontalAccuracy < 50.0f) {
					//We do have a okey accuracy
					EnableUI (true);
					//Do the work HERE!
					tmGPS.text = liTmp.horizontalAccuracy + " m";
					tmGPS.Commit ();
					
				} else {
					//Too bad accuracy
					tmGPS.text = "-- m";
					EnableUI (false);
				}
			} else if((dNowInEpoch() - dLastTimestamp ) > 5.0d ) {
				//No gps position update in 5 sec!
				tmGPS.text = "-- m";
				EnableUI (false);
			} else {
				//NOP
			}
		} else {
			//Gps not running
			tmGPS.text = "-- m";
			EnableUI (false);
		}
		
		// Do other updates
		
		
	}
	
	
	double dNowInEpoch() {
		TimeSpan span = DateTime.UtcNow.Subtract (new DateTime(1970,1,1,0,0,0));
		return (double)span.TotalSeconds;
	}
	
}
