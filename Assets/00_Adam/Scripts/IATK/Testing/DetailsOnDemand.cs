using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IATK;

namespace Photon_IATK
{

    public class DetailsOnDemand : MonoBehaviour
    {
        //public text
        private TMPro.TextMeshPro textMeshPro;
        private GameObject vis;

        private VisWrapperClass visWrapperClass;
        private DataSource dataSource;

        private Axis xAxis;
        private Axis yAxis;
        private Axis zAxis;

        public GameObject xIndicator;
        public GameObject yIndicator;
        public GameObject zIndicator;

        private void Awake()
        {
            if (!HelperFunctions.GetComponentInChild<TMPro.TextMeshPro>(out textMeshPro, gameObject, System.Reflection.MethodBase.GetCurrentMethod())){
                Debug.LogFormat(GlobalVariables.cError + "{0}." + GlobalVariables.cOnDestory + " {1}: {2} -> {3} -> {4}", "No TMP Pro Text Found, destroying self", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                Destroy(this);
            }

            if (!HelperFunctions.FindGameObjectOrMakeOneWithTag(GlobalVariables.visTag, out vis, false, System.Reflection.MethodBase.GetCurrentMethod()))
            {
                Debug.LogFormat(GlobalVariables.cError + "{0}." + GlobalVariables.cError + " {1}: {2} -> {3} -> {4}", "No Vis tags Found", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                return;
            }

            visWrapperClass = vis.GetComponent<VisWrapperClass>();
            if (visWrapperClass == null)
                Debug.LogFormat(GlobalVariables.cError + "{0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "visWrapperClass is null", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            dataSource = visWrapperClass.dataSource;
            if (dataSource == null)
                Debug.LogFormat(GlobalVariables.cError + "{0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "csvDataSource is null", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            foreach (Axis axis in vis.GetComponentsInChildren<IATK.Axis>())
            {
                switch (axis.AxisDirection){
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

            textMeshPro.text = xAxis.AttributeName;
        }

        private void Update()
        {
            if (transform.hasChanged)
            {
                transform.hasChanged = false;
                setXlabel();
            }
        }

        private void setXlabel()
        {
            //get data from csv


            DataSource.DimensionData.Metadata xMetaData = dataSource[xAxis.AttributeName].MetaData;
            Transform maxNormaliserTransform = xAxis.maxNormaliserObject;
            Transform minNormaliserTransform = xAxis.minNormaliserObject;

            HelperFunctions.getJson(xAxis, "xAxis");
            HelperFunctions.getJson(dataSource, "dataSource");
            HelperFunctions.getJson(dataSource[xAxis.AttributeName], "dataSource[xAxis.AttributeName]");
            HelperFunctions.getJson(xMetaData, "xMetaData");


            xIndicator.transform.position = new Vector3(this.transform.position.x, xIndicator.transform.position.y, xIndicator.transform.position.z);
            xIndicator.transform.rotation = xAxis.transform.rotation;
            xIndicator.transform.position = ClosestPoint(minNormaliserTransform.position, maxNormaliserTransform.position, this.transform.position);

            Vector3 minDelta = minNormaliserTransform.position - xIndicator.transform.position;
            float minDistance = Mathf.Sqrt(minDelta.x * minDelta.x + minDelta.y * minDelta.y + minDelta.z * minDelta.z);
            Debug.Log("Dist from Min: " + minDistance);

            Vector3 maxDelta = maxNormaliserTransform.position - xIndicator.transform.position;
            float maxDistance = Mathf.Sqrt(maxDelta.x * maxDelta.x + maxDelta.y * maxDelta.y + maxDelta.z * maxDelta.z);
            Debug.Log("Dist from Max: " + maxDistance);

            float xAxisValue = (xMetaData.maxValue - xMetaData.minValue) * minDistance + xMetaData.minValue;
            //float dist = DistanceLineSegmentPoint(minNormaliserTransform.position, maxNormaliserTransform.position, xIndicator.transform.position);
            Debug.Log("xAxisValue: " + xAxisValue);

            //CSVDataSource csv =  (CSVDataSource)dataSource;
            //var dict = csv.TextualDimensionsList;
            //foreach (var item in dict)
            //{
            //    Debug.Log("item: " + item.Key + " " + item.Value);
            //    foreach (var subItem in item.Value)
            //    {
            //        Debug.Log("subItem: " + subItem.Key + " " + subItem.Value);
            //    }
            //}

            var tmp = dataSource.getOriginalValue(Mathf.Round(xAxisValue), "State");
            
            CSVDataSource csv = (CSVDataSource)dataSource;
            var dict = csv.TextualDimensionsList;
            int itemIndex = (int)Mathf.Round(xAxisValue);
            Debug.Log(dict["State"][itemIndex]);
        }

        public Vector3 LerpByDistance(Vector3 A, Vector3 B, float x)
        {
            Vector3 P = x * Vector3.Normalize(B - A) + A;
            return P;
        }

        private Vector3 ClosestPoint(Vector3 limit1, Vector3 limit2, Vector3 point)
        {
            Vector3 lineVector = limit2 - limit1;

            float lineVectorSqrMag = lineVector.sqrMagnitude;

            // Trivial case where limit1 == limit2
            if (lineVectorSqrMag < 1e-3f)
                return limit1;

            float dotProduct = Vector3.Dot(lineVector, limit1 - point);

            float t = -dotProduct / lineVectorSqrMag;

            return limit1 + Mathf.Clamp01(t) * lineVector;
        }

    }
}

//Debug.LogFormat(GlobalVariables.cAlert + "xAxis.AttributeFilter.maxFilter: {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}." + GlobalVariables.endColor + " {8}: {9} -> {10} -> {11}", "0", "1", "2", "3", "4", "5", "6", "7", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());


//HelperFunctions.getJson(xAxis, "xAxis");
//HelperFunctions.getJson(dataSource, "dataSource");
//HelperFunctions.getJson(dataSource[xAxis.AttributeName], "dataSource[xAxis.AttributeName]");
//HelperFunctions.getJson(xMetaData, "xMetaData");