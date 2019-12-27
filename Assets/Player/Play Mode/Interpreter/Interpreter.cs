using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
//Class for handling the code for the robot
public class Interpreter
{
    public Dictionary<string, float> float_variables;//The stored float varialbes
    Dictionary<int, InteractablePiece> pieces;//The pieces's classes
    //The console that will be used when Print( arg )
    public string console = "";
    string code;//The actual code
    //Initialize everything
    public void InitCode(string _code, PieceScript[] _pieces) 
    {
        Debug.Log("Initialized code with " + _pieces.Length + " pieces");
        console = "";
        pieces = new Dictionary<int, InteractablePiece>();
        float_variables = new Dictionary<string, float>();
        SetupAllPiecesClasses(_pieces);
        code = _code;
    }
    //Check code and run it
    public void RunCode(int frame, float time) 
    {
        AssignVariable("time", time);
        console = "";
        string[] lines = code.Split('\n');        
        for (int i = 0; i < lines.Length; i++)
        {
            RunLines(lines[i], lines.Length - 1, i, frame);
        }
    }
    //Run the code lines
    public void RunLines(string linecontent, int endline, int currentline, int currentFrame) 
    {        
        string[] words = linecontent.Split(' ');
        if (words.Length == 0 || string.IsNullOrWhiteSpace(linecontent) || string.IsNullOrEmpty(linecontent) )
        {
            return;//No code
        }
        if (words.Length == 3)
        {
            if (words[1] == "=")//Assign variable
            {
                AssignVariable(words[0], GetVariable(words[2]));
            }
            if (words[0] == "ReadSensors(" && words[2] == ")") //Read sensors function
            {
                if (currentFrame % Mathf.Max(GetVariable(words[1]), 1) == 0) //Delay
                {
                    ReadSensors();
                }
            }
        }
        if (words.Length == 5)
        {
            if (words[1] == "=" && words[4] != ")")//Assign variable with mathematical operation
            {
                AssignVariable(words[0], MathFloat(GetVariable(words[2]), GetVariable(words[4]), words[3]));
            }
            if (words[1] == "=" && words[4] == ")") 
            {
                AssignVariable(words[0], MathFunction(GetVariable(words[3]), words[2]));
            }
        }
        if (words.Length == 4)
        {
            if (words[0] == "SetMotorSpeed(" && words[3] == ")")//Set motor speed
            {
                SetMotorSpeed(Mathf.RoundToInt(GetVariable(words[1])), GetVariable(words[2]));
            }
        }
        if (words.Length == 3)
        {
            if (words[0] == "Print(" && words[2] == ")" )//Write to console the variable
            {
                PrintToConsole(GetFilteredString(words[1]));
            }
        }
        if (words.Length == 6) 
        {
            if (words[1] == "=" && words[5] == ")")
            {
                AssignVariable(words[0], MathFunction(GetVariable(words[3]), GetVariable(words[4]), words[2]));
            }
            if (words[0] == "SCTSetTarget(" && words[5] == ")") //Command to set x y z target coordinates for servoturret control
            {
                SetServoControlledTurretTargetPosition(Mathf.RoundToInt(GetVariable(words[1])), GetVariable(words[2]), GetVariable(words[3]), GetVariable(words[4]));
            }
        }
    }
    #region Number stuff and math
    //Set or add float variable
    public void AssignVariable(string name, float value) 
    {
        if (float_variables.ContainsKey(name))
        {
            float_variables[name] = value;
        }
        else
        {
            float_variables.Add(name, value);
        }
    }
    //Try to get a variable, if not use the string equivalent of it
    public float GetVariable(string name)
    {
        float v = 0;
        if (float_variables.ContainsKey(name))
        {
            return float_variables[name];
        }
        else
        {
            float.TryParse(name, out v);
            return v;
        }
    }
    //Perform mathematical operation on two numbers
    public float MathFloat(float num1, float num2, string operation) 
    {
        if (operation == "+")
        {
            return num1 + num2;
        }
        if (operation == "-")
        {
            return num1 - num2;
        }
        if (operation == "*")
        {
            return num1 * num2;
        }
        if (operation == "/")
        {
            return num1 / num2;
        }
        return num1;
    }
    //Perform mathematical function on one numbers
    public float MathFunction(float num1, string operation) 
    {
        if (operation == "sin(") return Mathf.Sin(num1);
        if (operation == "cos(") return Mathf.Cos(num1);
        if (operation == "sqrt(")
        {
            if (num1 > 0) { return Mathf.Sqrt(num1); }
            else { return 0.0f; }//There is no possible way to get root of negative numbers (imaginery numbers)
        }
        if (operation == "abs(") return Mathf.Abs(num1);
        if (operation == "asin(") return Mathf.Asin(num1);
        if (operation == "acos(") return Mathf.Acos(num1);
        if (operation == "floor(") return Mathf.Floor(num1);
        if (operation == "ceil(") return Mathf.Ceil(num1);
        if (operation == "roun(") return Mathf.Round(num1);
        return 0.0f;//Return 0 if no function found
    }
    //Perform mathematical function on two numbers
    public float MathFunction(float num1, float num2, string operation) 
    {
        if (operation == "pow(") return Mathf.Pow(num1, num2);
        return 0.0f;//Return 0 if no function found
    }
    #endregion
    #region String stuff
    //Print to custom console
    public void PrintToConsole(string content) 
    {
        if (console != "")
        {
            console = console + "\n" + content;
        }
        else
        {
            console = content;
        }
        //Debug.Log(content);//Debugging
    }
    //Try to get variable out of string, if not possible then take the string itself
    public string GetFilteredString(string inputString) 
    {
        if (float_variables.ContainsKey(inputString))
        {
            return float_variables[inputString].ToString();
        }
        else
        {
            return inputString;
        }        
    }
    #endregion
    #region Interacteble pieces method
    //Set speed of rotation structural piece
    private void SetMotorSpeed(int motornum, float motorspeed) 
    {
        MotorJoint myMotorJoint;
        if (GetPieceFromIndex(motornum) != null)
        {
            if (GetPieceFromIndex(motornum) is MotorJoint)
            {
                myMotorJoint = (MotorJoint)GetPieceFromIndex(motornum);//Casting
                myMotorJoint.SetMotorSpeed(motorspeed);//Set speed
            }
        }
    }
    //Set xyz position of servo turret
    private void SetServoControlledTurretTargetPosition(int SCTnum, float x, float y, float z) 
    {
        ServoControlledTurret myServoControlledTurret;
        if (GetPieceFromIndex(SCTnum) != null) 
        {
            if (GetPieceFromIndex(SCTnum) is ServoControlledTurret) 
            {
                myServoControlledTurret = (ServoControlledTurret)GetPieceFromIndex(SCTnum);//Casting
                myServoControlledTurret.SetTargetPosition(x, y, z);
            }
        }
    }
    #endregion
    #region Class-correction and piece list stuff
    //Turns every piece into an interactablepiece script
    private void SetupAllPiecesClasses(PieceScript[] _pieces) 
    {
        _pieces = _pieces.Reverse().ToArray();
        InteractablePiece ip = new InteractablePiece();//Temporary variable since we dont have a constructor in our "InteractablePiece" class
        for (int i = 0; i < _pieces.Length; i++)
        {
            ip = new InteractablePiece();//Temporary variable since we dont have a constructor in our "InteractablePiece" class
            ip.SetupPiece(_pieces[i].myjoint, _pieces[i].myrigidbody, _pieces[i].myname);//Setup temporary var        
            ip = GetCorrectClass(ip);//Correct the class

            pieces.Add(i, ip);
        }
    }
    //Dedect the correct type of InteractablePiece class to use and returns it
    private InteractablePiece GetCorrectClass(InteractablePiece _oldclass) 
    {
        if (_oldclass.myRigidbody.GetComponent<RotationMotorJointScript>() != null)//If we are a Rotation Motor Joint
        {
            MotorJoint myNewMotorJoint = new MotorJoint();
            myNewMotorJoint.SetupPiece(_oldclass.myJoint, _oldclass.myRigidbody, _oldclass.myName);//Setup
            myNewMotorJoint.SetupScript(_oldclass.myRigidbody.GetComponent<RotationMotorJointScript>());//Setup motor only scripts
            return myNewMotorJoint;
        }
        if (_oldclass.myRigidbody.GetComponent<DistanceSensorScript>() != null)//If we are distance sensor
        {
            DistanceSensor myNewDistanceSensor = new DistanceSensor();
            myNewDistanceSensor.SetupPiece(_oldclass.myJoint, _oldclass.myRigidbody, _oldclass.myName);//Setup
            myNewDistanceSensor.SetupScript(_oldclass.myRigidbody.GetComponent<DistanceSensorScript>());//Setup distance sensor only scripts
            return myNewDistanceSensor;
        }
        if(_oldclass.myRigidbody.GetComponent<IMUSensorScript>() != null) //If we are IMU sensor
        {
            IMUSensor myNewIMUSensor = new IMUSensor();
            myNewIMUSensor.SetupPiece(_oldclass.myJoint, _oldclass.myRigidbody, _oldclass.myName);//Setup
            myNewIMUSensor.SetupScript(_oldclass.myRigidbody.GetComponent<IMUSensorScript>());//Setup IMU sensor only scripts
            return myNewIMUSensor;
        }
        if (_oldclass.myRigidbody.GetComponent<ServoControlledTurretScript>() != null) //If we are servo controlled turret
        {
            ServoControlledTurret myNewServoControlledTurret = new ServoControlledTurret();
            myNewServoControlledTurret.SetupPiece(_oldclass.myJoint, _oldclass.myRigidbody, _oldclass.myName);//Setup
            myNewServoControlledTurret.SetupScript(_oldclass.myRigidbody.GetComponent<ServoControlledTurretScript>());//Setup ServoControlledTurret only scripts
            return myNewServoControlledTurret;
        }
        return _oldclass;
    }
    //Gets and spits out the classes from index from dictionary
    private InteractablePiece GetPieceFromIndex(int index) 
    {
        if (pieces.ContainsKey(index))
        {
            return GetCorrectClass(pieces[index]);
        }
        else
        {
            return null;
        }
    }
    #endregion
    #region Misc
    //Reads all sensors mesurements
    private void ReadSensors() 
    {
        InteractablePiece currentPiece;
        for (int i = 0; i < pieces.Count; i++)
        {
            currentPiece = GetPieceFromIndex(i);
            if (currentPiece is DistanceSensor)//If we are a distance sensor
            {
                DistanceSensor distance_sensor = (DistanceSensor)currentPiece;
                AssignVariable("Distance_Sensor" + i, distance_sensor.GetSensorDistance());
            }
            if (currentPiece is IMUSensor)//If we are a IMU sensor 
            {
                IMUSensor imu_sensor = (IMUSensor)currentPiece;
                AssignVariable("IMU_Sensor" + i + ".Acc.X", imu_sensor.GetAcceleration().x);
                AssignVariable("IMU_Sensor" + i + ".Acc.Y", imu_sensor.GetAcceleration().y);
                AssignVariable("IMU_Sensor" + i + ".Acc.Z", imu_sensor.GetAcceleration().z);
                AssignVariable("IMU_Sensor" + i + ".AngVel.X", imu_sensor.GetAngularVelocity().x);
                AssignVariable("IMU_Sensor" + i + ".AngVel.Y", imu_sensor.GetAngularVelocity().y);
                AssignVariable("IMU_Sensor" + i + ".AngVel.Z", imu_sensor.GetAngularVelocity().z);
                AssignVariable("IMU_Sensor" + i + ".Gravity.X", imu_sensor.GetGravity().x);
                AssignVariable("IMU_Sensor" + i + ".Gravity.Y", imu_sensor.GetGravity().y);
                AssignVariable("IMU_Sensor" + i + ".Gravity.Z", imu_sensor.GetGravity().z);
            }
        }
    }
    #endregion
}
#region Pieces classes
public class InteractablePiece //A piece class of the robot that is the parent of multiple inherited classes
{
    public string myName;
    public Joint myJoint;
    public Rigidbody myRigidbody;
    public void SetupPiece(Joint _myJoint, Rigidbody _myRigidbody, string _myName)
    {
        myJoint = _myJoint;//Constructor
        myRigidbody = _myRigidbody;//Constructor
        myName = _myName;//Constructor
    }
}
public class MotorJoint : InteractablePiece//Class fro handling motor joint
{
    public RotationMotorJointScript rotationMotorJointScript;
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
    public void SetupScript(ServoControlledTurretScript _ServoControlledTurretScript) 
    {
        ServoControlledTurretScript = _ServoControlledTurretScript;//Constructor
    }
    public void SetTargetPosition(float x, float y, float z) //Set target pos (x, y, z) of turret with servo rotation
    {
        ServoControlledTurretScript.SetTargetPos(new Vector3(x, y, z));//Set new position and rotate
    }
}
#endregion