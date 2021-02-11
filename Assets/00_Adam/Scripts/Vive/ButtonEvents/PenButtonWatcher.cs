using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Photon_IATK
{
    public class PenButtonWatcher : MonoBehaviour
    {
        public PenButtonEvents penButtonEvents;

        void Awake()
        {
            penButtonEvents = GameObject.FindObjectOfType<PenButtonEvents>();

            if (penButtonEvents == null)
            {
                Debug.LogFormat(GlobalVariables.red + "No penButtonEvents found" + GlobalVariables.endColor + " : Awake()" + this.GetType());
            }

            penButtonEvents.penTriggerPress.AddListener(onPenTriggerPress);
            penButtonEvents.penTriggerPressedLocation.AddListener(OnPenTriggerPosition);
            Debug.LogFormat(GlobalVariables.blue + "Listeners started" + GlobalVariables.endColor + " : Start()" + this.GetType());
        }

        GameObject tmp_Line_Render_Prefab;
        public void onPenTriggerPress(bool pressed)
        {
            Debug.LogFormat(GlobalVariables.blue + "Trigger button pressed = {0}" + GlobalVariables.endColor + " : onTriggerPress()" + this.GetType(), pressed);

            if (pressed)
            {
                


                tmp_Line_Render_Prefab = PhotonNetwork.InstantiateRoomObject("LineDrawing", Vector3.zero, Quaternion.identity);

                PhotonLineDrawing photonLineDrawing = tmp_Line_Render_Prefab.GetComponent<PhotonLineDrawing>();
                photonLineDrawing.Initalize();
            }
            else
            {
                tmp_Line_Render_Prefab = null;
            }
        }

        public void OnPenTriggerPosition(Vector3 triggerPressPosition)
        {
            Debug.LogFormat(GlobalVariables.blue + "Trigger press position = {0}" + GlobalVariables.endColor + " : onPrimaryButtonEvent()" + this.GetType(), triggerPressPosition);

            if (tmp_Line_Render_Prefab != null)
            {
                tmp_Line_Render_Prefab.GetComponent<PhotonLineDrawing>().addPoint(triggerPressPosition);
            }


        }
    }
}
