using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
//Class handling saving and loading of pieces of craft
public class SaverLoader
{
    public void Save(string fileName, Rigidbody[] pieces) //Save pieces
    {
        SavePiece[] newPieces = new SavePiece[pieces.Length];
        for (int i = 0; i < pieces.Length; i++)
        {
            newPieces[i].pieceName = pieces[i].name;//Set name of piece
            if (pieces[i].GetComponent<FixedJoint>() != null)//Only for pieces who are not start piece
            {
                newPieces[i].parentPiece = pieces[i].GetComponent<FixedJoint>().connectedBody;//Set parent piece
            }
            newPieces[i].pieceType = NameFromStringName(pieces[i].name);//Get enum from gameobject name
            newPieces[i].eulerRotation = pieces[i].rotation.eulerAngles;
            newPieces[i].pieceNum = IntFromStringName(pieces[i].name);
            newPieces[i].position = pieces[i].transform.position;//Set position
        }
        string path = Application.dataPath + "/SavedMachines/" + fileName + ".txt";
        if (!Directory.Exists(Application.dataPath + "/SavedMachines"))//Dirrectory handling
        {
            Directory.CreateDirectory(Application.dataPath + "/SavedMachines");
        }
        string json = JsonUtility.ToJson(newPieces);//Transform into json file
        File.WriteAllText(path, json);//Write to file
    }
    public SavePiece[] Load(string fileName) //Load pieces
    {
        SavePiece[] newPieces = new SavePiece[0];
        string path = Application.dataPath + "/SavedMachines/" + fileName + ".txt";
        if (File.Exists(path))
        {
            newPieces = JsonUtility.FromJson<SavePiece[]>(File.ReadAllText(path));
            newPieces = newPieces.OrderBy(x => x.pieceNum).ToArray();
            return newPieces;
        }
        else
        {
            Debug.LogError("Path " + path + " does not exist !");
        }
        return newPieces;
    }
    public string[] GetFiles() //Get saved files from the base building folder
    {
        string[] filenames;
        string path = Application.dataPath + "/SavedMachines/";

        filenames = Directory.GetFiles(path);

        return filenames;
    }
    private int IntFromStringName(string name)//Get a number from a string that might contain letters 
    {
        string outstring = "";
        int outint = 0;
        for (int i = 0; i < name.Length; i++)
        {
            if (char.IsDigit(name[i]))//Check if it is digit
            {
                outstring += name[i];
            }
        }
        if (outstring.Length > 0)
        {
            outint = int.Parse(outstring);
        }
        return outint;//Return the final num
    }
    private string NameFromStringName(string name)//Remove numbers from string
    {
        string outstring = "";
        for (int i = 0; i < name.Length; i++)
        {
            if (!char.IsDigit(name[i]))//Check if it is digit
            {
                outstring += name[i];
            }
        }
        return outstring;//Return the final string
    }
    public struct SavePiece 
    {
        public string pieceName;
        public int pieceNum;
        public Rigidbody parentPiece;
        public string pieceType;
        public Vector3 position;
        public Vector3 eulerRotation;
    }
}
