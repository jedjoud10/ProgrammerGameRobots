using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Script that gets info from the outside world using IMUs and sends it to the code interpreter
public class IMUSensorScript : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 lastVelocity;
    private float gravityIntensity;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gravityIntensity = Physics.gravity.magnitude;
    }
    private void Update()
    {
        lastVelocity = rb.velocity;//Get last velocity so we can get delta of velocity, which is acceleration
    }

    //Read acceleration sensor data
    public Vector3 ReadAccelerationSensorData() 
    {
        return lastVelocity - rb.velocity;//Get acceleration
    }
    //Read rotation velocity sensor data
    public Vector3 ReadAngularVelocitySensorData() 
    {
        return rb.angularVelocity;
    }
    //Read gravity sensor data
    public Vector3 ReadGravitySensorData() 
    {
        return rb.transform.up * gravityIntensity;
    }
}
