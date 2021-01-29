using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;



//Saving is handled by saving a Json for each annotation
namespace Photon_IATK { 
    public class AnnotationManagerSaveLoadRPC : MonoBehaviour
    {
        public void saveAnnotations()
        {
            bool saveWasSuccessfull = false;

            //find all annotations and convert to serilizable annotation
            List<SerializeableAnnotation> listOfAnnotations = _getAllAnnotationsAndConvertToSerializeableAnnotations(out saveWasSuccessfull);
            if (!saveWasSuccessfull) { return; };

            //save with axis title
            _saveAnnotations(listOfAnnotations);

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

        private string _convertSerializableAnnotationsToJsonString(SerializeableAnnotation serializeableAnnotation, bool prettyPrint = false)
        {
#if UNITY_EDITOR
            prettyPrint = true;
#endif
            return JsonUtility.ToJson(serializeableAnnotation, prettyPrint);
        }

        private string _getFolderPath()
        {
            //Annotations are saved per VisState in a folder with the names of that vis axis
            string mainFolderName = GlobalVariables.annotationSaveFolder;
            string mainFolderPath = Path.Combine(Application.persistentDataPath, mainFolderName);
            //_checkAndMakeDirectory(mainFolderPath);

            string date = System.DateTime.Now.ToString("yyyyMMdd");
            string parentVisAxisKey = _getParentVisAxisKey();
            string subFolderName = date + "_" + parentVisAxisKey;
            string subfolderPath = Path.Combine(mainFolderPath, subFolderName);
            _checkAndMakeDirectory(subfolderPath);

            return subfolderPath;
        }

        private void _saveAnnotations(List<SerializeableAnnotation> listOfSerializeableAnnotations)
        {

            string subfolderPath = _getFolderPath();

            foreach (SerializeableAnnotation serializeableAnnotation in listOfSerializeableAnnotations)
            {
                string filename = serializeableAnnotation.myAnnotationNumber.ToString("D3");
                filename += "_" + serializeableAnnotation.myAnnotationType.ToString() + ".json";

                string jsonFormatAnnotion = _convertSerializableAnnotationsToJsonString(serializeableAnnotation);

                string fullFilePath = Path.Combine(subfolderPath, filename);
                Debug.LogFormat(GlobalVariables.cFileOperations + "Saving {0}, full path: {1} " + GlobalVariables.endColor + " {2}: {3} -> {4} -> {5}", filename, fullFilePath, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                System.IO.File.WriteAllText(fullFilePath, jsonFormatAnnotion);
            }

            Debug.LogFormat(GlobalVariables.cFileOperations + "Annotations saved for {0}, full path: {1} " + GlobalVariables.endColor + " {2}: {3} -> {4} -> {5}", _getParentVisAxisKey(), subfolderPath, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }

        private void _checkAndMakeDirectory(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Debug.LogFormat(GlobalVariables.cFileOperations + "Makeing new folder{0}, full path: {1} " + GlobalVariables.endColor + " {2}: {3} -> {4} -> {5}", "", directory, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                Directory.CreateDirectory(directory);
            }
        }

        private string _getParentVisAxisKey() {

            GameObject visGameObject;
            if (!HelperFunctions.FindGameObjectOrMakeOneWithTag(GlobalVariables.visTag, out visGameObject, false, System.Reflection.MethodBase.GetCurrentMethod())) { return "EmmulatedVisObject"; }

            VisualizationRPC_Calls visualizationRPC_Calls;
            if (!HelperFunctions.GetComponent<VisualizationRPC_Calls>(out visualizationRPC_Calls, System.Reflection.MethodBase.GetCurrentMethod())) { return "EmmulatedVisObject"; }

            return visualizationRPC_Calls.axisKey;
        }

        public void loadAnnotations()
        {
            //get file path
            string getFolderPath = _getFolderPath();

            string[] filePaths = Directory.GetFiles(getFolderPath, "*.json");

            Debug.LogFormat(GlobalVariables.cFileOperations + "{0} .json annotation records found in {1}, {2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", filePaths.Length, getFolderPath, "Loading annotations now.", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            foreach (string jsonPath in filePaths) {
                //Load file
                SerializeableAnnotation serializeableAnnotation = JsonUtility.FromJson<SerializeableAnnotation>(File.ReadAllText(jsonPath));

                //Don't load them if they were deleated
                if (serializeableAnnotation.isDeleted) { continue; }

                //Make the annotation holder game object
                GameObject annotationHolder = new GameObject(serializeableAnnotation.myAnnotationNumber + "_" + serializeableAnnotation.myAnnotationType);

                //add the Annotation class
                Annotation annotation = annotationHolder.AddComponent<Annotation>();

                //add the loaded info from the serialized annotation to the actual annotation
                annotation.setUpFromSerializeableAnnotation(serializeableAnnotation);
            } 

            //loop each file

        }
    }
}
