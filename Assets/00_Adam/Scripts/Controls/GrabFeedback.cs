using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;

namespace Photon_IATK
{
    /// <summary>
    /// When attached this class will provide a material change event that can be triggered by adding it to the "on manipulation started/stopped" events in the GUI for the manipulation handler script.
    /// </summary>

    [RequireComponent(typeof(Photon.Pun.PhotonView))]
    [DisallowMultipleComponent]
    public class GrabFeedback : MonoBehaviourPun
    {

        public Material grabbedMaterial;
        public Material grabbedRotateMaterial;
        public Material grabbeScaleMaterial;
        private Dictionary<Renderer, Material> renderersAndMats;
        private bool isSecondAttempt = false;

        private void OnEnable()
        {
            PhotonNetwork.NetworkingClient.EventReceived += OnEvent;

            Debug.LogFormat(GlobalVariables.cRegister + "Annotation registering OnEvent.{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }

        private void OnDisable()
        {
            Debug.LogFormat(GlobalVariables.cRegister + "Annotation unregistering OnEvent.{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
        }


        void Start()
        {
            if (grabbedMaterial == null)
            {
                grabbedMaterial = Resources.Load("GrabFeedback", typeof(Material)) as Material;
            }

            if (grabbedRotateMaterial == null)
            {
                grabbedRotateMaterial = Resources.Load("GrabHandleFeedback", typeof(Material)) as Material;
            }

            if (grabbeScaleMaterial == null)
            {
                grabbeScaleMaterial = Resources.Load("GrabScaleFeedback", typeof(Material)) as Material;
            }


            setupRenderers();

        }

        #region Events
        private void OnEvent(EventData photonEventData)
        {
            byte eventCode = photonEventData.Code;

            //Check that the event was one we made, photon reserves 0, 200+
            if (eventCode == 0 || eventCode > 199) { return; }

            object[] data = (object[])photonEventData.CustomData;
            int callerPhotonViewID = (int)data[0];

            //Debug.Log("reciving event: " + eventCode);

            //make sure that this object is the same as the sender object
            if (photonView.ViewID != callerPhotonViewID) { return; }

            switch (eventCode)
            {
                case GlobalVariables.RequestGrabEvent:
                    _grabedEvent();
                    break;
                case GlobalVariables.RequestReleaseEvent:
                    _ReleasedEvent();
                    break;
                case GlobalVariables.RequestGrabHandleEvent:
                    _grabedHandleEvent();
                    break;
                case GlobalVariables.RequestReleaseHandleEvent:
                    _ReleasedHandleEvent();
                    break;
                case GlobalVariables.RequestGrabScaleEvent:
                    _grabedScaleEvent();
                    break;
                default:
                    break;
            }

        }

        #endregion //events

        private void setupRenderers()
        {
            if (renderersAndMats == null)
            {
                renderersAndMats = new Dictionary<Renderer, Material>();

                Transform[] ts = this.transform.GetComponentsInChildren<Transform>(true);
                foreach (Transform t in ts)
                {
                    if (t.GetComponent<GrabFeedbackTarget>() == null) continue;
                    Renderer renderer = t.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        Material material = renderer.material;
                        if (material != null)
                        {
                            renderersAndMats.Add(renderer, renderer.material);
                        }
                    }
                }
            }

            if (renderersAndMats == null)
            {
                Transform[] ts = this.transform.GetComponentsInChildren<Transform>(true);
                foreach (Transform t in ts)
                {
                    if (t.GetComponent<GrabFeedbackTarget>() == null) continue;
                    LineRenderer renderer = t.GetComponent<LineRenderer>();
                    if (renderer != null)
                    {
                        Material material = renderer.material;
                        if (material != null)
                        {
                            renderersAndMats.Add(renderer, renderer.material);
                        }
                    }
                }
            }
        }

        public void Grabbed()
        {

            Debug.LogFormat(GlobalVariables.cEvent + "Any ~ Calling: {0}, Receivers: {1}, My Name: {2}, I am the Master Client: {3}, Server Time: {4}, Sending Event Code: {5}{6}{7}{8}." + GlobalVariables.endColor + " {9}: {10} -> {11} -> {12}", "RequestGrabEvent", "all", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, GlobalVariables.RequestGrabEvent, "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            object[] content = new object[] { photonView.ViewID, PhotonNetwork.NickName };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

            PhotonNetwork.RaiseEvent(GlobalVariables.RequestGrabEvent, content, raiseEventOptions, GlobalVariables.sendOptions);

            PhotonNetwork.SendAllOutgoingCommands();

            //_grabbed();
        }

        private void _grabedEvent()
        {
            Debug.LogFormat(GlobalVariables.cEvent + "Recived Code: {0}{1}, My Name: {2}, I am the Master Client: {3}, Server Time: {4}{5}, Recipents: {6}{7}{8}." + GlobalVariables.endColor + " {9}: {10} -> {11} -> {12}", GlobalVariables.RequestGrabEvent, "", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, "", "all", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            _grabbed();
        }

        public void _grabbed()
        {
            foreach (Renderer key in renderersAndMats.Keys)
            {
                key.material = grabbedMaterial;
            }
        }

        public void GrabbedHandle()
        {

            Debug.LogFormat(GlobalVariables.cEvent + "Any ~ Calling: {0}, Receivers: {1}, My Name: {2}, I am the Master Client: {3}, Server Time: {4}, Sending Event Code: {5}{6}{7}{8}." + GlobalVariables.endColor + " {9}: {10} -> {11} -> {12}", "RequestGrabEvent", "all", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, GlobalVariables.RequestGrabHandleEvent, "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            object[] content = new object[] { photonView.ViewID, PhotonNetwork.NickName };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

            PhotonNetwork.RaiseEvent(GlobalVariables.RequestGrabHandleEvent, content, raiseEventOptions, GlobalVariables.sendOptions);

            PhotonNetwork.SendAllOutgoingCommands();

            //_grabbed();
        }

        private void _grabedHandleEvent()
        {
            Debug.LogFormat(GlobalVariables.cEvent + "Recived Code: {0}{1}, My Name: {2}, I am the Master Client: {3}, Server Time: {4}{5}, Recipents: {6}{7}{8}." + GlobalVariables.endColor + " {9}: {10} -> {11} -> {12}", GlobalVariables.RequestGrabEvent, "", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, "", "all", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            _grabbedHandle();
        }

        public void _grabbedHandle()
        {
            foreach (Renderer key in renderersAndMats.Keys)
            {
                key.material = grabbedRotateMaterial;
            }
        }

        public void GrabbedScale()
        {

            Debug.LogFormat(GlobalVariables.cEvent + "Any ~ Calling: {0}, Receivers: {1}, My Name: {2}, I am the Master Client: {3}, Server Time: {4}, Sending Event Code: {5}{6}{7}{8}." + GlobalVariables.endColor + " {9}: {10} -> {11} -> {12}", "RequestGrabScaleEvent", "all", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, GlobalVariables.RequestGrabScaleEvent, "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            object[] content = new object[] { photonView.ViewID, PhotonNetwork.NickName };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

            PhotonNetwork.RaiseEvent(GlobalVariables.RequestGrabScaleEvent, content, raiseEventOptions, GlobalVariables.sendOptions);

            PhotonNetwork.SendAllOutgoingCommands();

            //_grabbed();
        }

        private void _grabedScaleEvent()
        {
            Debug.LogFormat(GlobalVariables.cEvent + "Recived Code: {0}{1}, My Name: {2}, I am the Master Client: {3}, Server Time: {4}{5}, Recipents: {6}{7}{8}." + GlobalVariables.endColor + " {9}: {10} -> {11} -> {12}", GlobalVariables.RequestGrabEvent, "", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, "", "all", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            _grabbedScaleHandle();
        }

        public void _grabbedScaleHandle()
        {
            foreach (Renderer key in renderersAndMats.Keys)
            {
                key.material = grabbeScaleMaterial;
            }
        }

        public void Released()
        {
            Debug.LogFormat(GlobalVariables.cEvent + "Any ~ Calling: {0}, Receivers: {1}, My Name: {2}, I am the Master Client: {3}, Server Time: {4}, Sending Event Code: {5}{6}{7}{8}." + GlobalVariables.endColor + " {9}: {10} -> {11} -> {12}", "RequestReleaseEvent", "Others", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, GlobalVariables.RequestReleaseHandleEvent, "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            object[] content = new object[] { photonView.ViewID, PhotonNetwork.NickName };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

            PhotonNetwork.RaiseEvent(GlobalVariables.RequestReleaseEvent, content, raiseEventOptions, GlobalVariables.sendOptions);

            PhotonNetwork.SendAllOutgoingCommands();

            //_released();
        }

        private void _ReleasedEvent()
        {
            Debug.LogFormat(GlobalVariables.cEvent + "Recived Code: {0}{1}, My Name: {2}, I am the Master Client: {3}, Server Time: {4}{5}, Recipents: {6}{7}{8}." + GlobalVariables.endColor + " {9}: {10} -> {11} -> {12}", GlobalVariables.RequestReleaseEvent, "", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, "", "Others", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            _released();
        }

        private void _released()
        {
            foreach (Renderer key in renderersAndMats.Keys)
            {
                key.material = renderersAndMats[key];
            }
        }

        public void ReleasedHandle()
        {
            Debug.LogFormat(GlobalVariables.cEvent + "Any ~ Calling: {0}, Receivers: {1}, My Name: {2}, I am the Master Client: {3}, Server Time: {4}, Sending Event Code: {5}{6}{7}{8}." + GlobalVariables.endColor + " {9}: {10} -> {11} -> {12}", "RequestReleaseEvent", "Others", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, GlobalVariables.RequestReleaseEvent, "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            object[] content = new object[] { photonView.ViewID, PhotonNetwork.NickName };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

            PhotonNetwork.RaiseEvent(GlobalVariables.RequestReleaseHandleEvent, content, raiseEventOptions, GlobalVariables.sendOptions);

            PhotonNetwork.SendAllOutgoingCommands();

            //_released();
        }

        private void _ReleasedHandleEvent()
        {
            Debug.LogFormat(GlobalVariables.cEvent + "Recived Code: {0}{1}, My Name: {2}, I am the Master Client: {3}, Server Time: {4}{5}, Recipents: {6}{7}{8}." + GlobalVariables.endColor + " {9}: {10} -> {11} -> {12}", GlobalVariables.RequestReleaseEvent, "", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, "", "Others", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            _releasedHandle();
        }

        private void _releasedHandle()
        {
            foreach (Renderer key in renderersAndMats.Keys)
            {
                key.material = renderersAndMats[key];
            }
        }

        private static void ApplyMaterialToAllRenderers(GameObject root, Material material)
        {
            if (material != null)
            {
                Renderer[] renderers = root.GetComponentsInChildren<Renderer>();

                for (int i = 0; i < renderers.Length; ++i)
                {
                    renderers[i].material = material;
                }
            }
        }
    }
}
