using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//These classes are a seralizeable form of the annotation class
namespace Photon_IATK
{

    [System.Serializable]
    public class SerializeableAnnotation
    {
        public string myVisXAxis;
        public string myVisYAxis;
        public string myVisZAxis;

        public float myLocalXPosition;
        public float myLocalYPosition;
        public float myLocalZPosition;

        public float myLocalXRotation;
        public float myLocalYRotation;
        public float myLocalZRotation;
        public float myLocalWRotation;

        public float myLocalScaleX;
        public float myLocalScaleY;
        public float myLocalScaleZ;

        public bool isDeleted = false;
        public string myAnnotationType;

        public int myAnnotationNumber;
        public float myCreationTime;
        public string myDataSource;
        public int myTimesMoved;

        public string myTextContent;
        public List<Vector3> myPoints;
    }
}
