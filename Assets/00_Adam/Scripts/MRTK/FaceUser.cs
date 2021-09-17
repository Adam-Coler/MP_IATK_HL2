using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceUser : MonoBehaviour
{
    public bool isFaceUser = true;

    // Update is called once per frame
    void Update()
    {
        if (isFaceUser)
        {
            //transform.LookAt(Camera.main.transform);
            Vector3 direction = transform.position - Camera.main.transform.position;
            transform.rotation = Quaternion.LookRotation(direction);
        }


    }
}
