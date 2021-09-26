using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Photon_IATK
{
    public class LockContent : MonoBehaviour
    {
        public Microsoft.MixedReality.Toolkit.UI.ObjectManipulator objectManipulator;

        private bool canFreezePos = true;

        private void Awake()
        {
            if (objectManipulator == null)
            {
                canFreezePos = this.TryGetComponent<Microsoft.MixedReality.Toolkit.UI.ObjectManipulator>(out objectManipulator);
            }
        }

        public void positionLock()
        {
            if (!canFreezePos) { return; }

            objectManipulator.enabled = !objectManipulator.enabled;
        }
    }
}
