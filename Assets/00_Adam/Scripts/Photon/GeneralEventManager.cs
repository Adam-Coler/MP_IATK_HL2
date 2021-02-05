using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;
using System;

namespace Photon_IATK
{
    /// <summary>
    /// This class is incharge of photon scene instantiation and destruction
    /// </summary>
    [RequireComponent(typeof(Photon.Pun.PhotonView))]
    public class GeneralEventManager : MonoBehaviourPun
    {
        public static GeneralEventManager instance;

        #region Setup

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                Debug.LogFormat(GlobalVariables.cCommon + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "EventManager Set", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            }
            else if(instance != this)
            {
                Destroy(instance.gameObject);
                instance = this;
                Debug.LogFormat(GlobalVariables.cCommon + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "EventManager Destoryed then Set", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        private void OnEnable()
        {
            Debug.LogFormat(GlobalVariables.cRegister + "EventManager registering OnEvent.{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
        }

        private void OnDisable()
        {
            Debug.LogFormat(GlobalVariables.cRegister + "EventManager unregistering OnEvent.{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

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
                    Debug.Log("PhotonVisSceneInstantiateEvent");
                    break;
                case GlobalVariables.PhotonDeleteAllObjectsWithComponentEvent:
                    PhotonProcessDeleteAllObjectsWithComponentEvent(data);
                    Debug.Log("PhotonDeleteAllObjectsWithComponentEvent");
                    break;
                case GlobalVariables.PhotonDeleteSingleObjectsWithViewIDEvent:
                    PhotonProcessDeleteSingleObjectsWithViewEvent(data);
                    Debug.Log("PhotonDeleteSingleObjectsWithViewIDEvent");
                    break;
                default:
                    break;
            }
        }

        #region Send Events
        public void SendVisSceneInstantiateEvent()
        {
            GameObject obj;

            if (!PhotonNetwork.IsConnected)
            {
                Debug.LogFormat(GlobalVariables.cEvent + "SendVisSceneInstantiateEvent() triggered but you are offline, loading Vis offline{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                GameObject prefab = Resources.Load("Vis") as GameObject;
                obj = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
            } 
            else
            {
                Debug.LogFormat(GlobalVariables.cEvent + "SendVisSceneInstantiateEvent() triggered raising PhotonVisSceneInstantiateEvent{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                object[] content = new object[] { photonView.ViewID };

                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient }; //Will not recived own message

                PhotonNetwork.RaiseEvent(GlobalVariables.PhotonVisSceneInstantiateEvent, content, raiseEventOptions, SendOptions.SendReliable);
            }
        }

        public void SendDeleteAllObjectsWithComponentRequest(string className)
        {
            object[] data = new object[] { photonView.ViewID, className };

            if (!PhotonNetwork.IsConnected)
            {
                PhotonProcessDeleteAllObjectsWithComponentEvent(data);
            }
            else
            {                
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient }; //Will not recived own message

                PhotonNetwork.RaiseEvent(GlobalVariables.PhotonDeleteAllObjectsWithComponentEvent, data, raiseEventOptions, SendOptions.SendReliable);
            }
        }

        public void SendDeleteSingleObjectRequest(GameObject obj)
        {


            //is connect and has veiw
            if (PhotonNetwork.IsConnected && obj.GetComponent<PhotonView>() != null)
            {
                object[] data = new object[] { photonView.ViewID, obj.GetComponent<PhotonView>().ViewID };

                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };

                PhotonNetwork.RaiseEvent(GlobalVariables.PhotonDeleteSingleObjectsWithViewIDEvent, data, raiseEventOptions, SendOptions.SendReliable);
            }
            else
            {
                SafeDestory(obj);
            }
        }

        #endregion

        #region Receive Events
        private void PhotonProcessVisSceneInstantiateEvent(object[] data)
        {
            GameObject obj;
            Debug.LogFormat(GlobalVariables.cEvent + "PhotonProcessVisSceneInstantiateEvent() triggered, procssing the request{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            obj = PhotonNetwork.InstantiateRoomObject("Vis", Vector3.zero, Quaternion.identity);
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

