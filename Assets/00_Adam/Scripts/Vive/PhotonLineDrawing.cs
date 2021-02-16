using UnityEngine;
using Photon.Pun;

namespace Photon_IATK
{
    [DisallowMultipleComponent]
    public class PhotonLineDrawing : MonoBehaviour
    {
        public LineRenderer lineRenderer;
        public MeshCollider meshCollider;

        private Vector3 NewPoint;
        private Vector3 oldPoint;

        [Header("Line Settings")]
        [SerializeField]
        private bool _useAnalog = true;

        [SerializeField]
        private Material _lineMaterial;

        public float _maxLineWidth = .005f;
        private LineRenderer _currentLine = null;

        private void Awake()
        {
            Initalize();
        }

        public void Initalize()
        {
            _currentLine = lineRenderer;
            _currentLine.material = new Material(Shader.Find("Sprites/Default")); ;
            _currentLine.material.color = DrawingVariables.Instance.currentColor;
            _currentLine.widthMultiplier = .005f;
            _currentLine.positionCount = 0;
            _currentLine.useWorldSpace = false;
            _currentLine.startWidth = 0.005f;
            _currentLine.endWidth = 0.005f;

        }


        public void addPoint(Vector3 pointToAdd)
        {
            NewPoint = pointToAdd;
            //pointToAdd = this.transform.InverseTransformPoint(pointToAdd);
            if (NewPoint == oldPoint || NewPoint == Vector3.zero) { return; }

            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, pointToAdd);

            oldPoint = NewPoint;
        }

        public void Simplify(float tolerance = 0.005f)
        {
            lineRenderer.Simplify(tolerance);

            Debug.LogFormat(GlobalVariables.cCommon + "{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "LineAnnotation Set, Simplifying to tolerance: ", tolerance, "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }

        public Vector3[] GetPoints()
        {
            Vector3[] newPos = new Vector3[lineRenderer.positionCount];
            lineRenderer.GetPositions(newPos);
            return newPos;
        }

        public void AddPoints(Vector3[] points)
        {
            if (points.Length < 1)
            {
                Debug.LogFormat(GlobalVariables.cError + "{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "LineAnnotation Points list is too short: ", points.Length, " points.", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                return;
            }
            Debug.LogFormat(GlobalVariables.cCommon + "{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "LineAnnotation adding ", points.Length, " points.", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            lineRenderer.positionCount = points.Length - 1;
            lineRenderer.SetPositions(points);
        }

    }

}
