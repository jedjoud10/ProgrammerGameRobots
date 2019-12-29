using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Handles battery and electricity for battery stuff
public class BatteryPowerScript : MonoBehaviour
{
    public float maxpower;//Maximum power the battery can hold
    public float currentpower;//The current power the battery is holding
    // Start is called before the first frame update
    void Start()
    {
        currentpower = maxpower;//Init current power
    }

    //Called when a piece or component wants to get power from the battery, will return how much power it got
    public float UsePower(float _neededpower) 
    {
        if (currentpower - _neededpower > 0) //Check if we have enough power to use
        {
            currentpower -= _neededpower;//If true then use that power
            return _neededpower;//The piece that has called this function got 100% of its demanded power
        }
        else 
        {
            return currentpower;//The piece that has called this function did not get 100% of its demanded power, but it drained the battery
        }
    }
    //Get percentage of power
    public float GetRemainingPower() 
    {
        return currentpower / maxpower * 100;//Percentage only
    }    
}
