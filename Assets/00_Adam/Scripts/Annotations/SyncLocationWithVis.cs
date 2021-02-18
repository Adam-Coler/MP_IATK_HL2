using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;
using System.Collections;
using IATK;

namespace Photon_IATK
{
    [DisallowMultipleComponent]
    public class SyncLocationWithVis : MonoBehaviour
    {
        private GameObject myVisParent;
        private Vector3 lastVisPosition;
        private Quaternion lastVisRotation;
        private void OnEnable()
        {
            Debug.LogFormat(GlobalVariables.cRegister + "GenericTransformSync registering OnEvent, RPCvisualisationUpdatedDelegate.{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            VisualizationEvent_Calls.RPCvisualisationUpdatedDelegate += UpdatedView;
        }

        private void OnDisable()
        {
            Debug.LogFormat(GlobalVariables.cRegister + "GenericTransformSync unregistering OnEvent, RPCvisualisationUpdatedDelegate.{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            VisualizationEvent_Calls.RPCvisualisationUpdatedDelegate -= UpdatedView;
        }

        private void UpdatedView(AbstractVisualisation.PropertyType propertyType)
        {
            HelperFunctions.FindGameObjectOrMakeOneWithTag(GlobalVariables.visTag, out myVisParent, false, System.Reflection.MethodBase.GetCurrentMethod());
        }

    private void LateUpdate()
        {

            if (myVisParent != null)
            {
                if (lastVisPosition != myVisParent.transform.localPosition || lastVisRotation != myVisParent.transform.localRotation)
                {
                    transform.localPosition = myVisParent.transform.localPosition;
                    transform.localRotation = myVisParent.transform.localRotation;

                    lastVisPosition = myVisParent.transform.localPosition;
                    lastVisRotation = myVisParent.transform.localRotation;
                }

            }
        }
    }
}



//using UnityEngine;
//using ExitGames.Client.Photon;
//using Photon.Realtime;
//using Photon.Pun;
//using System.Collections;
//using IATK;

//namespace Photon_IATK
//{
//    [DisallowMultipleComponent]
//    [RequireComponent(typeof(Photon.Pun.PhotonView))]
//    public class SyncLocationWithVis : MonoBehaviourPun
//    {

//        public Vector3 lastTrackedLocation;
//        public Vector3 lastTrackedRotation;

//        public int myVisParentViewID = 0;
//        /// <summary>
//        /// This class needs to:
//        /// Sync movement with Vis object
//        /// Sync movement with seperate annotaiton movements
//        /// Prevent overlap
//        /// </summary>

//        private void OnEnable()
//        {
//            Debug.LogFormat(GlobalVariables.cRegister + "GenericTransformSync registering OnEvent, RPCvisualisationUpdatedDelegate.{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
//            PhotonNetwork.NetworkingClient.EventReceived += OnEvent;

//            VisualizationEvent_Calls.RPCvisualisationUpdatedDelegate += UpdatedView;
//        }

//        private void OnDisable()
//        {
//            Debug.LogFormat(GlobalVariables.cRegister + "GenericTransformSync unregistering OnEvent, RPCvisualisationUpdatedDelegate.{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

//            PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
//            VisualizationEvent_Calls.RPCvisualisationUpdatedDelegate -= UpdatedView;
//        }

//        private void UpdatedView(AbstractVisualisation.PropertyType propertyType)
//        {
//            _setVisViewID();
//        }

//        private void _setVisViewID()
//        {
//            if (myVisParentViewID != 0) { return; }

//            GameObject visObject;
//            if (!HelperFunctions.FindGameObjectOrMakeOneWithTag(GlobalVariables.visTag, out visObject, false, System.Reflection.MethodBase.GetCurrentMethod()))
//            {

//                Debug.LogFormat(GlobalVariables.cError + "No Vis parent found, annotations will not be linked{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
//                return;
//            }

//            PhotonView photonView = visObject.GetComponent<PhotonView>();
//            if (photonView == null)
//            {
//                Debug.LogFormat(GlobalVariables.cError + "No Vis photon view found, annotations will not be linked{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
//                return;
//            }

//            myVisParentViewID = photonView.ViewID;

//        }

//        private void OnEvent(EventData photonEventData)
//        {
//            _setVisViewID();

//            byte eventCode = photonEventData.Code;
//            object[] data = (object[])photonEventData.CustomData;
//            int callerPhotonViewID = (int)data[0];

//            Debug.LogFormat(GlobalVariables.cEvent + "Event Found, Sender: {0}, Event code: {1}, Right Code: {2}, Right Caller: {3}, Parent ID: {4}, Caller ID: {5}{6}{7}." + GlobalVariables.endColor + " {8}: {9} -> {10} -> {11}", photonEventData.Sender, eventCode, eventCode == GlobalVariables.PhotonMoveEvent, callerPhotonViewID == myVisParentViewID, myVisParentViewID, callerPhotonViewID, "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

//            //Check that the event was one we made, photon reserves 0, 200+
//            if (eventCode != GlobalVariables.PhotonMoveEvent) { return; }

//            //make sure that this object is the same as the sender object
//            if (callerPhotonViewID != myVisParentViewID) { return; }

//            //route the event
//            switch (eventCode)
//            {
//                case GlobalVariables.PhotonMoveEvent:
//                    PhotonMoveEvent(data);
//                    break;
//                default:
//                    break;
//            }


//        }

//        private Vector3 lastLocalLocation;
//        private Quaternion lastLocalRotation;
//        private Vector3 lastLocalScale;

//        /// <summary>
//        /// Updates the objects movement to match the sent transform information.
//        /// Expected Data = (photonView.ViewID, myTransform.localPosition, myTransform.localRotation, myTransform.localScale);
//        /// </summary>
//        private void PhotonMoveEvent(object[] data)
//        {

//            Debug.LogFormat(GlobalVariables.cEvent + "Recived Code {0}: Any ~ {1}{2}, My Name: {3}, I am the Master Client: {4}, Server Time: {5}{6}{7}{8}{9}." + GlobalVariables.endColor + " {10}: {11} -> {12} -> {13}", GlobalVariables.PhotonMoveEvent, " Move Event", "", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, "", "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

//            Vector3 newLocalPosition = (Vector3)data[1];
//            Quaternion newLocalRotation = (Quaternion)data[2];

//            this.transform.localPosition = newLocalPosition;
//            this.transform.localRotation = newLocalRotation;

//        }
//    }
//}
