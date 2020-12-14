
namespace Photon_IATK
{
    public class GlobalVariables
    {
        public static byte maxPlayers = 5;

        public static string red = "<color=#ff0000>";
        public static string green = "<color=#00ff00>";
        public static string purple = "<color=#ff00ff>";
        public static string yellow = "<color=#ffff00>";
        public static string blue = "<color=#0000FF>";
        public static string endColor = "</color>";
        public static string newLine = "\n";

        public static string Desktop = "DESKTOP";
        public static string Vive = "VIVE";
        public static string HL2 = "HL2";

        public enum allSymbols
        {
            DESKTOP,
            VIVE,
            HL2
        };

        public enum PlayerPrefsKeys
        {
            ParticipantID
        };

    }
}
