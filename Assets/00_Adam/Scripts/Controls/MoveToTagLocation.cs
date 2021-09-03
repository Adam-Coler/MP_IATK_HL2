using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Photon_IATK
{
    public class MoveToTagLocation : MonoBehaviour
    {
        public string locationToMoveTo;
        public string myLocation;
        public bool isLocation;

        public void Start()
        {
            Debug.LogFormat(GlobalVariables.cCommon + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "MoveToTagLocation on " + this.name + ", is location (" + isLocation + "), set location: " + myLocation + ", Move to: " + locationToMoveTo + ". ", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            if (!isLocation)
            {
                MoveToTagLocation[] moveToTagLocations = FindObjectsOfType<MoveToTagLocation>();

                foreach (MoveToTagLocation tagLocation in moveToTagLocations)
                {
                    if (tagLocation.isLocation)
                    {
                        if (tagLocation.myLocation == locationToMoveTo)
                        {
                            Debug.LogFormat(GlobalVariables.cCommon + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "MoveToTagLocation on " + this.name + ", is moving to " + tagLocation.name + "" + locationToMoveTo + ". Location name: " + myLocation + ". ", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                            this.gameObject.transform.position = tagLocation.transform.position;
                            this.gameObject.transform.rotation = tagLocation.transform.rotation;
                            return;
                        }
                    }
                }
                Debug.LogFormat(GlobalVariables.cError + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "No suitable location found for " + this.name + ", set location: " + locationToMoveTo + ". ", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            }
        }
    }
}