using UnityEngine;
using Photon.Pun;

namespace Photon_IATK
{
    public class ButtonListeners : MonoBehaviour
    {
        public PrimaryButtonWatcher primaryWatcher;
        //public TriggerButtonWatcher triggerWatcher;
        AnnotationManagerSaveLoadEvents annotationManager;
        private DrawingVariables drawingVariables;

        void Awake()
        {
            drawingVariables = DrawingVariables.Instance;


            primaryWatcher = GameObject.FindObjectOfType<PrimaryButtonWatcher>();

            if (primaryWatcher == null)
            {
                Debug.LogFormat(GlobalVariables.cError + "No PrimaryButtonWatcher found" + GlobalVariables.endColor + " : Awake()" + this.GetType());
            }

                       
            if (!HelperFunctions.GetComponent(out annotationManager, System.Reflection.MethodBase.GetCurrentMethod())) {
                Debug.LogFormat(GlobalVariables.cError + "No AnnotationManagerSaveLoadEvents found" + GlobalVariables.endColor + " : Awake()" + this.GetType());
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
        public void onTriggerPress(bool pressed)
        {
            Debug.LogFormat(GlobalVariables.cFileOperations + "Trigger button pressed = {0}" + GlobalVariables.endColor + " : onTriggerPress()" + this.GetType(), pressed);

            drawingVariables.isDrawing = pressed;

            if (pressed)
            {
                annotationManager.RequestAnnotationCreation(Annotation.typesOfAnnotations.LINERENDER);

                //tmp_Line_Render_Prefab = PhotonNetwork.InstantiateRoomObject("LineDrawing", Vector3.zero, Quaternion.identity);

                //PhotonLineDrawing photonLineDrawing = tmp_Line_Render_Prefab.GetComponent<PhotonLineDrawing>();
                //photonLineDrawing.Initalize();
            }

        }

        public void onTriggerPressForce(float force)
        {
            //Debug.LogFormat(GlobalVariables.blue + "Trigger button force = {0}" + GlobalVariables.endColor + " : onTriggerPressForce()" + this.GetType(), force);

            drawingVariables.lineWidthFromButtonForce = force;
        }

        public void OnTriggerPosition(Vector3 triggerPressPosition)
        {

            drawingVariables.penTipPosition = triggerPressPosition;

            if (drawingVariables.isDrawing)
            {
                Annotation newAnnotation = PhotonView.Find(annotationManager.lastMadeAnnotationPhotonViewID).gameObject.GetComponent<Annotation>();

                Debug.LogFormat(GlobalVariables.blue + "Trigger press position = {0}" + GlobalVariables.endColor + " : onPrimaryButtonEvent()" + this.GetType(), triggerPressPosition);

                newAnnotation.AddPoint(triggerPressPosition);

                //tmp_Line_Render_Prefab.GetComponent<PhotonLineDrawing>().addPoint(triggerPressPosition);
            }
        }




    }
}