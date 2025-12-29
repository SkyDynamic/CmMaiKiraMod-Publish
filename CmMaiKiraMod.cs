using System;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace CmMaiKiraMod
{
    [BepInPlugin("CmMaiKiraMod", "CmMaiKiraMod", "1.3.0.0")]
    public class CmMaiKiraMod : BaseUnityPlugin
    {
        private ConfigEntry<bool> EnableMaiResourceFix { get; set; }
        private ConfigEntry<bool> EnableMaiRatingSpacingFix { get; set; }
        private ConfigEntry<bool> EnableIgnoreLoginVersion { get; set; }
        private ConfigEntry<bool> EnableInfiniteTimer { get; set; }
        
        void Awake()
        {
            EnableMaiResourceFix = Config.Bind("MaiResourcePatch", "Enable", true, "Enable Maimai ResourcePatch\nWill try to fix opt assets load when enable. if has any problem, please disable it");
            EnableMaiRatingSpacingFix = Config.Bind("MaiRatingSpacingFix", "Enable", false, "Enable Fix MaimaiDX Print Rating Spacing\nTry to fix rating counter's error spacing");
            EnableIgnoreLoginVersion = Config.Bind("IgnoreLoginVersion", "Enable", false, "Enable MaimaiDX Ignore Login Version\nIgnore version check after login");
            EnableInfiniteTimer = Config.Bind("DisableTimer", "Enable", false, "Disable Timer\nDisable timer when game start");
            
            Logger.LogInfo("Load CmMaiKiraMod Success");
            Patch(typeof(MaiResourcePatch));
            Patch(typeof(MaiRatingSpacingFix));
            Patch(typeof(IgnoreLoginVersion));
            Patch(typeof(DisableTimer));
        }

        private void Update()
        {
            MaiResourcePatch.Enable = EnableMaiResourceFix.Value;
            
            MaiRatingSpacingFix.Enable = EnableMaiRatingSpacingFix.Value;
            IgnoreLoginVersion.Enable = EnableIgnoreLoginVersion.Value;
            DisableTimer.enable = EnableInfiniteTimer.Value;
        }

        private bool Patch(Type type, bool noLoggerPrint = false)
        {
            try
            {
                Harmony.CreateAndPatchAll(type);
                Logger.LogInfo($"Patch {type.Name}");
                return true;
            } catch (Exception e)
            {
                if (!noLoggerPrint)
                {
                    Logger.LogError($"Failed to patch {type.Name}: {e}");
                }
                return false;
            }
        }
    }
}