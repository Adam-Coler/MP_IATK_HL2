using UnityEngine;
using IATK;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;

namespace Photon_IATK
{
    public class VisualizationEvent_Calls : MonoBehaviourPunCallbacks
    {
        public delegate void OnEventVisualisationUpdated(AbstractVisualisation.PropertyType propertyType);
        public static OnEventVisualisationUpdated RPCvisualisationUpdatedDelegate;

        public delegate void OnEventCVisualisationUpdateRequest(AbstractVisualisation.PropertyType propertyType);
        public static OnEventCVisualisationUpdateRequest RPCvisualisationUpdateRequestDelegate;

        private VisWrapperClass thisVis;
        private PhotonView thisPhotonView;

        public bool isWaitingForPhotonRequestStateEvent = false;

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

        public string axisKey
        {
            get
            {
                return thisVis.axisKey;
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

            Debug.LogFormat(GlobalVariables.cRegister + "Registering visUpdatedListener{0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "visUpdatedListener", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            VisWrapperClass.visualisationUpdatedDelegate += visUpdatedListener;

            Debug.LogFormat(GlobalVariables.cRegister + "Registering OnEvent.{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            PhotonNetwork.NetworkingClient.EventReceived += OnEvent;

            if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat(GlobalVariables.cEvent + "New client calling PhotonRequestStateEvent. View ID: {0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", photonView.ViewID, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                object[] content = new object[] { photonView.ViewID };

                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };

                PhotonNetwork.RaiseEvent(GlobalVariables.PhotonRequestStateEvent, content, raiseEventOptions, GlobalVariables.sendOptions);

                isWaitingForPhotonRequestStateEvent = true;
            }
        }

        private void OnDestroy()
        {
            Debug.LogFormat(GlobalVariables.cRegister + "Unregistering visUpdatedListener{0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "visUpdatedListener", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            VisWrapperClass.visualisationUpdatedDelegate -= visUpdatedListener;

            Debug.LogFormat(GlobalVariables.cRegister + "Unregistering OnEvent.{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
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

        private void visUpdatedListener(AbstractVisualisation.PropertyType propertyType)
        {
            if (thisVis == null)
                getVisWrapper();

            if (RPCvisualisationUpdatedDelegate != null)
                RPCvisualisationUpdatedDelegate(propertyType);
        }

        private void OnEvent(EventData photonEventData)
        {
            byte eventCode = photonEventData.Code;

            //Check that the event was one we made, photon reserves 0, 200+
            if (eventCode == 0 || eventCode > 199) { return; }

            object[] data = (object[])photonEventData.CustomData;
            int callerPhotonViewID = (int)data[0];
            if (photonView.ViewID != callerPhotonViewID) { return; }

            switch (eventCode)
            {
                case GlobalVariables.PhotonChangeX_AxisEvent:
                    _changeXAxis((string)data[1]);
                    Debug.Log("PhotonChangeX_AxisEvent");
                    break;
                case GlobalVariables.PhotonChangeY_AxisEvent:
                    _changeYAxis((string)data[1]);
                    Debug.Log("PhotonChangeY_AxisEvent");
                    break;
                case GlobalVariables.PhotonChangeZ_AxisEvent:
                    _changeZAxis((string)data[1]);
                    Debug.Log("PhotonChangeZ_AxisEvent");
                    break;
                case GlobalVariables.PhotonChangeColorDimensionEvent:
                    _changeColorDimension((string)data[1]);
                    Debug.Log("PhotonChangeSizeDimensionEvent");
                    break;
                case GlobalVariables.PhotonChangeSizeDimensionEvent:
                    _changeSizeDimension((string)data[1]);
                    Debug.Log("PhotonChangeColorDimensionEvent");
                    break;
                case GlobalVariables.PhotonRequestStateEvent:
                    _sendStateEvent();
                    Debug.Log("PhotonRequestStateEvent");
                    break;
                case GlobalVariables.PhotonRequestStateEventResponse:
                    _processRequestStateEventResponse(data);
                    Debug.Log("PhotonRequestStateEventResponse");
                    break;
                default:
                    break;
            }
        }


        /// <summary>
        /// Sends the Vis state from the master client
        /// Data = Object[] { photonView.ViewID, xDimension, yDimension, zDimension, colourDimension, sizeDimension };
        /// </summary>
        public void _sendStateEvent()
        {
            object[] content = new object[] { photonView.ViewID, xDimension, yDimension, zDimension, colourDimension, sizeDimension };

            Debug.LogFormat(GlobalVariables.cEvent + "_sendStateEvent from master, sent data = View:{0}, X Axis: {1}, Y Axis: {2}, Z Axis: {3}, Colur: {4}, Size: {5}." + GlobalVariables.endColor + " {6}: {7} -> {8} -> {9}", photonView.ViewID, xDimension, yDimension, zDimension, colourDimension, sizeDimension, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(),
System.Reflection.MethodBase.GetCurrentMethod());

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

            PhotonNetwork.RaiseEvent(GlobalVariables.PhotonRequestStateEventResponse, content, raiseEventOptions, GlobalVariables.sendOptions);
        }

        public void _processRequestStateEventResponse(object[] data)
        {
            string xAxis = (string)data[1];
            string yAxis = (string)data[2];
            string zAxis = (string)data[3];
            string colorDimension = (string)data[4];
            string sizeDimension = (string)data[5];

            Debug.LogFormat(GlobalVariables.cEvent + "_processRequestStateEventResponse from master, recived data = View:{0}, X Axis: {1}, Y Axis: {2}, Z Axis: {3}, Colur: {4}, Size: {5}." + GlobalVariables.endColor + " {6}: {7} -> {8} -> {9}", photonView.ViewID, xAxis, yAxis, zAxis, colorDimension, sizeDimension, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(),
System.Reflection.MethodBase.GetCurrentMethod());

            if (!isWaitingForPhotonRequestStateEvent) { return; }

            Debug.LogFormat(GlobalVariables.cEvent + "_processRequestStateEventResponse: Processed, recived data = View:{0}, X Axis: {1}, Y Axis: {2}, Z Axis: {3}, Colur: {4}, Size: {5}." + GlobalVariables.endColor + " {6}: {7} -> {8} -> {9}", photonView.ViewID, xAxis, yAxis, zAxis, colorDimension, sizeDimension, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(),
System.Reflection.MethodBase.GetCurrentMethod());

            _changeXAxis(xAxis);
            _changeYAxis(yAxis);
            _changeZAxis(zAxis);
            _changeColorDimension(colorDimension);
            _changeSizeDimension(sizeDimension);
        }

        public void RequestChangeXAxisEvent(string newAxisDimension)
        {
            Debug.LogFormat(GlobalVariables.cCommon + "{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "RequestChangeXAxisEvent, New dimension: ", newAxisDimension, "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(),
System.Reflection.MethodBase.GetCurrentMethod());

            if (RPCvisualisationUpdateRequestDelegate != null)
                RPCvisualisationUpdateRequestDelegate(AbstractVisualisation.PropertyType.DimensionChange);

            if (PhotonNetwork.IsConnected)
            {
                object[] content = new object[] { photonView.ViewID, newAxisDimension };

                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

                PhotonNetwork.RaiseEvent(GlobalVariables.PhotonChangeX_AxisEvent, content, raiseEventOptions, GlobalVariables.sendOptions);
            } else
            {
                _changeXAxis(newAxisDimension);
            }
        }

        public void RequestChangeYAxisEvent(string newAxisDimension)
        {
            Debug.LogFormat(GlobalVariables.cCommon + "{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "RequestChangeYAxisEvent, New dimension: ", newAxisDimension, "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(),
System.Reflection.MethodBase.GetCurrentMethod());

            if (RPCvisualisationUpdateRequestDelegate != null)
                RPCvisualisationUpdateRequestDelegate(AbstractVisualisation.PropertyType.DimensionChange);

            if (PhotonNetwork.IsConnected)
            {
                object[] content = new object[] { photonView.ViewID, newAxisDimension };

                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

                PhotonNetwork.RaiseEvent(GlobalVariables.PhotonChangeY_AxisEvent, content, raiseEventOptions, GlobalVariables.sendOptions);
            }
            else
            {
                _changeYAxis(newAxisDimension);
            }
        }

        public void RequestChangeZAxisEvent(string newAxisDimension)
        {
            Debug.LogFormat(GlobalVariables.cCommon + "{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "RequestChangeZAxisEvent, New dimension: ", newAxisDimension, "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(),
System.Reflection.MethodBase.GetCurrentMethod());

            if (RPCvisualisationUpdateRequestDelegate != null)
                RPCvisualisationUpdateRequestDelegate(AbstractVisualisation.PropertyType.DimensionChange);

            if (PhotonNetwork.IsConnected)
            {
                object[] content = new object[] { photonView.ViewID, newAxisDimension };

                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

                PhotonNetwork.RaiseEvent(GlobalVariables.PhotonChangeZ_AxisEvent, content, raiseEventOptions, GlobalVariables.sendOptions);
            }
            else
            {
                _changeZAxis(newAxisDimension);
            }
        }

        public void RequestChangeColorDimensionEvent(string newAxisDimension)
        {
            Debug.LogFormat(GlobalVariables.cCommon + "{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "RequestChangeColorDimensionEvent, New dimension: ", newAxisDimension, "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(),
System.Reflection.MethodBase.GetCurrentMethod());

            if (RPCvisualisationUpdateRequestDelegate != null)
                RPCvisualisationUpdateRequestDelegate(AbstractVisualisation.PropertyType.DimensionChange);

            if (PhotonNetwork.IsConnected)
            {
                object[] content = new object[] { photonView.ViewID, newAxisDimension };

                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

                PhotonNetwork.RaiseEvent(GlobalVariables.PhotonChangeColorDimensionEvent, content, raiseEventOptions, GlobalVariables.sendOptions);
            }
            else
            {
                _changeColorDimension(newAxisDimension);
            }
        }

        public void RequestChangeSizeDimensionEvent(string newAxisDimension)
        {

            Debug.LogFormat(GlobalVariables.cCommon + "{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "RequestChangeSizeDimensionEvent, New dimension: ", newAxisDimension, "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(),
System.Reflection.MethodBase.GetCurrentMethod());

            if (RPCvisualisationUpdateRequestDelegate != null)
                RPCvisualisationUpdateRequestDelegate(AbstractVisualisation.PropertyType.DimensionChange);

            if (PhotonNetwork.IsConnected)
            {
                object[] content = new object[] { photonView.ViewID, newAxisDimension };

                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

                PhotonNetwork.RaiseEvent(GlobalVariables.PhotonChangeSizeDimensionEvent, content, raiseEventOptions, GlobalVariables.sendOptions);
            }
            else
            {
                _changeSizeDimension(newAxisDimension);
            }
        }

        private void _changeXAxis(string newAxisDimension)
        {
            Debug.LogFormat(GlobalVariables.cCommon + "{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "Updated Mapping", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), 
System.Reflection.MethodBase.GetCurrentMethod());
            thisVis.xDimension = newAxisDimension;
            thisVis.updateVisPropertiesSafe();
        }

        private void _changeYAxis(string newAxisDimension)
        {
            Debug.LogFormat(GlobalVariables.cCommon + "{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "Updated Mapping", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            thisVis.yDimension = newAxisDimension;
            thisVis.updateVisPropertiesSafe();
        }

        private void _changeZAxis(string newAxisDimension)
        {
            Debug.LogFormat(GlobalVariables.cCommon + "{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "Updated Mapping", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            thisVis.zDimension = newAxisDimension;
            thisVis.updateVisPropertiesSafe();
        }

        private void _changeColorDimension(string newColorDimension)
        {
            Debug.LogFormat(GlobalVariables.cCommon + "{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "Updated Mapping", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            Color colorStart = Color.blue;
            Color colorEnd = Color.red;

            thisVis.dimensionColour = HelperFunctions.getColorGradient(colorStart, colorEnd);
            thisVis.colourDimension = newColorDimension;
            thisVis.updateVisPropertiesSafe();
        }

        private void _changeSizeDimension(string newAxisDimension)
        {
            Debug.LogFormat(GlobalVariables.cCommon + "{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "Updated Mapping", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            thisVis.sizeDimension = newAxisDimension;
            thisVis.updateVisPropertiesSafe();
        }

    }
}
