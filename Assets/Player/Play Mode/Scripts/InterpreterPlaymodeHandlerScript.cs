using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Class handling the interpreter and actual robot
public class InterpreterPlaymodeHandlerScript : MonoBehaviour
{
    Interpreter myinterpreter;
    public Text consoleText;//De console text
    // Inits the interpreter and values, called from saverloaderhandlerscript
    public void InitInterpreter(string code, PieceScript[] pieces)
    {
        myinterpreter = new Interpreter();
        myinterpreter.InitCode(code, pieces);
    }

    // Update is called once per frame
    void Update()
    {
        if (myinterpreter != null)
        {
            myinterpreter.RunCode(Time.frameCount, Time.time);//Run code every frame
            consoleText.text = myinterpreter.console;//Show console
        }
    }
}
