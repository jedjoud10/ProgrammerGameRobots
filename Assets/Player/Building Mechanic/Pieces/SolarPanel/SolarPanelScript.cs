using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Class handles rotation and energy production of solar panel
public class SolarPanelScript : MonoBehaviour
{
    private Transform sun;//Sun of world
    public Transform solarpanel_transform;//The upper part of solar panel
    public HingeJoint joint;//Joint of upper part of solar panel
    public JointSpring spring;
    public float damper;
    public float springiness;
    public float powerGenerationRate;
    private float rotation;
    // Start is called before the first frame update
    void Start()
    {
        sun = GameObject.Find("Sun").transform;
    }

    // Update is called once per frame
    void Update()
    {
        //Setup information for the joint
        spring.damper = damper;
        spring.spring = springiness;
        spring.targetPosition = rotation;
        joint.spring = spring;

        //Calculate rotation. This took me 4 hours to find and fix
        rotation = Vector3.Angle(transform.right, -sun.forward * 10000) - 90;
        Debug.Log(rotation);

    }
    //Gets how much power this solar panel is generating
    public float GetPowerGenerationRate() 
    {
        //Linecast to know if we are in shadow or not
        Debug.DrawRay(solarpanel_transform.position, -sun.forward * 10000);
        if (Physics.Raycast(solarpanel_transform.position, -sun.forward*10000)) //Returns true if we in shadow
        {
            return 0;
        }
        else
        {
            return powerGenerationRate;
        }
    }
}
