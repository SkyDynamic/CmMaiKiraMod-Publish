using CardMaker.Common;
using HarmonyLib;

namespace CmMaiKiraMod.Common
{
    public class ForceDisableTLS
    {
        public static bool Enable = false;
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(NetConfig), "get_UseTLS")]
        public static void NetHttpClient_request_Post(ref bool __result)
        {
            if (!Enable) return;
            
            __result = false;
        }
    }
}