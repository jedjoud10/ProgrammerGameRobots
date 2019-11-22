using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Class for handling the code for the robot
public class Interpreter
{
    Dictionary<string, float> float_variables;//The stored float varialbes
    //initialize everything
    public void InitCode(string code) 
    {
        
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
}
