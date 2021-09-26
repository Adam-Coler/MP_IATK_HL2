using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Photon_IATK
{
    [DisallowMultipleComponent]
    public class PenButtonWatcher : MonoBehaviour
    {
        public PenButtonEvents penButtonEvents;
        AnnotationManagerSaveLoadEvents annotationManager;
        void Awake()
        {
            penButtonEvents = GameObject.FindObjectOfType<PenButtonEvents>();

            if (penButtonEvents == null)
            {
                Debug.LogFormat(GlobalVariables.red + "No penButtonEvents found" + GlobalVariables.endColor + " : Awake()" + this.GetType());
            }

            if (!HelperFunctions.GetComponent(out annotationManager, System.Reflection.MethodBase.GetCurrentMethod()))
            {
                Debug.LogFormat(GlobalVariables.cError + "No AnnotationManagerSaveLoadEvents found" + GlobalVariables.endColor + " : Awake()" + this.GetType());
            }

            penButtonEvents.penTriggerPress.AddListener(onPenTriggerPress);
            penButtonEvents.penGripButtonPressEvent.AddListener(onPenGripPress);
            penButtonEvents.penTriggerPressedLocation.AddListener(OnPenTriggerPosition);
            Debug.LogFormat(GlobalVariables.blue + "Listeners started" + GlobalVariables.endColor + " : Start()" + this.GetType());
        }

        public void onPenGripPress(bool pressed)
        {
            Debug.LogFormat(GlobalVariables.blue + "Grip button pressed = {0}" + GlobalVariables.endColor + " : onTriggerPress()" + this.GetType(), pressed);

            if (pressed)
            {
                annotationManager.requestDisableFarInteractions();
            } else
            {
                annotationManager.requestEnableFarInteractions();
            }
        }

        bool tmp_Line_Render_Prefab;
        public void onPenTriggerPress(bool pressed)
        {
            Debug.LogFormat(GlobalVariables.blue + "Trigger button pressed = {0}" + GlobalVariables.endColor + " : onTriggerPress()" + this.GetType(), pressed);

            if (pressed)
            {
                annotationManager.RequestAnnotationCreation(Annotation.typesOfAnnotations.LINERENDER);
                tmp_Line_Render_Prefab = true;
            }
            else
            {
                tmp_Line_Render_Prefab = false;
            }
        }

        public void OnPenTriggerPosition(Vector3 triggerPressPosition)
        {
            //Debug.LogFormat(GlobalVariables.blue + "Trigger press position = {0}" + GlobalVariables.endColor + " : onPrimaryButtonEvent()" + this.GetType(), triggerPressPosition);

            //if (tmp_Line_Render_Prefab)
            //{
            //    var viewID = annotationManager.lastMadeAnnotationPhotonViewID;
            //    var view = PhotonView.Find(viewID);

            //    Debug.LogFormat(GlobalVariables.cAlert + "Line Render = View ID: {0}, Null: {1}{2}" + GlobalVariables.endColor + " : onPrimaryButtonEvent()" + this.GetType(), viewID, view == null, "");

            //    var annotation = view.gameObject.GetComponent<Annotation>();

            //    annotation.addPoint(triggerPressPosition);
            //}


        }
    }
}
