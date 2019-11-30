﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Class handling the interpreter and actual robot
public class InterpreterPlaymodeHandlerScript : MonoBehaviour
{
    Interpreter myinterpreter;
    // Inits the interpreter and values, called from saverloaderhandlerscript
    public void InitInterpreter(string code, Rigidbody[] pieces)
    {
        myinterpreter = new Interpreter();
        myinterpreter.InitCode(code, pieces);
    }

    // Update is called once per frame
    void Update()
    {
        if (myinterpreter != null)
        {
            myinterpreter.RunCode();
        }
    }
}
