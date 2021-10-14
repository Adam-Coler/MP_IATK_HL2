using UnityEngine;

public class FaceUser : MonoBehaviour
{
    public bool isFaceUser = true;

    public bool isRollLocked = false;
    public bool isPitchLocked = false;
    public bool isYawLocked = false;

    // Update is called once per frame
    void Update()
    {
            if (transform.hasChanged && isFaceUser)
            {
                //transform.LookAt(Camera.main.transform);
                Vector3 direction = transform.position - Camera.main.transform.position;

                Vector3 actualEulars = transform.rotation.eulerAngles;
                Vector3 eulars = Quaternion.LookRotation(direction).eulerAngles;

                if (isRollLocked)
                {
                    eulars.x = actualEulars.x;
                }

                if (isPitchLocked)
                {
                    eulars.y = actualEulars.y;
                }

                if (isYawLocked)
                {
                    eulars.z = actualEulars.z;
                }

                transform.rotation = Quaternion.Euler(eulars);

                transform.hasChanged = false;
            }
    }

    public void toggleFaceUser()
    {
        isFaceUser = !isFaceUser;
    }
}
