using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//A script atached to the "Damage cube" piece to apply damage to other pieces
public class DamageCubePieceScript : MonoBehaviour
{
    public int damage;//The damage applied to other pieces
    private PieceScript myPieceScript;//The piece script of this damage piece
    // Start is called before the first frame update
    void Start()
    {
        myPieceScript = GetComponent<PieceScript>();//Setup piece script
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<PieceScript>() != null)//If we are a piece
        {
            if (myPieceScript.myParentPiece != collision.gameObject.GetComponent<PieceScript>().myParentPiece)//Check if the piece we are going to apply damage to is not our own
            {
                collision.gameObject.GetComponent<PieceScript>().DamagePiece(damage);//Damage the piece
            }
        }
    }
}
