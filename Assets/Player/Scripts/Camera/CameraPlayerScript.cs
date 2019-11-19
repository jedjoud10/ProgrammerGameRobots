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
    private Vector3 dir;
    private Quaternion rot;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(2))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            currentX += Input.GetAxis("Mouse X") * sensivityX;
            currentY += Input.GetAxis("Mouse Y") * sensivityY;
            rot = Quaternion.Euler(currentY, currentX, 0);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        dir = new Vector3(0, 0, -distance);
        distance += Input.mouseScrollDelta.y;
        distance = Mathf.Clamp(distance, 1, 20);
        transform.position = player.position + rot * dir;
        transform.LookAt(player);
    }
}
