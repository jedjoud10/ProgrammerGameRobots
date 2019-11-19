using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Handles most of building mechanic

public class BuildingScript : MonoBehaviour
{
    public GameObject MainPlayerGameobject;//The main player for organisation
    public GameObject PreviewPiecePrefab;//A piece prefab used to preview the position of the next new piece
    public GameObject SelectedBuildingPiecePrefab;
    private AnchorScript lastanchor;//A variable used to dedect if the new selected anchor is updated so we can save performance
    private AnchorScript newanchor;//The new anchor to add the new block to
    private LayerMask mask;//Mask used to disable anchors in collision
    private GameObject PreviewPiece;//The gameobject of the spawned preview piece
    private int pieceNumber;//Piece number to help saving and loading
    // Start is called before the first frame update
    void Start()
    {
        mask = LayerMask.GetMask("Default");//Init mask
        PreviewPiece = Instantiate(PreviewPiecePrefab, Vector3.zero, Quaternion.identity);//Init preview piece
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
            }
        }
    }
    public void HitAnchor(AnchorScript anchor) //When we hover of an anchor
    {
        if (anchor != lastanchor)//Dedect if we should update
        {
            lastanchor = anchor;
            UpdateAnchors(anchor);
        }
    }
    public void UpdateAnchors(AnchorScript anchor) //Updates all acnhors to unselect and select the only anchor (unless it is null)
    {
        GameObject[] anchorsInScene = GameObject.FindGameObjectsWithTag("Anchor");
        for (int i = 0; i < anchorsInScene.Length; i++)
        {
            if (!anchorsInScene[i].activeSelf)
            {
                continue;//Loop to next anchor since this one is inactive
            }
            anchorsInScene[i].GetComponent<AnchorScript>().Unselect();//Unselect all anchors
            if (anchor != null)
            {
                if (anchorsInScene[i] == anchor.gameObject)//Dedect if its the selected anchor
                {
                    anchorsInScene[i].GetComponent<AnchorScript>().Select();//Select the anchor
                    PreviewPiece.SetActive(true);//Enable preview piece
                    PreviewPiece.GetComponent<MeshFilter>().mesh = SelectedBuildingPiecePrefab.transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh;//Use the mesh for the preview piece from the actual piece's first child gameobject
                    PreviewPiece.transform.localScale = SelectedBuildingPiecePrefab.transform.GetChild(0).transform.localScale;
                }
            }
            else
            {
                PreviewPiece.SetActive(false);//Disable preview piece since our anchor is null
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
        newanchor = anchor;//Set the new anchor
    }
    public void AddBuildingPiece(AnchorScript anchor)//Add piece to the anchor, then fix it to the anchor's parent
    {
        if (anchor != null)//Cuz we dont want to spawn nothing
        {
            GameObject newpiece = Instantiate(SelectedBuildingPiecePrefab, anchor.transform.position, anchor.transform.rotation);//Spawn the piece
            FixedJoint newjoint = newpiece.AddComponent<FixedJoint>();//Adds the joint to not let it move
            newpiece.name = SelectedBuildingPiecePrefab.name + "-" + pieceNumber;            
            newjoint.connectedBody = anchor.parent;//Connect the new piece to the anchor's parent piece
            anchor.gameObject.SetActive(false);//Disable the anchor sine we place a piece at it's place
            UpdateAnchors(null);
            newpiece.transform.parent = MainPlayerGameobject.transform;
            pieceNumber++;
        }
    }
}
