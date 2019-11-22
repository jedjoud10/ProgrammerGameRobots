using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//Handles scene management and scenery stuff
public class SceneManagerScript : MonoBehaviour
{
    private string scenenameVar;//Variable holding scene name
    public void Start()
    {
        Time.timeScale = 0.0f;//Set base time
        DontDestroyOnLoad(gameObject);
    }
    //Called from other scripts when we want to change scenes
    public void ChangeScene(string scenename) 
    {
        SceneManager.LoadScene(scenename, LoadSceneMode.Single);
        scenenameVar = scenename;        
    }
    //Checks our current scene and makes some logic
    public void UpdateScene(string _scenename) 
    {
        if (_scenename == "BuildingScene")
        {
            Time.timeScale = 0.0f;
            GameObject.FindObjectOfType<SaverLoaderHandlerScript>().LoadBuildingPieces("TemporarySave", false);
        }
        if (_scenename == "PreviewScene")
        {
            Time.timeScale = 1.0f;
            GameObject.FindObjectOfType<SaverLoaderHandlerScript>().LoadBuildingPieces("TemporarySave", true);
            Debug.Log("Moved into preview, will load pieces");
        }
    }
    //Called when the gameobject starts
    private void OnEnable()
    {
        SceneManager.sceneLoaded += SceneIsLoaded;
    }
    //Called when the gameobject disables
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneIsLoaded;
    }
    //Delegate for when the scene finished loading
    void SceneIsLoaded(Scene scene, LoadSceneMode mode) 
    {
        Debug.Log("Finished loading the scene " + scene.name);
        UpdateScene(scenenameVar);
    }
}
