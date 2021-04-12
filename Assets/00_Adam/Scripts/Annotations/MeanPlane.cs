using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Photon_IATK
{
    public class MeanPlane : MonoBehaviour
    {
        public GameObject plane;
        public enum axisSelection { xAxis, yAxis, zAxis };
        public axisSelection currentAxis = axisSelection.zAxis;



        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

