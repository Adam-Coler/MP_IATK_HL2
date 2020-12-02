using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MP_Destory : MonoBehaviour
{
    [Tooltip("Will destroy items when items are selected")]
    [SerializeField]
    public bool destoryOnDesktop = false;

    [SerializeField]
    public bool destoryOnHL2 = false;

    [SerializeField]
    public bool destoryOnVive = false;

    [SerializeField]
    public bool destoryOnDefault = false;


#if DESKTOP
    private void Awake()
    {
        if (destoryOnDesktop)
        {
            Debug.Log("<Color=RED>MP_Destory: " + gameObject.name + "</Color>");
        }
    }

    private void LateUpdate()
    {
        if (destoryOnDesktop)
        {
            Destroy(gameObject);
        }
    }

#elif HL2

    private void Awake()
    {
        if (destoryOnHL2)
        {
            Debug.Log("<Color=RED>MP_Destory: " + gameObject.name + "</Color>");
        }
    }

    private void LateUpdate()
    {
        if (destoryOnHL2)
        {
            Destroy(gameObject);
        }
    }

#elif VIVE

    private void Awake()
    {
        if (destoryOnVive)
        {
            Debug.Log("<Color=RED>MP_Destory: " + gameObject.name + "</Color>");
        }
    }

    private void LateUpdate()
    {
        if (destoryOnVive)
        {
            Destroy(gameObject);
        }
    }

#else

    private void Awake()
    {
        if (true)
        {
            Debug.Log("<Color=RED>MP_Destory: " + gameObject.name + "</Color>");
        }
    }

    private void LateUpdate()
    {
        Destroy(gameObject);
    }

#endif 
}
