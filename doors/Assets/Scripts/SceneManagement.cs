using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour {

	public string sceneName;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.Escape)) {
			Debug.Log ("TRAN ---- SITION");
			transition ();	
		}
			
		
	}

	void transition(){
	
		SceneManager.LoadScene (sceneName, LoadSceneMode.Single);
	
	}

}

