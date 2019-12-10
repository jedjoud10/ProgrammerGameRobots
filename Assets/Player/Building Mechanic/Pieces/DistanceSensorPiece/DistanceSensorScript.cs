using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceSensorScript : MonoBehaviour
{
    public float maxDistance;
    public Transform rayPointOrigin;//Origin of raycast
    private RaycastHit hit;
    private float distance;
    int layerMask;//The layer mask so we dont hit anchors
    // Start is called before the first frame update
    void Start()
    {
        layerMask = 1 << 8;
        layerMask = ~layerMask;
    }

    public float GetDistance() //CAlculate out distance and limit it
    {
        distance = maxDistance;
        if (Physics.Raycast(rayPointOrigin.position, rayPointOrigin.forward * maxDistance, out hit, Mathf.Infinity, layerMask))
        {
            distance = hit.distance;
        }
        return distance;
    }
}
