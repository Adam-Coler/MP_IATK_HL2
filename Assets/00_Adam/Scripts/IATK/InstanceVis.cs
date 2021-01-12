using UnityEngine;
using IATK;
using Photon.Pun;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Photon_IATK
{
    public class InstanceVis : MonoBehaviourPun, IPunObservable
    {

        Visualisation vis;

        public TextAsset myDataSource;
        CSVDataSource myCSVDataSource;

        private Vector3 networkLocalPosition;
        private Quaternion networkLocalRotation;

        void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(transform.localPosition);
                stream.SendNext(transform.localRotation);
                //stream.SendNext(this.gameObject.transform.InverseTransformDirection(NewPoint));
            }
            else
            {
                networkLocalPosition = (Vector3)stream.ReceiveNext();
                networkLocalRotation = (Quaternion)stream.ReceiveNext();
            }
        }

        private void FixedUpdate()
        {

            if (!PhotonNetwork.IsConnected) { return; }

            if (!photonView.IsMine && PhotonNetwork.IsConnected)
            {
                var trans = transform;

                trans.localPosition = networkLocalPosition;
                trans.localRotation = networkLocalRotation;
            }
        }

        public void Awake()
        {
            Debug.LogFormat(GlobalVariables.yellow + "Awaking {0}, {1}" + GlobalVariables.endColor + " : " + "Awake()" + " : " + this.GetType(), this.gameObject.name, Time.realtimeSinceStartup);

            myDataSource = Resources.Load("CarDriving") as TextAsset;
            myCSVDataSource = createCSVDataSource(myDataSource.text);
            myCSVDataSource.data = myDataSource;


            vis = this.gameObject.AddComponent<IATK.Visualisation>();

            vis.gameObject.tag = "Vis";
            vis.gameObject.name = "ScatterplotVis";

            setPropertiesToUndefined(vis);

            //setView();

            vis.dataSource = myCSVDataSource;
            vis.visualisationType = AbstractVisualisation.VisualisationTypes.SCATTERPLOT;
            vis.geometry = AbstractVisualisation.GeometryType.Points;

            vis.xDimension = myCSVDataSource[0].Identifier;
            vis.yDimension = myCSVDataSource[Mathf.CeilToInt(myCSVDataSource.DimensionCount / 3f)].Identifier;
            vis.zDimension = myCSVDataSource[Mathf.CeilToInt(myCSVDataSource.DimensionCount / 3f * 2)].Identifier;

            //Debug.LogFormat(GlobalVariables.purple + myCSVDataSource.DimensionCount  + " : " + Mathf.CeilToInt(myCSVDataSource.DimensionCount / 3f) + " : " + Mathf.CeilToInt(myCSVDataSource.DimensionCount / 3f) + " : " + Mathf.CeilToInt(myCSVDataSource.DimensionCount / 3f * 2) + GlobalVariables.endColor + " {0}: {1} -> {2} -> {3}", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), MethodBase.GetCurrentMethod());

            Invoke("updateProperties", 2);
        }

        private void setPropertiesToUndefined(Visualisation visualisation)
        {
            //IATK does not initalized these so they cause an error when we manually load a photon instanced graph.

            string undefined = "Undefined";

            visualisation.colourDimension = undefined;
            visualisation.sizeDimension = undefined;
            visualisation.linkingDimension = undefined;
            visualisation.originDimension = undefined;
            visualisation.destinationDimension = undefined;
            visualisation.graphDimension = undefined;
            visualisation.colorPaletteDimension = undefined;
        }

        private void updateProperties()
        {
            //Debug.LogFormat(Photon_IATK.GlobalVariables.yellow + "updateProperties::InstanceVis - {0}, {1}" + Photon_IATK.GlobalVariables.endColor + " : " + this.GetType(), this.gameObject.name, Time.realtimeSinceStartup);

            vis.updateViewProperties(AbstractVisualisation.PropertyType.Z);

            vis.gameObject.transform.localScale = new Vector3(.5f, .5f, .5f);
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