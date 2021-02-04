using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Photon_IATK
{
    public class HelperFunctions
    {
        public static Gradient getColorGradient(Color startColor, Color endColor)
        {
            Gradient gradient = new Gradient();

            // Populate the color keys at the relative time 0 and 1 (0 and 100%)
            GradientColorKey[] colorKey = new GradientColorKey[2];
            colorKey[0].color = startColor;
            colorKey[0].time = 0.0f;
            colorKey[1].color = endColor;
            colorKey[1].time = 1.0f;

            // Populate the alpha  keys at relative time 0 and 1  (0 and 100%)
            GradientAlphaKey[] alphaKey = new GradientAlphaKey[2];
            alphaKey[0].alpha = 1.0f;
            alphaKey[0].time = 0.0f;
            alphaKey[1].alpha = 0.0f;
            alphaKey[1].time = 1.0f;

            gradient.SetKeys(colorKey, alphaKey);

            return gradient;
        }

        public static List<List<T>> Split<T>(List<T> collection, int size)
        {
            var chunks = new List<List<T>>();
            var chunkCount = collection.Count() / size;

            if (collection.Count % size > 0)
                chunkCount++;

            for (var i = 0; i < chunkCount; i++)
                chunks.Add(collection.Skip(i * size).Take(size).ToList());

            return chunks;
        }

        public static void hideShowChildrenOfTag(string tag)
        {
            GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject obj in objectsWithTag)
            {
                Renderer[] renderes = obj.GetComponentsInChildren<Renderer>();
                foreach (Renderer renderer in renderes)
                {
                    renderer.enabled = !renderer.enabled;
                }
            }
        }

        public static bool GetComponentInChild<T>(out T component, GameObject parentObject, MethodBase fromMethodBase) where T : Component
        {
            
            T[] componenets =  parentObject.GetComponentsInChildren<T>();
            if (componenets == null)
            {
                component = null;
                Debug.LogFormat(GlobalVariables.cError + "{0}{1}{2}" + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6} -> {7}", component.GetType(), " not found, returning null", "", Time.realtimeSinceStartup, fromMethodBase.ReflectedType.Name, fromMethodBase.Name, MethodBase.GetCurrentMethod().Name, MethodBase.GetCurrentMethod().ReflectedType.Name);
                return false;
            }
            else
            {
                Debug.LogFormat(GlobalVariables.cCommon + "{0}{1}{2}" + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6} -> {7}", componenets.Length, " components found, returning the first", "", Time.realtimeSinceStartup, fromMethodBase.ReflectedType.Name, fromMethodBase.Name, MethodBase.GetCurrentMethod().Name, MethodBase.GetCurrentMethod().ReflectedType.Name);
                component = componenets[0];
                return true;

            }
        }

        public static void SetObjectLocalTransformToZero(GameObject obj, MethodBase fromMethodBase)
        {
            Debug.LogFormat(GlobalVariables.cCommon + "{0}{1}{2}" + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6} -> {7}", obj.name, " moving to local zero", "", Time.realtimeSinceStartup, fromMethodBase.ReflectedType.Name, fromMethodBase.Name, MethodBase.GetCurrentMethod().Name, MethodBase.GetCurrentMethod().ReflectedType.Name);

            obj.transform.localScale = new Vector3(1f, 1f, 1f);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
        }


        public static bool GetComponent<T>(out T component, MethodBase fromMethodBase) where T : Component
        {
            component = Object.FindObjectOfType(typeof(T)) as T;
            if (component != null)
            {
                Debug.LogFormat(GlobalVariables.cCommon + "Found {0}: On {1}{2}" + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6} -> {7}", component.GetType(), component.gameObject.name, "", Time.realtimeSinceStartup, fromMethodBase.ReflectedType.Name, fromMethodBase.Name, MethodBase.GetCurrentMethod().Name, MethodBase.GetCurrentMethod().ReflectedType.Name);
                return true;
            }
            else
            {
                Debug.LogFormat(GlobalVariables.cError + "No component found {0}{1}{2}" + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6} -> {7}", "", "", "", Time.realtimeSinceStartup, fromMethodBase.ReflectedType.Name, fromMethodBase.Name, MethodBase.GetCurrentMethod().Name, MethodBase.GetCurrentMethod().ReflectedType.Name);
                return false;
            }
        }


        public static bool FindGameObjectOrMakeOneWithTag(string tag, out GameObject returnedGameObject, bool makeOneIfNotFound, MethodBase fromMethodBase)
        {
            GameObject[] gameObjectsFound = GameObject.FindGameObjectsWithTag(tag);

            if (gameObjectsFound.Length == 0)
            {
                if (makeOneIfNotFound)
                {
                    Debug.LogFormat(GlobalVariables.cError + "No GameObjects found with tag: {0}. None will be made{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6} -> {7}", tag, "", "", Time.realtimeSinceStartup, fromMethodBase.ReflectedType.Name, fromMethodBase.Name, MethodBase.GetCurrentMethod().Name, MethodBase.GetCurrentMethod().ReflectedType.Name);

                    returnedGameObject = new GameObject("EmmulatedVisObject");
                    returnedGameObject.tag = GlobalVariables.visTag;
                } 
                else
                {
                    Debug.LogFormat(GlobalVariables.cError + "No GameObjects found with tag: {0}. None will be made{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6} -> {7}", tag, "", "", Time.realtimeSinceStartup, fromMethodBase.ReflectedType.Name, fromMethodBase.Name, MethodBase.GetCurrentMethod().Name, MethodBase.GetCurrentMethod().ReflectedType.Name);

                    returnedGameObject = null;
                    return false;
                }

            }
            else
            {
                Debug.LogFormat(GlobalVariables.cCommon + "{0} GameObejcts found with Tag: {1}{2}" + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6} -> {7}", gameObjectsFound.Length, tag, " returning the first found.", Time.realtimeSinceStartup, fromMethodBase.ReflectedType.Name, fromMethodBase.Name, MethodBase.GetCurrentMethod().Name, MethodBase.GetCurrentMethod().ReflectedType.Name);
                returnedGameObject = gameObjectsFound[0];
            }

            return true;
        }

        public static bool RemoveComponent<T>(GameObject self, MethodBase fromMethodBase) where T : Component
        {
            T component = self.gameObject.GetComponent<T>();
            if (component == null) { return false; }

            Debug.LogFormat(GlobalVariables.cOnDestory + "Destorying {0} on {1}{2}" + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6} -> {7}", component.GetType(), component.gameObject.name, "", Time.realtimeSinceStartup, fromMethodBase.ReflectedType.Name, fromMethodBase.Name, MethodBase.GetCurrentMethod().Name, MethodBase.GetCurrentMethod().ReflectedType.Name);

            Object.Destroy(component);
            return true;
        }

        public static bool RemoveComponent<T>(MethodBase fromMethodBase) where T : Component
        {
            T[] components = Object.FindObjectsOfType<T>();
            if (components == null) { return false; }

            foreach (T component in components)
            {
                Debug.LogFormat(GlobalVariables.cOnDestory + "Destorying {0} on {1}{2}" + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6} -> {7}", component.GetType(), component.gameObject.name, "", Time.realtimeSinceStartup, fromMethodBase.ReflectedType.Name, fromMethodBase.Name, MethodBase.GetCurrentMethod().Name, MethodBase.GetCurrentMethod().ReflectedType.Name);

                Object.Destroy(component);
            }
            return true;
        }

        public static bool ParentInSharedPlayspaceAnchor(GameObject objToParent, MethodBase fromMethodBase)
        {
            bool wasSucsessfull;
            PlayspaceAnchor playspaceAnchor = PlayspaceAnchor.Instance;

            if (playspaceAnchor != null)
            {
                objToParent.transform.parent = PlayspaceAnchor.Instance.transform;
                wasSucsessfull = true;
            }
            else
            {
                playspaceAnchor = GameObject.FindObjectOfType<PlayspaceAnchor>();
                if (playspaceAnchor != null)
                {
                    objToParent.transform.parent = playspaceAnchor.transform;
                    wasSucsessfull = true;
                } else
                {
                    wasSucsessfull = false;
                    Debug.LogFormat(GlobalVariables.cError + "No playspace anchor found. {0}{1}{2}" + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6} -> {7}", objToParent.name, "", "", Time.realtimeSinceStartup, fromMethodBase.ReflectedType.Name, fromMethodBase.Name, MethodBase.GetCurrentMethod().Name, MethodBase.GetCurrentMethod().ReflectedType.Name);
                }
            }

            if (wasSucsessfull)
                Debug.LogFormat(GlobalVariables.cCommon + "Parenting {0}{1}{2}" + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6} -> {7}", objToParent.name, " in ", playspaceAnchor.name, Time.realtimeSinceStartup, fromMethodBase.ReflectedType.Name, fromMethodBase.Name, MethodBase.GetCurrentMethod().Name, MethodBase.GetCurrentMethod().ReflectedType.Name);

            return wasSucsessfull;
        }

        public static bool SafeDestory(GameObject objToDestory, MethodBase fromMethodBase)
        {
            bool wasSucessfull = false;

            //find photon view

            Photon.Pun.PhotonView photonView = objToDestory.GetComponent<Photon.Pun.PhotonView>();

            if (photonView != null && Photon.Pun.PhotonNetwork.IsConnected)
            {
                if (photonView.IsMine)
                {
                    try
                    {
                        Photon.Pun.PhotonNetwork.Destroy(objToDestory);
                        wasSucessfull = true;
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogFormat(GlobalVariables.cError + "Error Network Destorying {0}{1}{2}" + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6} -> {7}", objToDestory.name, " E: ", e.Message, Time.realtimeSinceStartup, fromMethodBase.ReflectedType.Name, fromMethodBase.Name, MethodBase.GetCurrentMethod().Name, MethodBase.GetCurrentMethod().ReflectedType.Name);
                    }
                } else { Debug.LogFormat(GlobalVariables.cError + "Error Network Destorying {0}{1}{2}" + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6} -> {7}", objToDestory.name, ": is not mine to destory ", "", Time.realtimeSinceStartup, fromMethodBase.ReflectedType.Name, fromMethodBase.Name, MethodBase.GetCurrentMethod().Name, MethodBase.GetCurrentMethod().ReflectedType.Name); }

            }
            else if (photonView == null || !Photon.Pun.PhotonNetwork.IsConnected)
            {
                try
                {
                    Object.Destroy(objToDestory);
                    wasSucessfull = true;
                }
                catch (System.Exception e)
                {
                    Debug.LogFormat(GlobalVariables.cError + "Error Destorying {0}{1}{2}" + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6} -> {7}", objToDestory.name, " E: ", e.Message, Time.realtimeSinceStartup, fromMethodBase.ReflectedType.Name, fromMethodBase.Name, MethodBase.GetCurrentMethod().Name, MethodBase.GetCurrentMethod().ReflectedType.Name);
                }
            }

            if(wasSucessfull)
                Debug.LogFormat(GlobalVariables.cOnDestory + "Destorying {0}{1}{2}" + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6} -> {7}", objToDestory.name, "", "", Time.realtimeSinceStartup, fromMethodBase.ReflectedType.Name, fromMethodBase.Name, MethodBase.GetCurrentMethod().Name, MethodBase.GetCurrentMethod().ReflectedType.Name);

            return wasSucessfull;
        }


        public static bool getLocalPlayer(out Photon.Pun.PhotonView photonView, MethodBase fromMethodBase)
        {
            // Start is called before the first frame update
            var tmp = (GameObject.FindGameObjectsWithTag("Player"));
            foreach (GameObject obj in tmp)
            {
                Photon.Pun.PhotonView photon = obj.GetComponent<Photon.Pun.PhotonView>();
                Photon_Player photonPlayer = obj.GetComponent<Photon_Player>();
                Debug.Log(photon.name);
                if (photon != null & photonPlayer != null)
                {
                    if (photon.IsMine)
                    {
                        photonView = photon;

                        Debug.LogFormat(GlobalVariables.cCommon + "PlayerView found. Name:{0}, Owner:{1}{2}" + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6} -> {7}", photonView.name, photonView.Owner, "", Time.realtimeSinceStartup, fromMethodBase.ReflectedType.Name, fromMethodBase.Name, MethodBase.GetCurrentMethod().Name, MethodBase.GetCurrentMethod().ReflectedType.Name);

                        return true;
                    }
                }

            }

            Debug.LogFormat(GlobalVariables.cError + "No playerView found. {0}{1}{2}" + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6} -> {7}", "", "", "", Time.realtimeSinceStartup, fromMethodBase.ReflectedType.Name, fromMethodBase.Name, MethodBase.GetCurrentMethod().Name, MethodBase.GetCurrentMethod().ReflectedType.Name);

            photonView = null;
            return false;
        }

        public static void networkLoadObject(string nameOfPrefab, MethodBase fromMethodBase)
        {
            if (Photon.Pun.PhotonNetwork.IsConnected) {

                Photon.Pun.PhotonView photonView;
                if (HelperFunctions.getLocalPlayer(out photonView, System.Reflection.MethodBase.GetCurrentMethod()))
                    photonView.RPC("masterClientInstantiate", Photon.Pun.RpcTarget.MasterClient, nameOfPrefab);
            }
            else
            {
                try
                {
                    GameObject prefab = Resources.Load(nameOfPrefab) as GameObject;
                    Object.Instantiate(prefab, new Vector3(1.5f, 0, 0), Quaternion.identity);
                }
                catch (System.Exception e)
                {
                    Debug.LogFormat(GlobalVariables.cError + "No prefab found with name: {0}{1}{2}" + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6} -> {7}", nameOfPrefab, " E: ", e.Message, Time.realtimeSinceStartup, fromMethodBase.ReflectedType.Name, fromMethodBase.Name, MethodBase.GetCurrentMethod().Name, MethodBase.GetCurrentMethod().ReflectedType.Name);
                }
            }

            //PhotonNetwork.SetMasterClient(photonPlayer);
        }

    }
}