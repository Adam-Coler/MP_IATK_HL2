using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Photon_IATK
{
    public class ToggleCheckFaceUser : MonoBehaviour
    {
        public FaceUser[] faceUsers;

        private void Awake()
        {
            if (faceUsers == null)
            {
                faceUsers = this.GetComponentsInChildren<FaceUser>();
            }

            if (faceUsers.Length == 0) { Destroy(this); }
        }
        public void FaceUserOff()
        {
            foreach (FaceUser faceUser in faceUsers)
            {
                faceUser.isFaceUser = false;
            }
        }

        public void FaceUserOn()
        {
            foreach (FaceUser faceUser in faceUsers)
            {
                faceUser.isFaceUser = true;
            }
        }

        public void ToggleFaceUser()
        {
            foreach (FaceUser faceUser in faceUsers)
            {
                faceUser.toggleFaceUser();
            }
        }
    }
}
