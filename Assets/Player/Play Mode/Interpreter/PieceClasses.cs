using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Holds interpreter pieces classes



/// Multiple types of piece :
/// Power pieces (Gives electricity to other pieces)
/// Sensor pieces (Get information about the craft and the world)
/// Damage pieces (Applies damage to enemy pieces)
/// Motion pieces (Make pieces that are connected to it have motion (rotation, position))
/// Structural pieces (Just structural and aesthetics pieces)

///Pieces will require minimum power and optimal power from power pieces
///When the given power is less than the optimal power, the piece will work just fine (For now, still needs to make it more detailed so the piece become more weird the less power there is)
///If the given power is below miminum power, the piece will just not work, but it will still drain the power unless you turn it off
///The piece will always try to get its power to optimal power

//Type of piece like list above 
public enum PIECE_TYPE 
{
    POWER_PIECE,
    SENSOR_PIECE,
    DAMAGE_PIECE,
    MOTION_PIECE,
    STRUCTURAL_PIECE
}



// Piece class template
/*
    public class PieceClassName : InteractablePiece//Class for handling PieceName
    {
        public PieceScriptName pieceScriptName;
        public PieceClassName()//When instance is created
        {
            SetupProperities(10.0f, 50.0f, PIECE_TYPE.DAMAGE_PIECE);//Setup properities
        }
        public void SetupScript(PieceScriptName _pieceScript)
        {
            PieceScriptName = _pieceScript;//Constructor
        }
        ...Unique functions/methods here
    } 
*/

public class InteractablePiece //A piece class of the robot that is the parent of multiple inherited classes
{
    public string myName;
    public Joint myJoint;
    public Rigidbody myRigidbody;
    public PIECE_TYPE piecetype = PIECE_TYPE.STRUCTURAL_PIECE;//Init normal piece
    public float powermin = 0;//Minimum power so the piece can operate
    public float poweroptimal = 0;//Optimal power for piece
    public void SetupPiece(Joint _myJoint, Rigidbody _myRigidbody, string _myName)
    {
        myJoint = _myJoint;//Constructor
        myRigidbody = _myRigidbody;//Constructor
        myName = _myName;//Constructor
    }
    //Setup properities like power_min, power_optimal, piece_type, etc...
    public void SetupProperities(float _powermin, float _poweroptimal, PIECE_TYPE _piecetype) 
    {
        powermin = _powermin;
        poweroptimal = _poweroptimal;
        piecetype = _piecetype;
    }
}
public class MotorJoint : InteractablePiece//Class for handling motor joint
{
    public RotationMotorJointScript rotationMotorJointScript;
    public MotorJoint()//When instance is created
    {
        SetupProperities(10.0f, 50.0f, PIECE_TYPE.DAMAGE_PIECE);//Setup properities
    }
    public void SetupScript(RotationMotorJointScript _rotationMotorJointScript)
    {
        rotationMotorJointScript = _rotationMotorJointScript;//Constructor
    }
    public void SetMotorSpeed(float _speed) //Set my rotationmotorjointscripts's motor's speed
    {
        rotationMotorJointScript.SetMotorSpeed(_speed);
    }
}
public class DistanceSensor : InteractablePiece//Class for the "distance sensor" sensor
{
    public DistanceSensorScript distanceSensorScript;
    public DistanceSensor()//When instance is created
    {
        SetupProperities(4.0f, 10.0f, PIECE_TYPE.SENSOR_PIECE);//Setup properities
    }
    public void SetupScript(DistanceSensorScript _distanceSensorScript)
    {
        distanceSensorScript = _distanceSensorScript;//Constructor
    }
    public float GetSensorDistance() //Read distance from sensor
    {
        return distanceSensorScript.GetDistance();
    }
}
public class IMUSensor : InteractablePiece//Class for the "Inertial Mesurement Sensor" sensor
{
    public IMUSensorScript IMUSensorScript;
    public IMUSensor()//When instance is created
    {
        SetupProperities(15.0f, 40.0f, PIECE_TYPE.SENSOR_PIECE);//Setup properities
    }
    public void SetupScript(IMUSensorScript _IMUSensorScript)
    {
        IMUSensorScript = _IMUSensorScript;//Constructor
    }
    public Vector3 GetAcceleration()//Read acceleration from IMU sensor
    {
        return IMUSensorScript.ReadAccelerationSensorData();
    }
    public Vector3 GetAngularVelocity()//Read angular velocity from IMU sensor
    {
        return IMUSensorScript.ReadAngularVelocitySensorData();
    }
    public Vector3 GetGravity()//Read gravity from IMU sensor
    {
        return IMUSensorScript.ReadGravitySensorData();
    }
}
public class ServoControlledTurret : InteractablePiece//Class from the "Servo controlled turret" damage piece
{
    public ServoControlledTurretScript ServoControlledTurretScript;
    public ServoControlledTurret()//When instance is created
    {
        SetupProperities(30.0f, 60.0f, PIECE_TYPE.DAMAGE_PIECE);//Setup properities
    }
    public void SetupScript(ServoControlledTurretScript _ServoControlledTurretScript)
    {
        ServoControlledTurretScript = _ServoControlledTurretScript;//Constructor
    }
    public void SetTargetPosition(float x, float y, float z) //Set target pos (x, y, z) of turret with servo rotation
    {
        ServoControlledTurretScript.SetTargetPos(new Vector3(x, y, z));//Set new position and rotate
    }
}
public class BatteryPowerPiece : InteractablePiece//Class from the "Battery" piece
{
    public BatteryPowerScript BatteryScript;
    public BatteryPowerPiece()//When instance is created
    {
        ///Setting minmum and optimal powers to 0 since this is already a power piece
        SetupProperities(0.0f, 0.0f, PIECE_TYPE.POWER_PIECE);//Setup properities
    }
    public void SetupScript(BatteryPowerScript _BatteryScript) 
    {
        BatteryScript = _BatteryScript;//Constructor
    }
    //Get remaining power in POWERUNITS
    public float GetRemainingPower_pu() 
    {
        return BatteryScript.currentpower;
    }
    //Get remaining power in PERCENTAGE
    public float GetRemainingPower_pe()
    {
        return BatteryScript.GetRemainingPower();
    }
}