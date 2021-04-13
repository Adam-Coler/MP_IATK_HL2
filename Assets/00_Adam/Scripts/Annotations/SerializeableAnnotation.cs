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
        public string myVisColorDimension;
        public string myVisSizeDimension;

        public Vector3 myLocalPosition;
        public Quaternion myLocalRotation;
        public Vector3 myRelativeScale;

        public bool isDeleted = false;
        public string myAnnotationType;

        public int myAnnotationNumber;

        public string myDataSource;

        public string myTextContent;
        public Vector3[] myLineRenderPoints;

        public float myCreationTime;

        public List<float> myStartTimesofMoves;
        public List<float> myEndTimesofMoves;
        public List<Vector3> myLocations;
        public List<Quaternion> myRotations;
        public List<Vector3> myRelativeScales;

        public string axisSelection;
        public string summeryValueType;

        public bool wasLoaded;
    }
}
