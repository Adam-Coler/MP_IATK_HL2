using UnityEngine;

public class DontDestoryOnLoad : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

}
