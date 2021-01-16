using UnityEngine;
using Photon.Pun;
using System;
using IATK;

namespace Photon_IATK
{

    public class visPrefabInstantiate : MonoBehaviour
    {
        public TextAsset myDataSource;

        //this will not be used after instantiating the prefab
        private void Awake()
        {
            //label for later (not used as of now)
            this.gameObject.tag = "Vis";

            //Check if we have data
            if (myDataSource == null)
            {
                Debug.LogFormat(GlobalVariables.cError + "{0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "myDataSource is null, no visualisations will be loaded", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                throw new InvalidOperationException("Datasoucre cannot be null on Vis Prefab");
            }

            //Check to make sure we connected a datasource
            if (this.gameObject.GetComponent<PhotonView>() == null)
            {
                Debug.LogFormat(GlobalVariables.cComponentAddition + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "No Photon View attached, adding one", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                throw new InvalidOperationException("PhotonView cannot be null on Vis Prefab");
            }

            //set up datasource for the visWrapper
            CSVDataSource myCSVDataSource = createCSVDataSource(myDataSource.text);
            myCSVDataSource.data = myDataSource;

            //Add the vis wrapper to the game object this is on
            VisWrapperClass theVis = this.gameObject.AddComponent<VisWrapperClass>();

            theVis.dataSource = myCSVDataSource;
            theVis.gameObject.name = ("ScatterplotVis_" + PhotonNetwork.IsConnected);

            theVis.visualisationType = AbstractVisualisation.VisualisationTypes.SCATTERPLOT;
            theVis.geometry = AbstractVisualisation.GeometryType.Points;
            theVis.xDimension = myCSVDataSource[0].Identifier;
            theVis.yDimension = myCSVDataSource[Mathf.CeilToInt(myCSVDataSource.DimensionCount / 3f)].Identifier;
            theVis.zDimension = myCSVDataSource[Mathf.CeilToInt(myCSVDataSource.DimensionCount / 3f * 2)].Identifier;

            //theVis.updateView(AbstractVisualisation.PropertyType.GeometryType);

            theVis.CreateVisualisation(theVis.visualisationType);
            theVis.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);


            Debug.LogFormat(GlobalVariables.cInstance + "Instanced and set up a IATK visualisation, Online: {0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", PhotonNetwork.IsConnected, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());


            Debug.LogFormat(GlobalVariables.cOnDestory + "{0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Destorying this", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            Destroy(this);

        }

        CSVDataSource createCSVDataSource(string data)
        {
            CSVDataSource dataSource;
            dataSource = gameObject.AddComponent<CSVDataSource>();
            dataSource.load(data, null);
            return dataSource;
        }

    }
}