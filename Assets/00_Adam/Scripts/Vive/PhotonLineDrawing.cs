using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Photon.Pun;

namespace Photon_IATK
{
    public class PhotonLineDrawing : MonoBehaviourPun, IPunObservable
    {
        public LineRenderer lineRenderer;
        public MeshCollider meshCollider;
    
        [SerializeField] public bool isUser = default;

        private Vector3 networkLocalPosition;
        private Quaternion networkLocalRotation;

        private Vector3 startingLocalPosition;
        private Quaternion startingLocalRotation;

        private Vector3 NewPoint;
        private Vector3 oldPoint;

        [Header("Line Settings")]
        [SerializeField]
        private bool _useAnalog = true;

        [SerializeField]
        private Material _lineMaterial;

        public float _maxLineWidth = .005f;

        private DrawingVariables drawingVariables;

        private const float TimeInterval = 0f;

        private float _timer = 0f;
        private LineRenderer _currentLine = null;

        [SerializeField]
        private Transform _drawingParent;
        //playspace anchor for synced views

        private Vector3 _lastPosition = Vector3.zero;
        private const float MinimalDrawingDistance = 0.001f;

        [Header("Line Smoothing")]
        [SerializeField]
        private bool _isSmoothingActive = true;

        [SerializeField, Range(0, 11)]
        private int _windowSize = 2;

        private Vector3[] _lastPositionsBuffer;



        void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(NewPoint);
            }
            else
            {
                NewPoint = (Vector3)stream.ReceiveNext();
            }
        }

        private void Awake()
        {
            if (!photonView.IsMine)
            {
                Initalize();
            }
        }

        public void Initalize()
        {

            if (PlayspaceAnchor.Instance != null)
            {
                transform.parent = FindObjectOfType<PlayspaceAnchor>().transform;
                Debug.Log(GlobalVariables.green + "Parenting: " + this.gameObject.name + " in " + transform.parent.name + GlobalVariables.endColor + " : " + "Start()" + " : " + this.GetType());
            }
            else
            {
                Debug.Log(GlobalVariables.red + "No Playspace anchor exists, nothing parented" + GlobalVariables.endColor + " : " + "Start()" + " : " + this.GetType());
            }



            var trans = transform;
            startingLocalPosition = trans.localPosition;
            startingLocalRotation = trans.localRotation;

            networkLocalPosition = startingLocalPosition;
            networkLocalRotation = startingLocalRotation;

            _lastPosition = Vector3.zero;
            _currentLine = lineRenderer;

            _currentLine.material = new Material(Shader.Find("Sprites/Default")); ;
            _currentLine.material.color = DrawingVariables.Instance.currentColor;
            _currentLine.widthMultiplier = .005f;
            _currentLine.positionCount = 0;
            _currentLine.useWorldSpace = false;
            _currentLine.startWidth = 0.005f;
            _currentLine.endWidth = 0.005f;

        }


        public void addPoint(Vector3 pointToAdd)
        {
            NewPoint = pointToAdd;

            if (NewPoint == oldPoint || NewPoint == Vector3.zero) { return; }

            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, pointToAdd);

            oldPoint = NewPoint;
        }

        // private void FixedUpdate()
        private void FixedUpdate()
        {
            if (!photonView.IsMine && PhotonNetwork.IsConnected)
            {
                GameObject pen = GameObject.FindGameObjectWithTag("GameController");

                addPoint(NewPoint);
            }
        }

    }

}
