using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Class for handling the code for the robot
public class Interpreter
{
    Dictionary<string, float> float_variables;//The stored float varialbes
    Dictionary<int, InteractablePiece> pieces;//The pieces's classes
    string code;//The actual code
    //Initialize everything
    public void InitCode(string _code, Rigidbody[] _pieces) 
    {
        pieces = new Dictionary<int, InteractablePiece>();
        float_variables = new Dictionary<string, float>();
        SetupAllPiecesClasses(_pieces);
        code = _code;
    }
    //Check code and run it
    public void RunCode() 
    {
        string[] lines = code.Split('\n');
        ReadSensors();//Read sensors before running code interpreter
        for (int i = 0; i < lines.Length; i++)
        {
            RunLines(lines[i], lines.Length - 1, i);
        }
    }
    //Run the code lines
    public void RunLines(string linecontent, int endline, int currentline) 
    {
        string[] words = linecontent.Split(' ');
        if (words[1] == "=" && words.Length == 3)//Assign variable
        {
            AssignVariable(words[0], GetVariable(words[2]))                                                                            ;
        }
        if (words[1] == "=" && words.Length == 5)//Assign variable with mathematical operation
        {
            AssignVariable(words[0], MathFloat(GetVariable(words[2]), GetVariable(words[4]), words[3]));
        }
        if (words[0] == "SetMotorSpeed(" && words[3] == ")" && words.Length == 4)
        {
            SetMotorSpeed(Mathf.RoundToInt(GetVariable(words[1])), GetVariable(words[2]));
        }
    }
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
            return num1 + num2;
        }
        if (operation == "*")
        {
            return num1 + num2;
        }
        if (operation == "/")
        {
            return num1 + num2;
        }
        return num1;
    }
    //Set speed of rotation structural piece
    private void SetMotorSpeed(int motornum, float motorspeed) 
    {
        MotorJoint myMotorJoint;
        if (GetPieceFromIndex(motornum) is MotorJoint)
        {
            myMotorJoint = (MotorJoint)GetPieceFromIndex(motornum);//Casting
            myMotorJoint.SetMotorSpeed(motorspeed);//Set speed
        }
    }
    //Turns every piece into an interactablepiece script
    private void SetupAllPiecesClasses(Rigidbody[] _pieces) 
    {
        InteractablePiece ip = new InteractablePiece();//Temporary variable since we dont have a constructor in our "InteractablePiece" class
        for (int i = 0; i < _pieces.Length; i++)
        {
            ip.SetupPiece(_pieces[i].GetComponent<Joint>(), _pieces[i]);//Setup temporary var        
            ip = GetCorrectClass(ip);//Correct the class

            pieces.Add(((_pieces.Length-1)-i), ip);
        }
    }
    //Dedect the correct type of InteractablePiece class to use and returns it
    private InteractablePiece GetCorrectClass(InteractablePiece _oldclass) 
    {
        InteractablePiece outclass = _oldclass;
        if (outclass.myRigidbody.GetComponent<RotationMotorJointScript>() != null)//If we are a Rotation Motor Joint
        {
            MotorJoint myNewMotorJoint = new MotorJoint();
            myNewMotorJoint.SetupPiece(_oldclass.myJoint, _oldclass.myRigidbody);//Setup
            myNewMotorJoint.SetupScript(outclass.myRigidbody.GetComponent<RotationMotorJointScript>());//Setup motor only scripts
            return myNewMotorJoint;
        }
        if (outclass.myRigidbody.GetComponent<DistanceSensorScript>() != null)//If we are distance sensor
        {
            DistanceSensor myNewDistanceSensor = new DistanceSensor();
            myNewDistanceSensor.SetupPiece(_oldclass.myJoint, _oldclass.myRigidbody);//Setup
            myNewDistanceSensor.SetupScript(outclass.myRigidbody.GetComponent<DistanceSensorScript>());//Setup distance sensor only scripts
            return myNewDistanceSensor;
        }
        return outclass;
    }
    //Gets and spits out the classes from index from dictionary
    private InteractablePiece GetPieceFromIndex(int index) 
    {
        return GetCorrectClass(pieces[index]);
    }
    //Reads all sensors mesurements
    private void ReadSensors() 
    {        
        for (int i = 0; i < pieces.Count; i++)
        {
            if (GetPieceFromIndex(i) is DistanceSensor)//If we are a distance sensor
            {
                DistanceSensor distance_sensor = (DistanceSensor)GetPieceFromIndex(i);
                AssignVariable("Distance_Sensor" + i,distance_sensor.GetSensorDistance());
            }
        }
    }
}
public class InteractablePiece //A piece class of the robot that is the parent of multiple inherited classes
{
    public Joint myJoint;
    public Rigidbody myRigidbody;
    public void SetupPiece(Joint _myJoint, Rigidbody _myRigidbody)
    {
        myJoint = _myJoint;//Constructor
        myRigidbody = _myRigidbody;//Constructor
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
        return distanceSensorScript.distance;
    }
}
