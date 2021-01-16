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

            Debug.LogFormat(GlobalVariables.cError + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "No VisWrapper found.", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }

        [PunRPC]
        public void changeXAxis(string newAxisDimension, PhotonMessageInfo info)
        {
            thisVis.xDimension = newAxisDimension;
            thisVis.updateVisPropertiesSafe();

            Debug.LogFormat(GlobalVariables.cPRC + "PUN RPC call, Sender:{0}, View: {1}, SentServerTime: {3}," + GlobalVariables.endColor + "{4}: {5} -> {6} -> {7}", info.Sender, info.photonView, info.SentServerTime, this.name, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }

        //[PunRPC]
        //public void changeYAxis(string newAxisDimension, PhotonMessageInfo info)
        //{
        //    instanceVis.vis.yDimension = newAxisDimension;
        //    updateVisPropertiesSafe();

        //    Debug.LogFormat(GlobalVariables.cPRC + "PUN RPC call, Sender:{0}, View: {1}, SentServerTime: {3}," + GlobalVariables.endColor + "{4}: {5} -> {6} -> {7}", info.Sender, info.photonView, info.SentServerTime, this.name, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        //}

        //[PunRPC]
        //public void changeZAxis(string newAxisDimension, PhotonMessageInfo info)
        //{
        //    instanceVis.vis.zDimension = newAxisDimension;
        //    updateVisPropertiesSafe();

        //    Debug.LogFormat(GlobalVariables.cPRC + "PUN RPC call, Sender:{0}, View: {1}, SentServerTime: {3}," + GlobalVariables.endColor + "{4}: {5} -> {6} -> {7}", info.Sender, info.photonView, info.SentServerTime, this.name, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        //}

        //[PunRPC]
        //public void changeColorDimension(string newColorDimension, PhotonMessageInfo info)
        //{
        //    Color colorStart = Color.blue;
        //    Color colorEnd = Color.red;

        //    instanceVis.vis.dimensionColour = HelperFunctions.getColorGradient(colorStart, colorEnd);
        //    instanceVis.vis.colourDimension = newColorDimension;

        //    Debug.LogFormat(GlobalVariables.cPRC + "PUN RPC call, Sender:{0}, View: {1}, SentServerTime: {3}," + GlobalVariables.endColor + "{4}: {5} -> {6} -> {7}", info.Sender, info.photonView, info.SentServerTime, this.name, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        //}

        //[PunRPC]
        //public void changeSizeDimension(string newAxisDimension, PhotonMessageInfo info)
        //{
        //    instanceVis.vis.zDimension = newAxisDimension;
        //    updateVisPropertiesSafe();

        //    Debug.LogFormat(GlobalVariables.cPRC + "PUN RPC call, Sender:{0}, View: {1}, SentServerTime: {3}," + GlobalVariables.endColor + "{4}: {5} -> {6} -> {7}", info.Sender, info.photonView, info.SentServerTime, this.name, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        //}

    }
}
