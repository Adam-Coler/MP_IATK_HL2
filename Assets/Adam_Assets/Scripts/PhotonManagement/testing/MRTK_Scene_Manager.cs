using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.SceneSystem;
using Microsoft.MixedReality.Toolkit;

namespace Photon_IATK
{
    public static class MRTK_Scene_Manager
    {
        private static string _this = "MRTK_Scene_Manager";

        public static async System.Threading.Tasks.Task loadPIDEntrySceneAsync()
        {
            IMixedRealitySceneSystem sceneSystem = MixedRealityToolkit.Instance.GetService<IMixedRealitySceneSystem>();
            // First do an additive scene load
            // SceneOperationInProgress will be true for the duration of this operation
            // SceneOperationProgress will show 0-1 as it completes
            //await sceneSystem.LoadContent("PIDEntry");

            //this loads all scenes with this tag
            await sceneSystem.LoadContentByTag("PIDEntry");

            // Wait until stage 1 is complete

            Debug.Log(GlobalVariables.green + "PIDEntry Loaded" + GlobalVariables.endColor + " : " + "loadPIDEntrySceneAsync()" + " : " + _this);

        }

        public static async System.Threading.Tasks.Task unLoadPIDEntrySceneAsync()
        {
            IMixedRealitySceneSystem sceneSystem = MixedRealityToolkit.Instance.GetService<IMixedRealitySceneSystem>();
            //this unloads all scenes with this tag
            await sceneSystem.UnloadContentByTag("PIDEntry");

            // Wait until stage 1 is complete

            Debug.Log(GlobalVariables.green + "PIDEntry Unloaded" + GlobalVariables.endColor + " : " + "loadPIDEntrySceneAsync()" + " : " + _this);

        }


    }
}


//if (MRTK_Scene_Manager == null)
//{
//    MRTK_Scene_Manager = this.GetComponent<MRTK_Scene_Manager>();
//    if (MRTK_Scene_Manager == null)
//    {
//        MRTK_Scene_Manager = this.gameObject.AddComponent<MRTK_Scene_Manager>();
//        Debug.Log(GlobalVariables.green + "Scene Manager attached" + GlobalVariables.endColor + " : " + "Start()" + " : " + this.GetType());
//    }
//    else
//    {
//        Debug.Log(GlobalVariables.green + "Scene Manager found" + GlobalVariables.endColor + " : " + "Start()" + " : " + this.GetType());
//    }
//}