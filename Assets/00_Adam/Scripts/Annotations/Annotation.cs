using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

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
        public int myAnnotationNumber;

        public typesOfAnnotations myAnnotationType;

        public enum typesOfAnnotations {
            TEST_TRACKER
        }

        private bool isDeleted = false;

        private GameObject myVisParent;
        private GameObject myAnnotationCollectionParent;

        private void Awake()
        {
            Debug.LogFormat(GlobalVariables.cAlert + "{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "New annotation loaded", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            //attach to or make parents
            if (myVisParent == null || myAnnotationCollectionParent == null) { setupParentObjects(); }

            //set axis to that parent
            _setAxisNames();

            //setup the 
        }

        private void _setTrackerObject()
        {
            GameObject prefabGameObject;
            //this will add the visual representation to the annotation
            switch (myAnnotationType)
            {
                case typesOfAnnotations.TEST_TRACKER:

                    prefabGameObject = Resources.Load<GameObject>("Tracker");

                    break;
                default:
                    Debug.LogFormat(GlobalVariables.cAlert + "{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "Loading this annotation type is not supported or the type is null.", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                    return;
            }

            if (PhotonNetwork.IsConnectedAndReady)
            {
                prefabGameObject = PhotonNetwork.Instantiate(prefabGameObject.name, Vector3.zero, Quaternion.identity);
            }
            else
            {
                prefabGameObject = Instantiate(prefabGameObject, Vector3.zero, Quaternion.identity);
            }

            prefabGameObject.transform.parent = this.transform;
            prefabGameObject.transform.localPosition = Vector3.zero;
            prefabGameObject.transform.localRotation = Quaternion.identity;
        }

        private void _setAxisNames()
        {
            VisualizationRPC_Calls myParentsVisRPCClass = myVisParent.GetComponent<VisualizationRPC_Calls>();
            
            if (myParentsVisRPCClass == null)
            {
                myVisXAxis = "Fake X Axis Title";
                myVisYAxis = "Fake Y Axis Title";
                myVisZAxis = "Fake Z Axis Title";
            }
            else
            {
                myVisXAxis = myParentsVisRPCClass.xDimension;
                myVisYAxis = myParentsVisRPCClass.yDimension;
                myVisZAxis = myParentsVisRPCClass.zDimension;
            }
        }
        
        private void setupParentObjects()
        {
            HelperFunctions.FindGameObjectOrMakeOneWithTag(GlobalVariables.visTag, out myVisParent, true, System.Reflection.MethodBase.GetCurrentMethod());
            HelperFunctions.FindGameObjectOrMakeOneWithTag(GlobalVariables.annotationCollectionTag, out myAnnotationCollectionParent, true, System.Reflection.MethodBase.GetCurrentMethod());
        }

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
            serializeableAnnotation.myAnnotationNumber = myAnnotationNumber;

            serializeableAnnotation.myAnnotationType = myAnnotationType.ToString();

            return serializeableAnnotation;
        }
         
        public Annotation setUpFromSerializeableAnnotation(SerializeableAnnotation serializeableAnnotation)
        {
            Debug.LogFormat(GlobalVariables.cFileOperations + "{0}{1}" + GlobalVariables.endColor + " {2}: {3} -> {4} -> {5}", "Loading annotation from file", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            this.gameObject.tag = GlobalVariables.annotationTag;

            //Now we set up the annotation componenet
            isDeleted = serializeableAnnotation.isDeleted;
            myVisXAxis = serializeableAnnotation.myVisXAxis;
            myVisYAxis = serializeableAnnotation.myVisYAxis;
            myVisZAxis = serializeableAnnotation.myVisZAxis;
            myTextContent = serializeableAnnotation.myTextContent;
            myAnnotationNumber = serializeableAnnotation.myAnnotationNumber;

            myAnnotationType = (typesOfAnnotations)Enum.Parse(typeof(typesOfAnnotations), serializeableAnnotation.myAnnotationType, true);

            this.gameObject.transform.parent = myAnnotationCollectionParent.transform;

            _setTrackerObject();

            Vector3 localPosition = new Vector3(serializeableAnnotation.myLocalXPosition, serializeableAnnotation.myLocalYPosition, serializeableAnnotation.myLocalZPosition);
            this.gameObject.transform.localPosition = localPosition;

            Quaternion localRotation = new Quaternion(serializeableAnnotation.myLocalXRotation, serializeableAnnotation.myLocalYRotation, serializeableAnnotation.myLocalZRotation, serializeableAnnotation.myLocalWRotation);
            this.gameObject.transform.localRotation = localRotation;

            Vector3 localScale = new Vector3(serializeableAnnotation.myLocalScaleX, serializeableAnnotation.myLocalScaleY, serializeableAnnotation.myLocalScaleZ);
            this.gameObject.transform.localScale = localScale;

            return this;
        }
    }
}

