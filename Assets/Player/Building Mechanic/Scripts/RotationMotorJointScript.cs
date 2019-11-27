using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationMotorJointScript : MonoBehaviour
{
    private HingeJoint myHJ;
    private JointMotor myJM;
    public float MaxSpeed;//Maximum speed
    // Start is called before the first frame update
    void Start()
    {
        myHJ = GetComponent<HingeJoint>();
        myJM.targetVelocity = 0;
        myJM.force = 10f;
        myHJ.motor = myJM;
    }

    public void SetMotorSpeed(float speed) //Called from interpreter to set the motor speed
    {
        myHJ.useMotor = true;
        myJM.targetVelocity = Mathf.Clamp(speed, 0, MaxSpeed);
        myHJ.motor = myJM;
    }
}
