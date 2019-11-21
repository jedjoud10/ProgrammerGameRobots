using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu()]
//Handles the scriptable object for future piece creation
public class PieceScriptableObject : ScriptableObject
{
    public string UI_name;
    public Mesh mesh;
    public Material material;
    public Texture icon;
    public string description;
    public GameObject prefab_piece;
}
