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

        private WidthCurve _currentWidthCurve;
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
                stream.SendNext(transform.localPosition);
                stream.SendNext(transform.localRotation);
                stream.SendNext(NewPoint);
            }
            else
            {
                networkLocalPosition = (Vector3)stream.ReceiveNext();
                networkLocalRotation = (Quaternion)stream.ReceiveNext();
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
            _currentWidthCurve = new WidthCurve(false);

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

            if (pointToAdd == oldPoint || NewPoint == Vector3.zero) { return; }

            if (_isSmoothingActive)
            {
                _lastPositionsBuffer = null;
                AddMeanPoint(lineRenderer, _currentWidthCurve, pointToAdd, 1f);
            }
            else
            {
                AddPoint(lineRenderer, _currentWidthCurve, pointToAdd, 1f);
            }


            //lineRenderer.positionCount++;
            //lineRenderer.SetPosition(lineRenderer.positionCount - 1, lineRenderer.transform.InverseTransformPoint(pointToAdd));
      
            oldPoint = pointToAdd;
        }

        // private void FixedUpdate()
        private void FixedUpdate()
        {
            if (!photonView.IsMine && PhotonNetwork.IsConnected)
            {

                addPoint(NewPoint);
                // if not the local user

                //get PhotonUser transform 
                var trans = transform;

                //move the users local position to the network position
                // the network position is the information sent from the other users local poisitons
                trans.localPosition = networkLocalPosition;
                trans.localRotation = networkLocalRotation;
            }
        }

        private void AddPoint(LineRenderer line, WidthCurve curve, Vector3 newPosition, float width)
        {
            float distance = Vector3.Distance(_lastPosition, newPosition);
            if (distance < MinimalDrawingDistance && curve.Distances.Count > 0)
            {
                line.widthCurve = curve.GetCurve();
                line.SetPosition(line.positionCount - 1, line.transform.InverseTransformPoint(newPosition));
                return;
            }
            _lastPosition = newPosition;
            curve.AddPoint(width, distance);
            line.widthCurve = curve.GetCurve();
            line.positionCount++;
            line.SetPosition(line.positionCount - 1, line.transform.InverseTransformPoint(newPosition));
        }

        private void AddMeanPoint(LineRenderer line, WidthCurve curve, Vector3 newPosition, float width)
        {
            float distance = Vector3.Distance(_lastPosition, newPosition);
            if (distance < MinimalDrawingDistance && curve.Distances.Count > 0)
            {
                line.widthCurve = curve.GetCurve();
                line.SetPosition(line.positionCount - 1, line.transform.InverseTransformPoint(newPosition));
                return;
            }
            AddPointInBuffer(_lastPosition);
            _lastPosition = newPosition;
            curve.AddPoint(width, distance);
            line.widthCurve = curve.GetCurve();
            line.positionCount++;
            line.SetPosition(line.positionCount - 1, line.transform.InverseTransformPoint(newPosition));
            ApplyMean(line);
        }

        private void ApplyMean(LineRenderer line)
        {
            int meanSize = _windowSize;
            Vector3 meanAverage = Vector3.zero;
            foreach (Vector3 elem in _lastPositionsBuffer)
            {
                if (elem == Vector3.zero)
                {
                    meanSize--;
                }
                else
                {
                    meanAverage += elem;
                }
            }
            if (meanSize > 0)
            {
                meanAverage = meanAverage / meanSize;
            }

            if (line.positionCount >= 2)
            {
                line.SetPosition(line.positionCount - 2, line.transform.InverseTransformPoint(meanAverage));
            }
        }

        public void SetSmoothingMode(bool smoothingStatus)
        {
            _isSmoothingActive = smoothingStatus;
        }




        /// <summary>
        /// Remember the last 10 points to be able to apply a mean average on each elements with it's neighbor. Keep
        /// this array as a FIFO.
        /// </summary>
        /// <param name="newPosition">The new latest element.</param>
        private void AddPointInBuffer(Vector3 newPosition)
        {
            if (_lastPositionsBuffer == null || _lastPositionsBuffer.Length != _windowSize)
            {
                _lastPositionsBuffer = new Vector3[_windowSize];
                for (int i = 0; i < _lastPositionsBuffer.Length; i++)
                {
                    _lastPositionsBuffer[i] = Vector3.zero;
                }
            }

            var tempArray = new Vector3[_windowSize];
            for (int i = 0; i < _lastPositionsBuffer.Length; i++)
            {
                if (i == _lastPositionsBuffer.Length - 1)
                {
                    tempArray[i] = newPosition;
                }
                else
                {
                    tempArray[i] = _lastPositionsBuffer[i + 1];
                }
            }
            _lastPositionsBuffer = tempArray;
        }

        /// <summary>
        /// Holds information regarding the width of the line along its path.
        /// </summary>
        private class WidthCurve
        {
            public List<float> Distances;
            private List<float> _widths;
            private bool _simulateTaper;

            public WidthCurve(bool simulateTaper)
            {
                _simulateTaper = simulateTaper;
                _widths = new List<float>();
                Distances = new List<float>();
            }

            /// <summary>
            /// Add a point at a given distance from the last one and its corresponding width to the curve representing
            /// the overall width of the line along its path.
            /// </summary>
            public void AddPoint(float width, float distance)
            {
                _widths.Add(width);
                Distances.Add(distance);
            }

            /// <summary>
            /// Use this to get the current curve that a line renderer with points corresponding to
            /// these previously added to this data structure can understand as a width curve.
            /// </summary>
            public AnimationCurve GetCurve()
            {
                if (Distances[0] == 0)
                {
                    Distances[0] = 0.005f;
                }

                Debug.Assert(Distances.Count == _widths.Count, "Both lists should have the same length");
                //Debug.Assert(Distances[0] == 0, "The first length should be zero");

                if (_widths.Count == 0)
                {
                    Debug.LogError("Cannot get you a curve with no points.");
                    return null;
                }

                float totalDistance = Distances.Aggregate((sum, next) => sum + next);

                if (_simulateTaper)
                {
                    return TaperSimulation(0.05f / totalDistance);
                }
                else
                {
                    var keyframes = new Keyframe[_widths.Count];
                    float distanceToThisPoint = 0f;
                    for (int i = 0; i < keyframes.Length; i++)
                    {
                        distanceToThisPoint += Distances[i];
                        keyframes[i] = new Keyframe(distanceToThisPoint / totalDistance, _widths[i]);
                    }
                    return new AnimationCurve(keyframes);
                }
            }

            private static AnimationCurve TaperSimulation(float taperProportion)
            {
                if (taperProportion > 0.5f)
                {
                    taperProportion = 0.5f;
                }

                Keyframe[] easeIn = AnimationCurve.EaseInOut(0f, 0f, taperProportion, 1f).keys;
                Keyframe[] constantBit = AnimationCurve.Constant(taperProportion, 1f - taperProportion, 1f).keys;
                Keyframe[] easeOut = AnimationCurve.EaseInOut(1f - taperProportion, 1f, 1f, 0f).keys;

                var keyframes = new Keyframe[easeIn.Length + constantBit.Length + easeOut.Length];
                easeIn.CopyTo(keyframes, 0);
                constantBit.CopyTo(keyframes, easeIn.Length);
                easeOut.CopyTo(keyframes, easeIn.Length + constantBit.Length);

                return new AnimationCurve(keyframes);
            }
        }
    }

}
