using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;
using System;
using System.Linq;

namespace Photon_IATK
{
    /// <summary>
    /// This class is incharge of photon scene instantiation and destruction
    /// </summary>
    [RequireComponent(typeof(Photon.Pun.PhotonView))]
    public class GeneralEventManager : MonoBehaviourPun
    {
        public static GeneralEventManager instance;
        public bool isElictationOnPC = false;

        public bool showingGameControllers = true;
        public bool showingExtras = false;
        public bool showingTrackers = false;

        #region Setup

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                Debug.LogFormat(GlobalVariables.cCommon + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "GeneralEventManager Set", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            }
            else if(instance != this)
            {
                Destroy(instance.gameObject);
                instance = this;
                Debug.LogFormat(GlobalVariables.cCommon + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "GeneralEventManager Destoryed then Set", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        private void OnEnable()
        {
            Debug.LogFormat(GlobalVariables.cRegister + "GeneralEventManager registering OnEvent.{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
        }

        private void OnDisable()
        {
            Debug.LogFormat(GlobalVariables.cRegister + "GeneralEventManager unregistering OnEvent.{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
        }
        #endregion

        /// <summary>
        /// Checks the relavance of the event then routes the event to the right funciton.
        /// Data = Object[]
        /// This will recive events that are not specific to the photon view of the sender
        /// </summary>
        private void OnEvent(EventData photonEventData)
        {
            byte eventCode = photonEventData.Code;

            //Check that the event was one we made, photon reserves 0, 200+
            if (eventCode == 0 || eventCode > 199) { return; }

            object[] data = (object[])photonEventData.CustomData;

            //route the event
            switch (eventCode)
            {
                case GlobalVariables.PhotonVisSceneInstantiateEvent:
                    PhotonProcessVisSceneInstantiateEvent(data);
                    break;
                case GlobalVariables.PhotonDeleteAllObjectsWithComponentEvent:
                    PhotonProcessDeleteAllObjectsWithComponentEvent(data);
                    break;
                case GlobalVariables.PhotonDeleteSingleObjectsWithViewIDEvent:
                    PhotonProcessDeleteSingleObjectsWithViewEvent(data);
                    break;
                case GlobalVariables.PhotonRequestLatencyCheckEvent:
                    SendResponseToLatencyCheckEvent(data);
                    break;
                case GlobalVariables.PhotonRequestLatencyCheckResponseEvent:
                    PhotonProcessRequestLatencyCheckResponseEvent(data);
                    break;
                case GlobalVariables.RequestElicitationSetupEvent:
                    ElicitationSetUpEvent();
                    Debug.Log("RequestElicitationSetupEvent");
                    break;
                default:
                    break;
            }
        }

        #region Send Events

        /// <summary>
        ///Send a request to all clients to return their latency
        /// Sent Data = { photonView.ViewID, PhotonNetwork.ServerTimestamp, PhotonNetwork.NickName, PhotonNetwork.LocalPlayer.UserId };
        /// </summary>
        public void SendLatencyCheckEvent()
        {
            if (!PhotonNetwork.IsConnected) { return; }
            Debug.LogFormat(GlobalVariables.cEvent + "{0}Any ~ {1}, Receivers: {2}, My Name: {3}, I am the Master Client: {4}, Server Time: {5}, Sending Event Code: {6}{7}{8}{9}." + GlobalVariables.endColor + " {10}: {11} -> {12} -> {13}", "", "Requesting Latency Check", "MasterClient", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, GlobalVariables.PhotonRequestLatencyCheckEvent, "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            int timesToCheck = 1;
            for (int i = 0; i < timesToCheck; i++)
            {
                object[] content = new object[] { photonView.ViewID, PhotonNetwork.ServerTimestamp, PhotonNetwork.NickName, PhotonNetwork.LocalPlayer.UserId };

                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All}; //Will not recived own message

                PhotonNetwork.RaiseEvent(GlobalVariables.PhotonRequestLatencyCheckEvent, content, raiseEventOptions, GlobalVariables.sendOptions);

                PhotonNetwork.SendAllOutgoingCommands();

            }
        }

        /// <summary>
        ///Send a request to all clients to return their latency
        /// Recived Data = { photonView.ViewID, PhotonNetwork.ServerTimestamp, PhotonNetwork.NickName, PhotonNetwork.LocalPlayer.UserId };
        /// Sent Data =  { photonView.ViewID, requestTime, requesterNickName, requesterUserID, PhotonNetwork.ServerTimestamp, PhotonNetwork.NickName, PhotonNetwork.LocalPlayer.UserId };
        /// </summary>
        public void SendResponseToLatencyCheckEvent(object[] data)
        {
            if (!PhotonNetwork.IsConnected) { return; }
            Debug.LogFormat(GlobalVariables.cEvent + "Recived Event {0}: Any ~ {1}, Receivers: {2}, My Name: {3}, I am the Master Client: {4}, Server Time: {5}, Sending Event Code: {6}{7}{8}{9}." + GlobalVariables.endColor + " {10}: {11} -> {12} -> {13}", GlobalVariables.PhotonRequestLatencyCheckEvent, "Responding to Latency Check", "MasterClient", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, GlobalVariables.PhotonRequestLatencyCheckResponseEvent, "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            int requestTime = (int)data[1];
            string requesterNickName = (string)data[2];
            string requesterUserID = (string)data[3];

            object[] content = new object[] { photonView.ViewID, requestTime, requesterNickName, requesterUserID, PhotonNetwork.ServerTimestamp, PhotonNetwork.NickName, PhotonNetwork.LocalPlayer.UserId };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; //Will not recived own message

            PhotonNetwork.RaiseEvent(GlobalVariables.PhotonRequestLatencyCheckResponseEvent, content, raiseEventOptions, GlobalVariables.sendOptions);

            PhotonNetwork.SendAllOutgoingCommands();
        }

        public void SendVisSceneInstantiateEvent()
        {
            GameObject obj;

            if (!PhotonNetwork.IsConnected)
            {
                Debug.LogFormat(GlobalVariables.cEvent + "SendVisSceneInstantiateEvent() triggered but you are offline, loading Vis offline{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                GameObject prefab = Resources.Load("Vis") as GameObject;
                //GameObject prefabAnnotationStation = Resources.Load("AnnotationStation") as GameObject;
                GameObject prefabAnnotationStation = Resources.Load("AnnotationStationBtns") as GameObject;
                GameObject prefabTrashCube = Resources.Load("TrashCube") as GameObject;

                obj = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
                obj = Instantiate(prefabAnnotationStation, new Vector3(0, 0, 0), Quaternion.identity);
                obj = Instantiate(prefabTrashCube, new Vector3(0, 0, 0), Quaternion.identity);
            } 
            else
            {
                Debug.LogFormat(GlobalVariables.cEvent + "{0}Any ~ {1}, Receivers: {2}, My Name: {3}, I am the Master Client: {4}, Server Time: {5}, Sending Event Code: {6}{7}{8}{9}." + GlobalVariables.endColor + " {10}: {11} -> {12} -> {13}", "", "Requesting Vis Instantiate", "MasterClient", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, GlobalVariables.PhotonVisSceneInstantiateEvent, "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                object[] content = new object[] { photonView.ViewID, PhotonNetwork.NickName };

                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient }; //Will not recived own message

                PhotonNetwork.RaiseEvent(GlobalVariables.PhotonVisSceneInstantiateEvent, content, raiseEventOptions, GlobalVariables.sendOptions);
            }
        }

        public void SendDeleteAllObjectsWithComponentRequest(string className)
        {
            object[] data = new object[] { photonView.ViewID, className, PhotonNetwork.NickName };

            if (!PhotonNetwork.IsConnected)
            {
                PhotonProcessDeleteAllObjectsWithComponentEvent(data);
            }
            else
            {                
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient }; //Will not recived own message

                PhotonNetwork.RaiseEvent(GlobalVariables.PhotonDeleteAllObjectsWithComponentEvent, data, raiseEventOptions, GlobalVariables.sendOptions);
            }
        }

        public void SendDeleteSingleObjectRequest(GameObject obj)
        {


            //is connect and has veiw
            if (PhotonNetwork.IsConnected && obj.GetComponent<PhotonView>() != null)
            {
                object[] data = new object[] { photonView.ViewID, obj.GetComponent<PhotonView>().ViewID, PhotonNetwork.NickName, obj.name };

                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };

                PhotonNetwork.RaiseEvent(GlobalVariables.PhotonDeleteSingleObjectsWithViewIDEvent, data, raiseEventOptions, GlobalVariables.sendOptions);
            }
            else
            {
                SafeDestory(obj);
            }
        }

        /// <summary>
        /// Using an RPC so that new clients recive the call on loggin in
        /// </summary>
        public void SendSetupElicitatoinPCRequest()
        {
            if (PhotonNetwork.IsConnected)
            {
                //RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; //Will not recived own message

                //object[] content = new object[] { photonView.ViewID };

                //PhotonNetwork.RaiseEvent(GlobalVariables.RequestElicitationSetupEvent, content, raiseEventOptions, GlobalVariables.sendOptions);
                PhotonView photonView = PhotonView.Get(this);

                photonView.RPC("ElicitationSetUpEvent", RpcTarget.All);
            }
            else
            {
                ElicitationSetUpEvent();
            }
        }

        #endregion

        #region Receive Events

        [PunRPC]
        public void ElicitationSetUpEvent()
        {
            // All clients now have this flagged.
            isElictationOnPC = true;
#if !HL2
            Debug.LogFormat(GlobalVariables.cAlert + "{0}{1}{2}{3}" + GlobalVariables.endColor + " {4}: {5} -> {6} -> {7}", "Setup elicitation environment requested, doing nothing", "", "", "", this.name, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
#endif

#if HL2
            Debug.LogFormat(GlobalVariables.cAlert + "{0}{1}{2}{3}" + GlobalVariables.endColor + " {4}: {5} -> {6} -> {7}", "Setting up elicitation environment on HoloLens", "", "", "", this.name, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            RemoveVisItemsForElicitPC();
            DisableManipulations();



#endif
        }

        public void DisableManipulations()
        {
            var manipulatorScripts = FindObjectsOfType<Microsoft.MixedReality.Toolkit.UI.ObjectManipulator>();
            foreach (var script in manipulatorScripts)
            {
                script.ManipulationType = 0;
            }

            var manipulationControlsScripts = FindObjectsOfType<ManipulationControls>();
            foreach (var script in manipulationControlsScripts)
            {
                script.BoundingBoxActivation = ManipulationControls.BoundingBoxActivationType.ActivateManually;
            }
        }


        public void RemoveVisItemsForElicitPC()
        {
            //if exists find and remove
            //remove menu
            //remove platform
            GameObject[] taggedItems = GameObject.FindGameObjectsWithTag(GlobalVariables.pcElicitTag);
            foreach (GameObject obj in taggedItems)
            {
                Destroy(obj);
            }
        }

        private List<int> timesRequestToReturned = new List<int> { };
        private List<int> timesReturnedToRecived = new List<int> { };
        private List<int> timesRoundTrip = new List<int> { };
        /// <summary>
        ///Records all of the client latency information
        /// Recived Data = { photonView.ViewID, requestTime, requesterNickName, requesterUserID, PhotonNetwork.ServerTimestamp, PhotonNetwork.NickName, PhotonNetwork.LocalPlayer.UserId };
        /// </summary>
        private void PhotonProcessRequestLatencyCheckResponseEvent(object[] data)
        {

            int recivedTime = PhotonNetwork.ServerTimestamp;
            int requestTime = (int)data[1];
            int returnedTime = (int)data[4];

            int timeRequestToReturned = returnedTime - requestTime;
            int timeReturnedToRecived = recivedTime - returnedTime;
            int timeRoundTrip = recivedTime - requestTime;

            timesRequestToReturned.Add(timeRequestToReturned);
            timesReturnedToRecived.Add(timeReturnedToRecived);
            timesRoundTrip.Add(timeRoundTrip);

            string requesterNickName = (string)data[2];
            string requesterPlayerID = (string)data[3];
            string responderNickName = (string)data[5];
            string responderPlayerID = (string)data[6];

            Debug.LogFormat(GlobalVariables.cEvent + "Recived Code {0}: Any ~ {1}, My Name: {3}, I am the Master Client: {4}, Server Time: {5}{6}{7}{8}{9}." + GlobalVariables.endColor + " {10}: {11} -> {12} -> {13}", GlobalVariables.PhotonRequestLatencyCheckResponseEvent, "Recording Latency", "", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, "", "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            Debug.LogFormat(GlobalVariables.cAlert + "Time from request to response: {0}, requester: {1}, responder: {2}. Time from response to here: {3}, reponder: {4}, here: {5}, Time total: {6}, Start: {7}, first leg: {8}, end: {9}." + GlobalVariables.endColor + "Requester ID: {10}, Responder ID: {11}, My ID: {12}, {13} -> {14} -> {15} -> {16}, PING: {17}", timeRequestToReturned, requesterNickName, responderNickName, timeReturnedToRecived, responderNickName, PhotonNetwork.NickName, timeRoundTrip, requesterNickName, responderNickName, PhotonNetwork.NickName, requesterPlayerID, responderPlayerID, PhotonNetwork.LocalPlayer.UserId, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod(), PhotonNetwork.GetPing());

            double averageTimeRequestToReturned = timesRequestToReturned.Average();
            double averageTimeReturnedToRecived = timesReturnedToRecived.Average();
            double averageTimeRoundTrip = timesRoundTrip.Average();

            Debug.LogFormat(GlobalVariables.cAlert + "Average time request to response: {0}, average time returned to recived: {1}, average time total: {2}, Count of samples: {3}" + GlobalVariables.endColor, averageTimeRequestToReturned, averageTimeReturnedToRecived, averageTimeRoundTrip, timesRoundTrip.Count);
        }

        private void PhotonProcessVisSceneInstantiateEvent(object[] data)
        {
            Debug.LogFormat(GlobalVariables.cEvent + "Recived Code {0}: Master ~ {1}, My Name: {3}, I am the Master Client: {4}, Server Time: {5}{6}{7}{8}{9}." + GlobalVariables.endColor + " {10}: {11} -> {12} -> {13}", GlobalVariables.PhotonVisSceneInstantiateEvent, "InstantiateRoomObject Vis", "", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, "", "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            GameObject visObj;
            GameObject annotationObj;
            GameObject trashCube;


            visObj = PhotonNetwork.InstantiateRoomObject("Vis", Vector3.zero, Quaternion.identity);
            annotationObj = PhotonNetwork.InstantiateRoomObject("AnnotationStationBtns", Vector3.zero, Quaternion.identity);
            trashCube = PhotonNetwork.InstantiateRoomObject("TrashCube", Vector3.zero, Quaternion.identity);


        }

        //delete vis
        private void PhotonProcessDeleteAllObjectsWithComponentEvent(object[] data)
        {
            string className = (string)data[1];
            var type = Type.GetType(className);

            var obejectsWithComponent = FindObjectsOfType(type);

            if (obejectsWithComponent.Length == 0)
            {
                return;
            }

            foreach (Component _component in obejectsWithComponent)
            {
                SafeDestory(_component);
            }
        }

        private void PhotonProcessDeleteSingleObjectsWithViewEvent(object[] data)
        {
            int photonViewID = (int)data[1];
            PhotonNetwork.Destroy(PhotonView.Find(photonViewID).gameObject);
        }

        //instantiate annotation

        //delete annotation

        //save annotation

        //load annotation

        //delete all annotaitons

        #endregion

        private void SafeDestory(Component obj)
        {
            //check if photonconnected
            //check is has view
            if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient && obj.GetComponent<PhotonView>() != null)
            {
                try
                {
                    Photon.Pun.PhotonNetwork.Destroy(obj.gameObject);

                    Debug.LogFormat(GlobalVariables.cOnDestory + "Photon Destorying: {0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", obj.gameObject.name, "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                }
                catch (System.Exception e)
                {
                    Debug.LogFormat(GlobalVariables.cError + "Error Photon Destorying: {0}, E: {1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", obj.gameObject.name, e.Message, "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                }
            }
            else
            {
                Debug.LogFormat(GlobalVariables.cOnDestory + "Offline Destorying: {0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", obj.gameObject.name, "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                Destroy(obj.gameObject);
            }
        }

        private void SafeDestory(GameObject obj)
        {

                //check if photonconnected
                //check is has view
                if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient && obj.GetComponent<PhotonView>() != null)
                {
                    try
                    {
                        Photon.Pun.PhotonNetwork.Destroy(obj);

                        Debug.LogFormat(GlobalVariables.cOnDestory + "Photon Destorying: {0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", obj.gameObject.name, "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogFormat(GlobalVariables.cError + "Error Photon Destorying: {0}, E: {1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", obj.gameObject.name, e.Message, "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                    }
                }
                else
                {
                    Debug.LogFormat(GlobalVariables.cOnDestory + "Offline Destorying: {0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", obj.gameObject.name, "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                    Destroy(obj);
                }
            }

        }
    }


