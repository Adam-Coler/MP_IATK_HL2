using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Photon_IATK
{
    [DisallowMultipleComponent]
    public class UnParentReparent : MonoBehaviour
    {
        private Transform myParent;
        void Awake()
        {
            myParent = this.transform.parent;
        }

        public void deParent()
        {
            if (myParent == null) return;
            this.transform.parent = null;
        }

        public void reParent()
        {
            if (myParent == null) return;
            this.transform.parent = myParent;
        }

    }
}
