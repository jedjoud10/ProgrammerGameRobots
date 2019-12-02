using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
//Handles the information flow between the interpreter and our BuildingUI class
public class BuildingModeInterpreterHandlerScript : MonoBehaviour
{
    private Interpreter myinterpreter;
    public BuildingUIScript BuildingUIScript;
    // Start is called before the first frame update
    void Start()
    {
        myinterpreter = new Interpreter();
    }
    //Called from BuildingUI button "Compile code" to compile the code and return information to the BuildingUI class
    public void CompileCode(string code, Rigidbody[] rigidbodies) 
    {
        myinterpreter.InitCode(code, rigidbodies);
        myinterpreter.RunCode();
        BuildingUIScript.ShowVariables(myinterpreter.float_variables.Keys.ToArray());
        Debug.Log("There is " + myinterpreter.float_variables.Keys.Count + " variables");
    }
}
