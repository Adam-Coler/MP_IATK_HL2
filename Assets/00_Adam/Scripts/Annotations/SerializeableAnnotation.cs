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
        public string myTextContent;
        public int myAnnotationNumber;

        //type
        //content
        //time
        //datasource

        //line points?

        //moved?
    }
}
