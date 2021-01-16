using System.Collections.Generic;
using UnityEngine;
using TMPro;
using IATK;
using System.Linq;
using System.Reflection;
using Photon.Pun;

namespace Photon_IATK
{

    public class InterfaceMenu : MonoBehaviour
    {

        private Visualisation thisVisualisation;
        private CSVDataSource csvDataSource;
        private InstanceVis instanceVis;
        private PhotonView photonView;
        private string visUID;

        //the interface menu will be linked to a single visualization, the calls will impact that visualization only

        public TMP_Dropdown xAxisDropdown;
        public TMP_Dropdown yAxisDropdown;
        public TMP_Dropdown zAxisDropdown;

        public TMP_Dropdown colorDimensionDropdown;
        public TMP_Dropdown sizeDimensionDropdown;

        public bool isOfflineVis = false;

        private string[] csvDataHeaders;

        public delegate void UpdateViewAction(AbstractVisualisation.PropertyType propertyType);
        //private UnityEvent m_MyEvent;
        // Start is called before the first frame update
        void Awake()
        {
            OnEnable();
        }

        private void OnEnable()
        {
            if (xAxisDropdown == null || yAxisDropdown == null || zAxisDropdown == null)
            {
                Debug.LogFormat(GlobalVariables.cError + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "One or more axis dropdown menus not found.", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                return;
            }

            IATK.Visualisation.OnUpdateViewAction += findAndRegisterVisualisation;

            //to get the intial values set
            findAndRegisterVisualisation(AbstractVisualisation.PropertyType.Colour);

            Debug.LogFormat(GlobalVariables.cRegister + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "IATK.Visualisation.OnUpdateViewAction += findAndRegisterVisualisation.", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }

        private void OnDisable()
        {

            Debug.LogFormat(GlobalVariables.cRegister + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "IATK.Visualisation.OnUpdateViewAction -= findAndRegisterVisualisation.", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            IATK.Visualisation.OnUpdateViewAction -= findAndRegisterVisualisation;
        }

        public void updateVisPropertiesSafe()
        {
            AbstractVisualisation theVisObject = thisVisualisation.theVisualizationObject;

            if (theVisObject == null)
            {
                Debug.LogFormat(GlobalVariables.cError + "The Visualisation obect is null." + GlobalVariables.endColor + " {0}: {1} -> {2} -> {3}", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), MethodBase.GetCurrentMethod());

                OnEnable();

                return;
            }

            if (theVisObject.X_AXIS == null || theVisObject.Y_AXIS == null || theVisObject.Z_AXIS == null)
            {
                Debug.LogFormat(GlobalVariables.cError + "The Visualisation is missing an axis. X == null: " + (theVisObject.X_AXIS == null) + " , Y == null: " + (theVisObject.Y_AXIS == null) + " , Z == null: " + (theVisObject.Z_AXIS == null) + GlobalVariables.endColor + " {0}: {1} -> {2} -> {3}", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), MethodBase.GetCurrentMethod());
                return;
            }

            thisVisualisation.updateProperties();

        }


        public void changeXAxis()
        {
            if (isOfflineVis)
            {
                thisVisualisation.xDimension = xAxisDropdown.options[xAxisDropdown.value].text;
                updateVisPropertiesSafe();
                return;
            } 
            else
            {
                photonView.RPC("changeXAxis", RpcTarget.All, xAxisDropdown.options[xAxisDropdown.value].text);
            }
        }

        public void changeYAxis()
        {
            if (isOfflineVis)
            {
                thisVisualisation.yDimension = yAxisDropdown.options[yAxisDropdown.value].text;
                updateVisPropertiesSafe();
                return;
            }
            else
            {
                photonView.RPC("changeYAxis", RpcTarget.All, yAxisDropdown.options[yAxisDropdown.value].text);
            }

        }

        public void changeZAxis()
        {
            if (isOfflineVis)
            {
                thisVisualisation.zDimension = zAxisDropdown.options[zAxisDropdown.value].text;
                updateVisPropertiesSafe();
                return;
            }
            else
            {
                photonView.RPC("changeZAxis", RpcTarget.All, zAxisDropdown.options[zAxisDropdown.value].text);
            }

        }

        public void changeColorDimension()
        {
            Color colorStart = Color.blue;
            Color colorEnd = Color.red;

            if (isOfflineVis)
            {
                thisVisualisation.dimensionColour = HelperFunctions.getColorGradient(colorStart, colorEnd);
                thisVisualisation.colourDimension = colorDimensionDropdown.options[colorDimensionDropdown.value].text;
                updateVisPropertiesSafe();
                return;
            }
            else
            {
                photonView.RPC("changeColorDimension", RpcTarget.All, colorDimensionDropdown.options[colorDimensionDropdown.value].text);
            }

        }

        public void changeSizeDimension()
        {
            if (isOfflineVis)
            {
                thisVisualisation.sizeDimension = sizeDimensionDropdown.options[sizeDimensionDropdown.value].text;
                updateVisPropertiesSafe();
                return;
            }
            else
            {
                photonView.RPC("changeSizeDimension", RpcTarget.All, sizeDimensionDropdown.options[sizeDimensionDropdown.value].text);
            }
        }

        public void findAndRegisterVisualisation(AbstractVisualisation.PropertyType propertyType)
        {

            InstanceVis[] instanceViss = GameObject.FindObjectsOfType<InstanceVis>();



            //check if we have alread set everything up
            if (thisVisualisation != null && csvDataSource != null && instanceVis != null)
            {
                // check if there are loaded visualisations
                if (instanceViss.Length > 0)
                {
                    //check if the loaded one is the same one
                    if (instanceViss[0].vis.uid == instanceVis.vis.uid)
                    {
                        return;
                    }
                }
                setupMenus();
            }

            Visualisation[] visualisations = GameObject.FindObjectsOfType<Visualisation>();  //GameObject.FindGameObjectsWithTag("Vis");
            CSVDataSource[] csvDataSources = GameObject.FindObjectsOfType<CSVDataSource>();

            if (visualisations.Length == 0)
            {
                Debug.LogFormat(GlobalVariables.cError + "No visualisations found." + GlobalVariables.endColor + " {0}: {1} -> {2} -> {3}", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), MethodBase.GetCurrentMethod());
                return;
            }
            else
            {
                Debug.LogFormat(GlobalVariables.cCommon + visualisations.Length + " visualisations found." + GlobalVariables.endColor + " {0}: {1} -> {2} -> {3}", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), MethodBase.GetCurrentMethod());
            }

            if (csvDataSources.Length == 0)
            {
                Debug.LogFormat(GlobalVariables.cError + "No csvDataSources found." + GlobalVariables.endColor + " {0}: {1} -> {2} -> {3}", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), MethodBase.GetCurrentMethod());
                return;
            }
            else
            {
                Debug.LogFormat(GlobalVariables.cCommon + csvDataSources.Length + " csvDataSources found." + GlobalVariables.endColor + " {0}: {1} -> {2} -> {3}", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), MethodBase.GetCurrentMethod());
            }

            if (instanceViss.Length == 0)
            {
                Debug.LogFormat(GlobalVariables.cError + "No instanceVis found, menu will not use rpc calls." + GlobalVariables.endColor + " {0}: {1} -> {2} -> {3}", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), MethodBase.GetCurrentMethod());
                return;
            }
            else
            {
                Debug.LogFormat(GlobalVariables.cCommon + instanceViss.Length + " instanceVis's found." + GlobalVariables.endColor + " {0}: {1} -> {2} -> {3}", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), MethodBase.GetCurrentMethod());
            }

            instanceVis = instanceViss[0];
            isOfflineVis = instanceVis.isLoadedOffline;

            thisVisualisation = visualisations[0].GetComponent<Visualisation>();
            csvDataSource = csvDataSources[0].GetComponent<CSVDataSource>();

            if (thisVisualisation == null || csvDataSource == null)
            {
                Debug.LogFormat(GlobalVariables.cError + " thisVisualisation = " + (thisVisualisation != null) + " csvDataSource = " + (csvDataSource != null) + GlobalVariables.endColor + " {0}: {1} -> {2} -> {3}", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), MethodBase.GetCurrentMethod());
                return;
            }

            // in the future this needs to auto find which matches which
            if (thisVisualisation.dataSource != csvDataSource)
            {
                Debug.LogFormat(GlobalVariables.cError + " The dataset does not match the visualisation " + GlobalVariables.endColor + " {0}: {1} -> {2} -> {3}", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), MethodBase.GetCurrentMethod());
                return;
            }

            photonView = instanceVis.GetComponent<PhotonView>();

            if (photonView == null)
            {
                Debug.LogFormat(GlobalVariables.cError + "{0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "No PhotonView for Vis Menu RPCs found. Changing to use offline vis interface.", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                isOfflineVis = true;
            }

            visUID = instanceVis.vis.uid;

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
