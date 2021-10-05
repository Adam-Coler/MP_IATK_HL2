using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLevelObject : MonoBehaviour
{
    //private void OnDrawGizmos()
    //{
    //    float radius = .05f;
    //    Gizmos.DrawCube(this.transform.position, new Vector3(.05f, .05f, .05f));

    //    Vector3 rightMove = this.transform.position;
    //    rightMove = this.transform.position + this.transform.localScale.x/2 * this.transform.right;
    //    Vector3 middle = rightMove + this.transform.localScale.z / 2 * this.transform.forward;

    //    Gizmos.DrawWireSphere(middle, radius);

    //    //float line = Vector3.Distance(this.transform.position, middle);
    //    //float angle = Vector3.Angle(this.transform.position, middle);

    //    radius = .035f;
    //    Gizmos.color = Color.red;
    //    Vector3 upMove = middle;
    //    upMove = this.transform.position + (middle.y - this.transform.position.y) * Vector3.up;

    //    //Vector3 backMove = backrightMove + this.transform.localScale.x / 2 * -Vector3.right;

    //    ////Quaternion rotation = Quaternion.Euler(0, this.transform.eulerAngles.y, 0);
    //    Gizmos.DrawWireSphere(upMove, radius);
    //    Gizmos.DrawLine(upMove, middle);
    //    //Gizmos.DrawWireSphere(backMove, radius);
    //}
}

//Vector3 rightMove = this.transform.position;
//rightMove = this.transform.position + this.transform.localScale.x / 2 * this.transform.right;
//Vector3 middle = rightMove + this.transform.localScale.z / 2 * this.transform.forward;

//Vector3 backrightMove = middle;
//backrightMove = middle + this.transform.localScale.z / 2 * -Vector3.forward;

//Vector3 backMove = backrightMove + this.transform.localScale.x / 2 * -Vector3.right;