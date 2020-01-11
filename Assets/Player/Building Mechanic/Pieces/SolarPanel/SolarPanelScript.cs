using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarPanelScript : MonoBehaviour
{
    private Transform sun;//Sun of world
    public Transform solarpanel_transform;//The upper part of solar panel
    public float powerGenerationRate;
    // Start is called before the first frame update
    void Start()
    {
        sun = GameObject.Find("Sun").transform;
    }

    // Update is called once per frame
    void Update()
    {

    }
    //Gets how much power this solar panel is generating
    public float GetPowerGenerationRate() 
    {
        //Linecast to know if we are in shadow or not
        if (Physics.Linecast(solarpanel_transform.position, sun.position)) //Returns true if we in shadow
        {
            return 0;
        }
        else
        {
            return powerGenerationRate;
        }
    }
}
