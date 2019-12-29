using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Used to handle UI during gameplay
public class PlaymodeUIScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //Called from UI return to building mode button
    public void ReturnToBuildingMode() 
    {
        GameObject.FindObjectOfType<SceneManagerScript>().ChangeScene("BuildingScene");
    }
}
