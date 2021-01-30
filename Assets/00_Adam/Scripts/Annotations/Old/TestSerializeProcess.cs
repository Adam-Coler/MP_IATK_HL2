﻿//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System.IO;

//namespace Photon_IATK {
//    public class TestSerializeProcess : MonoBehaviour
//    {
//        string folderName = "AnnotationJsons";

//        // Start is called before the first frame update
//        void Start()
//        {
//            Debug.LogFormat(GlobalVariables.cSerialize + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Loading Serialize test class", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
//        }

//        public void recordAnnotations()
//        {

//            string fileName = GameObject.FindGameObjectWithTag("Vis").GetComponent<VisWrapperClass>().axisID + ".json";

//            Debug.LogFormat(GlobalVariables.cSerialize + "Saving annotations as: {0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", fileName, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
//            string jsonStringToSave = getAnnotationsJsonString();

//            _saveAnnotations(folderName, fileName, jsonStringToSave);

//        }

//        public void loadAnnotations()
//        {
//            string fileName = "temp.json";
//            string filepath = Path.Combine(Application.persistentDataPath, folderName);
//            string fullSavePath = Path.Combine(filepath, fileName);

//            Annotations annotations = JsonUtility.FromJson<Annotations>(File.ReadAllText(fullSavePath));
//            foreach (annotation annotation in annotations.annotations)
//            {
//                Debug.Log(annotation.myID);
//            }
            
//        }

//        private string getAnnotationsJsonString()
//        {
//            Annotations annotations = new Annotations();
//            annotations.parentVisAxisKey = "test01";

//            List<annotation> listOfAnnotations = new List<annotation>();

//            var listOfAnnotationObejcts = GameObject.FindGameObjectsWithTag("Annotation");
//            foreach (GameObject obj in listOfAnnotationObejcts)
//            {
//                var annotationObj = obj.GetComponent<AbstractAnnotationClass>();
//                annotation thisAnnotation = new annotation();
//                listOfAnnotations.Add(thisAnnotation);
//            }

//            annotations.annotations = listOfAnnotations;


//            bool prettyPrint = false;
//#if UNITY_EDITOR
//            prettyPrint = true;
//#endif
//            return JsonUtility.ToJson(annotations, prettyPrint);
//        }

//        private void _saveAnnotations(string folderName, string fileName, string jsonStringToSave)
//        {
//            string filepath = Path.Combine(Application.persistentDataPath, folderName);

//            if (!Directory.Exists(filepath))
//            {
//                Debug.LogFormat(GlobalVariables.cFileOperations + "Makeing new folder named {0}, full path: {1} " + GlobalVariables.endColor + " {2}: {3} -> {4} -> {5}", folderName, filepath, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

//                Directory.CreateDirectory(filepath);
//            }
//            string fullSavePath = Path.Combine(filepath, fileName);
//            System.IO.File.WriteAllText(fullSavePath, jsonStringToSave);

//            Debug.LogFormat(GlobalVariables.cFileOperations + "Annotations saved for {0}, full path: {1} " + GlobalVariables.endColor + " {2}: {3} -> {4} -> {5}", "AxisID?", fullSavePath, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
//        }
//    }

//    //[System.Serializable]
//    //public class Annotations
//    //{
//    //    public string parentVisAxisKey;
//    //    public List<annotation> annotations = new List<annotation>();
//    //}

//    //[System.Serializable]
//    //public class annotation
//    //{
//    //    public string myVisXAxis = "";
//    //    public string myVisYAxis = "";
//    //    public string myVisZAxis = "";

//    //    public bool myWasDeleted = false;

//    //    public float myLocalXPosition = 0;
//    //    public float myLocalYPosition = 0;
//    //    public float myLocalZPosition = 0;

//    //    public float myLocalXRotation = 0;
//    //    public float myLocalYRotation = 0;
//    //    public float myLocalZRotation = 0;

//    //    //type
//    //    //content
//    //}

//}