using UnityEngine;
using Microsoft.MixedReality.Toolkit;


namespace Photon_IATK { 
public class Test_Lambda : MonoBehaviour
{
        public Vector3 CurrentTransform;
        public Vector3 NewTransform;

        public GameObject localSpace;
        public GameObject worldSpace;

        // Start is called before the first frame update
        void Start()
    {
        CurrentTransform = MixedRealityPlayspace.Position;
        
    }

    //System.Func<int, int> square = x => x * x;
    //System.Action<Transform> action = transform =>
    //{
    //    Transform transform1 = transform;
    //};

    public void PerformTransform()
        {
            MixedRealityPlayspace.Position = NewTransform;
            Debug.Log(GlobalVariables.purple + "Setting Transform " + GlobalVariables.endColor + GlobalVariables.green + "New: " + NewTransform + GlobalVariables.endColor + " " + GlobalVariables.red + " Old: " + CurrentTransform + GlobalVariables.endColor);

            CurrentTransform = MixedRealityPlayspace.Position;

        }

    // Update is called once per frame
    void Update()
    {
        if (NewTransform != CurrentTransform)
            {
                PerformTransform();
            }
    }
}
}
