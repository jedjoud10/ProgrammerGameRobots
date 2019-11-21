﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSaverLoaderScript : MonoBehaviour
{
    private SaverLoader saverLoader;
    private Dictionary<string, GameObject> prefabs = new Dictionary<string, GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        saverLoader = new SaverLoader();
        LoadPrefabs();
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
        Rigidbody[] pieces = GameObject.FindObjectsOfType<Rigidbody>();
        saverLoader.Save(_filename, pieces);
    }
    public void LoadBuildingPiece(string _filename) //Called from UI Button to load pieces from file
    {
        SaverLoader.SavePiece[] newpieces = saverLoader.Load(_filename);
        Rigidbody[] oldpieces = GameObject.FindObjectsOfType<Rigidbody>();
        List<Rigidbody> currentpieces = new List<Rigidbody>();
        GameObject currentpiece;
        for (int i = 0; i < oldpieces.Length; i++)//Destroy all rigidbodies since we are reloading them
        {
            Destroy(oldpieces[i].gameObject);
        }
        for(int i = 0; i < newpieces.Length; i++)//Load every piece
        {
            SaverLoader.SavePiece piece = newpieces[i];
            currentpiece = Instantiate(prefabs[piece.pieceType]);//Spawn piece from name
            FixedJoint newjoint = currentpiece.AddComponent<FixedJoint>();
            currentpiece.name = piece.pieceType + "-" + piece.pieceNum;
            currentpiece.transform.position = piece.position;
            currentpiece.transform.eulerAngles = piece.eulerRotation;
            if (piece.parentPieceNum != piece.pieceNum)//Dedect if we are first piece
            {
                newjoint.connectedBody = currentpieces[piece.parentPieceNum];
            }
            currentpieces.Add(currentpiece.GetComponent<Rigidbody>());
        }
    }
    public string[] GetDataFilesNames() //Called from UI for dropdown selection
    {
        return saverLoader.GetFiles();
    }
}
