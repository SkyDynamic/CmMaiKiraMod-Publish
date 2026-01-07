using System;
using CardMaker.Common;
using CardMaker.MAI;
using HarmonyLib;

namespace CmMaiKiraMod.Common
{
    public class IgnoreGetUserSellingCard
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Scene_MAIAime), "UserSellingCard_Proc")]
        public static bool UserSellingCard_Proc_Prefix(Scene_MAIAime __instance)
        {
            if (ContextBase<CommonContext>.Instance.AMManager.RomVersion.Code < 14200) return true;
            
            MAIContext.Instance.SellingCardContext.setUserSellingList(new GameSellingCard[] {});
            
            var stateType = typeof(Scene_MAIAime).GetNestedType("State", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
    
            if (stateType != null)
            {
                var enumValues = Enum.GetValues(stateType);
                var targetState = enumValues.GetValue(18);
                
                var currentMode = AccessTools.Field(typeof(Scene_MAIAime), "mode_").GetValue(__instance);
                
                var setStateMethod = AccessTools.Method(currentMode.GetType(), "set");
                setStateMethod?.Invoke(currentMode, new [] { targetState });
            }
    
            return false;
        }
    }
}