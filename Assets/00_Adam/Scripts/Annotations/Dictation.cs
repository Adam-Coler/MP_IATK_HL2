using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using UnityEngine.XR.WSA.Input;

namespace Photon_IATK
{
    public class Dictation : MonoBehaviour
    {
        private DictationRecognizer dictationRecognizer;
        public TMPro.TMP_InputField resultedDictation;
        public TextAnnotationManager textAnnotationManager;
        private string hypothesis = "";


        public void StartDictation()
        {
            Debug.Log("Dictation running");

            textAnnotationManager.dictationUsed = true;

            resultedDictation.text = "Listening...";

            if (dictationRecognizer != null)
            {
                dictationRecognizer.Stop();
                dictationRecognizer.Dispose();
            }

            dictationRecognizer = new DictationRecognizer();


            dictationRecognizer.DictationResult += dictationRecognizer_Result;

            dictationRecognizer.DictationHypothesis += dictationRecognizer_Hypothesis;

            dictationRecognizer.DictationComplete += dictationRecognizer_Complete;

            dictationRecognizer.DictationError += dictationRecognizer_Error;

            dictationRecognizer.Start();
        }

        private void dictationRecognizer_Result(string textDA, ConfidenceLevel confidence)
        {
            resultedDictation.text = textDA;
            resultedDictation.onEndEdit.Invoke(resultedDictation.text);
            Debug.Log("Dictation result: " + resultedDictation.text);
        }

        private void dictationRecognizer_Hypothesis(string textDA)
        {
            hypothesis = textDA;
            Debug.Log("Dictation hypothesis: " + hypothesis);
        }

        private void dictationRecognizer_Complete(DictationCompletionCause cause)
        {
            if (cause != DictationCompletionCause.Complete)
            {
                Debug.Log("Dictation completed: " + cause);
            }
            dictationRecognizer.Stop();
            dictationRecognizer.Dispose();
        }

        private void dictationRecognizer_Error(string error, int result)
        {
            Debug.LogError(error);
        }

    }
}


