using CardMaker.Common;
using HarmonyLib;

namespace CmMaiKiraMod.Common
{
    public class DisableCryptography
    {
        public static bool Enable = false;
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Cryptography), "getCryptVersion")]
        public static void getCryptVersion_Prefix(ref int __result)
        {
            if (!Enable) return;
            __result = 0;
        }
    }
}