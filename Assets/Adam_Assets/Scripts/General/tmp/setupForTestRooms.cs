using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setupForTestRooms : MonoBehaviour
{

    public enum platformEnum // your custom enumeration
    {
        DESKTOP,
        VIVE,
        HL2
    };

    public platformEnum platformDropDown = platformEnum.DESKTOP;  // this public var should appear as a drop down

    // Start is called before the first frame update
    void Start()
    {
        if (platformDropDown == platformEnum.DESKTOP || platformDropDown == platformEnum.HL2)
        {
            disableVR();
        } else if (platformDropDown == platformEnum.VIVE)
        {
            enableVR();
        } else
        {
            Debug.LogError("setupForTestRoom Failed - no platform configuration found");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    static void enableVR()
    {
        UnityEngine.XR.XRSettings.enabled = true;
        UnityEngine.XR.XRSettings.LoadDeviceByName("OpenVR");
    }

    static void disableVR()
    {
        UnityEngine.XR.XRSettings.enabled = false;
        UnityEngine.XR.XRSettings.LoadDeviceByName("None");
    }
}
