using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class scriptMain : MonoBehaviour {
	private string sStatus = "-";
	private LocationInfo liOld;
	private LocationInfo liNew;
	
	private Color cRed = new Color(255.0f, 124.0f, 108.0f, 255.0f);
	private Color cBlue = new Color(62.0f, 190.0f, 255.0f, 255.0f);
	private Color cGrey = new Color(162.0f, 178.0f, 191.0f, 255.0f);
	private Color cWhite = new Color(255.0f, 255.0f, 255.0f, 255.0f);
	
	private tk2dTextMesh tmStatus; //DEBUG
	private tk2dTextMesh tmMaxSpeed;
	private tk2dTextMesh tmAvgSpeed;
	private tk2dTextMesh tmCurSpeed;
	private tk2dTextMesh tmLapNumber;
	private tk2dTextMesh tmLapTime;
	private tk2dTextMesh tmGPS;
	private tk2dTextMesh tmBigInfo;
	private tk2dTextMesh tmBigInfoCaption;
	
	
	 
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
		
		
		Input.location.Start(1.0f, 1.0f);
		
		DisableUI ();
	}
	
	void DisableUI() {
		tk2dTextMesh[] tms = (tk2dTextMesh)GameObject.FindGameObjectsWithTag("EnableDisableText");
		
		foreach (GameObject tm in tms) {
			
			
				
				tm.color = cRed;
				tm.Commit();
			
		}
	}
	
	// Update is called every frame, if the
	// MonoBehaviour is enabled.
	void Update () {
		//Check for gps operation
		
		//tmLatLon.text = Input.location.lastData.latitude.ToString ();
		if(GpsStatusOkey() == 2){
			/*tmLatLon.text = liNew.latitude.ToString () + "\n" + 
				liNew.longitude.ToString () + "\n" +
					liNew.horizontalAccuracy.ToString() + "\n" +
					liNew.timestamp.ToString()  + "\n" +
					NowInEpoch (); */
		} else if(GpsStatusOkey() == 1) {
			/*tmLatLon.text = "ORANGE" + "\nAccuracy:" + liNew.horizontalAccuracy.ToString() + "\nDiff:" + 
				(NowInEpoch() - liNew.timestamp).ToString();*/
		} else {
			//tmLatLon.text = "RED";
		}
		//tmLatLon.Commit ();
		
		sStatus = Input.location.status.ToString ();
		tmStatus.text = sStatus;
		tmStatus.Commit ();
	}
	
	int GpsStatusOkey() {
		//0 = false 1 = orange, some kind but not good, 2 = working good!'
		if(Input.location.status == LocationServiceStatus.Running ) {
			liNew = Input.location.lastData;
			//Check last update time
			if((NowInEpoch() - liNew.timestamp) > 5.0d ) {
				//okey, more than 5 sec = Orange
				return 1;
			}
			
			//Check accuracy
			if(liNew.horizontalAccuracy > 30.0f){
				//Too bad = Orange
				return 1;
			}
			//Okey, every thing is okey, I think
			return 2;
			
		} else {
			return 0;
		}
	}
	
	double NowInEpoch() {
		TimeSpan span = DateTime.UtcNow.Subtract (new DateTime(1970,1,1,0,0,0));
		return (double)span.TotalSeconds;
	}
	
}
