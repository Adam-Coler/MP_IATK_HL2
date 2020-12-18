using UnityEngine;

namespace Photon_IATK
{
    public class DontDestoryOnLoad : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {
            Debug.Log(GlobalVariables.green + "Moving " + this.gameObject.name + " to do not destroy on load" + GlobalVariables.endColor + " OnStatusChanged() : " + this.GetType());

            DontDestroyOnLoad(this.gameObject);
        }

    }
}
