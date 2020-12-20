using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using System.Xml;
using System.Xml.Linq;

public class Find_Base_Stations : MonoBehaviour
{

    public GameObject leftLightHouse;
    public GameObject rightLightHouse;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        getTrackedLocationsOfLighthouses();
    }

    private void getTrackedLocationsOfLighthouses()
    {
        //SteamVR_TrackedObject trackedObject;
        var nodeStates = new List<XRNodeState>();
        InputTracking.GetNodeStates(nodeStates);

        bool isLeft = true;
        foreach (var trackedNode in nodeStates)
        {
            if (trackedNode.nodeType == XRNode.TrackingReference)
            {

                Vector3 lightHouseLocation;

                trackedNode.TryGetPosition(out lightHouseLocation);

                Quaternion lightHouseRotation;
                trackedNode.TryGetRotation(out lightHouseRotation);

                if (isLeft)
                {
                    leftLightHouse.transform.position = lightHouseLocation;
                    leftLightHouse.transform.rotation = lightHouseRotation;
                    isLeft = false;

                    Debug.Log("Left Lighthouse Location Set, ID: " + trackedNode.uniqueID);
                }
                else if (!isLeft)
                {
                    rightLightHouse.transform.position = lightHouseLocation;
                    rightLightHouse.transform.rotation = lightHouseRotation;
                    isLeft = true;

                    Debug.Log("Right Lighthouse Location Set, ID: " + trackedNode.uniqueID);
                }
            }

            //Debug.Log(trackedNode.nodeType);
        }
    }
}
