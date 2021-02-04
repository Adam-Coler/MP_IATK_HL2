using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace Photon_IATK
{
    public class DESKTOP_Movement : MonoBehaviourPunCallbacks
    {

        private void Start()
        {
            this.gameObject.AddComponent<UnityEngine.EventSystems.EventSystem>();
        }

        // Update is called once per frame
        void Update()
        {
        if (Input.anyKey)
            {
                processMovement();
                moveToMouse();
            }
        }

        private void moveToMouse()
        {

            var mousePos =  Input.mousePosition;
            var playerPos =  Camera.main.WorldToScreenPoint(transform.position);

            var dir = mousePos - playerPos;

            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.AngleAxis(-angle, Vector3.up);

            Debug.LogFormat(GlobalVariables.cRegister + "Mouse moved, Position on screen: {0}, Mouse position: {1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", angle, dir, "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
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

            Debug.LogFormat(GlobalVariables.cRegister + "Input found, X:{0}, Y:{1}, Z:{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", x, y, z, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());


            this.gameObject.transform.position = new Vector3(x, y, z);
            Camera.main.transform.position = this.gameObject.transform.position;
            Camera.main.transform.rotation = this.gameObject.transform.rotation;
        }
    }
}
