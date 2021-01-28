using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Photon_IATK
{
    [System.Serializable]
    public class AbstractAnnotationClass : MonoBehaviour
    {
        public string axisBasedID = "";
        public string myID = "";
    }
}

//public enum TypeofAnnotation
//{
//    DICTATION,
//    TYPED
//}

//public TypeofAnnotation myEnumType = TypeofAnnotation.DICTATION;
//public string content = "";

//public Vector3 localPosition = Vector3.zero;
//public Vector3 localRotation = Vector3.zero;


//public void logSelf()
//{
//    Debug.LogFormat(GlobalVariables.cSerialize + "axisBasedID: {0}, myEnumType: {1}, content: {2}, localPosition: {3}, localRotation: {4}{5}" + GlobalVariables.endColor + " {6}: {7} -> {8} -> {9}", axisBasedID, myEnumType.ToString(), content, localPosition, localRotation, "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
//}

//private void OnEnable()
//{
//    logSelf();
//}

//private void OnServerInitialized()
//{
//    Debug.LogFormat(GlobalVariables.cSerialize + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Serialized", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

//    logSelf();

//}
