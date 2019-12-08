using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Script that gets info from the outside world using IMUs and sends it to the code interpreter
public class IMUSensorScript : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 lastVelocity;
    public Vector3 acceleration;
    public Vector3 angularVelocity;
    public Vector3 gravity;
    private float gravityIntensity;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gravityIntensity = Physics.gravity.magnitude;
    }

    // Update is called once per frame
    void Update()
    {
        ReadSensorData();
    }
    //Read sensor data from rigidbody and save it to variables
    private void ReadSensorData() 
    {
        acceleration = lastVelocity - rb.velocity;//Get acceleration
        angularVelocity = rb.angularVelocity;
        gravity = rb.transform.up * gravityIntensity;
        lastVelocity = rb.velocity;//Get last velocity so we can get delta of velocity, which is acceleration
    }
}
