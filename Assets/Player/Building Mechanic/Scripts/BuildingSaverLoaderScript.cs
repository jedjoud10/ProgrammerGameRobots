using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSaverLoaderScript : MonoBehaviour
{
    private SaverLoader saverLoader;
    private Dictionary<string, GameObject> prefabs;
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
        GameObject currentpiece;
        for (int i = 0; i < oldpieces.Length; i++)//Destroy all rigidbodies since we are reloading them
        {
            Destroy(oldpieces[i]);
        }
        foreach (var piece in newpieces)//Load every piece
        {
            currentpiece = Instantiate(prefabs[piece.pieceType]);//Spawn piece from name
            currentpiece.transform.position = piece.position;
            currentpiece.transform.eulerAngles = piece.eulerRotation;
            currentpiece.GetComponent<FixedJoint>().connectedBody = piece.parentPiece;            
        }
    }
}
