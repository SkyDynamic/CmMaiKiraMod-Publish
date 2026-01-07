using CardMaker.MAI;
using HarmonyLib;

namespace CmMaiKiraMod
{
    public class DisableTimer
    {
        public static bool enable = false;
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Scene_MAIHome), "Start")]
        public static bool Start_Prefix(Scene_MAIHome __instance)
        {
            CmMaiKiraMod.INSTANCE.UpdateConfig(true);
            return true;
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Scene_MAIHome), "toSomeProc")]
        public static void beginBuyCardProcess_Prefix(Scene_MAIHome __instance)
        {
            if (!enable) return;
            MAIContext.Instance.UIHeader.Timer.Pause = true;
            MAIContext.Instance.UIHeader.Timer.Pause2 = true;
        }
    }
}