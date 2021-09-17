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
        public Vector3 lastVisPosition;
        public Quaternion lastVisRotation;

        /// <summary>
        /// As all versions of this script will track the last location of the vis the instance will provide that. This will need ot be changed if more than one vis are used. 
        /// </summary>
        public static SyncLocationWithVis instance;

        private void Awake()
        {
            instance = this;
        }

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