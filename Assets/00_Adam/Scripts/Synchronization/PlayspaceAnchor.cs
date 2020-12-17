using UnityEngine;

namespace Photon_IATK
{
    public class PlayspaceAnchor : MonoBehaviour
    {
        public static PlayspaceAnchor Instance;

        private void Start()
        {
            if (Instance == null)
            {
                Debug.Log(GlobalVariables.green + "Setting PlayspaceAnchor.Instance " + GlobalVariables.endColor + " : " + "Awake()" + " : " + this.GetType());
                Instance = this;
            }
            else
            {
                if (Instance == this) return;

                Debug.Log(GlobalVariables.green + "Destroying then setting PlayspaceAnchor.Instance " + GlobalVariables.endColor + " : " + "Awake()" + " : " + this.GetType());

                Destroy(Instance.gameObject);
                Instance = this;
            }
        }
    }
}
