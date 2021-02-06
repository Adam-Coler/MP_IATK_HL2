using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Photon_IATK
{
    public class TestAddRemoveAnnotations : MonoBehaviour
    {

        public int countOfAnnotations = 0;
        public GameObject trackerPrefab;

        private void Awake()
        {
            if (trackerPrefab == null)
            {
                trackerPrefab = Resources.Load<GameObject>("Photon_Tracker");
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

            GameObject tracker = Instantiate(trackerPrefab, Vector3.zero, Quaternion.identity);
            tracker.transform.parent = newObj.transform;
            tracker.transform.localPosition = Vector3.zero;
            tracker.transform.localRotation = Quaternion.identity;


            newObj.transform.localScale = new Vector3(1.6f, 1.6f, 1.6f);

            randomizeAttributes(newObj);

            Annotation annotation = newObj.AddComponent<Annotation>();

            VisWrapperClass wrapperClass = GameObject.FindGameObjectWithTag("Vis").GetComponent<VisWrapperClass>();
            annotation.myUniqueAnnotationNumber = wrapperClass.getCountOfAnnotationsAndIncrement();
        }


        private void randomizeAttributes(GameObject obj)
        {
            float min = 0f;
            float max = .75f;

            obj.transform.Translate(new Vector3(Random.Range(min, max), Random.Range(min, max), Random.Range(min, max)));
            obj.transform.Rotate(new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
        }

        

        public void removeAnnotationsDummy()
        {
            countOfAnnotations = 0;

            Debug.LogFormat(GlobalVariables.cOnDestory + "Destorying annotations, using EventManager" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            GeneralEventManager.instance.SendDeleteAllObjectsWithComponentRequest("Annotation");
        }
    }
}
