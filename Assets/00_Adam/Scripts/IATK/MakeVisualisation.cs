using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IATK;
using System.Linq;
using Photon.Pun;

namespace Photon_IATK
{
    public class MakeVisualisation : MonoBehaviourPun, IPunObservable
    {
        //Use Unity Test assets to import text data (e.g. csv, tsv etc.)
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
            Debug.LogFormat(GlobalVariables.yellow + "Awaking {0}" + GlobalVariables.endColor + " : " + "Awake()" + " : " + this.GetType(), this.gameObject.name);

            myDataSource = Resources.Load("CarDriving") as TextAsset;
            myCSVDataSource = createCSVDataSource(myDataSource.text);
            myCSVDataSource.data = myDataSource;
            makeView();
            //setView();
        }



        // a reusable method to create an IATK CSVDataSource object.
        CSVDataSource createCSVDataSource(string data)
        {
            CSVDataSource dataSource;
            dataSource = gameObject.AddComponent<CSVDataSource>();
            dataSource.load(data, null);
            return dataSource;
        }


        private string state = "State";
        private string fatalitiesPerBillion = "Number of drivers involved in fatal collisions per billion miles";
        private string pctSpeeding = "Percentage Of Drivers Involved In Fatal Collisions Who Were Speeding";
        private string pctImpairedABV = "Percentage Of Drivers Involved In Fatal Collisions Who Were Alcohol-Impaired";


        private void makeView()
        {

            //this.gameObject.tag = "View";

            Visualisation vis = this.gameObject.AddComponent<IATK.Visualisation>();

            vis.dataSource = myCSVDataSource;

            vis.visualisationType = AbstractVisualisation.VisualisationTypes.SCATTERPLOT;
            vis.geometry = AbstractVisualisation.GeometryType.Points;

            vis.xDimension = state;
            vis.yDimension = fatalitiesPerBillion;
            vis.zDimension = pctImpairedABV;

            //vis.enabled = true;
            //this.gameObject.SetActive(true);
        }

        private void setView()
        {
            var csvds = myCSVDataSource;
            ViewBuilder vb = new ViewBuilder(MeshTopology.Points, "Impaired Driving Fatalities").
                     initialiseDataView(csvds.DataCount).
                     setDataDimension(csvds[state].Data, ViewBuilder.VIEW_DIMENSION.X).
                     setDataDimension(csvds[fatalitiesPerBillion].Data, ViewBuilder.VIEW_DIMENSION.Y).
                     setDataDimension(csvds[pctSpeeding].Data, ViewBuilder.VIEW_DIMENSION.Z);

            // use the IATKUtil class to get the corresponding Material mt 
            Material mt = IATKUtil.GetMaterialFromTopology(AbstractVisualisation.GeometryType.Points);
            mt.SetFloat("_MinSize", 0.01f);
            mt.SetFloat("_MaxSize", 0.05f);


            // create a view builder with the point topology
            View view = vb.updateView().apply(gameObject, mt);

            //Axis xAxis;
            //xAxis = new Axis();
            //xAxis.SetDirection(1);

        }

    }
}
