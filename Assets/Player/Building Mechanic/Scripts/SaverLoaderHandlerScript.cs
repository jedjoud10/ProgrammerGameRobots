﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaverLoaderHandlerScript : MonoBehaviour
{
    private SaverLoader saverLoader = new SaverLoader();
    private Dictionary<string, GameObject> prefabs = new Dictionary<string, GameObject>();
    public InterpreterPlaymodeHandlerScript InterpreterPlaymodeHandlerScript;
    public BuildingUIScript BuildingUIScript;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    //When the gameobject is enabled
    private void OnEnable()
    {
        LoadPrefabs();        
        if (saverLoader != null)
        {
            saverLoader.SetupPathes();
        }
        else
        {
            saverLoader = new SaverLoader();
            saverLoader.SetupPathes();
        }
    }
    private void LoadPrefabs() 
    {
        GameObject[] allLoadedPrefabs = Resources.LoadAll<GameObject>("Pieces prefabs");
        foreach (var prefab in allLoadedPrefabs)//Setup dictionary
        {
            prefabs.Add(prefab.name, prefab);
        }
        Debug.Log("Loaded " + prefabs.Count + " prefabs");
    }

    public void SaveBuildingPieces(string _filename) //Called from UI button to save pieces to file
    {
        PieceScript[] pieces = GameObject.FindObjectsOfType<PieceScript>();
        for (int i = 0; i < pieces.Length; i++)
        {
            pieces[i].gameObject.name = NameFromStringName(pieces[i].gameObject.name) + "-" + ((pieces.Length - 1) - i);
        }
        saverLoader.SavePieces(_filename, pieces);
    }
    public void LoadBuildingPieces(string _filename, bool enablePhysics) //Called from UI Button to load pieces from file
    {
        SaverLoader.SavePiece[] newpieces = saverLoader.LoadPieces(_filename);
        Debug.Log("Loaded " + newpieces.Length + " pieces", gameObject);
        PieceScript[] oldpieces = GameObject.FindObjectsOfType<PieceScript>();
        List<PieceScript> currentpieces = new List<PieceScript>();
        GameObject currentpiece;
        Transform parentplayer = GameObject.FindGameObjectWithTag("Player").transform;
        for (int i = 0; i < oldpieces.Length; i++)//Destroy all rigidbodies since we are reloading them
        {
            Destroy(oldpieces[i].gameObject);
        }
        for(int i = 0; i < newpieces.Length; i++)//Load every piece
        {
            SaverLoader.SavePiece piece = newpieces[i];
            currentpiece = Instantiate(prefabs[piece.pieceType]);//Spawn piece from name
            Joint newjoint = currentpiece.GetComponent<Joint>();
            currentpiece.name = piece.pieceType + "-" + piece.pieceNum;
            currentpiece.transform.position = piece.position;
            currentpiece.transform.eulerAngles = piece.eulerRotation;
            currentpiece.transform.parent = parentplayer;
            if (piece.parentPieceNum != piece.pieceNum)//Dedect if we are NOT first piece
            {
                newjoint.connectedBody = currentpieces[piece.parentPieceNum].myrigidbody;
            }
            if(enablePhysics && piece.parentPieceNum == piece.pieceNum)//Dedect if we ARE first piece and that we are allowed to remove the fixed joint from first piece
            {
                if (newjoint != null)
                {
                    Destroy(newjoint);
                }
            }
            if (enablePhysics)
            {
                currentpiece.GetComponent<PieceScript>().myrigidbody.isKinematic = false;
            }
            currentpiece.SetActive(false);//Disable and re-enable because bugs
            currentpiece.SetActive(true);
            currentpieces.Add(currentpiece.GetComponent<PieceScript>());            
        }
        if (GameObject.FindObjectOfType<BuildingScript>() != null)//Check if we are in the building scene
        {
            GameObject.FindObjectOfType<BuildingScript>().SetupBuildingNumber(currentpieces.Count);
        }
    }
    public string[] GetDataFilesNames() //Called from UI for dropdown selection
    {
        if (saverLoader != null)
        {
            return saverLoader.GetFilesPieces();
        }
        else
        {
            saverLoader = new SaverLoader();
            return saverLoader.GetFilesPieces();
        }
    }
    public void RemoveFile(string _filename) //Called from UI to remove selected file
    {
        saverLoader.RemoveFilePieces(_filename);
    }
    public void SaveCode(string _filename, string codecontent) //Save code with help from SaverLoader
    {
        saverLoader.SaveProgram(_filename, codecontent);
    }
    public string LoadCode(string _filename) //Load de code from file
    {
        return saverLoader.LoadProgram(_filename);
    }
    public void LoadRobotPhysics(string _codefilename, string _piecesfilename, bool enablePhysics) //Load robots when we are in preview mode, load also de code
    {
        LoadBuildingPieces(_piecesfilename, enablePhysics);
        if (InterpreterPlaymodeHandlerScript != null)
        {
            InterpreterPlaymodeHandlerScript.InitInterpreter(LoadCode(_codefilename), GameObject.FindObjectsOfType<PieceScript>());
        }
        if (BuildingUIScript != null)
        {
            BuildingUIScript.LoadCodeText(LoadCode(_codefilename));
        }
    }
    private string NameFromStringName(string name)//Remove numbers and other stuff from string
    {
        string outstring = "";
        for (int i = 0; i < name.Length; i++)
        {
            if (!char.IsDigit(name[i]) && name[i] != '-')//Check if it is digit
            {
                outstring += name[i];
            }
        }
        return outstring;//Return the final string
    }
}
