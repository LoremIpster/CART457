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
	private float timeStill;
	private bool songPlaying = false;

	public float timeRemaining = 60;
	public Text timerText;
	public GameObject music;
	//public SphereCollider startTrigger;


	void Start () {
		xPos = transform.position.x;
		zPos = transform.position.z;

		controller = GetComponent<CharacterController> ();
		fpController = GetComponent<FirstPersonController> ();
		//startTrigger = GetComponent<SphereCollider> ();

		timerText.text = "Il ne te reste que " + timeRemaining.ToString("N0") + " secondes.";
	}
	
	void FixedUpdate () {
		if (xPos != transform.position.x || zPos != transform.position.z) {
			timeStill = 0;

			timeRemaining -= Time.deltaTime;
			timerText.text = "Il ne te reste que " + timeRemaining.ToString ("N0") + " secondes.";

			xPos = transform.position.x;
			zPos = transform.position.z;

			if (timeRemaining <= 0) {
				timerText.enabled = false;
				controller.enabled = false;
				fpController.enabled = false;
			}
		} else {
			

			timeStill += Time.deltaTime;
			Debug.Log (timeStill);

			if (timeStill >= 15) {
				playSong ();
			}

		}
	}

//	void OnCollisionEnter (Collision col){
//		if(col.gameObject.name == startTrigger)
//		{
//			Debug.Log ("INSIIIIIDE");
//		}
//
//	}

	void playSong(){
		
		if (songPlaying == false) {
			music.GetComponent<AudioSource> ().Play ();
			songPlaying = true;
		} else {
			//music.GetComponent<AudioSource>().Stop();
			//songPlaying = false;
		}
	}



}
