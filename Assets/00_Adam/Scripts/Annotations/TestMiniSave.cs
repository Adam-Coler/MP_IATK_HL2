using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon_IATK;

public class TestMiniSave : MonoBehaviour
{
    [SerializeField] private PotionData _PotionData = new PotionData();

    public void SaveIntoJson()
    {
        string potion = JsonUtility.ToJson(_PotionData);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/PotionData.json", potion);

        Debug.LogFormat(GlobalVariables.cSerialize + "Path = {0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", Application.persistentDataPath, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
    }
}

[System.Serializable]
public class PotionData
{
    public string potion_name;
    public int value;
    public List<Effect> effect = new List<Effect>();
}

[System.Serializable]
public class Effect
{
    public string name;
    public string desc;
}

