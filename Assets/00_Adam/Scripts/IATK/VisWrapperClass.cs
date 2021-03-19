using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using IATK;
using System.Text.RegularExpressions;
using System.Linq;
using System.IO;

namespace Photon_IATK
{
    [ExecuteInEditMode]
    public class VisWrapperClass : Visualisation
    {
        public delegate void OnVisualisationUpdated(AbstractVisualisation.PropertyType propertyType);
        public static OnVisualisationUpdated visualisationUpdatedDelegate;

        public string[] loadedCSVHeaders;

        private Vector3 lastLocalScale;

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

        //private void Update()
        //{
        //    //if (lastLocalScale != this.transform.localScale)
        //    //{
        //    //    lastLocalScale = this.transform.localScale;
        //    //    updateVisPropertiesSafe(AbstractVisualisation.PropertyType.Scaling);
        //    //}
        //}

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

        private bool _isisTriggeringEvents;
        public bool isTriggeringEvents {
        get
            {
                return _isisTriggeringEvents;
            }
        set
            {
                _isisTriggeringEvents = value;
                if (value)
                {
                    UpdatedView(AbstractVisualisation.PropertyType.DimensionChange);

                    Debug.LogFormat(GlobalVariables.cEvent + "{0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Vis is now sending events", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                }
                else
                {
                    Debug.LogFormat(GlobalVariables.cEvent + "{0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Vis is not sending events", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                }
            }
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
            Debug.LogFormat(GlobalVariables.cRegister + "VisWrapper registering OnUpdateViewAction.{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            OnUpdateViewAction += UpdatedView;

            //this.transform.localScale = Vector3.one;
            lastLocalScale = this.transform.localScale;

            HelperFunctions.ParentInSharedPlayspaceAnchor(this.gameObject, System.Reflection.MethodBase.GetCurrentMethod());
        }

        private void OnDestroy()
        {
            Debug.LogFormat(GlobalVariables.cRegister + "Un-registering {0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "UpdatedView", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            OnUpdateViewAction -= UpdatedView;

            _deleteVisJsonFile();
        }

        private void _deleteVisJsonFile()
        {
            Debug.LogFormat(GlobalVariables.cOnDestory + "{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "Removing .json file for: ", this.uid, "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            string pathName = Application.streamingAssetsPath + Path.DirectorySeparatorChar + "SerializedFields";
            pathName += Path.DirectorySeparatorChar + this.uid + ".json";

            File.Delete(pathName);
        }

        private void UpdatedView(AbstractVisualisation.PropertyType propertyType)
        {
            if (_isisTriggeringEvents) 
            {
                if (visualisationUpdatedDelegate != null)
                    visualisationUpdatedDelegate(propertyType);
            }
            Debug.LogFormat(GlobalVariables.cEvent + "Vis updating but not sending events." + GlobalVariables.endColor + " {0}: {1} -> {2} -> {3}", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }

        public void updateVisPropertiesSafe(AbstractVisualisation.PropertyType propertyType = AbstractVisualisation.PropertyType.VisualisationType)
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
            UpdatedView(propertyType);

        }

    }
}
