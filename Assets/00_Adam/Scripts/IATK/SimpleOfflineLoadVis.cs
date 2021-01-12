using System.Reflection;
using UnityEngine;

namespace Photon_IATK
{
    public class SimpleOfflineLoadVis : MonoBehaviour
{
    public GameObject Prefab;

    // Start is called before the first frame update
    void Start()
    {
        if (Prefab != null)
        {
            GameObject vis = Instantiate(Prefab, new Vector3(1.5f,0,0), Quaternion.identity);

            Debug.LogFormat(GlobalVariables.purple + "Loading Prefab Offline!" + GlobalVariables.endColor + " {0}: {1} -> {2} -> {3}", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), MethodBase.GetCurrentMethod());
        } 
        else
        {
            Debug.LogFormat(GlobalVariables.red + "No prefab!" + GlobalVariables.endColor + " {0}: {1} -> {2} -> {3}", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), MethodBase.GetCurrentMethod());
        }
    }

    // Update is called once per frame
}

}
