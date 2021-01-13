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
            yield return new WaitForSeconds(4);

            GameObject Menu = GameObject.FindGameObjectWithTag("Menu");
            if (Menu == null)
            {
                Btn_Functions_For_In_Scene_Scripts Btns = gameObject.AddComponent<Btn_Functions_For_In_Scene_Scripts>();

                Debug.LogFormat(GlobalVariables.cLevel + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Loading Main Menu", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                Btns.sceneManager_Load_01_SetupMenu();
                Destroy(Btns);
            }

            Destroy(this);
        }

    }
}
