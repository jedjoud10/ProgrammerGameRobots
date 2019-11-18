using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Script that every achor has

public class AnchorScript : MonoBehaviour
{
    public MeshRenderer meshRenderer;//The anchor's mesh renderer
    public Material selectedMat;//The material when you select the anchor
    public Material unselectedMat;//The material when you unselect the anchor
    public Rigidbody parent;//The parent rigidbody
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Select() 
    {
        meshRenderer.material = selectedMat;//Select the correct material
    }
    public void Unselect() 
    {
        meshRenderer.material = unselectedMat;//Select the correct material
    }
}
