using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.IO;
//using UnityEditor.Build.Reporting;
using UnityEngine.XR;



namespace Photon_IATK
{
#if UNITY_EDITOR
    [ExecuteInEditMode]

    public class Tools : Editor
{

    #region Menu Items

    [MenuItem("Tools/Set Build to Desktop", false, 1)]
    private static void NewMenuOption1() => setPlatformDesktop();

    [MenuItem("Tools/Set Build to HoloLens 2", false, 2)]
    private static void NewMenuOption2() => setPlatformHL2();

    [MenuItem("Tools/Set Build to Vive Pro", false, 3)]
    private static void NewMenuOption3() => setPlatformVive();

    [MenuItem("Tools/Log Current Settings", false, 4)]
    private static void NewMenuOption4() => logSettings();

    #endregion


    #region Private Variables

    static private string className = "Tools.cs:";
    static private string ColorStartGreen = GlobalVariables.green;
    static private string ColorStartRed = GlobalVariables.red;
    static private string colorEnd = GlobalVariables.endColor;

    #endregion

    #region Public Variables
    #endregion

    static void setPlatformDesktop()
    {
        clearLog();
        BuildManager buildManager = new BuildManager { };
        if (buildManager.GetCurrentPlatform() != "DESKTOP")
        {
            setBuildPlatformDesktop();
            setPlatformSymbolsDesktop();
        } else
        {
            Debug.Log(ColorStartGreen + " Desktop already set as platform" + colorEnd + " Tools.cs");
        }
            logSettings();
    }

    static void setPlatformHL2()
    {
        clearLog();
        BuildManager buildManager = new BuildManager { };
        if (buildManager.GetCurrentPlatform() != "HL2")
        {
            setBuildPlatformHL2();
            setPlatformSymbolsHL2();
        }
        else
        {
            Debug.Log(ColorStartGreen + " HL2 already set as platform" + colorEnd + " Tools.cs");
        }
        logSettings();
    }
    static void setPlatformVive()
    {
        clearLog();
        BuildManager buildManager = new BuildManager { };
        if (buildManager.GetCurrentPlatform() != "VIVE")
        {
            setBuildPlatformVive();
            setPlatformSymbolsVive();
        }
        else
        {
            Debug.Log(ColorStartGreen + " VIVE already set as platform" + colorEnd + " Tools.cs");
        }
        logSettings();
    }

    static void logSettings()
    {
        var buildTarget = EditorUserBuildSettings.selectedBuildTargetGroup;
        var buildTargetStandalone = EditorUserBuildSettings.selectedStandaloneTarget;
        string buildPath = EditorUserBuildSettings.GetBuildLocation(EditorUserBuildSettings.selectedStandaloneTarget);

        //Script - Custom Tools: Current Build Settings: Build Target Group: Standalone, Build Standalone: StandaloneWindows, Build Path: C: \Users\Adam6\Desktop\PhotonProjects\PhotonIATK\PhotonDesktop\PhotonDesktop\Builds\DESKTOP

        //Script - Custom Tools: -Build Target Group: Metro, Build Standalone: StandaloneWindows, Build Path: C: \Users\Adam6\Desktop\PhotonProjects\PhotonIATK\PhotonDesktop\PhotonDesktop\Builds\HL2

        //Debug.Log(ColorStartGreen + className + " start log dump " + colorEnd);
        //Debug.Log(ColorStartGreen + "Build Target Group: " + colorEnd + ColorStartRed + buildTarget + colorEnd);
        //Debug.Log(ColorStartGreen + "Build Target: " + colorEnd + ColorStartRed + buildTargetStandalone + colorEnd);
        Debug.Log(ColorStartGreen + "Build Path: " + colorEnd + ColorStartRed + buildPath + colorEnd);
        BuildManager buildManager = new BuildManager { };
        Debug.Log(ColorStartGreen + className + " Current symbol: " + colorEnd + ColorStartRed + buildManager.GetCurrentPlatform() + colorEnd);
        Debug.Log(ColorStartGreen + className + " Current symbols for" + " project: " + colorEnd + ColorStartRed + buildManager.getBuildDefineSymbols() + colorEnd);
        //Debug.Log(ColorStartGreen + "XR Enabled: " + colorEnd + ColorStartRed + XRSettings.enabled + colorEnd);
        Debug.Log(ColorStartGreen + "XR Device Active: " + colorEnd + ColorStartRed + XRSettings.loadedDeviceName + colorEnd);
            //Debug.Log(ColorStartGreen + "XR Device Name: " + colorEnd + ColorStartRed + XRSettings.isDeviceActive + colorEnd);
            
        Debug.Log(ColorStartGreen + "productName: " + colorEnd + ColorStartRed + PlayerSettings.productName + colorEnd);
            Debug.Log(ColorStartGreen + className + " End log dump " + colorEnd);

    }

    static void clearLog()
    {
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }
    static void setBuildPlatformDesktop()
    {
        //Set Build Path
        string savePath = Path.GetFullPath(Path.Combine(Application.dataPath, "../Builds/DESKTOP"));
        EditorUserBuildSettings.SetBuildLocation(EditorUserBuildSettings.selectedStandaloneTarget, savePath);

        PlayerSettings.productName = "IATK_DESKTOP";
            //PlayerSettings.vuforiaEnabled = false;
        }

    static void setBuildPlatformHL2()
    {
        //Set Build Path
        string savePath = Path.GetFullPath(Path.Combine(Application.dataPath, "../Builds/HL2"));
        EditorUserBuildSettings.SetBuildLocation(EditorUserBuildSettings.selectedStandaloneTarget, savePath);

        PlayerSettings.productName = "IATK_HL2";
            PlayerSettings.vuforiaEnabled = true;
        }
    static void setBuildPlatformVive()
    {
        //Set Build Path
        string savePath = Path.GetFullPath(Path.Combine(Application.dataPath, "../Builds/VIVE"));
        EditorUserBuildSettings.SetBuildLocation(EditorUserBuildSettings.selectedStandaloneTarget, savePath);

        PlayerSettings.productName = "IATK_VIVE";
            PlayerSettings.vuforiaEnabled = false;
    }


    static void setPlatformSymbolsDesktop()
    {
        BuildManager buildManager = new BuildManager { };
        string newSymbols = buildManager.setBuildDefineSymbols(BuildManager.allSymbols.DESKTOP);

    }

    static void setPlatformSymbolsHL2()
    {
        BuildManager buildManager = new BuildManager { };
        string newSymbols = buildManager.setBuildDefineSymbols(BuildManager.allSymbols.HL2);
    }

    static void setPlatformSymbolsVive()
    {
        BuildManager buildManager = new BuildManager { };
        string newSymbols = buildManager.setBuildDefineSymbols(BuildManager.allSymbols.VIVE);
    }
    }
#endif

}

namespace Photon_IATK
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
    public class BuildManager
{

    public enum allSymbols
    {
        DESKTOP,
        VIVE,
        HL2
    };

    public string getBuildDefineSymbols()
    {
        return PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
    }

    private void trimSymbols()
    {
        //Get symbols
        string currentSymbols = getBuildDefineSymbols();

        //Remove old platform symbol
        string[] SymbolNames = System.Enum.GetNames(typeof(allSymbols));
        for (int i = 0; i < SymbolNames.Length; i++)
        {
            string symbolName = SymbolNames[i];
            currentSymbols = currentSymbols.Replace(symbolName + ";", "");
            currentSymbols = currentSymbols.Replace(symbolName, "");
        }
        // remove last semicolon
        currentSymbols = currentSymbols.TrimEnd(';');
        PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, currentSymbols);
    }

    public string setBuildDefineSymbols(allSymbols SymbolToAdd)
    {
        //Remove all added platform symbols
        trimSymbols();

        // get clean symbols
        string currentSymbols = getBuildDefineSymbols();

        //add new symbol to list
        currentSymbols = currentSymbols + ";" + SymbolToAdd.ToString();

        // set new list
        PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, currentSymbols);
        return getBuildDefineSymbols();
    }

    public string GetCurrentPlatform()
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

}
#endif
}



