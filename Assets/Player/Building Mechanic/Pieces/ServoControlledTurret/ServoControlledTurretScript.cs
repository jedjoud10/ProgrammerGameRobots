using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Controlls the ServoControlledTurret motion and rotation;
public class ServoControlledTurretScript : MonoBehaviour
{
    public Vector3 TargetPos;//Target pos the turret will try to look at
    public HingeJoint pitchJoint;//The pitch rigidbody to make the turret go up or down
    public HingeJoint yawJoint;//The yaw rigidbody to make the turret go left or right
    public Transform rotationController;//The object controlled to take the rotation from
    public float damper;//How much to dampen the joint spring
    public float spring;//How much force to apply to the joint spring
    public Transform targetPointMesh;//The mesh that represents the target point
    //Materials
    public Material pitchLEDMat;
    public Material yawLEDMat;
    //Set LED colors for emmisive multiplication
    public Color pitchLEDColor;
    public Color yawLEDColor;
    //How much emission LEDs should make
    public float emmisive;
    //Threshold value from last frame value to activate specific LED
    public float LEDThreshold;
    //Last frame values for LEDs
    private float lastPitch;
    private float lastYaw;
    //Should LED be on ?
    private bool LEDPitch;
    private bool LEDYaw;
    //Last LED state to optimize
    private bool LastLEDPitch;
    private bool LastLEDYaw;
    //Randomizing for noise effect like in real sensors and electronics
    public float noiseEffectAmplitude;
    

    //Set target pos from interpreter
    public void SetTargetPos(Vector3 _pos) 
    {
        TargetPos = _pos;//Set variable
        RotateTurret();//Rotate the turret
        targetPointMesh.position = _pos;//Also set position of mesh
        targetPointMesh.rotation = Quaternion.identity;
    }

    private void Update()
    {
        RotateTurret();//Only for testing
    }

    //Make the pitchController and yawController move so they look at the target pos
    private void RotateTurret() 
    {
        rotationController.LookAt(TargetPos);//Rotate rotation controller
        //Make instances because we cannot change the variables of the spring struct that is already on the spring joint
        JointSpring pitchSpring = new JointSpring();
        JointSpring yawSpring = new JointSpring();
        //Apply spring properities 
        pitchSpring.spring = spring;
        yawSpring.spring = spring;
        pitchSpring.damper = damper;
        yawSpring.damper = damper;
        //Actual rotation, pitch axis, yaw axis
        //Debug.Log(rotationController.localEulerAngles);
        float angle1 = rotationController.localEulerAngles.x + (Random.value * 2 - 1) * noiseEffectAmplitude;
        float angle2 = rotationController.localEulerAngles.y + (Random.value * 2 - 1) * noiseEffectAmplitude;
        angle1 = (angle1 > 180) ? angle1 - 360 : angle1;
        angle2 = (angle2 > 180) ? angle2 - 360 : angle2;
        pitchSpring.targetPosition = -angle1;
        Debug.Log(angle1);
        yawSpring.targetPosition = -angle2 + 90;
        //Apply new rotation
        pitchJoint.spring = pitchSpring;
        yawJoint.spring = yawSpring;
        //Set and update bool based on threshold
        LEDPitch = Mathf.Abs(lastPitch - angle1) > LEDThreshold;
        LEDYaw = Mathf.Abs(lastYaw - angle2) > LEDThreshold;

        float ledPitchPower = 0;
        float ledYawPower = 0;
        //Turn bool into float
        if (LEDPitch) ledPitchPower = 1;
        if (LEDYaw) ledYawPower = 1;
        //Set LED emissive
        if(LEDPitch != LastLEDPitch) pitchLEDMat.SetColor("_EmissionColor", ledPitchPower * pitchLEDColor * emmisive);
        if(LEDYaw != LastLEDYaw) yawLEDMat.SetColor("_EmissionColor", ledYawPower * yawLEDColor * emmisive);
        //Set values for next frame
        lastPitch = angle1;
        lastYaw = angle2;
        //For optimization
        LastLEDPitch = LEDPitch;
        LastLEDYaw = LEDYaw;
    }
    //Shoot potato out of turret

    public void ShootProjectile() 
    {
        //TO-DO
    }
}
