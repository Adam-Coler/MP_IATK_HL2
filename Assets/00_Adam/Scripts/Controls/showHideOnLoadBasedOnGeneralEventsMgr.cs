using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Photon_IATK
{
    public class showHideOnLoadBasedOnGeneralEventsMgr : MonoBehaviour
    {
        public enum type
        {
            Tracker,
            Extra,
            GameController,
            Default
        }

        public type myType = type.Default;

        private void Awake()
        {
            Invoke("hide", 3f);
        }

        private void hide()
        {
            if (GeneralEventManager.instance == null) { return; }

            GeneralEventManager gm = GeneralEventManager.instance;

            switch (myType)
            {
                case type.Tracker:
                    if (!gm.showingTrackers)
                    {
                        transform.GetChild(0).gameObject.SetActive(false);
                    }
                    break;
                case type.Extra:
                    if (!gm.showingExtras)
                    {
                        transform.GetChild(0).gameObject.SetActive(false);
                    }
                    break;
                case type.GameController:
                    if (!gm.showingGameControllers)
                    {
                        Renderer[] renderes = this.GetComponentsInChildren<Renderer>();
                        foreach (Renderer renderer in renderes)
                        {
                            renderer.enabled = false;
                        }
                    }
                    break;
                case type.Default:
                    transform.GetChild(0).gameObject.SetActive(false);
                    break;
                default:
                    break;
            }

        }
    }
}
