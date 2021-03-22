using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Photon_IATK
{
    [ExecuteInEditMode]
    public class TestDebugDrawGizmos : MonoBehaviour
    {
        public VisDataInterface visDataInterface;

        private void Start()
        {
            Debug.Log("Started test gizmos");
        }

        private void OnEnable()
        {
            Start();
        }
        private void OnDrawGizmos()
        {
            //Vector3 poisition = transform.position;

            //Vector3 normValue = visDataInterface.GetNormalizedPoint(poisition);

            //Vector3 worldPosition = visDataInterface.GetVisPointWorldLocation(normValue);

            //Vector3 closestX = visDataInterface.GetClosestPointOnAxis(1, poisition);

            //Vector3 closestY = visDataInterface.GetClosestPointOnAxis(2, poisition);

            //Vector3 closestZ = visDataInterface.GetClosestPointOnAxis(3, poisition);

            //Vector3 closestPoint = visDataInterface.GetClosestPoint(poisition);

            //float smallRadius = .015f;
            //float mediumRadius = .025f;
            //float largeRadius = .035f;

            //Gizmos.color = Color.red;
            //Gizmos.DrawWireSphere(closestX, smallRadius);

            //Gizmos.color = Color.green;
            //Gizmos.DrawWireSphere(closestY, mediumRadius);

            //Gizmos.color = Color.blue;
            //Gizmos.DrawWireSphere(closestZ, largeRadius);

            //Gizmos.color = Color.black;
            //Gizmos.DrawWireSphere(poisition, largeRadius);
            //Gizmos.DrawWireSphere(Vector3.zero, largeRadius);

            //Gizmos.color = Color.white;
            //Gizmos.DrawWireSphere(worldPosition, smallRadius);

            //Gizmos.color = Color.white;
            //Gizmos.DrawWireSphere(closestPoint, largeRadius);


        }
    }
}

//private void OnDrawGizmos()
//{
//    float radiusSmall = .015f;
//    float radiusMed = .025f;
//    float radiusBig = .035f;

//    foreach (Vector3 point in islastmesh)
//    {
//        //var point = csvItems[5];

//        Gizmos.color = Color.cyan;
//        Gizmos.DrawWireSphere(point, radiusSmall);

//    }


//    foreach (Vector3 point in getListOfWorldLocationPoints())
//    {
//        if (IsInsideMesh(point))
//        {
//            Gizmos.color = Color.blue;
//            Gizmos.DrawWireSphere(point, radiusSmall);
//        } else
//        {
//            Gizmos.color = Color.green;
//            Gizmos.DrawWireSphere(point, radiusSmall);
//        }
//    }

//}

