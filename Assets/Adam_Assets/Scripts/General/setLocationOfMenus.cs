using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Photon_IATK
{
    public class setLocationOfMenus : MonoBehaviour
    {
        Camera camera;
        Transform this_transform;
        Transform camera_transform;

        public Vector3 cameraOffset = Vector3.zero;
        float smoothSpeed = 1f;

        // Start is called before the first frame update
        void Start()
        {
            camera = Camera.allCameras[0];
            this_transform = this.gameObject.transform;
            camera_transform = camera.transform;
        }

        // Update is called once per frame
        void Update()
        {
            this_transform.position = camera_transform.position + cameraOffset;
            //this_transform.position = Vector3.Lerp(this_transform.position, camera_transform.position + camera_transform.TransformVector(cameraOffset), smoothSpeed * Time.deltaTime);
            this_transform.rotation = Quaternion.LookRotation(camera_transform.forward, camera_transform.up);
        }
    }
}
