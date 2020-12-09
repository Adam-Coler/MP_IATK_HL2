using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Photon_IATK
{

    public class MP_InputSetup : MonoBehaviour
    {

        //[HideInInspector]
        //public static SteamVR_Behaviour_Pose PrimaryDeviceBehaviourPose;

        [SerializeField]
        public GameObject contollerPrefab;

        private GameObject controller;

        // Start is called before the first frame update
        void Start()
        {
            setup();
        }

        // Update is called once per frame
        void Update()
        {

        }

#if DESKTOP
        private void setup()
        {
            Debug.Log(GlobalVariables.green + "Setting up room inputs for DESKTOP" + GlobalVariables.endColor + " : " + this.GetType().Name);
            this.gameObject.AddComponent<EventSystem>();
            this.gameObject.AddComponent<StandaloneInputModule>();
        }

#elif HL2

        private void setup()
        {
            Debug.Log(GlobalVariables.green + "Setting up room inputs for HL2" + GlobalVariables.endColor + " : " + this.GetType().Name);
#if UNITY_EDITOR
            this.gameObject.AddComponent<EventSystem>();
            this.gameObject.AddComponent<StandaloneInputModule>();
#endif
        }

#elif VIVE



        private void setup()
        {
            Debug.Log(GlobalVariables.green + "Setting up room inputs for VIVE" + GlobalVariables.endColor + " : " + this.GetType().Name);

            controller = Instantiate(contollerPrefab, new Vector3(0, 0, 0), Quaternion.identity);

            //If we are in the editor using the mouse and keyboard is nice
#if UNITY_EDITOR
            this.gameObject.AddComponent<EventSystem>();
            this.gameObject.AddComponent<StandaloneInputModule>();
#endif

        }

        ////This will be removed when more than one button exists
        //private void TriggerPressed(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        //{
        //    GameObject[] buttonTagedObjects = GameObject.FindGameObjectsWithTag("Button");
        //    Debug.Log(buttonTagedObjects.Length);
        //    foreach (GameObject obj in buttonTagedObjects)
        //    {
        //        Button btn = obj.GetComponent<Button>();
        //        if (btn != null)
        //        {
        //            btn.Select();
        //            btn.onClick.Invoke();
        //            Debug.Log(btn.name);
        //        }
        //    }
        //    Debug.Log(fromSource);
        //}

#else
        private void setup()
        { }

#endif
        }

}

