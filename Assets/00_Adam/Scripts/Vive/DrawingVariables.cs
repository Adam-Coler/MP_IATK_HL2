using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Photon_IATK
{
    public class DrawingVariables : MonoBehaviour
    {

        public static DrawingVariables Instance;
        public Color currentColor = Color.red;
        public bool isDrawing = false;
        public Vector3 penTipPosition = Vector3.zero;
        public float lineWidthFromButtonForce = 0f;

        public int lineCount = 0;

        private void Start()
        {
            if (Instance == null)
            {
                Debug.Log(GlobalVariables.green + "Setting DrawingVariables.Instance " + GlobalVariables.endColor + " : " + "Awake()" + " : " + this.GetType());
                Instance = this;
            }
            else
            {
                if (Instance == this) return;

                Debug.Log(GlobalVariables.green + "Destroying then setting DrawingVariables.Instance " + GlobalVariables.endColor + " : " + "Awake()" + " : " + this.GetType());

                Destroy(Instance.gameObject);
                Instance = this;
            }
        }
    }
}

