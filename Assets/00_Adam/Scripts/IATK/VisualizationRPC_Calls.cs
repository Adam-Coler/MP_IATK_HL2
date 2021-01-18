using UnityEngine;
using Photon.Pun;
using IATK;


namespace Photon_IATK
{
    public class VisualizationRPC_Calls : MonoBehaviourPunCallbacks
    {
        public delegate void OnRPCVisualisationUpdated(AbstractVisualisation.PropertyType propertyType);
        public static OnRPCVisualisationUpdated RPCvisualisationUpdatedDelegate;

        private VisWrapperClass thisVis;

        public PhotonView thisPhotonView;

        public string[] loadedCSVHeaders { get; set; }

        public string xDimension {
            get 
            {
                return thisVis.xDimension.Attribute;
            }
        }
        public string yDimension
        {
            get
            {
                return thisVis.yDimension.Attribute;
            }
        }
        public string zDimension
        {
            get
            {
                return thisVis.zDimension.Attribute;
            }
        }
        public string colourDimension
        {
            get
            {
                return thisVis.colourDimension;
            }
        }
        public string sizeDimension
        {
            get
            {
                return thisVis.sizeDimension;
            }
        }


        private void Awake()
        {
            //RPC calls need a photon view
            if (thisPhotonView == null)
            {
                Debug.LogFormat(GlobalVariables.cError + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "No Photon View attached! Addone in the prefab", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            }

            getVisWrapper();

            VisWrapperClass.visualisationUpdatedDelegate += visUpdatedListener;

        }

        private void OnDestroy()
        {
            Debug.LogFormat(GlobalVariables.cRegister + "Un-registering {0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "visUpdatedListener", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            VisWrapperClass.visualisationUpdatedDelegate -= visUpdatedListener;
        }

        private void visUpdatedListener(AbstractVisualisation.PropertyType propertyType)
        {
            if (thisVis == null)
                getVisWrapper();

            if (RPCvisualisationUpdatedDelegate != null)
                RPCvisualisationUpdatedDelegate(propertyType);
        }

        private void getVisWrapper()
        {
            if (thisVis == null)
            {
                thisVis = this.gameObject.GetComponent<VisWrapperClass>();
            }

            if (thisVis != null)
            {
                loadedCSVHeaders = thisVis.loadedCSVHeaders;
                return;
            }

            Debug.LogFormat(GlobalVariables.cError + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "No VisWrapper found.", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }


        public void changeXAxis(string newAxisDimension)
        {
            if (PhotonNetwork.IsConnected)
            {
                photonView.RPC("_changeXAxis", RpcTarget.All, newAxisDimension);
            } else
            {
                _changeXAxis(newAxisDimension, new PhotonMessageInfo());
            }
        }

        public void changeYAxis(string newAxisDimension)
        {
            if (PhotonNetwork.IsConnected)
            {
                photonView.RPC("_changeYAxis", RpcTarget.All, newAxisDimension);
            }
            else
            {
                _changeYAxis(newAxisDimension, new PhotonMessageInfo());
            }
        }

        public void changeZAxis(string newAxisDimension)
        {
            if (PhotonNetwork.IsConnected)
            {
                photonView.RPC("_changeZAxis", RpcTarget.All, newAxisDimension);
            }
            else
            {
                _changeZAxis(newAxisDimension, new PhotonMessageInfo());
            }
        }

        public void changeColorDimension(string newAxisDimension)
        {
            if (PhotonNetwork.IsConnected)
            {
                photonView.RPC("_changeColorDimension", RpcTarget.All, newAxisDimension);
            }
            else
            {
                _changeColorDimension(newAxisDimension, new PhotonMessageInfo());
            }
        }

        public void changeSizeDimension(string newAxisDimension)
        {
            if (PhotonNetwork.IsConnected)
            {
                photonView.RPC("_changeSizeDimension", RpcTarget.All, newAxisDimension);
            }
            else
            {
                _changeSizeDimension(newAxisDimension, new PhotonMessageInfo());
            }
        }

        [PunRPC]
        private void _changeXAxis(string newAxisDimension, PhotonMessageInfo info)
        {
            thisVis.xDimension = newAxisDimension;
            thisVis.updateVisPropertiesSafe();

            Debug.LogFormat(GlobalVariables.cPRC + "PUN RPC call, Sender:{0}, View: {1}, SentServerTime: {3}," + GlobalVariables.endColor + "{4}: {5} -> {6} -> {7}", info.Sender, info.photonView, info.SentServerTime, this.name, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }

        [PunRPC]
        private void _changeYAxis(string newAxisDimension, PhotonMessageInfo info)
        {
            thisVis.yDimension = newAxisDimension;
            thisVis.updateVisPropertiesSafe();

            Debug.LogFormat(GlobalVariables.cPRC + "PUN RPC call, Sender:{0}, View: {1}, SentServerTime: {3}," + GlobalVariables.endColor + "{4}: {5} -> {6} -> {7}", info.Sender, info.photonView, info.SentServerTime, this.name, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }

        [PunRPC]
        private void _changeZAxis(string newAxisDimension, PhotonMessageInfo info)
        {
            thisVis.zDimension = newAxisDimension;
            thisVis.updateVisPropertiesSafe();

            Debug.LogFormat(GlobalVariables.cPRC + "PUN RPC call, Sender:{0}, View: {1}, SentServerTime: {3}," + GlobalVariables.endColor + "{4}: {5} -> {6} -> {7}", info.Sender, info.photonView, info.SentServerTime, this.name, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }

        [PunRPC]
        private void _changeColorDimension(string newColorDimension, PhotonMessageInfo info)
        {
            Color colorStart = Color.blue;
            Color colorEnd = Color.red;

            thisVis.dimensionColour = HelperFunctions.getColorGradient(colorStart, colorEnd);
            thisVis.colourDimension = newColorDimension;

            thisVis.updateVisPropertiesSafe();

            Debug.LogFormat(GlobalVariables.cPRC + "PUN RPC call, Sender:{0}, View: {1}, SentServerTime: {3}," + GlobalVariables.endColor + "{4}: {5} -> {6} -> {7}", info.Sender, info.photonView, info.SentServerTime, this.name, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }

        [PunRPC]
        private void _changeSizeDimension(string newAxisDimension, PhotonMessageInfo info)
        {
            thisVis.sizeDimension = newAxisDimension;
            thisVis.updateVisPropertiesSafe();

            Debug.LogFormat(GlobalVariables.cPRC + "PUN RPC call, Sender:{0}, View: {1}, SentServerTime: {3}," + GlobalVariables.endColor + "{4}: {5} -> {6} -> {7}", info.Sender, info.photonView, info.SentServerTime, this.name, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }

    }
}
