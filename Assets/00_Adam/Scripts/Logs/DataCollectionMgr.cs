using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Photon_IATK
{
    public class DataCollectionMgr : MonoBehaviour
    {
        private const string SessionFolderRoot = "csvDataLogs";
        private const string delim = ",";

        public static DataCollectionMgr Instance;

        private static CSVWritter GeneralData;
        private string[] generalDataHeader = new string[] { "Test", "PID" };

        private void Start()
        {
            if (Instance != null)
            {
                Destroy(Instance);
            }
            Instance = this;

            setupGeneralDataRecording();

            logRowsTest();
        }

        public void logRowsTest()
        {
            GeneralData.AddRow(new string[] { "1", "2" });
            GeneralData.FlushData();
            GeneralData.AddRow(new string[] { "2", "2" });
            GeneralData.AddRow(new string[] { "3", "2" });
            GeneralData.FlushData();
            GeneralData.AddRow(new string[] { "4", "2" });
            GeneralData.AddRow(new string[] { "5", "2" });
            GeneralData.AddRow(new string[] { "6", "2" });
        }

        private async void setupGeneralDataRecording()
        {
            GeneralData = gameObject.AddComponent<CSVWritter>();
            GeneralData.Initalize(SessionFolderRoot, delim, "GeneralData", generalDataHeader);
            await GeneralData.MakeNewSession();
            GeneralData.StartNewCSV();
        }

    }
}
