using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ContentResizer : MonoBehaviour
{
    private RectTransform m_Rect;
    private RectTransform m_inputFieldRect;

    private RectTransform rectTransform
    {
        get
        {
            if (m_Rect == null)
                m_Rect = GetComponent<RectTransform>();
            return m_Rect;
        }
    }

    private RectTransform inputFieldTextRectTransform
    {
        get
        {
            if (m_inputFieldRect == null)
                m_inputFieldRect = GetComponent<TMP_InputField>().textComponent.rectTransform;
            return m_inputFieldRect;
        }
    }

    private void Update()
    {
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, LayoutUtility.GetPreferredSize(inputFieldTextRectTransform, 1) + 5);
        inputFieldTextRectTransform.localPosition = Vector3.zero; // stops the text scrolling sideways - it doesn't need to
    }
}