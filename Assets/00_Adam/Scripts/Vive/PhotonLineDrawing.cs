﻿using UnityEngine;
using Photon.Pun;

namespace Photon_IATK
{
    [DisallowMultipleComponent]
    public class PhotonLineDrawing : MonoBehaviour
    {
        public LineRenderer lineRenderer;
        public MeshCollider meshCollider;

        private Vector3 NewPoint;
        private Vector3 oldPoint;

        [Header("Line Settings")]
        [SerializeField]
        private bool _useAnalog = true;

        [SerializeField]
        private Material _lineMaterial;

        public float _maxLineWidth = .005f;
        private LineRenderer _currentLine = null;

        private void Awake()
        {
            Initalize();
        }

        public void Initalize()
        {
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
            //pointToAdd = this.transform.InverseTransformPoint(pointToAdd);
            if (NewPoint == oldPoint || NewPoint == Vector3.zero) { return; }

            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, pointToAdd);

            oldPoint = NewPoint;
        }

    }

}
