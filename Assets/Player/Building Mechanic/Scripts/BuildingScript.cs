using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Handles most of building mechanic

public class BuildingScript : MonoBehaviour
{
    private BuildingType BuildingTypeVar;//How we are going to change the robot from the camera rays ? (ex : Adding pieces/Removing pieces)
    private GameObject MainPlayerGameobject;//The main player
    public GameObject PreviewPiecePrefab;//A piece prefab used to preview the position of the next new piece
    public GameObject SelectedBuildingPiecePrefab;
    private AnchorScript lastanchor;//A variable used to dedect if the new selected anchor is updated so we can save performance
    private AnchorScript newanchor;//The new anchor to add the new block to
    private GameObject lastpiece;//The last hovered piece
    private GameObject PreviewPiece;//The gameobject of the spawned preview piece
    private int pieceNumber = 1;//Piece number to help saving and loading
    private Vector3 rotationOffset;//The relative rotation that is applied
    public BuildingUIScript BuildingUIScript;//UI handler
    public bool canInteract = true;//CAn we interact with the world ?
    // Start is called before the first frame update
    void Start()
    {
        PreviewPiece = Instantiate(PreviewPiecePrefab, Vector3.zero, Quaternion.identity);//Init preview piece
        UpdateAnchors(null);
        MainPlayerGameobject = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (canInteract)
        {
            if (newanchor != null && BuildingTypeVar == BuildingType.Building)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    AddBuildingPiece(newanchor);
                }
                else
                {
                    PreviewPiece.transform.position = newanchor.transform.position;//Set the position of the preview piece
                    PreviewPiece.transform.rotation = newanchor.transform.rotation;//Set the rotation of the preview piece to be the same as the anchor's
                    PreviewPiece.transform.Rotate(rotationOffset);
                }
                #region Rotation Offset Handling
                if (Input.GetKeyDown(KeyCode.T))
                {
                    rotationOffset += new Vector3(90, 0, 0);
                }
                if (Input.GetKeyDown(KeyCode.Y))
                {
                    rotationOffset += new Vector3(-90, 0, 0);
                }
                if (Input.GetKeyDown(KeyCode.G))
                {
                    rotationOffset += new Vector3(0, 90, 0);
                }
                if (Input.GetKeyDown(KeyCode.H))
                {
                    rotationOffset += new Vector3(0, -90, 0);
                }
                if (Input.GetKeyDown(KeyCode.B))
                {
                    rotationOffset += new Vector3(0, 0, 90);
                }
                if (Input.GetKeyDown(KeyCode.N))
                {
                    rotationOffset += new Vector3(0, 0, -90);
                }
                #endregion
            }
            if (Input.GetKeyDown(KeyCode.Z))//Switching types
            {
                SwitchBuildingType();
            }
            if (lastpiece != null && BuildingTypeVar == BuildingType.Destroy)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    DestroyBuildingPiece(lastpiece);
                }
                else
                {
                    PreviewPiece.transform.position = lastpiece.transform.position;
                    PreviewPiece.transform.rotation = lastpiece.transform.rotation;
                    PreviewPiece.transform.localScale = lastpiece.transform.localScale * 1.1f;
                }
            }
            if (BuildingTypeVar == BuildingType.Destroy)
            {
                RemoveNullParentedPieces();
            }
        }
    }
    //Called when the CameraBuildingScript ray collides with a piece
    public void HitPiece(GameObject piece) 
    {
        if (piece != lastpiece)//Dedect if we should update
        {
            lastpiece = piece;
            SetPreviewMesh(lastpiece.transform.GetChild(0).GetComponent<MeshFilter>().mesh, lastpiece.transform.localScale);
            if (piece != null)
            {
                BuildingUIScript.SetHoveredPieceName(piece.name);//Find the root object's name
            }
        }
    }
    //When we hover over an anchor
    public void HitAnchor(AnchorScript anchor) 
    {
        if (anchor != lastanchor)//Dedect if we should update
        {
            lastanchor = anchor;
            UpdateAnchors(anchor);
        }        
    }
    //Updates all anchors to unselect and select the only anchor (unless it is null)
    public void UpdateAnchors(AnchorScript anchor) 
    {
        if (BuildingTypeVar == BuildingType.Destroy)
        {
            PreviewPiece.SetActive(true);
            return;//We wont use anchors for removing pieces
        }
        GameObject[] anchorsInScene = GameObject.FindGameObjectsWithTag("Anchor");
        for (int i = 0; i < anchorsInScene.Length; i++)
        {
            if (anchorsInScene[i].GetComponent<AnchorScript>() == null)
            {
                continue;
            }
            anchorsInScene[i].GetComponent<AnchorScript>().Unselect();
            if (anchor != null && BuildingTypeVar == BuildingType.Building)
            {
                if (anchorsInScene[i] == anchor.gameObject)
                {
                    anchorsInScene[i].GetComponent<AnchorScript>().Select();
                    PreviewPiece.SetActive(true);
                    SetPreviewMesh();
                }
            }
            else
            {
                PreviewPiece.SetActive(false);
                DisablePreviewMesh();
            }
        }
        newanchor = anchor;
    }
    //Add piece to the anchor, then fix it to the anchor's parent
    public void AddBuildingPiece(AnchorScript anchor)
    {
        if (anchor != null && BuildingTypeVar == BuildingType.Building)
        {
            GameObject newpiece = Instantiate(SelectedBuildingPiecePrefab, anchor.transform.position, PreviewPiece.transform.rotation);//Spawn the piece
            Joint newjoint = newpiece.GetComponent<Joint>(); 
            newpiece.name = SelectedBuildingPiecePrefab.name + "-" + pieceNumber;            
            newjoint.connectedBody = anchor.parent;
            UpdateAnchors(null);
            newpiece.transform.parent = MainPlayerGameobject.transform;
            pieceNumber++;
            ReorderGM();
        }
    }
    //Destroy bulding piece and remove children pieces of that destroyed piece
    public void DestroyBuildingPiece(GameObject piece) 
    {
        if (piece != null && piece.tag != "ControlUnit")
        {
            Destroy(piece);
            UpdateAnchors(null);
            RemoveNullParentedPieces();
            ReorderGM();
        }
    }
    //Remove pieces that have no parent exept the control unit
    public void RemoveNullParentedPieces() 
    {
        PieceScript[] totalPieces = GameObject.FindObjectsOfType<PieceScript>();
        for (int i = 0; i < totalPieces.Length; i++)//Removing one piece at a time
        {
            if (totalPieces[i].tag != "ControlUnit")//We are not the main block
            {
                if (totalPieces[i].GetComponent<Joint>() != null)
                {
                    if (totalPieces[i].GetComponent<Joint>().connectedBody == null)//If we dont have a parent piece
                    {
                        Destroy(totalPieces[i].gameObject);
                    }
                }
            }
        }
    }
    #region Update preview mesh
    //-------------Update the preview mesh---------------
    //Set the preview mesh to the current selected building piece
    public void SetPreviewMesh()
    {
        if (PreviewPiece != null)
        {
            PreviewPiece.GetComponent<MeshFilter>().mesh = SelectedBuildingPiecePrefab.transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh;//Use the mesh for the preview piece from the actual piece's first child gameobject
            PreviewPiece.transform.localScale = SelectedBuildingPiecePrefab.transform.GetChild(0).transform.localScale;
        }
    }
    //Set the preview mesh to the mesh argument
    public void SetPreviewMesh(Mesh mesh, Vector3 scale) 
    {
        if (PreviewPiece != null)
        {
            PreviewPiece.GetComponent<MeshFilter>().mesh = mesh;
            PreviewPiece.transform.localScale = scale;
        }
    }
    //Set the preview mesh to nothing
    public void DisablePreviewMesh() 
    {
        if (PreviewPiece != null)
        {
            PreviewPiece.GetComponent<MeshFilter>().mesh = null;
        }
    }
    #endregion
    //How are we building ?
    private enum BuildingType 
    {
        Building, Destroy
    }
    //Called when loading pieces since we need to add and not duplicate
    public void SetupBuildingNumber(int _newnumber) 
    {
        pieceNumber = _newnumber;
    }
    //Called when we want to switch over the next building type
    private void SwitchBuildingType() 
    {
        if (BuildingTypeVar == BuildingType.Building)
        {
            BuildingTypeVar = BuildingType.Destroy;
            return;
        }
        if (BuildingTypeVar == BuildingType.Destroy)
        {
            BuildingTypeVar = BuildingType.Building;
            return;
        }
    }
    //Rename every object by their preset order
    private void ReorderGM() 
    {
        PieceScript[] rbs = FindObjectsOfType<PieceScript>();
        for (int i = 0; i < rbs.Length; i++)
        {
            rbs[i].name = NameFromStringName(rbs[i].name) + "-" + ((rbs.Length - 1) - i);
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
