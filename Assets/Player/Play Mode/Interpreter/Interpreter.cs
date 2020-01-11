using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
//Class for handling the code for the robot
public class Interpreter
{
    public Dictionary<string, float> float_variables;//The stored float varialbes

    //The pieces's classes. Get/set for when variable changed
    Dictionary<int, InteractablePiece> pieces;    
    Dictionary<PIECE_TYPE, List<InteractablePiece>> sortedpieces = new Dictionary<PIECE_TYPE, List<InteractablePiece>>();//Pieces when sorted with piece_type enum
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
        CallStartCode();//Call startcode on all pieces
        SortPieces(pieces);//Sort pieces for sortedpieces variable
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
        CallLoopCode();//Call loopcode on all pieces
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
    //Uses the power of first powerpiece it dedects. Returns how much power we used
    public float UsePower(float power)
    {
        InteractablePiece chosenpowerpiece = GetPowerPieceWithHighestPower();//The chosen power piece since it is first
        if (chosenpowerpiece == null) return 0;
        if (chosenpowerpiece is BatteryPowerPiece)
        {
            BatteryPowerPiece batterypiece = (BatteryPowerPiece)chosenpowerpiece;//Cast piece to battery piece if it is battery piece
            return batterypiece.GetPower(power);//Return power used
        }
        return 0;
    }
    //Adds power to battery with lowest power
    public void AddPower(float power) 
    {
        InteractablePiece chosenpowerpiece = GetPowerPieceWithLowestPower();
        if (chosenpowerpiece == null) return;
        if (chosenpowerpiece is BatteryPowerPiece)
        {
            BatteryPowerPiece batterypiece = (BatteryPowerPiece)chosenpowerpiece;//Cast piece to battery piece if it is battery piece
            batterypiece.AddPower(power);//Add power
        }
    }
    //Get the power piece with the highest power from the craft
    private InteractablePiece GetPowerPieceWithHighestPower() 
    {
        InteractablePiece outpiece = null;//The return piece
        InteractablePiece piece = null;//Loop piece
        float maxpower = 0;//The maximum power piece's power
        for (int i = 0; i < sortedpieces[PIECE_TYPE.POWER_PIECE].Count; i++) //loop over power pieces
        {
            piece = sortedpieces[PIECE_TYPE.POWER_PIECE][i];//Variable to make it more clearer
            if (piece is BatteryPowerPiece) //If power piece is battery
            {
                BatteryPowerPiece batterypiece = (BatteryPowerPiece)piece;//Cast to actual battery piece 
                if (batterypiece.GetRemainingPower_pu() > maxpower) //If the battery power is maximum power
                {
                    maxpower = batterypiece.GetRemainingPower_pu();//Make that as new threshold
                    outpiece = piece;
                }                
            }
        }
        return outpiece;
    }
    //Get the power piece with the lowest power from the craft
    private InteractablePiece GetPowerPieceWithLowestPower()
    {
        InteractablePiece outpiece = null;//The return piece
        InteractablePiece piece = null;//Loop piece
        float maxpower = float.MaxValue;//The maximum power piece's power
        for (int i = 0; i < sortedpieces[PIECE_TYPE.POWER_PIECE].Count; i++) //loop over power pieces
        {
            piece = sortedpieces[PIECE_TYPE.POWER_PIECE][i];//Variable to make it more clearer
            if (piece is BatteryPowerPiece) //If power piece is battery
            {
                BatteryPowerPiece batterypiece = (BatteryPowerPiece)piece;//Cast to actual battery piece 
                if (batterypiece.GetRemainingPower_pu() < maxpower) //If the battery power is maximum power
                {
                    maxpower = batterypiece.GetRemainingPower_pu();//Make that as new threshold
                    outpiece = piece;
                }
            }
        }
        return outpiece;
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
            ip.SetupPiece(_pieces[i].myjoint, _pieces[i].myrigidbody, _pieces[i].myname, this);//Setup temporary var        
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
            myNewMotorJoint.SetupPiece(_oldclass.myJoint, _oldclass.myRigidbody, _oldclass.myName, this);//Setup
            myNewMotorJoint.SetupScript(_oldclass.myRigidbody.GetComponent<RotationMotorJointScript>());//Setup motor only scripts
            return myNewMotorJoint;
        }
        if (_oldclass.myRigidbody.GetComponent<DistanceSensorScript>() != null)//If we are distance sensor
        {
            DistanceSensor myNewDistanceSensor = new DistanceSensor();
            myNewDistanceSensor.SetupPiece(_oldclass.myJoint, _oldclass.myRigidbody, _oldclass.myName, this);//Setup
            myNewDistanceSensor.SetupScript(_oldclass.myRigidbody.GetComponent<DistanceSensorScript>());//Setup distance sensor only scripts
            return myNewDistanceSensor;
        }
        if (_oldclass.myRigidbody.GetComponent<IMUSensorScript>() != null) //If we are IMU sensor
        {
            IMUSensor myNewIMUSensor = new IMUSensor();
            myNewIMUSensor.SetupPiece(_oldclass.myJoint, _oldclass.myRigidbody, _oldclass.myName, this);//Setup
            myNewIMUSensor.SetupScript(_oldclass.myRigidbody.GetComponent<IMUSensorScript>());//Setup IMU sensor only scripts
            return myNewIMUSensor;
        }
        if (_oldclass.myRigidbody.GetComponent<ServoControlledTurretScript>() != null) //If we are servo controlled turret
        {
            ServoControlledTurret myNewServoControlledTurret = new ServoControlledTurret();
            myNewServoControlledTurret.SetupPiece(_oldclass.myJoint, _oldclass.myRigidbody, _oldclass.myName, this);//Setup
            myNewServoControlledTurret.SetupScript(_oldclass.myRigidbody.GetComponent<ServoControlledTurretScript>());//Setup ServoControlledTurret only scripts
            return myNewServoControlledTurret;
        }
        if (_oldclass.myRigidbody.GetComponent<BatteryPowerScript>() != null)//If we are battery
        {
            BatteryPowerPiece myNewBattery = new BatteryPowerPiece();
            myNewBattery.SetupPiece(_oldclass.myJoint, _oldclass.myRigidbody, _oldclass.myName, this);//Setup
            myNewBattery.SetupScript(_oldclass.myRigidbody.GetComponent<BatteryPowerScript>());//Setup Battery only scripts
            return myNewBattery;
        }
        if(_oldclass.myRigidbody.GetComponent<PowerSensorScript>() != null)//If we are powersensor
        {
            PowerSensor myNewPowerSensor = new PowerSensor();
            myNewPowerSensor.SetupPiece(_oldclass.myJoint, _oldclass.myRigidbody, _oldclass.myName, this);//Setup
            myNewPowerSensor.SetupScript(_oldclass.myRigidbody.GetComponent<PowerSensorScript>(), sortedpieces);//Setup powesensor only scripts
            return myNewPowerSensor;
        }
        if (_oldclass.myRigidbody.GetComponent<SolarPanelScript>() != null)//If we are solarpanel
        {
            SolarPanelPowerPiece myNewSolarPanelPowerPiece = new SolarPanelPowerPiece();
            myNewSolarPanelPowerPiece.SetupPiece(_oldclass.myJoint, _oldclass.myRigidbody, _oldclass.myName, this);//Setup
            myNewSolarPanelPowerPiece.SetupScript(_oldclass.myRigidbody.GetComponent<SolarPanelScript>());//Setup solarpanel only scripts
            return myNewSolarPanelPowerPiece;
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
    //Reorder the pieces and set that for the sortedpieces variable
    private void SortPieces(Dictionary<int, InteractablePiece> _pieces) 
    {
        sortedpieces.Clear();//Clear array to add new elements
        foreach (PIECE_TYPE piecetype in Enum.GetValues(typeof(PIECE_TYPE)))//Add each type of pieces into the dictionary
        {
            sortedpieces.Add(piecetype, new List<InteractablePiece>(0));//Add piece type
        }
        for (int i = 0; i < _pieces.Count; i++) 
        {
            List<InteractablePiece> oldlist = sortedpieces[_pieces[i].piecetype];//Old list of pieces from this type
            oldlist.Add(_pieces[i]);//Add the new piece
            sortedpieces[_pieces[i].piecetype] = oldlist;//Change the oldlist to the new list with added piece
        }
    }
    //Call loopcode on all pieces
    private void CallLoopCode() 
    {
        for(int i  = 0; i < pieces.Count; i++) 
        {
            pieces[i].CodeLoop();
        }
    }
    //Call startcode on all pieces
    private void CallStartCode() 
    {
        for (int i = 0; i < pieces.Count; i++)
        {
            pieces[i].CodeStart();
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
            if (currentPiece is BatteryPowerPiece)//If we are the onboard sensor of the battery piece
            {
                BatteryPowerPiece batterypiece = (BatteryPowerPiece)currentPiece;
                AssignVariable("BatterySensor" + i + ".Percentage", batterypiece.GetRemainingPower_pe());
                AssignVariable("BatterySensor" + i + ".Units", batterypiece.GetRemainingPower_pu());
            }

            if (currentPiece is PowerSensor)//If we are a Power Sensor
            {
                PowerSensor powersensor = (PowerSensor)currentPiece;
                AssignVariable("PowerSensor" + i + ".Percentage", powersensor.CraftPowerPercentage());
                AssignVariable("PowerSensor" + i + ".Units", powersensor.CraftPowerUnits());
            }
        }
    }
    #endregion
}