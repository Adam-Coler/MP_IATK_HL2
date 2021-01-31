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
                trackerPrefab = Resources.Load<GameObject>("Tracker");
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


            newObj.transform.localScale = new Vector3(1f, 1f, 1f);

            randomizeAttributes(newObj);

            Annotation annotation = newObj.AddComponent<Annotation>();

            VisWrapperClass wrapperClass = GameObject.FindGameObjectWithTag("Vis").GetComponent<VisWrapperClass>();
            annotation.myAnnotationNumber = wrapperClass.getCountOfAnnotationsAndIncrement();
        }


        private void randomizeAttributes(GameObject obj)
        {
            float min = 0f;
            float max = 1.5f;

            obj.transform.Translate(new Vector3(Random.Range(min, max), Random.Range(min, max), Random.Range(min, max)));
            obj.transform.Rotate(new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
        }

        

        public void removeAnnotationsDummy()
        {
            countOfAnnotations = 0;
            var annotations = GameObject.FindGameObjectsWithTag("Annotation");

            Debug.LogFormat(GlobalVariables.cOnDestory + "Destorying {0} annotations" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", annotations.Length, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            foreach (GameObject annotation in annotations)
            {

                Debug.LogFormat(GlobalVariables.cOnDestory + "Destorying {0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", annotation.name, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                Destroy(annotation);
            }
        }
    }
}
