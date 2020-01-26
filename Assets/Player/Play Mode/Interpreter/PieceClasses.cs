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
    public Interpreter interpreter;//The piece's interpreter
    public void SetupPiece(Joint _myJoint, Rigidbody _myRigidbody, string _myName, Interpreter _interpreter)
    {
        myJoint = _myJoint;//Constructor
        myRigidbody = _myRigidbody;//Constructor
        myName = _myName;//Constructor
        interpreter = _interpreter;//Constructor
        CodeStart();//Call first frame method
    }
    //Setup properities like power_min, power_optimal, piece_type, etc...
    public void SetupProperities(float _powermin, float _poweroptimal, PIECE_TYPE _piecetype) 
    {
        powermin = _powermin;
        poweroptimal = _poweroptimal;
        piecetype = _piecetype;
    }
    //When code is executed every frame
    //Is virtual to be able to be overridden
    public virtual void CodeLoop() { }
    //When code is executed first frame
    //Is virtual to be able to be overridden
    public virtual void CodeStart() { }
}
public class MotorJoint : InteractablePiece//Class for handling motor joint
{
    public RotationMotorJointScript rotationMotorJointScript;
    private float maxspeed;//Max speed of motor
    private float force;//Force of motor
    private float estimatedpower;//The estimated power of this motor
    public MotorJoint()//When instance is created
    {
        SetupProperities(1.0f, 20.0f, PIECE_TYPE.DAMAGE_PIECE);//Setup properities
    }
    public void SetupScript(RotationMotorJointScript _rotationMotorJointScript)
    {
        rotationMotorJointScript = _rotationMotorJointScript;//Constructor
        maxspeed = rotationMotorJointScript.MaxSpeed;//Max speed of world rotation motor script
        force = rotationMotorJointScript.Force;//Force of world rotation motor script
    }
    public void SetMotorSpeed(float _speed) //Set my rotationmotorjointscripts's motor's speed
    {
        estimatedpower = Mathf.Lerp(powermin, poweroptimal, Mathf.Abs(_speed) / maxspeed);//Calculation for power estimation with speed and force
        Debug.Log(estimatedpower);
        float v = interpreter.UsePower(estimatedpower);//Use power every time we set the speed of motors
        if (v >= powermin) { rotationMotorJointScript.SetMotorSpeed(_speed); rotationMotorJointScript.EnableMotor(); }
        else { rotationMotorJointScript.SetMotorSpeed(0); rotationMotorJointScript.DisableMotor(); }
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
        if(interpreter.UsePower(poweroptimal) >= powermin) return distanceSensorScript.GetDistance();//Use power when getting sensor data and return disctance
        return 0;//Non sufficent power
    }
    public override void CodeLoop()
    {
        base.CodeLoop();
        interpreter.UsePower(powermin);//Use min power when not doing anything
    }
}
public class IMUSensor : InteractablePiece//Class for the "Inertial Mesurement Sensor" sensor
{
    public IMUSensorScript IMUSensorScript;
    public IMUSensor()//When instance is created
    {
        SetupProperities(2.0f, 3.0f, PIECE_TYPE.SENSOR_PIECE);//Setup properities
    }
    public void SetupScript(IMUSensorScript _IMUSensorScript)
    {
        IMUSensorScript = _IMUSensorScript;//Constructor
    }
    public Vector3 GetAcceleration()//Read acceleration from IMU sensor
    {
        if(interpreter.UsePower(poweroptimal) >= powermin) 
        {
            return IMUSensorScript.ReadAccelerationSensorData();
        }
        return new Vector3(Random.value, Random.value, Random.value);
    }
    public Vector3 GetAngularVelocity()//Read angular velocity from IMU sensor
    {
        if (interpreter.UsePower(poweroptimal) >= powermin)
        {
            return IMUSensorScript.ReadAngularVelocitySensorData();
        }
        return new Vector3(Random.value, Random.value, Random.value);
    }
    public Vector3 GetGravity()//Read gravity from IMU sensor
    {    
        if (interpreter.UsePower(poweroptimal) >= powermin)
        {
            return IMUSensorScript.ReadGravitySensorData();
        }
        return new Vector3(Random.value, Random.value, Random.value);
    }
}
public class ServoControlledTurret : InteractablePiece//Class from the "Servo controlled turret" damage piece
{
    public ServoControlledTurretScript ServoControlledTurretScript;
    public ServoControlledTurret()//When instance is created
    {
        SetupProperities(6.0f, 10.0f, PIECE_TYPE.DAMAGE_PIECE);//Setup properities
    }
    public void SetupScript(ServoControlledTurretScript _ServoControlledTurretScript)
    {
        ServoControlledTurretScript = _ServoControlledTurretScript;//Constructor
    }
    public void SetTargetPosition(float x, float y, float z) //Set target pos (x, y, z) of turret with servo rotation
    {
        if (interpreter.UsePower(poweroptimal) >= powermin)
        {
            ServoControlledTurretScript.SetTargetPos(new Vector3(x, y, z));//Set new position and rotate when have suficcent power
            ServoControlledTurretScript.isEnabled = true;
        }
        else
        {
            ServoControlledTurretScript.SetTargetPos(new Vector3(x, y, z));//Set new position and rotate
            ServoControlledTurretScript.isEnabled = false;//Disable turret when not enough power
        }
    }    

    public override void CodeLoop()
    {
        base.CodeLoop();
        interpreter.UsePower(powermin);
    }
}
public class BatteryPowerPiece : InteractablePiece//Class from the "Battery" piece
{
    public BatteryPowerScript BatteryScript;
    public BatteryPowerPiece()//When instance is created
    {
        //Setting minmum and optimal powers to 0 since this is already a power piece
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
    //Get max power of battery
    public float GetMaxPower() 
    {
        return BatteryScript.maxpower;
    }
    //Get power from battery and returns how much it got for optimal/minimum power estimations
    public float GetPower(float optimalpower) 
    {
        return BatteryScript.GetPower(optimalpower);
    }
    //Add power to baterry
    public void AddPower(float newpower) 
    {
        BatteryScript.AddPower(newpower);
    }    
}
public class PowerSensor : InteractablePiece//Class from the "Power Sensor" sensor piece
{
    public PowerSensorScript PowerSensorScript;
    private Dictionary<PIECE_TYPE, List<InteractablePiece>> pieces;
    private float craftEnergy;//The energy that the craft holds

    public PowerSensor()//When instance is created
    {
        SetupProperities(3.0f, 10.0f, PIECE_TYPE.SENSOR_PIECE);//Setup properities
    }
    public void SetupScript(PowerSensorScript _PowerSensorScript, Dictionary<PIECE_TYPE, List<InteractablePiece>> _pieces)
    {
        PowerSensorScript = _PowerSensorScript;//Constructor
        pieces = _pieces;//Constructor
    }

    
    //Gets current powerunits of pieces that hold energy of craft
    public float CraftPowerUnits() 
    {
        craftEnergy = 1;
        for (int i = 0; i < pieces[PIECE_TYPE.POWER_PIECE].Count; i++)
        {
            if (pieces[PIECE_TYPE.POWER_PIECE][i] is BatteryPowerPiece)//Check if this piece is battery
            {
                BatteryPowerPiece battery = (BatteryPowerPiece)pieces[PIECE_TYPE.POWER_PIECE][i];
                craftEnergy += battery.GetRemainingPower_pu();//Add battery power to val to make sum of all batteries                
            }
        }
        return craftEnergy;
    }
    //Get current percentage of piece that hold energy of craft
    public float CraftPowerPercentage() 
    {
        float craftMaxPower = 1;//All max power summed together to calculated percentage
        craftEnergy = 1;
        for (int i = 0; i < pieces[PIECE_TYPE.POWER_PIECE].Count; i++)
        {
            if (pieces[PIECE_TYPE.POWER_PIECE][i] is BatteryPowerPiece)//Check if this piece is battery
            {
                BatteryPowerPiece battery = (BatteryPowerPiece)pieces[PIECE_TYPE.POWER_PIECE][i];
                craftEnergy += battery.GetRemainingPower_pu();//Add battery power to val to make sum of all batteries    
                craftMaxPower += battery.GetMaxPower();//Add battery power to val to make sum of all batteries's max powers  
            }
        }
        return craftEnergy / craftMaxPower * 100;
    }
    //Get the curerntly generated power from generators (solar panels, etc)
    public float CraftPowerGeneration() 
    {
        float craftPowerGeneration = 0;//The currently generated power
        InteractablePiece currentPiece;//Current piece from loop
        for(int i = 0; i < pieces[PIECE_TYPE.POWER_PIECE].Count; i++) 
        {
            currentPiece = pieces[PIECE_TYPE.POWER_PIECE][i];
            if(currentPiece is SolarPanelPowerPiece) //Instance of solar panel piece
            {
                SolarPanelPowerPiece solarPanel = (SolarPanelPowerPiece)currentPiece;
                craftPowerGeneration += solarPanel.generatedPower;
            }
        }        
        return craftPowerGeneration;
    }

    public override void CodeLoop()
    {
        base.CodeLoop();
        interpreter.UsePower(powermin);
    }    
}
public class SolarPanelPowerPiece : InteractablePiece //Class for the "Solar Panel" powering piece
{
    public SolarPanelScript SolarPanelScript;
    public float generatedPower;//The currently generated power
    public SolarPanelPowerPiece()//When instance is created
    {
        //Not setting them both to zero because the servo motor used to rotate the solar panel require energy
        SetupProperities(0.5f, 1.0f, PIECE_TYPE.POWER_PIECE);//Setup properities
    }
    public void SetupScript(SolarPanelScript _SolarPanelScript)
    {
        SolarPanelScript = _SolarPanelScript;//Constructor
    }
    public override void CodeLoop()
    {
        base.CodeLoop();
        generatedPower = SolarPanelScript.GetPowerGenerationRate();
        interpreter.AddPower(generatedPower);
    }
}