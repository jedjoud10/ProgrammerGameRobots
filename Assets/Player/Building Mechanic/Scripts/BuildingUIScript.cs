using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Handles information between the UI and the other building classes
public class BuildingUIScript : MonoBehaviour
{
    public BuildingSaverLoaderScript BuildingSaverLoaderScript;
    public BuildingScript BuildingScript;
    private PieceScriptableObject[] pieces;
    public RawImage currentPiece_RI;
    public Text pieceNameText;
    private int currentPieceIndex = 0;
    private PieceScriptableObject currentPiece;
    public RawImage lastPiece_RI;
    public RawImage nextPiece_RI;
    public Dropdown dropdown;
    public Text saveNameText;
    //Gets all piece's data and objects
    private void GetAllPiecesScriptableObjects() 
    {
        pieces = Resources.LoadAll<PieceScriptableObject>("Pieces data");    
    }
    //Set the dropdown's options to the file names
    private void GiveLoadingNames() 
    {
        string[] fileNames = BuildingSaverLoaderScript.GetDataFilesNames();
        dropdown.ClearOptions();
        foreach (var name in fileNames)
        {
            dropdown.options.Add(new Dropdown.OptionData(name));
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        GetAllPiecesScriptableObjects();
        currentPiece = pieces[currentPieceIndex];
        UpdateDataPieceUI(currentPiece);
        GiveLoadingNames();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //Switch current piece to the next
    public void NextPiece() 
    {
        if (currentPieceIndex < pieces.Length - 1)
        {
            currentPieceIndex++;
            currentPiece = pieces[currentPieceIndex];
            UpdateDataPieceUI(currentPiece);
        }
    }
    //Switch current piece to the last
    public void LastPiece() 
    {
        if (currentPieceIndex > 0)
        {
            currentPieceIndex--;
            currentPiece = pieces[currentPieceIndex];
            UpdateDataPieceUI(currentPiece);
        }
    }
    //Update the UI information of the selected piece and neighbouring pieces
    private void UpdateDataPieceUI(PieceScriptableObject piece) 
    {
        pieceNameText.text = piece.UI_name;
        currentPiece_RI.texture = piece.icon;
        //Set last and next pieces's icons
        if (currentPieceIndex > 0)
        {
            lastPiece_RI.enabled = true;
            lastPiece_RI.texture = pieces[currentPieceIndex - 1].icon;//Get last piece icon if possible
        }
        else
        {
            lastPiece_RI.texture = null;
            lastPiece_RI.enabled = false;
        }
        if (currentPieceIndex < pieces.Length - 1)
        {
            nextPiece_RI.enabled = true;
            nextPiece_RI.texture = pieces[currentPieceIndex + 1].icon;//Get last piece icon if possible
        }
        else
        {
            nextPiece_RI.texture = null;
            nextPiece_RI.enabled = false;
        }
        if (BuildingScript != null || currentPiece != null)
        {
            BuildingScript.SelectedBuildingPiecePrefab = currentPiece.prefab_piece;//Set the new piece
            BuildingScript.UpdatePreviewMesh();
        }
    }
    //Load pieces
    public void LoadPieces() 
    {
        BuildingSaverLoaderScript.LoadBuildingPiece(dropdown.captionText.text);
    }
    //Save pieces
    public void SavePieces() 
    {
        BuildingSaverLoaderScript.SaveBuildingPieces(saveNameText.text);
        GiveLoadingNames();
    }
}
