using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class scriptMain : MonoBehaviour {
	private string sStatus = "-";
	private LocationInfo liOld;
	private LocationInfo liNew;
	private tk2dTextMesh tmStatus;
	private tk2dTextMesh tmLatLon;
	
	 
	// Start is called just before any of the
	// Update methods is called the first time.
	void Start () {
		// First, check if user has location service enabled
    	tmStatus = GameObject.Find ("tmStatus").GetComponent<tk2dTextMesh>();
		tmLatLon = GameObject.Find ("tmLatLon").GetComponent<tk2dTextMesh>();
		Input.location.Start(1.0f, 1.0f);
	}
	
	// Update is called every frame, if the
	// MonoBehaviour is enabled.
	void Update () {
		//Check for gps operation
		
		//tmLatLon.text = Input.location.lastData.latitude.ToString ();
		if(GpsStatusOkey() == 2){
			tmLatLon.text = liNew.latitude.ToString () + "\n" + 
				liNew.longitude.ToString () + "\n" +
					liNew.horizontalAccuracy.ToString() + "\n" +
					liNew.timestamp.ToString()  + "\n" +
					NowInEpoch ();
		} else if(GpsStatusOkey() == 1) {
			tmLatLon.text = "ORANGE" + "\nAccuracy:" + liNew.horizontalAccuracy.ToString() + "\nDiff:" + 
				(NowInEpoch() - liNew.timestamp).ToString();
		} else {
			tmLatLon.text = "RED";
		}
		tmLatLon.Commit ();
		
		sStatus = Input.location.status.ToString () + " : " + NowInEpoch ();
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
