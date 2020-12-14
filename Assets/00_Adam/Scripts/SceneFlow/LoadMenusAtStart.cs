using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Photon_IATK {
    public class LoadMenusAtStart : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(LoadMenus());
        }

        IEnumerator LoadMenus()
        {
            yield return new WaitForSeconds(2);

            
            GameObject Menu = GameObject.FindGameObjectWithTag("Menu");
            if (Menu == null)
            {
                Debug.Log(GlobalVariables.green + "Loading menu :" + GlobalVariables.endColor + "LoadMenus() : " + this.GetType());
                Btn_Functions_For_In_Scene_Scripts Btns = new Btn_Functions_For_In_Scene_Scripts();
                Debug.Log(GlobalVariables.green + "Loading Main Menu " + GlobalVariables.endColor + " : " + "Start()" + " : " + this.GetType());
                Btns.sceneManager_Load_01_SetupMenu();
            }

            Destroy(this);
        }

    }
}
