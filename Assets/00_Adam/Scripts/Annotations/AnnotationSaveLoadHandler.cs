using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;



//Saving is handled by saving a Json for each annotation
namespace Photon_IATK { 
    public class AnnotationSaveLoadHandler : MonoBehaviour
    {
        public void saveAnnotations()
        {
            bool saveWasSuccessfull = false;

            //find all annotations and convert to serilizable annotation
            List<SerializeableAnnotation> listOfAnnotations = _getAllAnnotationsAndConvertToSerializeableAnnotations(out saveWasSuccessfull);
            if (!saveWasSuccessfull) { return; };

            //convert to a json string
            string jsonSerializedAnnotationCollection = _convertListOfAnnotationsToJsonString(listOfAnnotations);

            //save with axis title
            _saveAnnotations(jsonSerializedAnnotationCollection);

            Debug.LogFormat(GlobalVariables.cCommon + "{0} {1} {2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "Annotations uccessfully saved", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

        }

        private List<SerializeableAnnotation> _getAllAnnotationsAndConvertToSerializeableAnnotations(out bool wasSuccessfull)
            {
            int countOfAnnotationsFound = 0;

            List<SerializeableAnnotation> listOfAnnotations = new List<SerializeableAnnotation>();
            GameObject[] annotationHolderObjects = GameObject.FindGameObjectsWithTag("Annotation");

            if (annotationHolderObjects.Length == 0)
            {
                Debug.LogFormat(GlobalVariables.cAlert + "{0} {1} {2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "No annotation holders found. Nothing saved", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                wasSuccessfull = false;
            }
            else
            {
                foreach (GameObject annotationHolder in annotationHolderObjects)
                {
                    Annotation annotation = annotationHolder.GetComponent<Annotation>();
                    if (annotation != null)
                    {
                        listOfAnnotations.Add(annotation.getSerializeableAnnotation());
                        countOfAnnotationsFound++;
                    }
                }
            }

            Debug.LogFormat(GlobalVariables.cCommon + "{0} {1} {2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", countOfAnnotationsFound, " Annotations Found.", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            wasSuccessfull = true;
            return listOfAnnotations;
        }

        private string _convertListOfAnnotationsToJsonString(List<SerializeableAnnotation> listOfAnnotations, bool prettyPrint = false)
        {

            SerializableAnnotationCollection serializableAnnotationCollection = new SerializableAnnotationCollection();

            serializableAnnotationCollection.annotations = listOfAnnotations;
            serializableAnnotationCollection.parentVisAxisKey = _parentVisAxisKey();


#if UNITY_EDITOR
            prettyPrint = true;
#endif
            return JsonUtility.ToJson(serializableAnnotationCollection, prettyPrint);
        }

        private void _saveAnnotations(string jsonStringToSave)
        {

            string folderName = GlobalVariables.annotationSaveFolder;
            string fileName = _parentVisAxisKey() + ".json";

            string filepath = Path.Combine(Application.persistentDataPath, folderName);

            if (!Directory.Exists(filepath))
            {
                Debug.LogFormat(GlobalVariables.cFileOperations + "Makeing new folder named {0}, full path: {1} " + GlobalVariables.endColor + " {2}: {3} -> {4} -> {5}", folderName, filepath, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                Directory.CreateDirectory(filepath);
            }
            string fullSavePath = Path.Combine(filepath, fileName);
            System.IO.File.WriteAllText(fullSavePath, jsonStringToSave);

            Debug.LogFormat(GlobalVariables.cFileOperations + "Annotations saved for {0}, full path: {1} " + GlobalVariables.endColor + " {2}: {3} -> {4} -> {5}", "AxisID?", fullSavePath, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }

        private string _parentVisAxisKey() {

            string visAxisKey = "";

            GameObject[] visGameObjects = GameObject.FindGameObjectsWithTag("Vis");

            if (visGameObjects.Length == 0)
            {
                Debug.LogFormat(GlobalVariables.cAlert + "{0} {1} {2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "No Vis objects found, Using EmmulatedVisKey.", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                visAxisKey = "EmmulatedVisObject";
            }
            else
            {
                var visGameObject = visGameObjects[0].GetComponent<VisWrapperClass>();
                if (visGameObject == null) {
                    visAxisKey = "EmmulatedVisObject";
                }
                else
                {
                    visAxisKey = visGameObject.axisKey;
                }
            }

            return visAxisKey;
        }

        public void loadAnnotations()
        {

        }
    }
}
