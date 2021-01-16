using UnityEngine;
using Photon.Pun;
using IATK;


namespace Photon_IATK
{
    public class VisualizationRPC_Calls : MonoBehaviourPunCallbacks
    {
        public InstanceVis instanceVis;

        private void Awake()
        {
            //RPC calls need a photon view
            if (photonView == null)
            {
                Debug.LogFormat(GlobalVariables.cComponentAddition + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "No Photon View attached, adding one", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                this.gameObject.AddComponent<PhotonView>();

                if (!photonView.IsMine)
                {
                    Debug.LogFormat(GlobalVariables.cError + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Added photonView is not setting ismine to true.", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                }
            }

            //check the instance is set
            if (instanceVis == null)
            {
                instanceVis = this.gameObject.GetComponent<InstanceVis>();
            }

            if (instanceVis == null)
            {
                Debug.LogFormat(GlobalVariables.cError + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "No InstanceVis found, this must be attached before runtime", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                return;
            }

        }

        [PunRPC]
        public void changeXAxis(string newAxisDimension, PhotonMessageInfo info)
        {
            instanceVis.vis.xDimension = newAxisDimension;
            updateVisPropertiesSafe();

            Debug.LogFormat(GlobalVariables.cPRC + "PUN RPC call, Sender:{0}, View: {1}, SentServerTime: {3}," + GlobalVariables.endColor + "{4}: {5} -> {6} -> {7}", info.Sender, info.photonView, info.SentServerTime, this.name, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }

        [PunRPC]
        public void changeYAxis(string newAxisDimension, PhotonMessageInfo info)
        {
            instanceVis.vis.yDimension = newAxisDimension;
            updateVisPropertiesSafe();

            Debug.LogFormat(GlobalVariables.cPRC + "PUN RPC call, Sender:{0}, View: {1}, SentServerTime: {3}," + GlobalVariables.endColor + "{4}: {5} -> {6} -> {7}", info.Sender, info.photonView, info.SentServerTime, this.name, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }

        [PunRPC]
        public void changeZAxis(string newAxisDimension, PhotonMessageInfo info)
        {
            instanceVis.vis.zDimension = newAxisDimension;
            updateVisPropertiesSafe();

            Debug.LogFormat(GlobalVariables.cPRC + "PUN RPC call, Sender:{0}, View: {1}, SentServerTime: {3}," + GlobalVariables.endColor + "{4}: {5} -> {6} -> {7}", info.Sender, info.photonView, info.SentServerTime, this.name, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }

        [PunRPC]
        public void changeColorDimension(string newColorDimension, PhotonMessageInfo info)
        {
            Color colorStart = Color.blue;
            Color colorEnd = Color.red;

            instanceVis.vis.dimensionColour = HelperFunctions.getColorGradient(colorStart, colorEnd);
            instanceVis.vis.colourDimension = newColorDimension;

            Debug.LogFormat(GlobalVariables.cPRC + "PUN RPC call, Sender:{0}, View: {1}, SentServerTime: {3}," + GlobalVariables.endColor + "{4}: {5} -> {6} -> {7}", info.Sender, info.photonView, info.SentServerTime, this.name, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }

        [PunRPC]
        public void changeSizeDimension(string newAxisDimension, PhotonMessageInfo info)
        {
            instanceVis.vis.zDimension = newAxisDimension;
            updateVisPropertiesSafe();

            Debug.LogFormat(GlobalVariables.cPRC + "PUN RPC call, Sender:{0}, View: {1}, SentServerTime: {3}," + GlobalVariables.endColor + "{4}: {5} -> {6} -> {7}", info.Sender, info.photonView, info.SentServerTime, this.name, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }


        public void updateVisPropertiesSafe()
        {
            AbstractVisualisation theVisObject = instanceVis.vis.theVisualizationObject;

            if (theVisObject == null)
            {
                Debug.LogFormat(GlobalVariables.cError + "The Visualisation obect is null." + GlobalVariables.endColor + " {0}: {1} -> {2} -> {3}", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                return;
            }

            if (theVisObject.X_AXIS == null || theVisObject.Y_AXIS == null || theVisObject.Z_AXIS == null)
            {
                Debug.LogFormat(GlobalVariables.cError + "The Visualisation is missing an axis. X == null: " + (theVisObject.X_AXIS == null) + " , Y == null: " + (theVisObject.Y_AXIS == null) + " , Z == null: " + (theVisObject.Z_AXIS == null) + GlobalVariables.endColor + " {0}: {1} -> {2} -> {3}", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                return;
            }

            instanceVis.vis.updateProperties();

        }
    }
}
