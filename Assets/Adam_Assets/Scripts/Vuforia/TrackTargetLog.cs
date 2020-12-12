using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackTargetLog : MonoBehaviour
{
    // Start is called before the first frame update

    GameObject TrackedObj;
    bool isTracking = false;

    public void onTargetFound()
    {
        Debug.Log("Target Found" + this.GetType());
        TrackedObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        TrackedObj.transform.localScale = new Vector3(0.079f, 0.049f, 0.0849f);
        TrackedObj.transform.position = this.transform.position;
        TrackedObj.transform.rotation = this.transform.rotation;
        isTracking = true;

    }

    public void onTargetLost()
    {
        Debug.Log("Target Lost" + this.GetType());
        Destroy(TrackedObj);
        isTracking = false;
    }

    private void Update()
    {
        if (isTracking)
        {
            TrackedObj.transform.position = this.transform.position;
            TrackedObj.transform.rotation = this.transform.rotation;
        }
    }


}
