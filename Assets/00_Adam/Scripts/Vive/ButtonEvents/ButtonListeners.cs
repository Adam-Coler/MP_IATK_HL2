using UnityEngine;
using Photon.Pun;

namespace Photon_IATK
{
    public class ButtonListeners : MonoBehaviour
    {
        public PrimaryButtonWatcher primaryWatcher;
        //public TriggerButtonWatcher triggerWatcher;

        private DrawingVariables drawingVariables;

        void Awake()
        {
            drawingVariables = DrawingVariables.Instance;


            primaryWatcher = GameObject.FindObjectOfType<PrimaryButtonWatcher>();

            if (primaryWatcher == null)
            {
                Debug.LogFormat(GlobalVariables.red + "No PrimaryButtonWatcher found" + GlobalVariables.endColor + " : Awake()" + this.GetType());
            }

            //triggerWatcher = GameObject.FindObjectOfType<TriggerButtonWatcher>();

            //if (triggerWatcher == null)
            //{
            //    Debug.LogFormat(GlobalVariables.red + "No TriggerButtonWatcher found" + GlobalVariables.endColor + " : Awake()" + this.GetType());
            //}

            //triggerWatcher.TriggerButtonPress.AddListener(onTriggerPress);
            //triggerWatcher.TriggerButtonPressForce.AddListener(onTriggerPressForce);
            //triggerWatcher.triggerPressedLocation.AddListener(OnTriggerPosition);
            //primaryWatcher.primaryButtonPress.AddListener(onPrimaryButtonEvent);


            Debug.LogFormat(GlobalVariables.blue + "Listeners started" + GlobalVariables.endColor + " : Start()" + this.GetType());
        }

        public void onPrimaryButtonEvent(bool pressed)
        {
            Debug.LogFormat(GlobalVariables.blue + "Primary button pressed = {0}" + GlobalVariables.endColor + " : onPrimaryButtonEvent()" + this.GetType(), pressed);
        }


        GameObject tmp_Line_Render_Prefab;
        LineRenderer lineRenderer;
        public void onTriggerPress(bool pressed)
        {
            Debug.LogFormat(GlobalVariables.blue + "Trigger button pressed = {0}" + GlobalVariables.endColor + " : onTriggerPress()" + this.GetType(), pressed);

            drawingVariables.isDrawing = pressed;

            if (pressed)
            {
                tmp_Line_Render_Prefab = PhotonNetwork.Instantiate("LineDrawing", Vector3.zero, Quaternion.identity);

                PhotonLineDrawing photonLineDrawing = tmp_Line_Render_Prefab.GetComponent<PhotonLineDrawing>();
                photonLineDrawing.isUser = true;
                photonLineDrawing.Initalize();
            } else
            {
                tmp_Line_Render_Prefab = null;
            }


        }

        public void onTriggerPressForce(float force)
        {
            //Debug.LogFormat(GlobalVariables.blue + "Trigger button force = {0}" + GlobalVariables.endColor + " : onTriggerPressForce()" + this.GetType(), force);

            drawingVariables.lineWidthFromButtonForce = force;
        }

        public void OnTriggerPosition(Vector3 triggerPressPosition)
        {
            Debug.LogFormat(GlobalVariables.blue + "Trigger press position = {0}" + GlobalVariables.endColor + " : onPrimaryButtonEvent()" + this.GetType(), triggerPressPosition);

            drawingVariables.penTipPosition = triggerPressPosition;

            if (tmp_Line_Render_Prefab != null)
            {
                tmp_Line_Render_Prefab.GetComponent<PhotonLineDrawing>().addPoint(triggerPressPosition);
            }


        }




    }
}