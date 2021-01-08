using UnityEngine;
using IATK;
using Photon.Pun;

namespace Photon_IATK
{
    public class InstanceVis : MonoBehaviourPun, IPunObservable
    {
        private string state = "State";
        private string fatalitiesPerBillion = "Number of drivers involved in fatal collisions per billion miles";
        private string pctSpeeding = "Percentage Of Drivers Involved In Fatal Collisions Who Were Speeding";
        private string pctImpairedABV = "Percentage Of Drivers Involved In Fatal Collisions Who Were Alcohol-Impaired";

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

            setPropertiesToUndefined(vis);

            //setView();

            vis.dataSource = myCSVDataSource;

            vis.visualisationType = AbstractVisualisation.VisualisationTypes.SCATTERPLOT;

            vis.geometry = AbstractVisualisation.GeometryType.Points;

            vis.xDimension = state;
            vis.yDimension = fatalitiesPerBillion;
            vis.zDimension = pctImpairedABV;

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
            Debug.LogFormat(Photon_IATK.GlobalVariables.yellow + "updateProperties::InstanceVis - {0}, {1}" + Photon_IATK.GlobalVariables.endColor + " : " + this.GetType(), this.gameObject.name, Time.realtimeSinceStartup);
            vis.updateViewProperties(AbstractVisualisation.PropertyType.GraphDimension);
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