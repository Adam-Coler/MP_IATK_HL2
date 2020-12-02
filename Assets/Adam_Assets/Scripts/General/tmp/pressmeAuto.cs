using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Photon_IATK
{

    public class pressmeAuto : MonoBehaviour
    {

        [SerializeField]
        public Button ButtonToClick;

        public float SecondsToPress = 5.0f;

        private void Update()
        {
            if (Time.time > SecondsToPress)
            {
                ButtonToClick.Select();
                ButtonToClick.onClick.Invoke();
                Debug.Log(ButtonToClick.name);
            }
        }

    }
}