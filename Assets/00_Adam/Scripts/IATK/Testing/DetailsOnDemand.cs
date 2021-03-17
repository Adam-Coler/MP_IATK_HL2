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
                setlabel(xAxis, xIndicator);
                setlabel(yAxis, yIndicator);
                setlabel(zAxis, zIndicator);
            }
        }

        private void setlabel(Axis axis, GameObject indicator)
        {
            DataSource.DimensionData.Metadata metaData = dataSource[axis.AttributeName].MetaData;
            Transform maxNormaliserTransform = axis.maxNormaliserObject;
            Transform minNormaliserTransform = axis.minNormaliserObject;

            switch (axis.AxisDirection)
            {
                case 1:
                    indicator.transform.position = new Vector3(this.transform.position.x, indicator.transform.position.y, indicator.transform.position.z);
                    break;
                case 2:
                    indicator.transform.position = new Vector3(indicator.transform.position.x, this.transform.position.y, indicator.transform.position.z);
                    break;
                case 3:
                    indicator.transform.position = new Vector3(indicator.transform.position.x, indicator.transform.position.y, this.transform.position.z);
                    break;
            }

            indicator.transform.rotation = axis.transform.rotation;
            indicator.transform.position = ClosestPoint(minNormaliserTransform.position, maxNormaliserTransform.position, this.transform.position);

            Vector3 minDelta = minNormaliserTransform.position - indicator.transform.position;
            float minDistance = Mathf.Sqrt(minDelta.x * minDelta.x + minDelta.y * minDelta.y + minDelta.z * minDelta.z);
            float axisValue = (metaData.maxValue - metaData.minValue) * minDistance + metaData.minValue;

            CSVDataSource csv = (CSVDataSource)dataSource;
            var normVal = csv.normaliseValue(axisValue, metaData.minValue, metaData.maxValue, 0f, 1f);
            var closestPointValue = csv.valueClosestTo(metaData.categories, normVal);

            var closestPointOriginalValue = dataSource.getOriginalValue(closestPointValue, axis.AttributeName);
            var closestPointOriginalValuePrecise = dataSource.getOriginalValuePrecise(closestPointValue, axis.AttributeName);

            var originalValue = dataSource.getOriginalValue(normVal, axis.AttributeName);
            var originalValuePrecise = dataSource.getOriginalValuePrecise(normVal, axis.AttributeName);

            Debug.LogFormat(GlobalVariables.cCommon + "normVal: {0}, closestPointValue: {1}, originalValue: {2}, originalValuePrecise: {3}, closestPointOriginalValue: {4}, closestPointOriginalValuePrecise: {5}, axisValue: {6}, {7}." + GlobalVariables.endColor + " {8}: {9} -> {10} -> {11}", normVal, closestPointValue, originalValue, originalValuePrecise, closestPointOriginalValue, closestPointOriginalValuePrecise, axisValue, "7", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

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