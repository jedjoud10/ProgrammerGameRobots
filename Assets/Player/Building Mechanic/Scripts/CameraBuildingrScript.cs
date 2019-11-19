using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Raycasts from the mouse position to infinity and sends the info to other building scripts
public class CameraBuildingrScript : MonoBehaviour
{
    private Vector3 lastmousepos;//The last mouse position to calculate mouse delta
    public BuildingScript BuildingScript;//The script we are going to call to give raycast info
    private LayerMask layerMask;//Ignore other layers
    private Ray ray;//The ray from the camera
    private Camera cam;//This camera component
    private RaycastHit hit;//Hit result
    // Start is called before the first frame update
    void Start()
    {
        layerMask = LayerMask.GetMask("AnchorPointsLayer");//Set the layer mask to be the only anchorpointslayer layer
        cam = GetComponent<Camera>();//Set our own component
    }

    // Update is called once per frame
    void Update()
    {
        if (lastmousepos != Input.mousePosition)//Dedect if delta changed
        {            
            ray = cam.ScreenPointToRay(Input.mousePosition);//Transform to ray from cam
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                if (hit.collider.gameObject.GetComponent<AnchorScript>() != null)//Check if the collider's gameobject is an anchor
                {
                    BuildingScript.HitAnchor(hit.collider.gameObject.GetComponent<AnchorScript>());
                }                
            }
            else
            {
                BuildingScript.HitAnchor(null);//Tell it that the anchor is null so it can update only once and unselect all anchors
            }
        }
        lastmousepos = Input.mousePosition;//Used for delta calculations
    }
}
