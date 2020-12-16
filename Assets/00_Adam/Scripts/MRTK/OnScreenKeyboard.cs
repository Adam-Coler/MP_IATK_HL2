using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Photon_IATK {
    public class OnScreenKeyboard : MonoBehaviour
    {

        public TouchScreenKeyboard keyboard;

        // Start is called before the first frame update
        void Start()
        {

        }

        public void OpenSystemKeyboard()
        {
            //keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false);
            Debug.Log(GlobalVariables.green + "Opening System Keyboard, " + GlobalVariables.endColor + "OpenSystemKeyboard()" + " : " + this.GetType());
        }


        // Update is called once per frame
        void Update()
        {
            //if (keyboard != null)
            //{
            //    keyboardText = keyboard.text;
            //    // Do stuff with keyboardText
            //}
        }
    }
}