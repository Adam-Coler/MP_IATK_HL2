using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Photon_IATK
{
    /// <summary>
    /// When attached this class will provide a material change event that can be triggered by adding it to the "on manipulation started/stopped" events in the GUI for the manipulation handler script.
    /// </summary>

    [DisallowMultipleComponent]
    public class GrabFeedbackLocal : MonoBehaviour
    {

        public Material grabbedMaterial;
        private Dictionary<Renderer, Material> renderersAndMats;
        private bool isSecondAttempt = false;


        void Start()
        {
            if (grabbedMaterial == null)
            {
                grabbedMaterial = Resources.Load("GrabFeedback", typeof(Material)) as Material;
            }

            setupRenderers();

        }

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
        }
        public void Grabbed()
        {
            foreach (Renderer key in renderersAndMats.Keys)
            {
                key.material = grabbedMaterial;
            }
        }
        public void Released()
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
