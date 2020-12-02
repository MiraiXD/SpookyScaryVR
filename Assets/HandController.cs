using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVR;
public class HandController : MonoBehaviour
{
    public OVRInput.Handedness controllerHandedness;
    [SerializeField] private LaserController laserController;
   
    void Update()
    {
        if(OVRInput.GetDown(controllerHandedness == OVRInput.Handedness.LeftHanded ? OVRInput.Button.PrimaryIndexTrigger : OVRInput.Button.SecondaryIndexTrigger) || Input.GetKeyDown(KeyCode.Space))
        {            
            if (Physics.Raycast(laserController.start.position, transform.forward, out RaycastHit hit, laserController.maxShrinkDistance, laserController.laserMask))
            {                
                laserController.end = hit.transform;
                laserController.ShootLaser = true;
            }
        }
        if (OVRInput.GetUp(controllerHandedness == OVRInput.Handedness.LeftHanded ? OVRInput.Button.PrimaryIndexTrigger : OVRInput.Button.SecondaryIndexTrigger) || Input.GetKeyUp(KeyCode.Space))
        {
            //laserController.ShootLaser = false;
        }
        if (OVRInput.GetDown(controllerHandedness == OVRInput.Handedness.LeftHanded ? OVRInput.Button.PrimaryHandTrigger : OVRInput.Button.SecondaryHandTrigger)) laserController.shrink = !laserController.shrink;
    }
}
