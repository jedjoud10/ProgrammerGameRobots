using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaverLoader
{
    public void Save(string fileName, Rigidbody[] pieces) //Save pieces
    {
        Piece[] newPieces = new Piece[pieces.Length];
        string finalText;
        for (int i = 0; i < pieces.Length; i++)
        {
            newPieces[i].pieceName = pieces[i].name;//Set name of piece
            if (pieces[i].GetComponent<FixedJoint>() != null)//Only for pieces who are not start piece
            {
                newPieces[i].parentPiece = pieces[i].GetComponent<FixedJoint>().connectedBody;//Set parent piece
            }
            if (pieces[i].name == "StructuralPiece")
            {
                newPieces[i].pieceType = PieceType.StructuralPiece;
            }
            if (pieces[i].name == "ControlUnit")
            {
                newPieces[i].pieceType = PieceType.ControlUnit;
            }
            newPieces[i].position = pieces[i].transform.position;//Set position
        }
        string path = Application.dataPath + "/SavedMachines/" + fileName;
        if (!Directory.Exists(Application.dataPath + "/SavedMachines"))//Dirrectory handling
        {
            Directory.CreateDirectory(Application.dataPath + "/SavedMachines");
        }
        foreach (var newpiece in newPieces)
        {
            //finalText.Insert();
        }
        //File.WriteAllText();
    }
    public void Load(string fileName) //Load pieces
    {
    
    }
    private struct Piece 
    {
        public string pieceName;
        public Rigidbody parentPiece;
        public PieceType pieceType;
        public Vector3 position;
    }
    private enum PieceType
    {
        StructuralPiece, ControlUnit
    }
}
