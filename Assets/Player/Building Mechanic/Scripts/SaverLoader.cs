using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
//Class handling saving and loading of craft properities
public class SaverLoader
{
    string robots_SavePath = "/SavedRobots";
    string programs_SavePath = "/SavedPrograms";
    string enemybots_SavePath = "/EnemyRobots";
    public void SetupPathes() //Setup the pathes with the correct path
    {
        robots_SavePath = Application.dataPath + "/SavedRobots";
        programs_SavePath = Application.dataPath + "/SavedPrograms";
        enemybots_SavePath = Application.dataPath + "/EnemyRobots";
    }
    public void SavePieces(string fileName, Rigidbody[] pieces) //Save pieces
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
        string path = robots_SavePath + "/" + fileName + ".txt";
        if (!Directory.Exists(robots_SavePath))//Dirrectory handling
        {
            Directory.CreateDirectory(robots_SavePath);
        }
        string json = JsonHelper.ToJson(newPieces, true);//Transform into json file
        File.WriteAllText(path, json);//Write to file
    }
    public SavePiece[] LoadPieces(string fileName) //Load pieces
    {
        if (fileName == "")
        {
            Debug.LogError("File name is nothing !");            
        }
        SavePiece[] newPieces = new SavePiece[0];
        string path = robots_SavePath + "/" + fileName + ".txt";
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
    public string[] GetFilesPieces() //Get saved files from the base building folder
    {
        string[] filenames = new string[0];
        List<string> endfiles = new List<string>();
        string path = robots_SavePath + "/";
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
            Directory.CreateDirectory(robots_SavePath);
        }
        return endfiles.ToArray();
    }
    public void RemoveFilePieces(string filename)//Delete the specified file 
    {
        string path = robots_SavePath + "/" + filename + ".txt";
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        else
        {
            Debug.LogError("File " + path + " cannot be removed since it dose not exist !");
        }
    }
    public void SaveProgram(string fileName, string programContent) //Save program from code editor
    {
        if (fileName == "")
        {
            Debug.LogError("File name is nothing !");
            return;
        }
        string path = programs_SavePath + "/" + fileName + ".txt";
        if (!Directory.Exists(programs_SavePath))//Dirrectory handling
        {
            Directory.CreateDirectory(programs_SavePath);
        }
        File.WriteAllText(path, programContent);//Write to file
    }
    public string LoadProgram(string fileName) //Loads program from file
    {
        string path = programs_SavePath + "/" + fileName + ".txt";
        if (File.Exists(path))
        {
            return File.ReadAllText(path);
        }
        else
        {
            Debug.LogError("File does not exist!");
        }
        return "";
    }
    public List<EnemyRobot> LoadEnemyRobots() //Load enemies from enemy folder
    {
        if (Directory.Exists(enemybots_SavePath + "/"))
        {
            List<EnemyRobot> enemyRobots = new List<EnemyRobot>();
            string[] dirs = Directory.GetDirectories(enemybots_SavePath + "/");
            foreach (var dir in dirs)
            {
                //Reading and loading of pieces & program of correct robot
                string program_path = dir + "/" + "program.txt";
                string pieces_path = dir + "/" + "pieces.txt";
                SavePiece[] newPieces = JsonHelper.FromJson<SavePiece>(File.ReadAllText(pieces_path));
                newPieces = newPieces.OrderBy(x => x.pieceNum).ToArray();
                string program_content = File.ReadAllText(program_path);
                Debug.Log("Succsessfully loaded " + dir + " robot");
                EnemyRobot enemyRobot = new EnemyRobot();//To add to list of robots
                enemyRobot.pieces = newPieces;
                enemyRobot.program = program_content;
                enemyRobots.Add(enemyRobot);//Add to robots list
            }
            return enemyRobots;
        }
        else
        {
            Debug.LogError("Directory does not exist !");
            return null;
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
    public struct EnemyRobot 
    {
        public SavePiece[] pieces;
        public string program;
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
