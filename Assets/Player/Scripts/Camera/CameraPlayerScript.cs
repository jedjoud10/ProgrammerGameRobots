using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Camera rotation and movement around player
public class CameraPlayerScript : MonoBehaviour
{
    public Transform player;
    public float sensivityX;
    public float sensivityY;
    public float distanceSensivity;
    private float currentX;
    private float currentY;
    private float distance;
    private float smoothedDistance;
    private Vector3 dir;
    private Quaternion rot;
    public float rotationSmoothness;
    public float distanceSmoothness;
    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {            
            if (GameObject.FindGameObjectWithTag("ControlUnit") != null)
            {
                player = GameObject.FindGameObjectWithTag("ControlUnit").transform;
            }
        }
        if (Input.GetMouseButton(2))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            currentX += Input.GetAxis("Mouse X") * sensivityX;
            currentY += Input.GetAxis("Mouse Y") * sensivityY;
            //currentX = Mathf.Clamp(currentX, -80f, 80f);
            currentY = Mathf.Clamp(currentY, -80f, 80f);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        dir = new Vector3(0, 0, -smoothedDistance);
        distance += Input.mouseScrollDelta.y * distanceSensivity;
        smoothedDistance = Mathf.Lerp(smoothedDistance, distance, distanceSmoothness);
        smoothedDistance = Mathf.Clamp(smoothedDistance, 1, 200); 
        rot = Quaternion.Lerp(rot, Quaternion.Euler(currentY, currentX, 0), rotationSmoothness);
        if (player != null)
        {
            transform.position = player.position + rot * dir;            
            transform.LookAt(player);
        }
    }
}
