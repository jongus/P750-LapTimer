using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class scriptMain : MonoBehaviour {
	private string sStatus = "-";
	tk2dTextMesh tmStatus;
	tk2dTextMesh tmLatLon;
	 
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
		//tmLatLon.text = Input.location.lastData.latitude.ToString ();
		if(Input.location.status == LocationServiceStatus.Running){
			tmLatLon.text = Input.location.lastData.latitude.ToString () + "\n" + 
				Input.location.lastData.longitude.ToString () + "\n" +
					Input.location.lastData.horizontalAccuracy.ToString() + "\n" +
					Input.location.lastData.timestamp.ToString();	
		}
		tmLatLon.Commit ();
		
		sStatus = Input.location.status.ToString ();
		tmStatus.text = sStatus;
		tmStatus.Commit ();
	}
	
	void OnBecameInvisible() {
        Input.location.Stop ();
    }
	
}
