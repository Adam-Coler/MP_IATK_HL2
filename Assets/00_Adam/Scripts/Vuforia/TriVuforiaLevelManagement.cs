#if HL2
using Vuforia;
#endif

using UnityEngine;

namespace Photon_IATK
{
    public class TriVuforiaLevelManagement : MonoBehaviour
    {

#if HL2

        public GameObject Tracker2;
        public GameObject Tracker3;
        public GameObject Tracker4;

        private Vector3 lastPositionOffset = Vector3.zero; //With my setup it looks like (0, -0.05f, -0.065f)
        private Vector3 lastRotationOffset = Vector3.zero; // (90, 0, 0)

        public Vector3 positionOffset;
        public Vector3 rotationOffset;

        private TrackableBehaviour mTrackableBehaviour;

        bool isInvert = false;

        private void Update()
        {

            if (lastRotationOffset != rotationOffset)
            {
                lastRotationOffset = rotationOffset;
                centerPlayspace();
            }

            if (lastPositionOffset != positionOffset)
            {

                lastPositionOffset = positionOffset;
                centerPlayspace();
            }
        }



        void Awake()
        {
            Debug.LogFormat(GlobalVariables.cCommon + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Vuforia setup", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            Init_Unload_Vuforia();

            TrackerManager.Instance.GetStateManager().ReassociateTrackables();

            enableDisableVuforia();

        }

        private void OnDestroy()
        {
            Debug.LogFormat(GlobalVariables.cCommon + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Vuforia takedown", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            enableDisableVuforia();

            Init_Unload_Vuforia();

            destroyVuforiaEmptyObjects();

            Debug.LogFormat(GlobalVariables.cRegister + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Un-registering Vuforia OnStatusChanged", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

#if UNITY_EDITOR
            Camera.main.clearFlags = CameraClearFlags.Skybox;
#endif
        }

        private void destroyVuforiaEmptyObjects()
        {
            if (FindObjectsOfType<ImageTargetBehaviour>() != null)
            {
                foreach (ImageTargetBehaviour obj in FindObjectsOfType<ImageTargetBehaviour>())
                {
                    if (obj.gameObject.name.Contains("New"))
                    {
                        Debug.LogFormat(GlobalVariables.cLevel + "Destorying: {0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", obj.name, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                        Destroy(obj.gameObject);

                    }
                }
            }
        }


        private Vector3 getMiddle() {
            return (Tracker2.transform.position + Tracker3.transform.position + Tracker4.transform.position) / 3;
        }

        private Quaternion getRotation()
        {
            Vector3 point2 = Tracker2.transform.position;
            Vector3 point3 = Tracker3.transform.position;
            Vector3 point4 = Tracker4.transform.position;
            Vector3 FacedPoint;

            float dist23 = Vector3.Distance(point2, point3);
            float dist24 = Vector3.Distance(point2, point4);
            float dist34 = Vector3.Distance(point3, point4);

            if (dist23 > dist24 && dist23 > dist34)
            {
                FacedPoint = point4;

            }
            else if (dist24 > dist23 && dist24 > dist34)
            {
                FacedPoint = point3;
            }
            else
            {
                FacedPoint = point2;
            }

            Vector3 a = point2;
            Vector3 b = point3;
            Vector3 c = point4;

            Vector3 side1 = b - a;
            Vector3 side2 = c - a;

            Plane p = new Plane(point2, point3, point4);
            Vector3 norm = -p.normal;

            if (isInvert) { norm = -1f * norm; }

            return Quaternion.LookRotation((FacedPoint - getMiddle()).normalized, norm);
        }

        public void isInverted()
        {
            isInvert = !isInvert;
            centerPlayspace();
        }

        public void centerPlayspace()
        {
            Debug.LogFormat(GlobalVariables.cLevel + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Centering Playspace", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            if (PlayspaceAnchor.Instance != null)
            {

                this.transform.position = getMiddle();
                this.transform.rotation = getRotation();

                //Move playspace to the tracker
                Transform playspaceAnchorTransform = PlayspaceAnchor.Instance.transform;
                playspaceAnchorTransform.position = getMiddle();
                playspaceAnchorTransform.rotation = getRotation();

                //get location
                Vector3 oldPosition = playspaceAnchorTransform.position;
                Quaternion oldRotation = playspaceAnchorTransform.rotation;

                //rotate the anchor by that much
                playspaceAnchorTransform.transform.Rotate(rotationOffset, Space.Self);
                playspaceAnchorTransform.transform.Translate(positionOffset, Space.Self);

                Vector3 distanceMoved = oldPosition - playspaceAnchorTransform.transform.position;

                Debug.LogFormat(GlobalVariables.cCommon + "Moving playspace anchor. Position offset: {0}, Rotation offset: {1}" + GlobalVariables.endColor + GlobalVariables.cAlert + ", Moved distance: {2}, X distance: {3}, Y distance: {4}, Z distance: {5}" + GlobalVariables.endColor + " {6}: {7} -> {8} -> {9}", positionOffset, rotationOffset, distanceMoved, oldPosition.x, oldPosition.y, oldPosition.z, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());


#if UNITY_EDITOR
                //Can't press virtual button or hard to press virtual button in editor
                //if (Btn_Functions_For_In_Scene_Scripts.Instance != null)
                //{
                //    Debug.Log(GlobalVariables.purple + "In Editor loading new scene on centerplayspace" + GlobalVariables.endColor + " : " + "centerPlayspace()" + " : " + this.GetType());

                //    Btn_Functions_For_In_Scene_Scripts.Instance.sceneManager_Load_01_SetupMenu();
                //}
#endif
            }
        }

        public void Init_Unload_Vuforia()
        {
            if (VuforiaRuntime.Instance.InitializationState == VuforiaRuntime.InitState.NOT_INITIALIZED)
            {
                VuforiaRuntime.Instance.InitVuforia();
            }

            Debug.LogFormat(GlobalVariables.cInstance + "Vuforia: {0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", VuforiaRuntime.Instance.InitializationState, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }

        public void enableDisableVuforia()
        {
            VuforiaBehaviour.Instance.enabled = !VuforiaBehaviour.Instance.enabled;
        }

#endif
        private void OnDrawGizmos()
        {
            float r = .1f;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(getMiddle(), r);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(getMiddle(), r);

        }
    }
}

//private void OnDrawGizmos()
//{
//    float r = .1f;

//    Vector3 point2 = Tracker2.transform.position;
//    Vector3 point3 = Tracker3.transform.position;
//    Vector3 point4 = Tracker4.transform.position;
//    Vector3 middle = (point4 + point2 + point3) / 3;


//    Gizmos.color = Color.red;
//    Gizmos.DrawWireSphere(point2, r);

//    Gizmos.color = Color.green;
//    Gizmos.DrawWireSphere(point3, r);

//    Gizmos.color = Color.blue;
//    Gizmos.DrawWireSphere(point4, r);

//    Gizmos.color = Color.black;
//    Gizmos.DrawWireSphere(middle, r);

//    Gizmos.color = Color.cyan;
//    Gizmos.DrawLine(point2, point3);
//    Gizmos.DrawLine(point2, point4);
//    Gizmos.DrawLine(point4, point3);

//    Vector3 LongestLineStart;
//    Vector3 LongestLineEnd;
//    Vector3 FacedPoint;

//    float dist23 = Vector3.Distance(point2, point3);
//    float dist24 = Vector3.Distance(point2, point4);
//    float dist34 = Vector3.Distance(point3, point4);

//    if (dist23 > dist24 && dist23 > dist34)
//    {
//        FacedPoint = point4;

//    }
//    else if (dist24 > dist23 && dist24 > dist34)
//    {
//        FacedPoint = point3;
//    }
//    else
//    {
//        FacedPoint = point2;
//    }

//    r = .075f;
//    Gizmos.color = Color.white;
//    Gizmos.DrawWireSphere(FacedPoint, r);

//    Quaternion dir = Quaternion.FromToRotation(middle, FacedPoint);

//}