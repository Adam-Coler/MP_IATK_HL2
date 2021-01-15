using UnityEngine;
using IATK;
using Photon.Pun;

namespace Photon_IATK
{
    public class InstanceVis : MonoBehaviourPunCallbacks, IPunObservable
    {

        public Visualisation vis;

        public bool isLoadedOffline = false;

        public TextAsset myDataSource;
        CSVDataSource myCSVDataSource;

        private Vector3 networkLocalPosition;
        private Quaternion networkLocalRotation;

        #region PunObservable
        void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (!PhotonNetwork.IsConnected || isLoadedOffline) { return; }

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

            if (!PhotonNetwork.IsConnected || isLoadedOffline) { return; }

            if (!photonView.IsMine && PhotonNetwork.IsConnected)
            {
                var trans = transform;

                trans.localPosition = networkLocalPosition;
                trans.localRotation = networkLocalRotation;
            }
        }
        #endregion

        public void Awake()
        {
            if (myDataSource == null)
            {
                Debug.LogFormat(GlobalVariables.cCommon + "{0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "myDataSource is null, loading datasource from resources folder", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                myDataSource = Resources.Load("CarDriving") as TextAsset;
            }
            else
            {
                Debug.LogFormat(GlobalVariables.cCommon + "{0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "myDataSource loaded", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            }

            if (photonView == null)
            {
                Debug.LogFormat(GlobalVariables.cComponentAddition + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "No Photon View attached, adding one", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                this.gameObject.AddComponent<PhotonView>();

                if (!photonView.IsMine)
                {
                    Debug.LogFormat(GlobalVariables.cError + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Added photonView is not setting ismine to true.", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                }
            }

            myCSVDataSource = createCSVDataSource(myDataSource.text);
            myCSVDataSource.data = myDataSource;


            vis = this.gameObject.AddComponent<IATK.Visualisation>();
            vis.gameObject.tag = "Vis";

            //named where instantiated
            //vis.gameObject.name = "ScatterplotVis";

            setPropertiesToUndefined(vis);

            vis.dataSource = myCSVDataSource;
            vis.visualisationType = AbstractVisualisation.VisualisationTypes.SCATTERPLOT;
            vis.geometry = AbstractVisualisation.GeometryType.Points;

            vis.xDimension = myCSVDataSource[0].Identifier;
            vis.yDimension = myCSVDataSource[Mathf.CeilToInt(myCSVDataSource.DimensionCount / 3f)].Identifier;
            vis.zDimension = myCSVDataSource[Mathf.CeilToInt(myCSVDataSource.DimensionCount / 3f * 2)].Identifier;

            Debug.LogFormat(GlobalVariables.cCommon + "{0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Calling update properties", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

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

            vis.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
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