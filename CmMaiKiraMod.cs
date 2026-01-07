using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using CmMaiKiraMod.Common;
using CmMaiKiraMod.Fix;
using CmMaiKiraMod.Utils;
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
        private ConfigEntry<bool> EnableAllOpen { get; set; }
        private ConfigEntry<bool> EnableDisableCryptography { get; set; }
        private ConfigEntry<bool> EnableDisableTLS { get; set; }
        private ConfigEntry<bool> EnableNetworkLogger { get; set; }
        private ConfigEntry<bool> NetworkLoggerPrintToConsole { get; set; }
        private ConfigEntry<bool> EnableIgnoreGetUserSellingCard { get; set; }
        
        public static CmMaiKiraMod INSTANCE = null;
        public static ManualLogSource Log;
        
        void Awake()
        {
            EnableMaiResourceFix = Config.Bind("Maimai.MaiResourcePatch", "Enable", true, "Enable Maimai ResourcePatch\nWill try to fix opt assets load when enable. if has any problem, please disable it");
            EnableMaiRatingSpacingFix = Config.Bind("Maimai.MaiRatingSpacingFix", "Enable", false, "Enable Fix MaimaiDX Print Rating Spacing\nTry to fix rating counter's error spacing");
            EnableIgnoreLoginVersion = Config.Bind("Maimai.IgnoreLoginVersion", "Enable", false, "Enable MaimaiDX Ignore Login Version\nIgnore version check after login");
            EnableInfiniteTimer = Config.Bind("Maimai.InfiniteTimer", "Enable", false, "Enable Infinite Timer");
            EnableAllOpen = Config.Bind("Maimai.AllOpen", "Enable", false, "Open All Card!!!!!!!!!!!!!");
            EnableNetworkLogger = Config.Bind("CmSystem.NetworkLogger", "Enable", false, "Enable network logger");
            NetworkLoggerPrintToConsole = Config.Bind("CmSystem.NetworkLogger", "PrintToConsole", false, "Enable network logger print to console");
            EnableDisableCryptography = Config.Bind("CmSystem.ForceDisableCryptography", "Enable", true, "Disable CardMaker Packet Cryptography");
            EnableDisableTLS = Config.Bind("CmSystem.ForceDisableTLS", "Enable", true, "Disable CardMaker send packet use TLS");
            EnableIgnoreGetUserSellingCard = Config.Bind("CmSystem.IgnoreGetUserSellingCard", "Enable", true, "Skip GetUserSellingCardApi. If your CardMaker version < 14200, will ignore this patch");
            
            
            Logger.LogInfo("Load CmMaiKiraMod Success");
            UpdateConfig();
            Patch(typeof(MaiResourcePatch));
            Patch(typeof(MaiRatingSpacingFix));
            Patch(typeof(IgnoreLoginVersion));
            Patch(typeof(DisableTimer));
            Patch(typeof(AllOpen));
            Patch(typeof(NetworkLogger));
            var harmony = new Harmony("CmMaiKiraMod.NetworkLogger");
            NetworkLogger.PatchAllNetQueryImplementations(harmony);
            Patch(typeof(DisableCryptography));
            Patch(typeof(ForceDisableTLS));
            Patch(typeof(IgnoreGetUserSellingCard));
            INSTANCE = this;
            Log = Logger;
        }

        public void UpdateConfig(bool reload = false)
        {
            if (reload) Config.Reload();
            
            MaiResourcePatch.Enable = EnableMaiResourceFix.Value;
            
            MaiRatingSpacingFix.Enable = EnableMaiRatingSpacingFix.Value;
            IgnoreLoginVersion.Enable = EnableIgnoreLoginVersion.Value;
            DisableTimer.enable = EnableInfiniteTimer.Value;
            
            AllOpen.Enable = EnableAllOpen.Value;
            
            DisableCryptography.Enable = EnableDisableCryptography.Value;
            ForceDisableTLS.Enable = EnableDisableTLS.Value;
            
            NetworkLogger.Enable = EnableNetworkLogger.Value;
            NetworkLogger.EnablePrintToConsole = NetworkLoggerPrintToConsole.Value;
            
            Logger.LogInfo("Update Config");
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