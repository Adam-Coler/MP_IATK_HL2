using UnityEngine;

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
    }
}