using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SplashTimer : MonoBehaviour {


	public float wait_time = 2f;
	// Use this for initialization
	void Start () {
		StartCoroutine(SwitchScene());
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private IEnumerator SwitchScene(){
	
		Debug.Log ("SPLASH");
		yield return new WaitForSeconds (wait_time);
		SceneManager.LoadScene ("Loading");
	}


}
