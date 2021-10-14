using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Photon_IATK
{
    public class HighlightScript : MonoBehaviour
    {
        public VisDataInterface visDataInterface;
        public Collider mesh;
        public GameObject highlightSphereCollection;
        Material newMat;
        public TMPro.TextMeshPro countText;
        private GameObject text;


        // Start is called before the first frame update
        private void Awake()
        {
            GameObject vis;
            if (!HelperFunctions.FindGameObjectOrMakeOneWithTag(GlobalVariables.visTag, out vis, false, System.Reflection.MethodBase.GetCurrentMethod()))
            {
                Debug.LogFormat(GlobalVariables.cError + "No Vis tag object found.{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                return;
            }

            if (!HelperFunctions.FindGameObjectOrMakeOneWithTag(GlobalVariables.HighlightSphereCollection, out highlightSphereCollection, false, System.Reflection.MethodBase.GetCurrentMethod()))
            {
                Debug.LogFormat(GlobalVariables.cError + "No HighlightSphereCollection tag object found.{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                return;
            }

            if (!HelperFunctions.GetComponent<VisDataInterface>(out visDataInterface, System.Reflection.MethodBase.GetCurrentMethod()))
            {
                Debug.LogFormat(GlobalVariables.cError + "No VisDataInterface found.{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                return;
            }

            mesh = GetComponentInParent<Collider>();
            if (mesh == null)
            {
                Debug.LogFormat(GlobalVariables.cError + "No Collider found.{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                return;
            }

            text = (GameObject)Resources.Load("MainText");
            text = Instantiate(text);
            text.transform.parent = highlightSphereCollection.transform;

            text.TryGetComponent<TMPro.TextMeshPro>(out countText);

            newMat = Resources.Load("Highlight", typeof(Material)) as Material;
            DrawnSpheres = new List<GameObject>();
        }

        private void Update()
        {
            if (transform.hasChanged)
            {
                transform.hasChanged = false;
                drawEncapsalatedPoints();
            }
        }

        private Vector3 scale = new Vector3(.025f,.025f, .025f);
        List<GameObject> DrawnSpheres;
        private void drawEncapsalatedPoints()
        {
            //Debug.LogFormat(GlobalVariables.cCommon + "Drawing encapsalated points.{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            List<Vector3> encapsalatedPoints = visDataInterface.IsInsideMesh(mesh);
            

            var tmp = new List<GameObject>(DrawnSpheres);
            foreach (GameObject obj in tmp)
            {
                if (!encapsalatedPoints.Contains(obj.transform.position))
                {
                    DrawnSpheres.Remove(obj);
                    Destroy(obj);
                } else
                {
                    encapsalatedPoints.Remove(obj.transform.position);
                }
            }

            foreach (Vector3 point in encapsalatedPoints.Distinct())
            {
                //Debug.LogFormat(GlobalVariables.cCommon + "Generating Sphere.{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                GameObject newSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                newSphere.transform.position = point;
                newSphere.transform.localScale = scale;
                newSphere.transform.parent = highlightSphereCollection.transform;
                newSphere.GetComponent<Renderer>().material = newMat;
                Destroy(newSphere.GetComponent<Collider>());
                DrawnSpheres.Add(newSphere);
            }

            if (countText != null)
            {
                float largestScale = Mathf.Max(transform.localScale.x, transform.localScale.y, transform.localScale.z);
                countText.text = DrawnSpheres.Count.ToString() + " points highlighted";
                Vector3 pos = transform.position + (largestScale / 1.5f) * Vector3.up;

                Collider collider;

                if (this.gameObject.TryGetComponent<Collider>(out collider))
                {
                    countText.transform.position = collider.ClosestPoint(pos) + .025f * Vector3.up;
                }


            }
        }


        private void OnDestroy()
        {
            foreach (GameObject obj in DrawnSpheres)
            {
                    Destroy(obj);
            }

            Destroy(text);
        }
    }
}
