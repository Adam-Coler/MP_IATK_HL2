using System.Collections;
using UnityEngine;

namespace Photon_IATK {
    public class LoadMenusAtStart : MonoBehaviour
    {
        public bool isLoadMenusOnStart = false;

        // Start is called before the first frame update
        void Start()
        {
            if (isLoadMenusOnStart)
            {
                StartCoroutine(LoadMenus());
            }
        }

        IEnumerator LoadMenus()
        {
            yield return new WaitForSeconds(2);

            GameObject Menu = GameObject.FindGameObjectWithTag("Menu");
            if (Menu == null)
            {
                Btn_Functions_For_In_Scene_Scripts Btns = gameObject.AddComponent<Btn_Functions_For_In_Scene_Scripts>();

                Debug.Log(GlobalVariables.purple + "Loading Main Menu " + GlobalVariables.endColor + " : " + "Start()" + " : " + this.GetType());

                Btns.sceneManager_Load_01_SetupMenu();
                Destroy(Btns);
            }

            Destroy(this);
        }

    }
}
