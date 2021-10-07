using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
#if WINDOWS_UWP
using Windows.Storage;
#endif

namespace Photon_IATK
{
    public class CSVWritter : MonoBehaviour
    {
        #region Consts to modify
        private const int FlushAfter = 1000;
        #endregion

        #region Statics to modify
        public static string SessionFolderRoot;
        public static string Delim;
        public static string DataSuffix;
        public static string CSVHeader;
        #endregion

        public void Initalize(string sessionFolderRoot, string delim, string dataSuffix, string[] header)
        {
            SessionFolderRoot = sessionFolderRoot;
            Delim = delim;
            DataSuffix = dataSuffix;
            CSVHeader = string.Join(delim, header);
            CSVHeader += Delim + "TimeStamp";
        }

        #region private members
        private string m_sessionPath;
        private string m_filePath;
        private string m_recordingId;
        private string m_sessionId;

        private StringBuilder m_csvData;
        #endregion

        #region public members
        public string RecordingInstance => m_recordingId;
        #endregion

        public async Task MakeNewSession()
        {
            m_sessionId = DateTime.Now.ToString("yyyyMMdd_HHmm");
            string rootPath = "";
#if WINDOWS_UWP
            StorageFolder sessionParentFolder = await KnownFolders.PicturesLibrary
                .CreateFolderAsync(SessionFolderRoot,
                CreationCollisionOption.OpenIfExists);
            rootPath = sessionParentFolder.Path;
#else
            rootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), SessionFolderRoot);
            if (!Directory.Exists(rootPath)) Directory.CreateDirectory(rootPath);
#endif
            m_sessionPath = Path.Combine(rootPath, m_sessionId);
            Directory.CreateDirectory(m_sessionPath);
        }

        public void StartNewCSV()
        {
            //Debug.LogFormat(GlobalVariables.cDataCollection + "CSVLogger logging data to {0}" + GlobalVariables.endColor + " {1}{2}{3}{4}{5}: {6} -> {7} -> {8}", m_sessionPath, "", "", "", "", "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            m_recordingId = DateTime.Now.ToString("yyyyMMdd_HHmmssfff");
            var filename = m_recordingId + "-" + DataSuffix + ".csv";
            m_filePath = Path.Combine(m_sessionPath, filename);
            if (m_csvData != null)
            {
                EndCSV();
            }
            m_csvData = new StringBuilder();
            m_csvData.AppendLine(CSVHeader);
        }


        public void EndCSV()
        {
            if (m_csvData == null)
            {
                return;
            }
            using (var csvWriter = new StreamWriter(m_filePath, true))
            {
                csvWriter.Write(m_csvData.ToString());
            }
            m_recordingId = null;
            m_csvData = null;
        }

        public void OnDestroy()
        {
            EndCSV();
        }

        public void AddRow(List<String> rowData)
        {
            AddRow(string.Join(Delim, rowData.ToArray()));
        }

        public void AddRow(string[] rowData)
        {
            AddRow(string.Join(Delim, rowData));
        }

        public void AddRow(string row)
        {
            m_csvData.AppendLine(row + Delim + DateTime.Now.ToString("yyyyMMdd_HHmmss_fff"));

            if (m_csvData.Length >= FlushAfter)
            {
                FlushData();
            }
        }

        /// <summary>
        /// Writes all current data to current file
        /// </summary>
        public void FlushData()
        {
            using (var csvWriter = new StreamWriter(m_filePath, true))
            {
                csvWriter.Write(m_csvData.ToString());
            }
            m_csvData.Clear();
        }

        /// <summary>
        /// Returns a row populated with common start data like
        /// recording id, session id, timestamp
        /// </summary>
        /// <returns></returns>
        public List<String> RowWithStartData()
        {
            List<String> rowData = new List<String>();
            rowData.Add(Time.timeSinceLevelLoad.ToString("##.000"));
            rowData.Add(m_recordingId);
            rowData.Add(m_recordingId);
            return rowData;
        }
    }
}

//if (Instance != null) { Destroy(Instance); }
//Instance = this;

//pathString = Path.Combine(Application.persistentDataPath, "ExpData");
//if (!System.IO.Directory.Exists(pathString))
//{
//    System.IO.Directory.CreateDirectory(pathString);
//}

//string subFolderName = DateTime.Now.ToString("yyyyMMddHHmmssffff");
//subFolderPath = Path.Combine(pathString, subFolderName);
//if (!System.IO.Directory.Exists(subFolderPath))
//{
//    System.IO.Directory.CreateDirectory(subFolderPath);
//}

//GeneralInfoCSV = subFolderPath + "/GeneralInfoCSV.csv";

//if (!File.Exists(GeneralInfoCSV))
//{
//    string FileHeader = "PID" + "," + "MarksMade" + "," + "Time" + Environment.NewLine;
//    File.WriteAllText(GeneralInfoCSV, FileHeader);
//}
//File.AppendAllText(GeneralInfoCSV, "Test" + DateTime.Now.ToString("yyyy/MM/dd/HH/mm/ss/ffff") + "\r\n");

//Debug.LogFormat(GlobalVariables.cDataCollection + "{0}" + GlobalVariables.endColor + " {1}{2}{3}{4}{5}: {6} -> {7} -> {8}", pathString, "", "", "", "", "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());