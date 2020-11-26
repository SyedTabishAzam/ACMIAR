using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SwitchScene : MonoBehaviour {

    public GameObject status;
    // Use this for initialization
    public void loadSceneOnClick(string SceneName)
    {
        SceneManager.LoadScene(SceneName, LoadSceneMode.Additive);
    }
    public void UnloadScene(string SceneName)
    {
        SceneManager.UnloadSceneAsync(SceneName);
    }
	public void switchScene(string scenename)
	{


		SceneManager.LoadScene (scenename);



	}
    public void LoadScene()
    {

        if(PlayerPrefs.GetString("Current_MissionID")!= "")
        {
            SceneManager.LoadScene("ARScene");
        }
        else
        {
            DisplaySuccess(false);
        }


    }

    void DisplaySuccess(bool isSuccessful, System.Exception exception = null)
    {

        status.SetActive(true);
        Invoke("HideStatus", 2.0f);
    }

    void HideStatus()
    {
        status.SetActive(false);
    }

    public void Quit()
	{

		Application.Quit ();


	}


}
