using System.Collections.Generic;
using UnityEngine;
using System.IO;
using IATK;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;
using Photon.Compression;
using Photon.Utilities;
using System.Runtime.Serialization.Formatters.Binary;


namespace Photon_IATK
{
    public class TestAddRemoveAnnotations : MonoBehaviour
    {

        public int countOfAnnotations = 0;
        public GameObject annotationPrefab;

        private void Awake()
        {
            if (annotationPrefab == null)
            {
                annotationPrefab = Resources.Load<GameObject>("GenericAnnotation");
            }
        }

        public void makeAnnotationDummy()
        {
            countOfAnnotations += 1;

            Debug.LogFormat(GlobalVariables.cComponentAddition + "Creating an annotation number: {0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", countOfAnnotations, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            GameObject newObj = new GameObject("Annotation_" + countOfAnnotations);
            newObj.tag = "Annotation";
            newObj.transform.parent = GameObject.FindGameObjectWithTag("Vis").transform;
            newObj.transform.localPosition = Vector3.zero;
            newObj.transform.localRotation = Quaternion.identity;

            GameObject tracker = PhotonNetwork.InstantiateRoomObject(annotationPrefab.name, Vector3.zero, Quaternion.identity);
            tracker.transform.parent = newObj.transform;
            tracker.transform.localPosition = Vector3.zero;
            tracker.transform.localRotation = Quaternion.identity;

            newObj.transform.localScale = new Vector3(1f, 1f, 1f);

            Annotation annotation = newObj.AddComponent<Annotation>();

            VisWrapperClass wrapperClass = GameObject.FindGameObjectWithTag("Vis").GetComponent<VisWrapperClass>();
            annotation.myUniqueAnnotationNumber = wrapperClass.getCountOfAnnotationsAndIncrement();
        }




        

        public void removeAnnotationsDummy()
        {
            countOfAnnotations = 0;

            Debug.LogFormat(GlobalVariables.cOnDestory + "Destorying annotations, using EventManager" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            GeneralEventManager.instance.SendDeleteAllObjectsWithComponentRequest("Annotation");
        }
    }
}
