using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//A script that handles the piece health and other piece information
public class PieceScript : MonoBehaviour
{
    [HideInInspector]
    public float health;//The pieces health
    public float max_health = 100;//The pieces maximum health
    [HideInInspector]
    public GameObject myParentPiece;//The parent piece of this piece

    //Piece information for data saving
    [HideInInspector]
    public Rigidbody myrigidbody;
    [HideInInspector]
    public Joint myjoint;
    [HideInInspector]
    public string myname;
    // Start is called before the first frame update
    void Start()
    {
        health = max_health;
        if (transform.parent != null)
        {
            myParentPiece = transform.parent.gameObject;//Setup parent
        }
        SetupPieceInfo();
    }
    //When script is loaded
    private void OnValidate()
    {
        SetupPieceInfo();//Setup info
    }
    //Sets piece info
    private void SetupPieceInfo() 
    {
        myrigidbody = GetComponent<Rigidbody>();//Setup rigidbody;
        myjoint = GetComponent<Joint>();//Setup joint
        myname = gameObject.name;//Setup name
    }
    //Called from "DamageCube" piece to damage this piece
    public void DamagePiece(float damage) 
    {
        health -= damage;
        if (health < 0)//If this piece is dead
        {
            Destroy(gameObject);//Destroy this piece
        }
    }
}
