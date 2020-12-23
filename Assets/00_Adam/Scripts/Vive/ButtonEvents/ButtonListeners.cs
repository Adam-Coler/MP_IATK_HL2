using UnityEngine;
using Logitech.XRToolkit.Actions;

namespace Photon_IATK
{
    public class ButtonListeners : MonoBehaviour
    {
        public PrimaryButtonWatcher primaryWatcher;
        public TriggerButtonWatcher triggerWatcher;

        private LineDrawing _lineDrawing;

        private DrawingVariables drawingVariables;

        void Awake()
        {

            _lineDrawing = this.gameObject.AddComponent<LineDrawing>();

            drawingVariables = DrawingVariables.Instance;


            primaryWatcher = GameObject.FindObjectOfType<PrimaryButtonWatcher>();

            if (primaryWatcher == null)
            {
                Debug.LogFormat(GlobalVariables.red + "No PrimaryButtonWatcher found" + GlobalVariables.endColor + " : Awake()" + this.GetType());
            }

            triggerWatcher = GameObject.FindObjectOfType<TriggerButtonWatcher>();

            if (triggerWatcher == null)
            {
                Debug.LogFormat(GlobalVariables.red + "No TriggerButtonWatcher found" + GlobalVariables.endColor + " : Awake()" + this.GetType());
            }

            triggerWatcher.TriggerButtonPress.AddListener(onTriggerPress);
            triggerWatcher.TriggerButtonPressForce.AddListener(onTriggerPressForce);
            triggerWatcher.triggerPressedLocation.AddListener(OnTriggerPosition);
            primaryWatcher.primaryButtonPress.AddListener(onPrimaryButtonEvent);


            Debug.LogFormat(GlobalVariables.blue + "Listeners started" + GlobalVariables.endColor + " : Start()" + this.GetType());
        }

        public void onPrimaryButtonEvent(bool pressed)
        {
            Debug.LogFormat(GlobalVariables.blue + "Primary button pressed = {0}" + GlobalVariables.endColor + " : onPrimaryButtonEvent()" + this.GetType(), pressed);
        }

        public void onTriggerPress(bool pressed)
        {
            Debug.LogFormat(GlobalVariables.blue + "Trigger button pressed = {0}" + GlobalVariables.endColor + " : onTriggerPress()" + this.GetType(), pressed);

            drawingVariables.isDrawing = pressed;
        }

        public void onTriggerPressForce(float force)
        {
            Debug.LogFormat(GlobalVariables.blue + "Trigger button force = {0}" + GlobalVariables.endColor + " : onTriggerPressForce()" + this.GetType(), force);

            drawingVariables.lineWidthFromButtonForce = force;
        }

        public void OnTriggerPosition(Vector3 triggerPressPosition)
        {
            Debug.LogFormat(GlobalVariables.blue + "Trigger press position = {0}" + GlobalVariables.endColor + " : onPrimaryButtonEvent()" + this.GetType(), triggerPressPosition);

            drawingVariables.penTipPosition = triggerPressPosition;
        }

    }
}