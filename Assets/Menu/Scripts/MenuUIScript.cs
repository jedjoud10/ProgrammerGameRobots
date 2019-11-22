using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUIScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //Called from ui button to start game and go building
    public void EnterBuildingScene() 
    {
        GameObject.FindObjectOfType<SceneManagerScript>().ChangeScene("BuildingScene");
    }
}
