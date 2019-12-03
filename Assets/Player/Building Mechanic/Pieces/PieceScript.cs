﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//A script that handles the piece health and other piece information
public class PieceScript : MonoBehaviour
{
    public float health;//The pieces health
    public float max_health = 100;//The pieces maximum health

    //Piece information for data saving
    public Rigidbody myrigidbody;
    public Joint myjoint;
    public string myname;
    // Start is called before the first frame update
    void Start()
    {
        health = max_health;
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
