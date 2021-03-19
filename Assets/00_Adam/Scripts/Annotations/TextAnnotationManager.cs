using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;

namespace Photon_IATK
{
    public class TextAnnotationManager : MonoBehaviour
    {
        public DictationHandler dictationHandler;
        public TMPro.TextMeshProUGUI content;
        public TMPro.TextMeshProUGUI placeholder;
        public Annotation myAnnotationParent;

        private string placeholderText;
        public bool dictationUsed = false;

        private void Awake()
        {
            placeholderText = placeholder.text;
        }

        public void onContentUpdate()
        {
            if (myAnnotationParent != null)
            {
                myAnnotationParent.UpdateText(content.text);
            }
        }

        public void updateContent(string text)
        {
            if (text != "")
                placeholder.text = "";

            content.text = text;
            Debug.LogFormat(GlobalVariables.cCommon + "{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "Dictation: ", text, "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            onContentUpdate();
        }

        public void DictationComplete(string text)
        {
            Debug.LogFormat(GlobalVariables.cCommon + "{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "DictationComplete: ", text, "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }

        public void DictationHypothisis(string text)
        {
            Debug.LogFormat(GlobalVariables.cCommon + "{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "DictationHypothisis: ", text, "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }

        public void DictationError(string text)
        {
            Debug.LogFormat(GlobalVariables.cCommon + "{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "DictationError: ", text, "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }

        public void DictationResult(string text)
        {
            updateContent(text);
            Debug.LogFormat(GlobalVariables.cCommon + "{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "DictationResult: ", text, "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }

        public void startStopDictation()
        {
            //if (dictationHandler != null)
            //    Destroy(dictationHandler);

            //dictationHandler = this.gameObject.AddComponent<DictationHandler>();
            //if (dictationHandler == null)
            //{
            //    Debug.LogError("null handler");
            //}

            //print(dictationHandler.OnDictationComplete == null ? "Delegate not assigned." : "Delegate assigned.");
            
            //dictationHandler.OnDictationComplete.AddListener(DictationComplete);
            ////dictationHandler.OnDictationError.AddListener(DictationError);
            ////dictationHandler.OnDictationHypothesis.AddListener(DictationHypothisis);
            ////dictationHandler.OnDictationResult.AddListener(DictationResult);

            dictationHandler.StartRecording();
            if (placeholder.text == "" && content.text == "")
            {
                placeholder.text = placeholderText;
            }
        }

    }
}