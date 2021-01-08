using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using IATK;
using System.Reflection;
using UnityEngine.Events;

namespace Photon_IATK
{

    public class InterfaceMenu : MonoBehaviour
    {

        private Visualisation thisVisualisation;
        private CSVDataSource csvDataSource;

        public TMP_Dropdown xAxisDropdown;

        public delegate void UpdateViewAction(AbstractVisualisation.PropertyType propertyType);
        private UnityEvent m_MyEvent;
        // Start is called before the first frame update
        void Awake()
        {
            IATK.Visualisation.OnUpdateViewAction += findAndRegisterVisualisation;

        }

        public void changeXAxis()
        {
            thisVisualisation.xDimension = xAxisDropdown.options[xAxisDropdown.value].text;
        }

        public void findAndRegisterVisualisation(AbstractVisualisation.PropertyType propertyType)
        {
            Visualisation[] visualisations = GameObject.FindObjectsOfType<Visualisation>();  //GameObject.FindGameObjectsWithTag("Vis");
            

            if (visualisations == null)
            {
                Debug.LogFormat(GlobalVariables.red + "No visualisations found." + GlobalVariables.endColor + " {0}: {1} -> {2} -> {3}", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), MethodBase.GetCurrentMethod());
                return;
            }
            else
            {
                Debug.LogFormat(GlobalVariables.green + visualisations.Length + " visualisations found." + GlobalVariables.endColor + " {0}: {1} -> {2} -> {3}", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), MethodBase.GetCurrentMethod());
            }

            GameObject visObj = visualisations[0].gameObject;
            thisVisualisation = visObj.GetComponent<Visualisation>();
            csvDataSource = visObj.GetComponent<CSVDataSource>();

            if (thisVisualisation == null || csvDataSource == null)
            {
                Debug.LogFormat(GlobalVariables.red + " thisVisualisation = " + (thisVisualisation != null) + " csvDataSource = " + (csvDataSource != null) + GlobalVariables.endColor + " {0}: {1} -> {2} -> {3}", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), MethodBase.GetCurrentMethod());
                return;
            }

            setupMenus();
        }

        private void setupMenus()
        {
            getCSVData();
        }

        private void clearDropdownOptions()
        {
            xAxisDropdown.ClearOptions();
        }

        private void getCSVData()
        {
            clearDropdownOptions(); 

            string[] csvDataHeaders;
            csvDataHeaders = new string[csvDataSource.DimensionCount];

            for (int i = 0; i < csvDataSource.DimensionCount; i++)
            {
                //Debug.LogFormat(GlobalVariables.purple + csvDataSource[i].Identifier + GlobalVariables.endColor + " {0}: {1} -> {2} -> {3} -> {4}", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), MethodBase.GetCurrentMethod(), csvDataSource.gameObject.name);

                xAxisDropdown.options.Add(new TMP_Dropdown.OptionData() { text = csvDataSource[i].Identifier });

                csvDataHeaders[i] = csvDataSource[i].Identifier;
            }
        }

        private void OnDestroy()
        {
            IATK.Visualisation.OnUpdateViewAction -= findAndRegisterVisualisation;
        }
    }
}
