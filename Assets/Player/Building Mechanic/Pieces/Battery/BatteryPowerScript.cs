using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Handles battery and electricity for battery stuff
public class BatteryPowerScript : MonoBehaviour
{
    public float maxpower;//Maximum power the battery can hold
    public float currentpower;//The current power the battery is holding
    public Text batteryLevelText;//The text that tells us how much battery power is left
    // Start is called before the first frame update
    void Start()
    {
        currentpower = maxpower;//Init current power
    }

    //Called when a piece or component wants to get power from the battery, will return how much power it got
    public float GetPower(float _neededpower) 
    {
        if (currentpower - _neededpower * Time.deltaTime > 0) //Check if we have enough power to use
        {
            currentpower -= _neededpower * Time.deltaTime;//If true then use that power
            batteryLevelText.text = GetRemainingPower().ToString("G1") + "%";
            return _neededpower;//The piece that has called this function got 100% of its demanded power

        }
        else 
        {            
            float currentpower1 = currentpower;
            currentpower = 0;            
            return currentpower1;//The piece that has called this function did not get 100% of its demanded power, but it drained the battery
        }
        
    }
    //Get percentage of power
    public float GetRemainingPower() 
    {
        return currentpower / maxpower * 100;//Percentage only
    }
    //Add power to battery
    public void AddPower(float _addedpower) 
    {
        currentpower += _addedpower * Time.deltaTime;//Add power
        currentpower = Mathf.Clamp(currentpower, 0, maxpower);
        batteryLevelText.text = GetRemainingPower() + "%";
    }
}
