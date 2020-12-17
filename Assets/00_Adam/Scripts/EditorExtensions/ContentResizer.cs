using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Photon_IATK { 
    [ExecuteInEditMode]
    public class ContentResizer : MonoBehaviour
    {
        private RectTransform m_Rect;
        private RectTransform m_inputFieldRect;        
        public TMP_InputField tMP_InputField;


        private RectTransform rectTransform
        {
            get
            {
                if (m_Rect == null)
                    m_Rect = GetComponent<RectTransform>();
                return m_Rect;
            }
        }

        private RectTransform inputFieldTextRectTransform
        {
            get
            {
                if (m_inputFieldRect == null)
                    m_inputFieldRect = GetComponent<TMP_InputField>().textComponent.rectTransform;
                return m_inputFieldRect;
            }
        }

        private TMP_InputField _tMP_InputField
        {
            get
            {
                if (tMP_InputField == null)
                    tMP_InputField = GetComponent<TMP_InputField>();
                return tMP_InputField;
            }
        }

        private void Awake()
        {
            if (Application.isPlaying)
            {
                _tMP_InputField.onValueChanged.AddListener(delegate { this.setSize(); });
                Debug.Log(GlobalVariables.green + "Adding Listener for onValueChanged " + GlobalVariables.endColor + "Start()" + " : " + this.GetType());
                setSize();
            } 
        }

        private void OnApplicationQuit()
        {
            _tMP_InputField.onValueChanged.RemoveAllListeners();
            Debug.Log(GlobalVariables.green + "Removing all Listeners " + GlobalVariables.endColor + "OnApplicationQuit()" + " : " + this.GetType());
            setSize();
        }


        private void setSize()
        {
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, LayoutUtility.GetPreferredSize(inputFieldTextRectTransform, 1) + 5);
            inputFieldTextRectTransform.localPosition = Vector3.zero; // stops the text scrolling sideways - it doesn't need to
        }

#if UNITY_EDITOR
        // updates on called on scene change in editor
        public void Update()
        {
            Debug.Log(GlobalVariables.green + "Editor Mode: Updating Size " + GlobalVariables.endColor + "Update()" + " : " + this.GetType());
            setSize();
        }
#endif
    }

}

