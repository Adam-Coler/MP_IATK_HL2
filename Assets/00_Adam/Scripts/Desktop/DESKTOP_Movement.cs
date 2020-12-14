using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace Photon_IATK
{
    public class DESKTOP_Movement : MonoBehaviourPunCallbacks
    {


        // Update is called once per frame
        void Update()
        {
        if (Input.anyKey & photonView.IsMine)
            {
                processMovement();
            }
        }

        private void processMovement()
        {
            float forwards;
            float backwards;
            float left;
            float right;
            float up;
            float down;
            float scale = .25f;

            if (Input.GetKey("w"))
            {
                forwards = 1;
            }
            else
            {
                forwards = 0;
            }


            if (Input.GetKey("s"))
            {
                backwards = -1;
            }
            else
            {
                backwards = 0;
            }


            if (Input.GetKey("a"))
            {
                left = -1;
            }
            else
            {
                left = 0;
            }


            if (Input.GetKey("d"))
            {
                right = 1;
            }
            else
            {
                right = 0;
            }


            if (Input.GetKey("left shift"))
            {
                up = 1;
            }
            else
            {
                up = 0;
            }


            if (Input.GetKey("space"))
            {
                down = -1;
            }
            else
            {
                down = 0;
            }

            float z = (forwards + backwards) * scale;
            float x = (left + right) * scale;
            float y = (up + down) * scale;


            x += this.gameObject.transform.position.x;
            y += this.gameObject.transform.position.y;
            z += this.gameObject.transform.position.z;


            this.gameObject.transform.position = new Vector3(x, y, z);
        }
    }
}
