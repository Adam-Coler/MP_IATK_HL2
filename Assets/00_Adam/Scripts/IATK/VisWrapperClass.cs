using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using IATK;
using System.Text.RegularExpressions;
using System.Linq;

namespace Photon_IATK
{
    [ExecuteInEditMode]
    public class VisWrapperClass : Visualisation
    {
        public delegate void OnVisualisationUpdated(AbstractVisualisation.PropertyType propertyType);
        public static OnVisualisationUpdated visualisationUpdatedDelegate;

        public string[] loadedCSVHeaders;

        private CSVDataSource _wrapperCSVDataSource;
        public CSVDataSource wrapperCSVDataSource
        {
            get
            {
                return _wrapperCSVDataSource;
            }

            set
            {
                _wrapperCSVDataSource = value;
                dataSource = _wrapperCSVDataSource;
                updateHeaders();
            }
        }

        public string axisKey
        {
            get
            {
                string axisID = "";
                string xAxis = getCleanString(this.xDimension.Attribute);
                string yAxis = getCleanString(this.yDimension.Attribute);
                string zAxis = getCleanString(this.zDimension.Attribute);                
                axisID = xAxis + "_" + yAxis + "_" + zAxis;
                return axisID;
            }
        }

        private string getCleanString(string str)
        {
            str = Regex.Replace(str, "[^a-zA-Z0-9 ]", string.Empty).ToLower();
            List<string> strList = str.Split(' ').Distinct<string>().ToList<string>();
            string resturnStr = "";
            foreach (string item in strList)
            {
                if (item.Length > 0)
                {
                    resturnStr += item[0].ToString();
                }
            }
            return resturnStr;
        }

        private void updateHeaders()
        {
            Debug.LogFormat(GlobalVariables.cCommon + "{0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "New wrapperCSVDataSource loaded, setting headers", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            loadedCSVHeaders = new string[wrapperCSVDataSource.DimensionCount];
            for (int i = 0; i < wrapperCSVDataSource.DimensionCount; i++)
            {
                loadedCSVHeaders[i] = wrapperCSVDataSource[i].Identifier;
            }
        }

        private void Awake()
        {
            Debug.LogFormat(GlobalVariables.cRegister + "VisWrapper registering OnUpdateViewAction." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            OnUpdateViewAction += UpdatedView;
        }

        private void OnDestroy()
        {
            Debug.LogFormat(GlobalVariables.cRegister + "Un-registering {0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "UpdatedView", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            OnUpdateViewAction -= UpdatedView;
        }


        private void UpdatedView(AbstractVisualisation.PropertyType propertyType)
        {
            if (visualisationUpdatedDelegate != null)
                visualisationUpdatedDelegate(propertyType);
        }

        public void updateVisPropertiesSafe()
        {
            AbstractVisualisation theVisObject = this.theVisualizationObject;

            if (theVisObject == null)
            {
                Debug.LogFormat(GlobalVariables.cError + "The Visualisation obect is null." + GlobalVariables.endColor + " {0}: {1} -> {2} -> {3}", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                return;
            }

            if (theVisObject.X_AXIS == null || theVisObject.Y_AXIS == null || theVisObject.Z_AXIS == null)
            {
                Debug.LogFormat(GlobalVariables.cError + "The Visualisation is missing an axis. X == null: " + (theVisObject.X_AXIS == null) + " , Y == null: " + (theVisObject.Y_AXIS == null) + " , Z == null: " + (theVisObject.Z_AXIS == null) + GlobalVariables.endColor + " {0}: {1} -> {2} -> {3}", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                return;
            }

            updateProperties();
            visualisationUpdatedDelegate(AbstractVisualisation.PropertyType.VisualisationType);
        }

    }
}
