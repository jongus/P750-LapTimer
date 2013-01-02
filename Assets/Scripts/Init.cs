using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Init : MonoBehaviour {
	
	// Start is called just before any of the
	// Update methods is called the first time.
	void Start () {
		if(SystemInfo.deviceType == DeviceType.Desktop) {
			//Debug version on pc or mac	
		} else if(SystemInfo.deviceType == DeviceType.Handheld) {
			//Okey, on a hand held, what now?
			if(iPhone.generation == iPhoneGeneration.iPhone5 ) {
				//Okey, we got an iPhone5, ride on!
				
			} else {
				//We are running on something strange... Die garcefully!
				
			}
		}
	}
	
	// Update is called every frame, if the
	// MonoBehaviour is enabled.
	void Update () {
		
	}
	
}
