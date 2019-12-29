using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationMotorJointScript : MonoBehaviour
{
    public Transform stator;//Part of motor that does not move
    private HingeJoint myHJ;
    private JointMotor myJM;
    public float MaxSpeed;//Maximum speed
    public float Force;//Force
    // Start is called before the first frame update
    void Start()
    {
        myHJ = GetComponent<HingeJoint>();//Get component component from our object
        myJM.targetVelocity = 0;
        myJM.force = Force;
        myHJ.motor = myJM;        
        myHJ.useMotor = true;
    }

    public void SetMotorSpeed(float speed) //Called from interpreter to set the motor speed
    {        
        myJM.targetVelocity = Mathf.Clamp(speed, -MaxSpeed, MaxSpeed);
        myHJ.motor = myJM;
    }
}
