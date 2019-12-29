using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Handles information between the UI and the other building classes
public class BuildingUIScript : MonoBehaviour
{
    public SaverLoaderHandlerScript BuildingSaverLoaderScript;//BuildingSaverLoaderSCript handler
    public BuildingScript BuildingScript;//BuildingScript handler
    private PieceScriptableObject[] pieces;//All pieces in assets
    public RawImage currentPiece_RI;//Image of current piece
    public Text pieceNameText;//Name of current piece
    private int currentPieceIndex = 0;//Index of current piece
    private PieceScriptableObject currentPiece;//Current piece while in building mode
    public RawImage lastPiece_RI;//Image of the last piece while in building mode
    public RawImage nextPiece_RI;//Image of the next piece while in building mode
    public Dropdown dropdown;//Dropdown to select saves
    public Text saveNameText;//The name of the save
    public GameObject destroybutton;//The button to destroy the save file
    public Text codeeditor;//The text of the code editor
    public GameObject codeEditorPanel;//The code editor panel
    public InputField codeeditor_textboxInputfield;//The input field of the code editr
    public Text hoveredPieceText;//The data of the currently hovered piece
    public Text variablesText;//Variables of interpreter
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
            BuildingScript.ChangePiece(currentPiece);//Set preview mesh and new prefab piece
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
    //Change to play map
    public void PlayCraft() 
    {
        SaveToTemporaryFile();
        GameObject.FindObjectOfType<SceneManagerScript>().ChangeScene("PlayScene");
    }
    //Make and restart the robot building
    public void MakeNewRobot() 
    {
        BuildingSaverLoaderScript.LoadBuildingPieces("BaseRobot", false);
    }
    //Show code editor
    public void ShowCodeEditor() 
    {
        codeEditorPanel.SetActive(true);
        BuildingScript.canInteract = false;

        GetComponent<Camera>().enabled = false;
    }
    //Hide code editor
    public void HideCodeEditor() 
    {
        codeEditorPanel.SetActive(false);
        BuildingScript.canInteract = true;
        GetComponent<Camera>().enabled = true;
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
        BuildingModeInterpreterHandlerScript.CompileCode(codeeditor.text, FindObjectsOfType<PieceScript>());
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
