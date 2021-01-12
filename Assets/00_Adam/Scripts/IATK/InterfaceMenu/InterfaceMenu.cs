using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using IATK;
using System.Reflection;
using UnityEngine.Events;
using System.Linq;

namespace Photon_IATK
{

    public class InterfaceMenu : MonoBehaviour
    {

        private Visualisation thisVisualisation;
        private CSVDataSource csvDataSource;

        public TMP_Dropdown xAxisDropdown;
        public TMP_Dropdown yAxisDropdown;
        public TMP_Dropdown zAxisDropdown;

        public TMP_Dropdown colorDimensionDropdown;
        public TMP_Dropdown sizeDimensionDropdown;

        private string[] csvDataHeaders;

        public delegate void UpdateViewAction(AbstractVisualisation.PropertyType propertyType);
        private UnityEvent m_MyEvent;
        // Start is called before the first frame update
        void Awake()
        {
            if (xAxisDropdown == null || yAxisDropdown == null || zAxisDropdown == null)
            {
                Debug.LogFormat(GlobalVariables.red + "One or more axis dropdown menus not found." + GlobalVariables.endColor + " {0}: {1} -> {2} -> {3}", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), MethodBase.GetCurrentMethod());
                return;
            }

            IATK.Visualisation.OnUpdateViewAction += findAndRegisterVisualisation;
        }

        public void updateVisPropertiesSafe()
        {
            AbstractVisualisation theVisObject = thisVisualisation.theVisualizationObject;

            if (theVisObject == null)
            {
                Debug.LogFormat(GlobalVariables.red + "The Visualisation obect is null." + GlobalVariables.endColor + " {0}: {1} -> {2} -> {3}", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), MethodBase.GetCurrentMethod());
                return;
            }

            if (theVisObject.X_AXIS == null || theVisObject.Y_AXIS == null || theVisObject.Z_AXIS == null)
            {
                Debug.LogFormat(GlobalVariables.red + "The Visualisation is missing an axis. X == null: " + (theVisObject.X_AXIS == null) + " , Y == null: " + (theVisObject.Y_AXIS == null) + " , Z == null: " + (theVisObject.Z_AXIS == null) + GlobalVariables.endColor + " {0}: {1} -> {2} -> {3}", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), MethodBase.GetCurrentMethod());
                return;
            }

            thisVisualisation.updateProperties();

        }

        public void changeXAxis()
        {
            thisVisualisation.xDimension = xAxisDropdown.options[xAxisDropdown.value].text;
            updateVisPropertiesSafe();
        }

        public void changeYAxis()
        {
            thisVisualisation.yDimension = yAxisDropdown.options[yAxisDropdown.value].text;
            updateVisPropertiesSafe();
        }

        public void changeZAxis()
        {
            thisVisualisation.zDimension = zAxisDropdown.options[zAxisDropdown.value].text;
            updateVisPropertiesSafe();
        }

        public void changeColorDimension()
        {
            thisVisualisation.dimensionColour = HelperFunctions.getColorGradient(Color.blue, Color.red);
            thisVisualisation.colourDimension = colorDimensionDropdown.options[colorDimensionDropdown.value].text;
            updateVisPropertiesSafe();
        }

        public void changeSizeDimension()
        {
            thisVisualisation.sizeDimension = sizeDimensionDropdown.options[sizeDimensionDropdown.value].text;
            updateVisPropertiesSafe();
        }

        public void findAndRegisterVisualisation(AbstractVisualisation.PropertyType propertyType)
        {
            //check if we have alread set everything up
            if (thisVisualisation != null && csvDataSource != null)
            {
                setupMenus();
                return;
            }

            Visualisation[] visualisations = GameObject.FindObjectsOfType<Visualisation>();  //GameObject.FindGameObjectsWithTag("Vis");
            CSVDataSource[] csvDataSources = GameObject.FindObjectsOfType<CSVDataSource>();

            if (visualisations == null)
            {
                Debug.LogFormat(GlobalVariables.red + "No visualisations found." + GlobalVariables.endColor + " {0}: {1} -> {2} -> {3}", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), MethodBase.GetCurrentMethod());
                return;
            }
            else
            {
                Debug.LogFormat(GlobalVariables.green + visualisations.Length + " visualisations found." + GlobalVariables.endColor + " {0}: {1} -> {2} -> {3}", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), MethodBase.GetCurrentMethod());
            }

            if (csvDataSources == null)
            {
                Debug.LogFormat(GlobalVariables.red + "No csvDataSources found." + GlobalVariables.endColor + " {0}: {1} -> {2} -> {3}", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), MethodBase.GetCurrentMethod());
                return;
            }
            else
            {
                Debug.LogFormat(GlobalVariables.green + visualisations.Length + " csvDataSources found." + GlobalVariables.endColor + " {0}: {1} -> {2} -> {3}", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), MethodBase.GetCurrentMethod());
            }

            GameObject visObj = visualisations[0].gameObject;
            GameObject csvObj = csvDataSources[0].gameObject;

            thisVisualisation = visObj.GetComponent<Visualisation>();
            csvDataSource = csvObj.GetComponent<CSVDataSource>();

            if (thisVisualisation == null || csvDataSource == null)
            {
                Debug.LogFormat(GlobalVariables.red + " thisVisualisation = " + (thisVisualisation != null) + " csvDataSource = " + (csvDataSource != null) + GlobalVariables.endColor + " {0}: {1} -> {2} -> {3}", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), MethodBase.GetCurrentMethod());
                return;
            }

            // in the future this needs to auto find which matches which
            if (thisVisualisation.dataSource != csvDataSource)
            {
                Debug.LogFormat(GlobalVariables.red + " The dataset does not match the visualisation " + GlobalVariables.endColor + " {0}: {1} -> {2} -> {3}", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), MethodBase.GetCurrentMethod());
                return;
            }

            setupMenus();
        }

        private void setupMenus()
        {
            string[] dataDimensionLabels;
            dataDimensionLabels = getCSVData();

            setAxisDropdowns(dataDimensionLabels, true);

            xAxisDropdown.value = xAxisDropdown.options.FindIndex(option => option.text == thisVisualisation.xDimension.Attribute);
            xAxisDropdown.RefreshShownValue();

            yAxisDropdown.value = yAxisDropdown.options.FindIndex(option => option.text == thisVisualisation.yDimension.Attribute);
            yAxisDropdown.RefreshShownValue();

            zAxisDropdown.value = zAxisDropdown.options.FindIndex(option => option.text == thisVisualisation.zDimension.Attribute);
            zAxisDropdown.RefreshShownValue();

            colorDimensionDropdown.value = colorDimensionDropdown.options.FindIndex(option => option.text == thisVisualisation.colourDimension);
            colorDimensionDropdown.RefreshShownValue();

            sizeDimensionDropdown.value = sizeDimensionDropdown.options.FindIndex(option => option.text == thisVisualisation.sizeDimension);
            sizeDimensionDropdown.RefreshShownValue();

        }

        private void clearDropdownOptions()
        {
            xAxisDropdown.ClearOptions();
            yAxisDropdown.ClearOptions();
            zAxisDropdown.ClearOptions();
            colorDimensionDropdown.ClearOptions();
            sizeDimensionDropdown.ClearOptions();
        }

        private string[] getCSVData()
        {
            if (csvDataHeaders != null)
            {
                return csvDataHeaders;
            }

            csvDataHeaders = new string[csvDataSource.DimensionCount];

            for (int i = 0; i < csvDataSource.DimensionCount; i++)
            {
                //Debug.LogFormat(GlobalVariables.purple + csvDataSource[i].Identifier + GlobalVariables.endColor + " {0}: {1} -> {2} -> {3} -> {4}", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), MethodBase.GetCurrentMethod(), csvDataSource.gameObject.name);

                //xAxisDropdown.options.Add(new TMP_Dropdown.OptionData() { text = csvDataSource[i].Identifier });

                csvDataHeaders[i] = csvDataSource[i].Identifier;
            }
            return csvDataHeaders;
        }

        private void setAxisDropdowns(string[] dataDimensions, bool isOptionsDivdedIntoThreeAxis = false)
        {
            clearDropdownOptions();

            List<TMP_Dropdown.OptionData> listDataDimensions = new List<TMP_Dropdown.OptionData>();

            foreach (string dimension in dataDimensions)
            {
                listDataDimensions.Add(new TMP_Dropdown.OptionData() { text = dimension });
            }

            if (isOptionsDivdedIntoThreeAxis)
            {
                List<List<TMP_Dropdown.OptionData>> partititionedList = Split(listDataDimensions, 3);

                xAxisDropdown.AddOptions(partititionedList[0]);
                yAxisDropdown.AddOptions(partititionedList[1]);
                zAxisDropdown.AddOptions(partititionedList[2]);

            }
            else
            {
                xAxisDropdown.AddOptions(listDataDimensions);
                yAxisDropdown.AddOptions(listDataDimensions);
                zAxisDropdown.AddOptions(listDataDimensions);
            }

            // add undefined as an option
            var listDataDimensionsWithUndefined = listDataDimensions;
            listDataDimensionsWithUndefined.Add(new TMP_Dropdown.OptionData() { text = "Undefined" });

            colorDimensionDropdown.AddOptions(listDataDimensionsWithUndefined);
            sizeDimensionDropdown.AddOptions(listDataDimensionsWithUndefined);
        }

        private void OnDestroy()
        {
            IATK.Visualisation.OnUpdateViewAction -= findAndRegisterVisualisation;
        }

        public static List<List<T>> Split<T>(List<T> collection, int size)
        {
            var chunks = new List<List<T>>();
            var chunkCount = collection.Count() / size;

            if (collection.Count % size > 0)
                chunkCount++;

            for (var i = 0; i < chunkCount; i++)
                chunks.Add(collection.Skip(i * size).Take(size).ToList());

            return chunks;
        }

    }
}
