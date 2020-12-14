using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class DisableEnableVuforia : MonoBehaviour
{

    public bool startWithVuforia = false;

    void Start()
    {
        VuforiaBehaviour.Instance.enabled = startWithVuforia;

        if (startWithVuforia)
        {
            CameraDevice.Instance.Start();
        } else
        {
            CameraDevice.Instance.Stop();
        }

    }




}
