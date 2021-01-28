using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace Photon_IATK
{
    // This class will store the information relvevent to annotations in this sytem
    //as well as handing converting itself to and from the annotation serizliation class
    public class Annotation : MonoBehaviour
    {
        public string myVisXAxis;
        public string myVisYAxis;
        public string myVisZAxis;
        public string myTextContent;

        public typesOfAnnotations myAnnotationType;

        public enum typesOfAnnotations {
            TEST_TRACKER
        }

        private bool isDeleted = false;

        public SerializeableAnnotation getSerializeableAnnotation()
        {
            SerializeableAnnotation serializeableAnnotation = new SerializeableAnnotation();

            serializeableAnnotation.myLocalXPosition = this.transform.localPosition.x;
            serializeableAnnotation.myLocalYPosition = this.transform.localPosition.y;
            serializeableAnnotation.myLocalZPosition = this.transform.localPosition.z;

            serializeableAnnotation.myLocalXRotation = this.transform.localRotation.x;
            serializeableAnnotation.myLocalYRotation = this.transform.localRotation.y;
            serializeableAnnotation.myLocalZRotation = this.transform.localRotation.z;
            serializeableAnnotation.myLocalWRotation = this.transform.localRotation.w;

            serializeableAnnotation.myLocalScaleX = this.transform.localScale.x;
            serializeableAnnotation.myLocalScaleY = this.transform.localScale.y;
            serializeableAnnotation.myLocalScaleZ = this.transform.localScale.z;

            serializeableAnnotation.isDeleted = isDeleted;

            serializeableAnnotation.myVisXAxis = myVisXAxis;
            serializeableAnnotation.myVisYAxis = myVisYAxis;
            serializeableAnnotation.myVisZAxis = myVisZAxis;
            serializeableAnnotation.myTextContent = myTextContent;

            serializeableAnnotation.myAnnotationType = myAnnotationType.ToString();

            return serializeableAnnotation;
        }
         
        public Annotation loadFromSerializeableAnnotation(SerializeableAnnotation serializeableAnnotation)
        {
            GameObject annotationHolder = new GameObject("AnnotationHolder");
            attachToVisObject(annotationHolder);
            setObjectLocalTransformToZero(annotationHolder);

            //Now we set the transform of the clean gameObject that holds the annotation
            //Later we will add the actual visualized annotation that will pull its representation from this class

            Vector3 localPosition = new Vector3(serializeableAnnotation.myLocalXPosition, serializeableAnnotation.myLocalYPosition, serializeableAnnotation.myLocalZPosition);
            annotationHolder.transform.localPosition = localPosition;

            Quaternion localRotation = new Quaternion(serializeableAnnotation.myLocalXRotation, serializeableAnnotation.myLocalYRotation, serializeableAnnotation.myLocalZRotation, serializeableAnnotation.myLocalWRotation);
            annotationHolder.transform.localRotation = localRotation;

            Vector3 localScale = new Vector3(serializeableAnnotation.myLocalScaleX, serializeableAnnotation.myLocalScaleY, serializeableAnnotation.myLocalScaleZ);
            annotationHolder.transform.localScale = localScale;

            annotationHolder.tag = GlobalVariables.annotationTag;

            //Now we set up the annotation componenet
            Annotation thisAnnotation = annotationHolder.AddComponent<Annotation>();

            thisAnnotation.isDeleted = serializeableAnnotation.isDeleted;
            thisAnnotation.myVisXAxis = serializeableAnnotation.myVisXAxis;
            thisAnnotation.myVisYAxis = serializeableAnnotation.myVisYAxis;
            thisAnnotation.myVisZAxis = serializeableAnnotation.myVisZAxis;
            thisAnnotation.myTextContent = serializeableAnnotation.myTextContent;

            thisAnnotation.myAnnotationType = (typesOfAnnotations)Enum.Parse(typeof(typesOfAnnotations), serializeableAnnotation.myAnnotationType, true);

            return thisAnnotation;
        }

        public void attachToVisObject(GameObject annotationHolder)
        {
            GameObject[] visGameObjects = GameObject.FindGameObjectsWithTag("Vis");
            GameObject visGameObject;

            if (visGameObjects.Length == 0)
            {
                Debug.LogFormat(GlobalVariables.cAlert + "{0} {1} {2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "No Vis objects found, making an empty object at (0,0,0) and attaching annotation to it.", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                visGameObject = new GameObject("EmmulatedVisObject");
            }
            else
            {
                Debug.LogFormat(GlobalVariables.cCommon + "{0} {1} {2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", visGameObjects.Length, " Vis objects found. Attaching the annotation to the first one found.", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                visGameObject = visGameObjects[0];
            }

            annotationHolder.transform.parent = visGameObject.transform;
        }

        private void setObjectLocalTransformToZero(GameObject obj)
        {
            obj.transform.localScale = new Vector3 (1f, 1f, 1f);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
        }

    }
}

