using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Photon_IATK
{
    public class TextAnnotationManager : MonoBehaviour
    {
        public TMPro.TextMeshPro title;
        public TMPro.TextMeshProUGUI content;
        public TMPro.TextMeshProUGUI placeholder;
        public Annotation myAnnotationParent;

        public void onContentUpdate()
        {
            if (myAnnotationParent != null)
            {
                myAnnotationParent.UpdateText(content.text);
            }
        }

        public void updateContent(string text)
        {
            content.text = text;
        }
    }
}