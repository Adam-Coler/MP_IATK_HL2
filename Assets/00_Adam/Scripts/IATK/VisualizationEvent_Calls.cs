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
        public PhotonView thisPhotonView;

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

        public string getVisDimension(AbstractVisualisation.PropertyType property)
        {
            switch (property){
                case (AbstractVisualisation.PropertyType.X):
                    return xDimension;
                    break;
                case (AbstractVisualisation.PropertyType.Y):
                    return yDimension;
                    break;
                case (AbstractVisualisation.PropertyType.Z):
                    return zDimension;
                    break;
                case (AbstractVisualisation.PropertyType.Colour):
                    return colourDimension;
                    break;
                case (AbstractVisualisation.PropertyType.Size):
                    return sizeDimension;
                    break;
                default:
                    return "Null";
                    break;
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
                Debug.LogFormat(GlobalVariables.cEvent + "{0}Client ~ {1}, Receivers: {2}, My Name: {3}, I am the Master Client: {4}, Server Time: {5}, Sending Event Code: {6}{7}{8}{9}." + GlobalVariables.endColor + " {10}: {11} -> {12} -> {13}", "", "Requesting Vis State", "MasterClient", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, GlobalVariables.PhotonRequestStateEvent, "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

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


        private void _RPCvisualisationUpdateRequestDelegate()
        {
            if (RPCvisualisationUpdateRequestDelegate != null)
            {
                Debug.Log("RPCvisualisationUpdateRequestDelegate from VisEventCalls");
                RPCvisualisationUpdateRequestDelegate(AbstractVisualisation.PropertyType.DimensionChange);
            }
        }

        private void OnEvent(EventData photonEventData)
        {
            byte eventCode = photonEventData.Code;

            //Check that the event was one we made, photon reserves 0, 200+
            if (eventCode == 0 || eventCode > 199) { return; }

            object[] data = (object[])photonEventData.CustomData;
            int callerPhotonViewID = (int)data[0];
            if (photonView.ViewID != callerPhotonViewID) { return; }

            AbstractVisualisation.PropertyType propertyType = AbstractVisualisation.PropertyType.GeometryType;

            switch (eventCode)
            {
                case GlobalVariables.PhotonChangeX_AxisEvent:
                    _RPCvisualisationUpdateRequestDelegate();
                    _changeXAxis((string)data[1]);
                    Debug.Log("PhotonChangeX_AxisEvent");
                    propertyType = AbstractVisualisation.PropertyType.X;
                    break;
                case GlobalVariables.PhotonChangeY_AxisEvent:
                    _RPCvisualisationUpdateRequestDelegate();
                    _changeYAxis((string)data[1]);
                    Debug.Log("PhotonChangeY_AxisEvent");
                    propertyType = AbstractVisualisation.PropertyType.Y;
                    break;
                case GlobalVariables.PhotonChangeZ_AxisEvent:
                    _RPCvisualisationUpdateRequestDelegate();
                    _changeZAxis((string)data[1]);
                    propertyType = AbstractVisualisation.PropertyType.Z;
                    Debug.Log("PhotonChangeZ_AxisEvent");
                    break;
                case GlobalVariables.PhotonChangeColorDimensionEvent:
                    _changeColorDimension((string)data[1]);
                    propertyType = AbstractVisualisation.PropertyType.Colour;
                    Debug.Log("PhotonChangeSizeDimensionEvent");
                    break;
                case GlobalVariables.PhotonChangeSizeDimensionEvent:
                    _changeSizeDimension((string)data[1]);
                    propertyType = AbstractVisualisation.PropertyType.Size;
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

            //if (RPCvisualisationUpdatedDelegate != null && propertyType != AbstractVisualisation.PropertyType.GeometryType)
            //{
            //    Debug.Log("RPCvisualisationUpdatedDelegate from VisEventCalls");
            //    RPCvisualisationUpdatedDelegate(AbstractVisualisation.PropertyType.DimensionChange);
            //}



        }


        /// <summary>
        /// Sends the Vis state from the master client
        /// Data = Object[] { photonView.ViewID, xDimension, yDimension, zDimension, colourDimension, sizeDimension };
        /// </summary>
        private void _sendStateEvent()
        {
            object[] content = new object[] { photonView.ViewID, xDimension, yDimension, zDimension, colourDimension, sizeDimension };

            Debug.LogFormat(GlobalVariables.cEvent + "Recived Code {0}: Masterclient ~ {1}, Receivers: {2}, My Name: {3}, I am the Master Client: {4}, Server Time: {5}, Sending Event Code: {6} Sending Data =  X Axis: {7}, Y Axis: {8}, Z Axis: {9}, Color: {10}, Size: {11}." + GlobalVariables.endColor + " {12}: {13} -> {14} -> {15}", GlobalVariables.PhotonRespondToRequestTransformEvent, "Requesting transform from master", "Others", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, GlobalVariables.PhotonRequestStateEventResponse, xDimension, yDimension, zDimension, colourDimension, sizeDimension, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };

            PhotonNetwork.RaiseEvent(GlobalVariables.PhotonRequestStateEventResponse, content, raiseEventOptions, GlobalVariables.sendOptions);
        }

        private void _processRequestStateEventResponse(object[] data)
        {
            string xAxis = (string)data[1];
            string yAxis = (string)data[2];
            string zAxis = (string)data[3];
            string colorDimension = (string)data[4];
            string sizeDimension = (string)data[5];

            if (!isWaitingForPhotonRequestStateEvent) { return; }

            Debug.LogFormat(GlobalVariables.cEvent + "Recived Code {0}: Client ~ {1}, My Name: {2}, I am the Master Client: {3}, Server Time: {4}, Recived Data =  X Axis: {5}, Y Axis: {6}, Z Axis: {7}, Color: {8}, Size: {9}." + GlobalVariables.endColor + " {10}: {11} -> {12} -> {13}", GlobalVariables.PhotonRequestStateEventResponse, " procssing the request", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, xAxis, yAxis, zAxis, colorDimension, sizeDimension, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            thisVis.isTriggeringEvents = false;
            _changeXAxis(xAxis);
            _changeYAxis(yAxis);
            _changeZAxis(zAxis);
            _changeColorDimension(colorDimension);
            _changeSizeDimension(sizeDimension);
            thisVis.isTriggeringEvents = true;
        }

        public void RequestChangeXAxisEvent(string newAxisDimension)
        {
            if (newAxisDimension == xDimension) { return; }

            //Debug.LogFormat(GlobalVariables.cEvent + "Recived Code {0}: Masterclient ~ {1}, Receivers: {2}, My Name: {3}, I am the Master Client: {4}, Server Time: {5}, Sending Event Code: {6}{7}{8}{9}." + GlobalVariables.endColor + " {10}: {11} -> {12} -> {13}", GlobalVariables.PhotonRequestTransformEvent, "Requesting transform from master", "Others", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, GlobalVariables.PhotonRespondToRequestTransformEvent, "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            Debug.LogFormat(GlobalVariables.cCommon + "{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "RequestChangeXAxisEvent, New dimension: ", newAxisDimension, "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(),
System.Reflection.MethodBase.GetCurrentMethod());

            if (RPCvisualisationUpdateRequestDelegate != null)
                RPCvisualisationUpdateRequestDelegate(AbstractVisualisation.PropertyType.DimensionChange);

            if (PhotonNetwork.IsConnected)
            {
                object[] content = new object[] { photonView.ViewID, newAxisDimension, PhotonNetwork.NickName };

                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

                PhotonNetwork.RaiseEvent(GlobalVariables.PhotonChangeX_AxisEvent, content, raiseEventOptions, GlobalVariables.sendOptions);
            } else
            {
                _changeXAxis(newAxisDimension);
            }
        }

        public void RequestChangeYAxisEvent(string newAxisDimension)
        {
            if (newAxisDimension == yDimension) { return; }

            Debug.LogFormat(GlobalVariables.cCommon + "{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "RequestChangeYAxisEvent, New dimension: ", newAxisDimension, "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(),
System.Reflection.MethodBase.GetCurrentMethod());

            if (RPCvisualisationUpdateRequestDelegate != null)
                RPCvisualisationUpdateRequestDelegate(AbstractVisualisation.PropertyType.DimensionChange);

            if (PhotonNetwork.IsConnected)
            {
                object[] content = new object[] { photonView.ViewID, newAxisDimension , PhotonNetwork.NickName};

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
            if (newAxisDimension == zDimension) { return; }

            Debug.LogFormat(GlobalVariables.cCommon + "{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "RequestChangeZAxisEvent, New dimension: ", newAxisDimension, "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(),
System.Reflection.MethodBase.GetCurrentMethod());

            if (RPCvisualisationUpdateRequestDelegate != null)
                RPCvisualisationUpdateRequestDelegate(AbstractVisualisation.PropertyType.DimensionChange);

            if (PhotonNetwork.IsConnected)
            {
                object[] content = new object[] { photonView.ViewID, newAxisDimension, PhotonNetwork.NickName };

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
            if (newAxisDimension == colourDimension) { return; }

            Debug.LogFormat(GlobalVariables.cCommon + "{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "RequestChangeColorDimensionEvent, New dimension: ", newAxisDimension, "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(),
System.Reflection.MethodBase.GetCurrentMethod());

            if (RPCvisualisationUpdateRequestDelegate != null)
                RPCvisualisationUpdateRequestDelegate(AbstractVisualisation.PropertyType.DimensionChange);

            if (PhotonNetwork.IsConnected)
            {
                object[] content = new object[] { photonView.ViewID, newAxisDimension, PhotonNetwork.NickName };

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
            if (newAxisDimension == sizeDimension) { return; }

            Debug.LogFormat(GlobalVariables.cCommon + "{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "RequestChangeSizeDimensionEvent, New dimension: ", newAxisDimension, "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(),
System.Reflection.MethodBase.GetCurrentMethod());

            if (RPCvisualisationUpdateRequestDelegate != null)
                RPCvisualisationUpdateRequestDelegate(AbstractVisualisation.PropertyType.DimensionChange);

            if (PhotonNetwork.IsConnected)
            {
                object[] content = new object[] { photonView.ViewID, newAxisDimension, PhotonNetwork.NickName };

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
