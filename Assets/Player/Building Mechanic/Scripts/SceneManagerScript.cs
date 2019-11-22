using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//Handles scene management and scenery stuff
public class SceneManagerScript : MonoBehaviour
{
    public void Start()
    {
        Time.timeScale = 0.0f;//Set base time
        DontDestroyOnLoad(gameObject);
    }
    //Called from other scripts when we want to change scenes
    public void ChangeScene(string scenename) 
    {
        SceneManager.LoadScene(scenename, LoadSceneMode.Single);
        StartCoroutine(UpdateScene(scenename));
    }
    //Checks our current scene and makes some logic
    public IEnumerator UpdateScene(string _scenename) 
    {
        yield return new WaitForSecondsRealtime(1.0f);
        if (_scenename == "BuildingScene")
        {
            Time.timeScale = 0.0f;
            GameObject.FindObjectOfType<SaverLoaderHandlerScript>().LoadBuildingPieces("TemporarySave");
        }
        if (_scenename == "PreviewScene")
        {
            Time.timeScale = 1.0f;
            GameObject.FindObjectOfType<SaverLoaderHandlerScript>().LoadBuildingPieces("TemporarySave");
            Debug.Log("Moved into preview, will load pieces");
        }
    }
}
