using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToTopCorner : MonoBehaviour
{
    // Start is called before the first frame update


    // Update is called once per frame
    public void MoveToTop()
    {
        GameObject parent = this.transform.parent.gameObject;
        
        Bounds bounds = getBounds();
        
        this.transform.position = bounds.center;
        this.transform.position = bounds.max + (this.transform.parent.forward * -(.5f * bounds.size.z) + (this.transform.parent.up * .05f));
    }

    private Bounds getBounds()
    {
        Collider collider = gameObject.GetComponentInParent<Collider>();
        if (collider != null)
        {
            Debug.Log("collider found");
            return collider.bounds;
        }
        Debug.Log("collider not found");
        return new Bounds();
    }

    //private void OnDrawGizmos()
    //{
    //    float radius = .01f;
    //    Gizmos.color = Color.red;
    //    Bounds bounds = getBounds();

    //    Gizmos.DrawWireSphere(bounds.center, radius);

    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawWireSphere(bounds.extents, radius);

    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(bounds.min, radius);

    //    Gizmos.color = Color.white;
    //    Gizmos.DrawWireSphere(bounds.max, radius);

    //    Vector3 topRight = bounds.max + (this.transform.parent.forward * -(.5f * bounds.size.z));
    //    Gizmos.color = Color.cyan;
    //    Gizmos.DrawWireSphere(topRight, radius);

    //    Gizmos.DrawWireSphere(Vector3.zero, radius);
    //}
}
