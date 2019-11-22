﻿using System.Collections;
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
    private LayerMask mask;//Mask used to disable anchors in collision
    private GameObject PreviewPiece;//The gameobject of the spawned preview piece
    private int pieceNumber = 1;//Piece number to help saving and loading
    private Vector3 rotationOffset;//The relative rotation that is applied
    // Start is called before the first frame update
    void Start()
    {
        mask = LayerMask.GetMask("Default");//Init mask
        PreviewPiece = Instantiate(PreviewPiecePrefab, Vector3.zero, Quaternion.identity);//Init preview piece
        UpdateAnchors(null);
        MainPlayerGameobject = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (newanchor != null)
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
    }
    //Called when the CameraBuildingScript ray collides with a piece
    public void HitPiece(GameObject piece) 
    {
        
    }
    //When we hover of an anchor
    public void HitAnchor(AnchorScript anchor) 
    {
        if (anchor != lastanchor)//Dedect if we should update
        {
            lastanchor = anchor;
            UpdateAnchors(anchor);
        }
    }
    //Updates all acnhors to unselect and select the only anchor (unless it is null)
    public void UpdateAnchors(AnchorScript anchor) 
    {
        GameObject[] anchorsInScene = GameObject.FindGameObjectsWithTag("Anchor");
        for (int i = 0; i < anchorsInScene.Length; i++)
        {
            if (!anchorsInScene[i].activeSelf)
            {
                continue;
            }
            anchorsInScene[i].GetComponent<AnchorScript>().Unselect();
            if (anchor != null)
            {
                if (anchorsInScene[i] == anchor.gameObject)
                {
                    anchorsInScene[i].GetComponent<AnchorScript>().Select();
                    PreviewPiece.SetActive(true);
                    UpdatePreviewMesh();
                }
            }
            else
            {
                PreviewPiece.SetActive(false);
                UpdatePreviewMesh();
            }
            if (Physics.CheckSphere(anchorsInScene[i].transform.position, 0.25f, mask))
            {
                anchorsInScene[i].gameObject.SetActive(false);
            }
            else
            {
                anchorsInScene[i].gameObject.SetActive(true);
            }
        }
        newanchor = anchor;
    }
    //Add piece to the anchor, then fix it to the anchor's parent
    public void AddBuildingPiece(AnchorScript anchor)
    {
        if (anchor != null)
        {
            GameObject newpiece = Instantiate(SelectedBuildingPiecePrefab, anchor.transform.position, PreviewPiece.transform.rotation);//Spawn the piece
            Joint newjoint = newpiece.GetComponent<Joint>(); 
            newpiece.name = SelectedBuildingPiecePrefab.name + "-" + pieceNumber;            
            newjoint.connectedBody = anchor.parent;
            anchor.gameObject.SetActive(false);
            UpdateAnchors(null);
            newpiece.transform.parent = MainPlayerGameobject.transform;
            pieceNumber++;
        }
    }
    //Update the preview mesh
    public void UpdatePreviewMesh()
    {
        if (PreviewPiece != null)
        {
            PreviewPiece.GetComponent<MeshFilter>().mesh = SelectedBuildingPiecePrefab.transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh;//Use the mesh for the preview piece from the actual piece's first child gameobject
            PreviewPiece.transform.localScale = SelectedBuildingPiecePrefab.transform.GetChild(0).transform.localScale;
        }
    }
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
}
