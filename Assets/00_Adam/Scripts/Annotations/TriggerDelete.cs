using UnityEngine;


namespace Photon_IATK {
    public class TriggerDelete : MonoBehaviour
    {
        private void OnTriggerEnter(Collider collidedObject)
        {
            Debug.LogFormat(GlobalVariables.cOnDestory + "Initiating the request for deletaion of: {0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", collidedObject.gameObject.name, "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            GameObject annotationObj;
            if (findParentWithTag(out annotationObj, collidedObject.gameObject, GlobalVariables.annotationTag))
            {
                Annotation annotation = annotationObj.GetComponent<Annotation>();
                if (annotation != null)
                {
                    Debug.LogFormat(GlobalVariables.cOnDestory + "Requesting deletaion of: {0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", collidedObject.gameObject.name, "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                    annotation.RequestDelete();
                }
            }

        }

        private bool findParentWithTag(out GameObject returnedObject, GameObject startingGameObject, string tagToFind)
        {

            var parent = startingGameObject.transform.parent;

            while (parent != null)
            {
                if (parent.tag == tagToFind)
                {
                    returnedObject = parent.gameObject;
                    return true;
                        
                }
                parent = parent.transform.parent;
            }
            returnedObject = null;
            return false;
        }
    }
}
