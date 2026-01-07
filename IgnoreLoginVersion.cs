using CardMaker.MAI;
using HarmonyLib;

namespace CmMaiKiraMod
{
    public class IgnoreLoginVersion
    {
        public static bool Enable = false;
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MAIContext), "isBlockLoginForPreviewUserVersion")]
        public static void isBlockLoginForPreviewUserVersion_Postfix(ref bool __result)
        {
            if (Enable)
            {
                __result = false;
            }
        }
    }
}