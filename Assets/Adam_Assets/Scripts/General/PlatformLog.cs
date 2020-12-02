using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace Photon_IATK
{
    public class PlatformLog : MonoBehaviour
    {
#if UNITY_EDITOR
        public TMPro.TextMeshProUGUI Log;
        // Start is called before the first frame update
        void Start()
        {
            Log.text = GlobalVariables.green + LogPlatform() + " : " + SceneManager.GetActiveScene().name + GlobalVariables.endColor;
        }
        private string LogPlatform()
        {
            //Get symbols
            string currentSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            string currentPlatfrom = "";
            //Remove old platform symbol
            string[] SymbolNames = System.Enum.GetNames(typeof(GlobalVariables.allSymbols));
            for (int i = 0; i < SymbolNames.Length; i++)
            {
                string symbolName = SymbolNames[i];
                if (currentSymbols.Contains(symbolName))
                {
                    currentPlatfrom = symbolName;
                }
            }
            return (currentPlatfrom);
        }


#endif
    }
}