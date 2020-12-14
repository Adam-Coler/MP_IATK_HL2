using Unity;
using UnityEngine;

namespace Photon_IATK
{
    public class ShowHideMenu : MonoBehaviour
    {

        private bool isMenuActive = true;
        private GameObject menu;

        private void Start()
        {
            menu = GameObject.FindGameObjectWithTag("Menu");
        }

        public void Menu()
        {
            if (isMenuActive == false)
            {
                isMenuActive = true;
                showMenu();
            } else if (isMenuActive == true)
            {
                isMenuActive = false;
                hideMenu();
            }
        }

        public void showMenu()
        {
            if (menu != null)
            {
                menu.SetActive(true);
                Debug.Log(GlobalVariables.green + "Showing Menu" + GlobalVariables.endColor + " : " + "Menu() : " + this.GetType());
            }
        }

        public void hideMenu()
        {
            if (menu != null)
            {
                menu.SetActive(false);
                Debug.Log(GlobalVariables.green + "Hiding Menu" + GlobalVariables.endColor + " : " + "Menu() : " + this.GetType());
            }
        }
    }
}
