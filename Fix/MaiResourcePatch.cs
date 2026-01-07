using System.Collections.Generic;
using CardMaker.Common;
using CardMaker.MAI;
using AMDaemon;
using CmMaiKiraMod.Utils;
using HarmonyLib;

namespace CmMaiKiraMod.Fix
{
    public class MaiResourcePatch
    {
        public static bool Enable = false;
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MAIContext), "Awake")]
        public static void Awake_Postfix(MAIContext __instance)
        {
            if (Enable)
            {
                var list = new List<CardMakerUtils.Entry>();
                CardMakerUtils.Find(list, AppImage.OptionMountRootPath, "/MAI");
                for (int index = 0; index < list.Count; ++index)
                {
                    if (list[index].type_ == CardMakerUtils.Entry.Type_OS)
                    {
                        AssetBundleDB.getInstance(AssetBundleDB.Title.Maimai).appendAssetBundleSet(list[index].path_);
                    }
                }
            }
        }
    }
}
