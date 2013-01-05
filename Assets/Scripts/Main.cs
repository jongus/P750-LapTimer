using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Main : MonoBehaviour {
	string sStatus;
	
	// Start is called just before any of the
	// Update methods is called the first time.
	void Start () {
		// First, check if user has location service enabled
    	if (Input.location.isEnabledByUser == false ) {
			sStatus = "Input.location.isEnabledByUser == false";
			return;
		}
        
    	// Start service before querying location
    	Input.location.Start (1.0f, 1.0f);
	}
	
	// Update is called every frame, if the
	// MonoBehaviour is enabled.
	void Update () {
			
	}
	
}
