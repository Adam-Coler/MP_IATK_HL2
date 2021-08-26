using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

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
                    Debug.LogFormat(GlobalVariables.cError + "No GameObjects found with tag: {0}. One will be made but has been disabled{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6} -> {7}", tag, "", "", Time.realtimeSinceStartup, fromMethodBase.ReflectedType.Name, fromMethodBase.Name, MethodBase.GetCurrentMethod().Name, MethodBase.GetCurrentMethod().ReflectedType.Name);

                    //returnedGameObject = new GameObject("EmmulatedVisObject");
                    //returnedGameObject.tag = GlobalVariables.visTag;
                    returnedGameObject = null;
                    return false;
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



        public static bool getLocalPlayer(out Photon.Pun.PhotonView photonView, out Photon_Player photon_Player, MethodBase fromMethodBase)
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
                        photon_Player = photonPlayer;
                        Debug.LogFormat(GlobalVariables.cCommon + "PlayerView found. Name:{0}, Owner:{1}{2}" + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6} -> {7}", photonView.name, photonView.Owner, "", Time.realtimeSinceStartup, fromMethodBase.ReflectedType.Name, fromMethodBase.Name, MethodBase.GetCurrentMethod().Name, MethodBase.GetCurrentMethod().ReflectedType.Name);

                        return true;
                    }
                }

            }

            Debug.LogFormat(GlobalVariables.cError + "No playerView found. {0}{1}{2}" + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6} -> {7}", "", "", "", Time.realtimeSinceStartup, fromMethodBase.ReflectedType.Name, fromMethodBase.Name, MethodBase.GetCurrentMethod().Name, MethodBase.GetCurrentMethod().ReflectedType.Name);

            photonView = null;
            photon_Player = null;
            return false;
        }



            public static bool getLocalPlayer(out Photon.Pun.PhotonView photonView, MethodBase fromMethodBase)
        {
            Photon_Player photon_Player;
            return getLocalPlayer(out photonView, out photon_Player, fromMethodBase);
        }

        public static bool doListsMatch<T>(List<T> myList, List<T> comparedList, out List<T> outList, MethodBase fromMethodBase)
        {
            var firstNotSecond = myList.Except(comparedList).ToList();
            outList = comparedList.Except(myList).ToList();

            Debug.LogFormat(GlobalVariables.cCommon + "doListsMatch, !firstNotSecond.Any() :{0}, !outList.Any(): {1}, (!firstNotSecond.Any() && !outList.Any()): {2}" + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6} -> {7}", !firstNotSecond.Any(), !outList.Any(), !firstNotSecond.Any() && !outList.Any(), Time.realtimeSinceStartup, fromMethodBase.ReflectedType.Name, fromMethodBase.Name, MethodBase.GetCurrentMethod().Name, MethodBase.GetCurrentMethod().ReflectedType.Name);

            return !firstNotSecond.Any() && !outList.Any();
        }

        private const char delim = ' ';
        public static string IntListToString(List<int> intList, System.Reflection.MethodBase fromMethodBase)
        {
            if (intList == null || intList.Count == 0) 
            {
                Debug.LogFormat(GlobalVariables.cError + "List cannot be zero length. {0}{1}{2}" + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6} -> {7}", "", "", "", Time.realtimeSinceStartup, fromMethodBase.ReflectedType.Name, fromMethodBase.Name, MethodBase.GetCurrentMethod().Name, MethodBase.GetCurrentMethod().ReflectedType.Name);
                return "";
            }

            StringBuilder builder = new StringBuilder("");
            foreach (int oneInt in intList)
            {
                builder.Append(oneInt.ToString());
                builder.Append(delim);
            }
            // remove the last delimeter;
            builder.Remove(builder.Length - 1, 1);
            return builder.ToString();
        }

        public static List<int> StringWithDelimToListInt(string stringOfInts, System.Reflection.MethodBase fromMethodBase)
        {
            if (stringOfInts == null || stringOfInts.Length == 0) 
            {
                Debug.LogFormat(GlobalVariables.cError + "List cannot be zero length. {0}{1}{2}" + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6} -> {7}", "", "", "", Time.realtimeSinceStartup, fromMethodBase.ReflectedType.Name, fromMethodBase.Name, MethodBase.GetCurrentMethod().Name, MethodBase.GetCurrentMethod().ReflectedType.Name);
                return new List<int> { }; 
            }

            string[] intsAsString = stringOfInts.Split(delim);
            List<int> output = new List<int> { };
            foreach (string intAsString in intsAsString)
            {
                output.Add(int.Parse(intAsString));
            }

            return output;
        }


        public static byte[] SerializeToByteArray<T>(T serializableObject, System.Reflection.MethodBase fromMethodBase)
        {

            byte[] bytes;
            IFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();

            formatter.Serialize(stream, serializableObject);
            bytes = stream.ToArray();

            Debug.LogFormat(GlobalVariables.cCommon + "Successful Serizalization. Input Type: {0}{1}{2}" + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6} -> {7}", serializableObject.GetType(), "", "", Time.realtimeSinceStartup, fromMethodBase.ReflectedType.Name, fromMethodBase.Name, MethodBase.GetCurrentMethod().Name, MethodBase.GetCurrentMethod().ReflectedType.Name);

            return bytes;
        }

        public static Vector3 StringToVector3(string sVector)
        {
            // Remove the parentheses
            if (sVector.StartsWith("(") && sVector.EndsWith(")"))
            {
                sVector = sVector.Substring(1, sVector.Length - 2);
            }

            // split the items
            string[] sArray = sVector.Split(',');

            // store as a Vector3
            Vector3 result = new Vector3(
                float.Parse(sArray[0]),
                float.Parse(sArray[1]),
                float.Parse(sArray[2]));

            return result;
        }


        public static T DeserializeFromByteArray<T>(byte[] bytes, System.Reflection.MethodBase fromMethodBase)
        {
            IFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream(bytes);
            T output = default(T);

            try
            {
                output = (T)formatter.Deserialize(stream);
            } 
            catch (System.Exception e)
            {
                Debug.LogFormat(GlobalVariables.cError + "Error with deserialization. {0}{1}{2}" + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6} -> {7}", e.Message, "", "", Time.realtimeSinceStartup, fromMethodBase.ReflectedType.Name, fromMethodBase.Name, MethodBase.GetCurrentMethod().Name, MethodBase.GetCurrentMethod().ReflectedType.Name);
            }

            Debug.LogFormat(GlobalVariables.cCommon + "Successful Deserizalization. Type: {0}{1}{2}" + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6} -> {7}", output.GetType(), "", "", Time.realtimeSinceStartup, fromMethodBase.ReflectedType.Name, fromMethodBase.Name, MethodBase.GetCurrentMethod().Name, MethodBase.GetCurrentMethod().ReflectedType.Name);

            return output;
        }

        public static void randomizeAttributes(GameObject obj)
        {
            float min = 0f;
            float max = .75f;

            obj.transform.Translate(new Vector3(Random.Range(min, max), Random.Range(min, max), Random.Range(min, max)));
            obj.transform.Rotate(new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
        }



        /// <summary>
        /// Position Relative to Anchor
        /// </summary>
        public static Vector3 PRA(GameObject obj)
        {
            return PlayspaceAnchor.Instance.transform.InverseTransformPoint(obj.transform.localPosition);

            //This is wrong
            //return obj.transform.transform.InverseTransformPoint(PlayspaceAnchor.Instance.transform.position);
        }

        /// <summary>
        /// Position Relative to Anchor
        /// </summary>
        public static Vector3 PRA(Vector3 point)
        {
            return PlayspaceAnchor.Instance.transform.InverseTransformPoint(point);
        }

        /// <summary>
        /// Rotation Relative to Anchor
        /// </summary>
        public static Quaternion RRA(GameObject obj)
        {
            return Quaternion.Inverse(PlayspaceAnchor.Instance.transform.rotation) * obj.transform.rotation;
        }

        /// <summary>
        /// Scale Relative to Anchor
        /// </summary>
        public static Vector3 SRA(GameObject obj)
        {

            return obj.transform.lossyScale;
            //return obj.transform.localScale;
        }

        public static float GetTime()
        {
            return Time.realtimeSinceStartup;
        }

        public static string getJson(object obj, string objName)
        {
            string output = JsonUtility.ToJson(obj, GlobalVariables.JSONPrettyPrint);
            Debug.Log(GlobalVariables.cJSON + objName + GlobalVariables.endColor + output);
            return output;
        }


        static public GameObject getChildGameObject(GameObject fromGameObject, string withName)
        {
            Transform[] ts = fromGameObject.transform.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in ts) if (t.gameObject.name == withName) return t.gameObject;
            return null;
        }

        static public float getMedian(float[] values)
        {
            int numberCount = values.Count();
            int halfIndex = values.Count() / 2;
            var sortedNumbers = values.OrderBy(n => n);
            float median;
            if ((numberCount % 2) == 0)
            {
                var tmp = halfIndex - 1;
                median = (sortedNumbers.ElementAt(halfIndex) + sortedNumbers.ElementAt(tmp)) / 2;
            }
            else
            {
                median = sortedNumbers.ElementAt(halfIndex);
            }
            return median;
        }

    }
}