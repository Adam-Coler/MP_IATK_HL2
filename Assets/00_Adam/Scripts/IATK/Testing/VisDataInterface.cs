using UnityEngine;
using IATK;

namespace Photon_IATK
{
    //[ExecuteInEditMode]
    public class VisDataInterface : MonoBehaviour
    {
        public GameObject vis;
        public VisWrapperClass visWrapperClass;
        public CSVDataSource csv;

        public Axis xAxis;
        public Axis yAxis;
        public Axis zAxis;

        public Vector3[] csvItems;

        public GameObject obj;

        private void OnEnable()
        {
            Debug.LogFormat(GlobalVariables.cRegister + "GenericTransformSync registering OnEvent, RPCvisualisationUpdatedDelegate.{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            VisualizationEvent_Calls.RPCvisualisationUpdatedDelegate += UpdatedView;
        }

        private void OnDisable()
        {
            Debug.LogFormat(GlobalVariables.cRegister + "GenericTransformSync unregistering OnEvent, RPCvisualisationUpdatedDelegate.{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            VisualizationEvent_Calls.RPCvisualisationUpdatedDelegate -= UpdatedView;
        }

        private void Awake()
        {
            Invoke("matchToCurrentVis", .5f);
        }

        private void UpdatedView(AbstractVisualisation.PropertyType propertyType)
        {
            matchToCurrentVis();
        }

        private void matchToCurrentVis()
        {
            if (!HelperFunctions.FindGameObjectOrMakeOneWithTag(GlobalVariables.visTag, out vis, false, System.Reflection.MethodBase.GetCurrentMethod()))
            {
                Debug.LogFormat(GlobalVariables.cError + "{0}." + GlobalVariables.cError + " {1}: {2} -> {3} -> {4}", "No Vis tags Found", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                return;
            }

            visWrapperClass = vis.GetComponent<VisWrapperClass>();
            if (visWrapperClass == null)
                Debug.LogFormat(GlobalVariables.cError + "{0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "visWrapperClass is null", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            DataSource dataSource = visWrapperClass.dataSource;
            if (dataSource == null)
                Debug.LogFormat(GlobalVariables.cError + "{0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "csvDataSource is null", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            csv = (CSVDataSource)dataSource;

            foreach (Axis axis in vis.GetComponentsInChildren<IATK.Axis>())
            {
                switch (axis.AxisDirection)
                {
                    case 1:
                        xAxis = axis;
                        break;
                    case 2:
                        yAxis = axis;
                        break;
                    case 3:
                        zAxis = axis;
                        break;
                }
            }

            csvItems = getListOfPoints();
        }

        public Vector3[] getListOfPoints()
        {
            Debug.Log(csv.DataCount);
            Vector3[] csvArrayOfDataPoints = new Vector3[csv.DataCount];
            for (int i = 0; i < csv.DataCount; i++)
            {
                float x = csv[xAxis.AttributeName].Data[i];
                float y = csv[yAxis.AttributeName].Data[i];
                float z = csv[zAxis.AttributeName].Data[i];
                csvArrayOfDataPoints[i] = new Vector3(x, y, z);
            }

            return csvArrayOfDataPoints;
        }

        public void GetPointsInRectangularCuboid(GameObject obj)
        {

        }

        //public int i = 17;
        private void OnDrawGizmos()
        {
            float radiusSmall = .015f;
            float radiusBig = .025f;

            Bounds bounds = obj.GetComponent<MeshRenderer>().bounds;
            Vector3 center = bounds.center;
            Vector3 extents = obj.transform.localScale / 2f;

            Debug.DrawRay(center, obj.transform.up, Color.green);
            Debug.DrawRay(center, obj.transform.forward, Color.blue);
            Debug.DrawRay(center, obj.transform.right, Color.red);


            Gizmos.DrawWireSphere(center, radiusSmall);

            Vector3 forwardTopRight = center + (obj.transform.up * extents.y) + (obj.transform.forward * extents.z) + (obj.transform.right * extents.x);
            Vector3 forwardTopLeft = center + (obj.transform.up * extents.y) + (obj.transform.forward * extents.z) + (-obj.transform.right * extents.x);
            Vector3 backwardTopRight = center + (obj.transform.up * extents.y) + (-obj.transform.forward * extents.z) + (obj.transform.right * extents.x);
            Vector3 backwardTopLeft = center + (obj.transform.up * extents.y) + (-obj.transform.forward * extents.z) + (-obj.transform.right * extents.x);

            Vector3 forwardBottomRight = center + (-obj.transform.up * extents.y) + (obj.transform.forward * extents.z) + (obj.transform.right * extents.x);
            Vector3 forwardBottomLeft = center + (-obj.transform.up * extents.y) + (obj.transform.forward * extents.z) + (-obj.transform.right * extents.x);
            Vector3 backwardBottomRight = center + (-obj.transform.up * extents.y) + (-obj.transform.forward * extents.z) + (obj.transform.right * extents.x);
            Vector3 backwardBottomLeft = center + (-obj.transform.up * extents.y) + (-obj.transform.forward * extents.z) + (-obj.transform.right * extents.x);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(forwardTopRight, radiusSmall);
            Gizmos.DrawWireSphere(forwardTopLeft, radiusSmall);
            Gizmos.DrawWireSphere(backwardTopRight, radiusSmall);
            Gizmos.DrawWireSphere(backwardTopLeft, radiusSmall);

            Gizmos.DrawWireSphere(forwardBottomRight, radiusSmall);
            Gizmos.DrawWireSphere(forwardBottomLeft, radiusSmall);
            Gizmos.DrawWireSphere(backwardBottomRight, radiusSmall);
            Gizmos.DrawWireSphere(backwardBottomLeft, radiusSmall);

            //Vector3 closestPoint = csvItems[testPoints[i]];

            MeshCollider mesh = obj.GetComponent<MeshCollider>();

            foreach (Vector3 point in csvItems)
            {
                //var point = csvItems[5];

                var worldLocation = GetVisPointWorldLocation(point);
                var closestPoint = bounds.ClosestPoint(point);


                //Gizmos.color = Color.red;
                //Gizmos.DrawWireSphere(worldLocation, radiusSmall);

                //Gizmos.color = Color.blue;
                //Gizmos.DrawWireSphere(closestPoint, radiusSmall);

                //Gizmos.color = Color.cyan;
                ////Gizmos.DrawWireSphere(closestPoin2, radiusSmall);

                //break;
                if (IsInsideMesh(point))
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(worldLocation, radiusBig);
                }
                else
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawWireSphere(worldLocation, radiusSmall);
                }
            }

        }
        private bool IsInsideMesh(Vector3 point)
        {
            RaycastHit[] _hitsUp = new RaycastHit[100];
            RaycastHit[] _hitsDown = new RaycastHit[100];

            Physics.queriesHitBackfaces = true;
            int hitsUp = Physics.RaycastNonAlloc(point, Vector3.up, _hitsUp);
            int hitsDown = Physics.RaycastNonAlloc(point, Vector3.down, _hitsDown);
            Physics.queriesHitBackfaces = false;
            for (var i = 0; i < hitsUp; i++)
                if (_hitsUp[i].normal.y > 0)
                    for (var j = 0; j < hitsDown; j++)
                        if (_hitsDown[j].normal.y < 0 && _hitsDown[j].collider == _hitsUp[i].collider)
                            return true;

            return false;
        }

        public Vector3 GetVisPointWorldLocation(Vector3 normalizedAxisValues)
        {
            float eps = .001f;
            if (normalizedAxisValues.x == 0)
            {
                normalizedAxisValues.x += eps;
            }
            if (normalizedAxisValues.x == 1)
            {
                normalizedAxisValues.x -= eps;
            }

            if (normalizedAxisValues.y == 0)
            {
                normalizedAxisValues.y += eps;
            }
            if (normalizedAxisValues.y == 1)
            {
                normalizedAxisValues.y -= eps;
            }

            if (normalizedAxisValues.z == 0)
            {
                normalizedAxisValues.z += eps;
            }
            if (normalizedAxisValues.z == 1)
            {
                normalizedAxisValues.z -= eps;
            }

            Vector3 closestPointWorldLocationX = Vector3.MoveTowards(xAxis.minNormaliserObject.position, xAxis.maxNormaliserObject.position, normalizedAxisValues.x * Vector3.Distance(xAxis.minNormaliserObject.position, xAxis.maxNormaliserObject.position));

            Vector3 closestPointWorldLocationY = Vector3.MoveTowards(yAxis.minNormaliserObject.position, yAxis.maxNormaliserObject.position, normalizedAxisValues.y * Vector3.Distance(yAxis.minNormaliserObject.position, yAxis.maxNormaliserObject.position));

            Vector3 closestPointWorldLocationZ = Vector3.MoveTowards(zAxis.minNormaliserObject.position, zAxis.maxNormaliserObject.position, normalizedAxisValues.z * Vector3.Distance(zAxis.minNormaliserObject.position, zAxis.maxNormaliserObject.position));

            Vector3 pointIndc = getIntersectionOfThreeAxis(closestPointWorldLocationX, closestPointWorldLocationY, closestPointWorldLocationZ);

            return pointIndc;
        }

        public Vector3 getIntersectionOfThreeAxis(Vector3 x, Vector3 y, Vector3 z)
        {

            Transform xAxisT = xAxis.maxNormaliserObject.transform;
            Transform yAxisT = yAxis.maxNormaliserObject.transform;
            Transform zAxisT = zAxis.maxNormaliserObject.transform;

            //First intersection
            Vector3 xyIntersection;
            ClosestPointsOnTwoLines(out xyIntersection, x, xAxisT.up, y, yAxisT.up);

            //perpendicular to first intersection
            var xySide1 = x - y;
            var xySide2 = xyIntersection - x;
            var xyPerpandicularDirection = Vector3.Cross(xySide1, xySide2);

            //Second Intersection
            Vector3 yzIntersection;
            ClosestPointsOnTwoLines(out yzIntersection, z, zAxisT.right, y, -yAxisT.right);

            //perpendicular to second intersection
            var yzSide1 = y - z;
            var yzSide2 = yzIntersection - z;
            var yzPerpandicularDirection = Vector3.Cross(yzSide1, yzSide2);

            //third intersection
            Vector3 xyzclosestPoint;
            ClosestPointsOnTwoLines(out xyzclosestPoint, xyIntersection, xyPerpandicularDirection, yzIntersection, yzPerpandicularDirection);

            return xyzclosestPoint;
        }

        public Vector3 closest1 = Vector3.zero;
        public Vector3 closest2 = Vector3.zero;
        public bool ClosestPointsOnTwoLines(out Vector3 closestPointLine, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
        {
            Vector3 closestPointLine1 = Vector3.zero;
            Vector3 closestPointLine2 = Vector3.zero;

            float a = Vector3.Dot(lineVec1, lineVec1);
            float b = Vector3.Dot(lineVec1, lineVec2);
            float e = Vector3.Dot(lineVec2, lineVec2);

            float d = a * e - b * b;

            //lines are not parallel
            if (d != 0.0f)
            {

                Vector3 r = linePoint1 - linePoint2;
                float c = Vector3.Dot(lineVec1, r);
                float f = Vector3.Dot(lineVec2, r);

                float s = (b * f - c * e) / d;
                float t = (a * f - c * b) / d;

                closestPointLine1 = linePoint1 + lineVec1 * s;
                closestPointLine2 = linePoint2 + lineVec2 * t;
                closestPointLine = (closestPointLine1 + closestPointLine2) / 2f;

                closest1 = closestPointLine1;
                closest2 = closestPointLine2;
                return true;
            }

            else
            {
                Debug.Log("ERROR Closest point fail");
                closestPointLine = Vector3.zero;
                return false;
            }
        }
    }
}
