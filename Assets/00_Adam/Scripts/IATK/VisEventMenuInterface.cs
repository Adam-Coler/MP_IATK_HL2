using System.Collections.Generic;
using UnityEngine;
using TMPro;
using IATK;
using System.Linq;
using Photon.Pun;

namespace Photon_IATK
{
    public class VisEventMenuInterface : MonoBehaviour
    {

        public TMP_Dropdown xAxisDropdown;
        public TMP_Dropdown yAxisDropdown;
        public TMP_Dropdown zAxisDropdown;

        public TMP_Dropdown colorDimensionDropdown;
        public TMP_Dropdown sizeDimensionDropdown;

        private VisualizationEvent_Calls theVisualizationEvent_Calls;


        // Start is called before the first frame update
        //void Awake()
        //{
        //    OnEnable();
        //}

        public void changeXAxis()
        {
            theVisualizationEvent_Calls.RequestChangeXAxisEvent(xAxisDropdown.options[xAxisDropdown.value].text);
        }

        public void changeYAxis()
        {
            theVisualizationEvent_Calls.RequestChangeYAxisEvent(yAxisDropdown.options[yAxisDropdown.value].text);
        }

        public void changeZAxis()
        {
            theVisualizationEvent_Calls.RequestChangeZAxisEvent(zAxisDropdown.options[zAxisDropdown.value].text);
        }

        public void changeColorDimension()
        {
            theVisualizationEvent_Calls.RequestChangeColorDimensionEvent(colorDimensionDropdown.options[colorDimensionDropdown.value].text);
        }

        public void changeSizeDimension()
        {
            theVisualizationEvent_Calls.RequestChangeSizeDimensionEvent(sizeDimensionDropdown.options[sizeDimensionDropdown.value].text);
        }

        private void OnEnable()
        {
            //We need the dropdowns to be able to do anything to the RPCS
            if (xAxisDropdown == null || yAxisDropdown == null || zAxisDropdown == null)
            {
                Debug.LogFormat(GlobalVariables.cError + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "One or more axis dropdown menus not found.", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                return;
            }

            VisualizationEvent_Calls.RPCvisualisationUpdatedDelegate += UpdatedView;
            Debug.LogFormat(GlobalVariables.cRegister + "Registering {0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "UpdatedView", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            //Now we need to get the RPC interface assuming one VIS object and that the vis object is loaded before the menu
            findAndStoreVisualizationRPC_Calls();
        }

        private void UpdatedView(AbstractVisualisation.PropertyType propertyType)
        {
            Debug.LogFormat(GlobalVariables.cTest + "Vis view {0} updated." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", propertyType, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            findAndStoreVisualizationRPC_Calls();

            updateDropDown(propertyType);

        }

        private void updateDropDown(AbstractVisualisation.PropertyType propertyType)
        {
            switch (propertyType)
            {
                case AbstractVisualisation.PropertyType.X:
                    Debug.LogFormat(GlobalVariables.cCommon + "Vis view {0} updated." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", propertyType, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                    xAxisDropdown.value = xAxisDropdown.options.FindIndex(option => option.text == theVisualizationEvent_Calls.xDimension);
                    xAxisDropdown.RefreshShownValue();
                    break;
                case AbstractVisualisation.PropertyType.Y:
                    yAxisDropdown.value = yAxisDropdown.options.FindIndex(option => option.text == theVisualizationEvent_Calls.yDimension);
                    yAxisDropdown.RefreshShownValue();
                    Debug.LogFormat(GlobalVariables.cCommon + "Vis view {0} updated." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", propertyType, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                    break;
                case AbstractVisualisation.PropertyType.Z:

                    zAxisDropdown.value = zAxisDropdown.options.FindIndex(option => option.text == theVisualizationEvent_Calls.zDimension);
                    zAxisDropdown.RefreshShownValue();
                    Debug.LogFormat(GlobalVariables.cCommon + "Vis view {0} updated." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", propertyType, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                    break;
                case AbstractVisualisation.PropertyType.Colour:
                    colorDimensionDropdown.value = colorDimensionDropdown.options.FindIndex(option => option.text == theVisualizationEvent_Calls.colourDimension);
                    colorDimensionDropdown.RefreshShownValue();
                    Debug.LogFormat(GlobalVariables.cCommon + "Vis view {0} updated." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", propertyType, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                    break;
                case AbstractVisualisation.PropertyType.Size:
                    sizeDimensionDropdown.value = sizeDimensionDropdown.options.FindIndex(option => option.text == theVisualizationEvent_Calls.sizeDimension);
                    sizeDimensionDropdown.RefreshShownValue();
                    Debug.LogFormat(GlobalVariables.cCommon + "Vis view {0} updated." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", propertyType, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                    break;
                default:
                    Debug.LogFormat(GlobalVariables.cCommon + "DEFUALT: Vis view {0} updated." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", propertyType, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                    break;
            }
        }

        private void findAndStoreVisualizationRPC_Calls()
        {
            //get all VisualizationRPC_Calls
            VisualizationEvent_Calls[] visualizationRPC_CallsCollection = GameObject.FindObjectsOfType<VisualizationEvent_Calls>();

            //If there are none or more than one we have a problem
            if (visualizationRPC_CallsCollection.Length == 0)
            {
                Debug.LogFormat(GlobalVariables.cError + "No VisualizationRPC_Calls found." + GlobalVariables.endColor + " {0}: {1} -> {2} -> {3}", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                return;
            }

            //Nothing has been setup
            if (theVisualizationEvent_Calls == null)
            {
                theVisualizationEvent_Calls = visualizationRPC_CallsCollection[0];

                Debug.LogFormat(GlobalVariables.cCommon + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "New VisPRC referance set.", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                setUpMenus();
                return;
            }

            //Check if a New vis is loaded
            if (theVisualizationEvent_Calls != null)
            {
                if (theVisualizationEvent_Calls != visualizationRPC_CallsCollection[0])
                {
                    theVisualizationEvent_Calls = visualizationRPC_CallsCollection[0];

                    Debug.LogFormat(GlobalVariables.cCommon + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "New VisRPC found, VisPRC referance updated.", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                    setUpMenus();
                    return;
                }
            }
            Debug.LogFormat(GlobalVariables.cCommon + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "The registered VisRPC is still valid.", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            }

        private void setUpMenus()
        {
            setAxisDropdowns(theVisualizationEvent_Calls.loadedCSVHeaders, true);

            Debug.LogFormat(GlobalVariables.cCommon + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Updating menu options.", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
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
                List<List<TMP_Dropdown.OptionData>> partititionedList = HelperFunctions.Split(listDataDimensions, 3);

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

        private void OnDisable()
        {
            Debug.LogFormat(GlobalVariables.cRegister + "Un-registering {0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "UpdatedView", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            VisWrapperClass.visualisationUpdatedDelegate -= UpdatedView;
        }

        private void clearDropdownOptions()
        {
            xAxisDropdown.ClearOptions();
            yAxisDropdown.ClearOptions();
            zAxisDropdown.ClearOptions();
            colorDimensionDropdown.ClearOptions();
            sizeDimensionDropdown.ClearOptions();
        }

    }
}