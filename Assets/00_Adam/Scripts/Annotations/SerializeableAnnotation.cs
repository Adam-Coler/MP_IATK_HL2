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

        public float myLocalXRotation;
        public float myLocalYRotation;
        public float myLocalZRotation;
        public float myLocalWRotation;

        public Vector3 myLocalPosition;
        public Quaternion myLocalRotation;
        public Vector3 myLocalScale;
        public Vector3 myRelativeScale;

        public bool isDeleted = false;
        public string myAnnotationType;

        public int myAnnotationNumber;
        public float myCreationTime;
        public string myDataSource;
        public int myTimesMoved;

        public string myTextContent;
    }
}
