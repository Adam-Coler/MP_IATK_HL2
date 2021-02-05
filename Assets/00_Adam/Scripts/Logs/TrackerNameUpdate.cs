using UnityEngine;

namespace Photon_IATK
{

public class TrackerNameUpdate : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
            setTrackerName();
    }

    public void setTrackerName()
    {
            if (this.gameObject.GetComponentInChildren<TMPro.TextMeshPro>() == null)
            {
                Debug.Log(GlobalVariables.red + "No tracker name text found" + GlobalVariables.endColor + ", setTrackerName() : " + this.GetType());
                return;
            }


            if (this.gameObject.transform.parent == null)
            {
                Debug.Log(GlobalVariables.red + "No parent object found" + GlobalVariables.endColor + ", setTrackerName() : " + this.GetType());
                return;
            }

            string newName = this.gameObject.transform.parent.gameObject.name;
            if (newName.Contains("lone")) { newName = ""; };
            this.gameObject.GetComponentInChildren<TMPro.TextMeshPro>().text = newName;

            Debug.LogFormat(GlobalVariables.green + "Updating tracker name: {0}" + GlobalVariables.endColor + ", setTrackerName() : " + this.GetType(), this.gameObject.name);
        }
        private void OnDestroy()
        {

        }
    }

}
