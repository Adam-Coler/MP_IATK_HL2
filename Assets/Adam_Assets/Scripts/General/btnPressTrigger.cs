using UnityEngine;
using UnityEngine.Events;
using Microsoft.MixedReality.Toolkit.Input;

public class btnPressTrigger : MonoBehaviour
{

    // Start is called before the first frame update
    //void Start()
    //{
    //    var allInteractables = GameObject.FindObjectsOfType<Interactable>();
    //    foreach (var i in allInteractables)
    //    {
    //        i.OnClick.AddListener(() => Debug.Log(Time.time + ": " + i.gameObject.name + " was clicked"));
    //    }
    //}
    UnityEvent m_MyEvent = new UnityEvent();
    public void log()
    {
        Debug.Log("Clicked " + this.gameObject.name);
    }
    private void Start()
    {
        m_MyEvent.AddListener(MyAction);
    }

    private void Update()
    {
        System.Array values = System.Enum.GetValues(typeof(KeyCode));
        foreach (KeyCode code in values)
        {
            if (Input.GetKeyDown(code)) { print(System.Enum.GetName(typeof(KeyCode), code)); }
        }

    }

    void MyAction()
    {
        //Output message to the console
        Debug.Log("Do Stuff");
    }
}