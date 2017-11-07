using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class Timer : MonoBehaviour {

	private CharacterController controller;
	private FirstPersonController fpController;

	private float xPos;
	private float zPos;

	public float timeRemaining = 60;
	public Text gameOverText;
	public Text timerText;



	// Use this for initialization
	void Start () {
		xPos = transform.position.x;
		zPos = transform.position.z;

		controller = GetComponent<CharacterController> ();
		fpController = GetComponent<FirstPersonController> ();
		timerText.text = "Il ne te reste que " + timeRemaining.ToString("N0") + " secondes.";
	}
	
	// Update is called once per frame
	void Update () {
		if (xPos != transform.position.x || zPos != transform.position.z) {
			timeRemaining -= Time.deltaTime;
			timerText.text = "Il ne te reste que " + timeRemaining.ToString("N0") + " secondes à vivre.";

			xPos = transform.position.x;
			zPos = transform.position.z;

			if(timeRemaining <= 0){
				gameOverText.text = "c'est fini";
				timerText.enabled = false;
				controller.enabled = false;
				fpController.enabled = false;
			}
		}
	}

	// for debug only
	void OnGUI(){
	
		if (timeRemaining > 0) {
			//GUI.Label (new Rect (100, 100, 200, 100), "Time Remaining : " + timeRemaining);
		//	timerText.text = "Il ne te reste que " + timeRemaining + "secondes à vivre.";
		} else {
			//GUI.Label (new Rect (100, 100, 100, 100), "Time's up!");

		}

	}
}
