using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceSensorScript : MonoBehaviour
{
    [Range(0.1f, 2f)]
    public float refreshRate;
    public float maxDistance;
    [HideInInspector()]
    public float distance;//The calculated final distance
    public Transform rayPointOrigin;//Origin of raycast
    private RaycastHit hit;
    int layerMask;//The layer mask so we dont hit anchors
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("GetDistance", 0, refreshRate);
        layerMask = 1 << 8;
        layerMask = ~layerMask;
    }

    public void GetDistance() //CAlculate out distance and limit it
    {
        distance = maxDistance;
        if (Physics.Raycast(rayPointOrigin.position, rayPointOrigin.forward * maxDistance, out hit, Mathf.Infinity, layerMask))
        {
            distance = hit.distance;
        }
        Debug.DrawRay(rayPointOrigin.position, rayPointOrigin.forward * distance);
    }
}
