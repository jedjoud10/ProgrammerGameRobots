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
        if (fileName == "")
        {
            Debug.LogError("File name is nothing !");
            return;
        }
        SavePiece[] newPieces = new SavePiece[pieces.Length];
        Debug.Log("We have " + pieces.Length + " pieces to save");
        for (int i = 0; i < pieces.Length; i++)
        {            
            newPieces[i].pieceName = pieces[i].name;//Set name of piece
            if (pieces[i].GetComponent<Joint>() != null && pieces[i].GetComponent<Joint>().connectedBody != null)//Only for pieces who are not start piece
            {
                newPieces[i].parentPieceNum = IntFromStringName(pieces[i].GetComponent<Joint>().connectedBody.name);//Set parent piece
            }
            newPieces[i].pieceType = NameFromStringName(pieces[i].name);//Get enum from gameobject name
            newPieces[i].eulerRotation = pieces[i].rotation.eulerAngles;
            newPieces[i].pieceNum = IntFromStringName(pieces[i].name);
            newPieces[i].position = pieces[i].transform.position;//Set position
        }
        string path = Application.dataPath + "/SavedRobots/" + fileName + ".txt";
        if (!Directory.Exists(Application.dataPath + "/SavedRobots"))//Dirrectory handling
        {
            Directory.CreateDirectory(Application.dataPath + "/SavedRobots");
        }
        string json = JsonHelper.ToJson(newPieces, true);//Transform into json file
        File.WriteAllText(path, json);//Write to file
    }
    public SavePiece[] Load(string fileName) //Load pieces
    {
        if (fileName == "")
        {
            Debug.LogError("File name is nothing !");            
        }
        SavePiece[] newPieces = new SavePiece[0];
        string path = Application.dataPath + "/SavedRobots/" + fileName + ".txt";
        if (File.Exists(path))
        {
            newPieces = JsonHelper.FromJson<SavePiece>(File.ReadAllText(path));
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
        string[] filenames = new string[0];
        List<string> endfiles = new List<string>();
        string path = Application.dataPath + "/SavedRobots/";
        if (Directory.Exists(path))
        {
            filenames = Directory.GetFiles(path);
            for (int i = 0; i < filenames.Length; i++)
            {
                if (Path.GetExtension(filenames[i]) != ".meta")
                {
                    endfiles.Add(Path.GetFileNameWithoutExtension(filenames[i]));
                }
            }
        }
        else
        {
            Directory.CreateDirectory(Application.dataPath + "/SavedRobots");
        }
        return endfiles.ToArray();
    }

    public void RemoveFile(string filename)//Delete the specified file 
    {
        string path = Application.dataPath + "/SavedRobots/" + filename + ".txt";
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        else
        {
            Debug.LogError("File " + path + " cannot be removed since it dose not exist !");
        }
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
    [System.Serializable]
    public struct SavePiece 
    {
        public string pieceName;
        public int pieceNum;
        public int parentPieceNum;
        public string pieceType;
        public Vector3 position;
        public Vector3 eulerRotation;
    }
}
//Class to make arrays to json. Found on StackOverflow
public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}
