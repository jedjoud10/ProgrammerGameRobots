using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Handles information between the UI and the other building classes
public class BuildingUIScript : MonoBehaviour
{
    public SaverLoaderHandlerScript BuildingSaverLoaderScript;
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
    public GameObject destroybutton;
    public Text codeeditor;
    public GameObject codeEditorPanel;
    public InputField codeeditor_textboxInputfield;
    public Text hoveredPieceText;
    public Text variablesText;
    public BuildingModeInterpreterHandlerScript BuildingModeInterpreterHandlerScript;
    
    //Gets all piece's data and objects
    private void GetAllPiecesScriptableObjects() 
    {
        pieces = Resources.LoadAll<PieceScriptableObject>("Pieces data");    
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
    //Set the dropdown's options to the file names
    private void GiveLoadingNames()
    {
        string[] fileNames = BuildingSaverLoaderScript.GetDataFilesNames();
        dropdown.ClearOptions();
        foreach (var name in fileNames)
        {
            if (name != "BaseRobot" && name != "TemporarySave")
            {
                dropdown.options.Add(new Dropdown.OptionData(name));
            }            
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
            BuildingScript.SetPreviewMesh();
        }
    }
    //Load pieces
    public void LoadPieces() 
    {
        BuildingSaverLoaderScript.LoadBuildingPieces(dropdown.captionText.text, false);
    }
    //Save pieces
    public void SavePieces() 
    {
        BuildingSaverLoaderScript.SaveBuildingPieces(saveNameText.text);
        GiveLoadingNames();
    }
    //Destroy selected file
    public void RemoveFile() 
    {
        if (dropdown.captionText.text != "")
        {
            BuildingSaverLoaderScript.RemoveFile(dropdown.captionText.text);
        }
        else
        {
            Debug.LogError("Dropdown not selected yet !");
        }
        GiveLoadingNames();
    }
    //Called when dropdown value has changed
    public void OnDropdownValueChange() 
    {
        if (dropdown.captionText.text != "")
        {
            destroybutton.SetActive(true);
        }
        else
        {
            destroybutton.SetActive(false);
        }
    }
    //Save the current piece to a temporary file that can be loaded after coming back
    private void SaveToTemporaryFile() 
    {
        BuildingSaverLoaderScript.SaveBuildingPieces("TemporarySave");
        BuildingSaverLoaderScript.SaveCode("TemporarySave", codeeditor.text);
    }
    //Change to preview map
    public void PreviewCraft() 
    {
        SaveToTemporaryFile();
        GameObject.FindObjectOfType<SceneManagerScript>().ChangeScene("PreviewScene");
    }
    //Make and restart the robot buildin
    public void MakeNewRobot() 
    {
        BuildingSaverLoaderScript.LoadBuildingPieces("BaseRobot", false);
    }
    //Show code editor
    public void ShowCodeEditor() 
    {
        codeEditorPanel.SetActive(true);
        BuildingScript.canInteract = false;
    }
    //Hide code editor
    public void HideCodeEditor() 
    {
        codeEditorPanel.SetActive(false);
        BuildingScript.canInteract = true;
    }
    //Shows every variable that is going to be used in the interpreter
    public void ShowVariables(string[] variables)
    {
        string vars = "";
        foreach (var variable in variables)
        {
            if (vars == "")
            {
                vars = variable;//If we are the first variable to set, set the output string to it since there is no need to add another line
            }
            else
            {
                vars = vars + "\n" + variable;
            }
        }
        variablesText.text = vars;
    }
    //Called from button to compile code and show variables
    public void CompileCode() 
    {
        BuildingModeInterpreterHandlerScript.CompileCode(codeeditor.text, FindObjectsOfType<Rigidbody>());
    }
    //Set code text
    public void LoadCodeText(string _newCode) 
    {
        codeeditor_textboxInputfield.text = _newCode;
        codeeditor.text = _newCode;
    }
    //Set the hovered anchor's piece's name
    public void SetHoveredPieceName(string name) 
    {
        hoveredPieceText.text = GetIntStringFromString(name);
    }
    //Gets the int from a string even though the string might contain alphabetical charachters
    private string GetIntStringFromString(string _string) 
    {
        string outstring = "";//The string composed of numbers only
        foreach (var stringChar in _string.ToCharArray())
        {
            if (char.IsDigit(stringChar))
            {
                if (outstring == "")//To avoid adding a blank space on the first charachter
                {
                    outstring = stringChar.ToString();
                }
                else
                {
                    outstring = outstring + stringChar;
                }
            }
        }
        return outstring;
    }
}
