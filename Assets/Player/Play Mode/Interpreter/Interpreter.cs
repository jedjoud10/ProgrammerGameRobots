using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Class for handling the code for the robot
public class Interpreter
{
    Dictionary<string, float> float_variables;//The stored float varialbes
    //initialize everything
    public void InitCode(string code, Rigidbody[] pieces) 
    {
        float_variables = new Dictionary<string, float>();
    }
    //Check code and run it
    public void RunCode(string code) 
    {
        string[] lines = code.Split('\n');
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
            SetMotorSpeed(words[1], GetVariable(words[2]));
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
    private void SetMotorSpeed(string motornum, float motorspeed) 
    {
        
    }
}
