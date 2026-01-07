using System;
using System.IO;
using System.Linq;
using System.Reflection;
using CardMaker.Common;
using HarmonyLib;

namespace CmMaiKiraMod.Utils
{
    public class NetworkLogger
    {
        public static bool Enable = false;
        public static bool EnablePrintToConsole = false;
        
        private static readonly object LOGLock = new object();
        private static CommonStatic.Server _nowServer;
        private static string _url = "";
        
        private enum HttpMessageType
        {
            Request,
            Response,
            Null
        }
        
        public static void PatchAllNetQueryImplementations(Harmony harmony)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                try
                {
                    var netQueryTypes = assembly.GetTypes()
                        .Where(t => typeof(INetQuery).IsAssignableFrom(t) && 
                                    !t.IsInterface && 
                                    t != typeof(INetQuery));
    
                    foreach (var type in netQueryTypes)
                    {
                        var method = AccessTools.Method(type, "setResponse");
                        if (method != null)
                        {
                            try
                            {
                                harmony.Patch(method, prefix: new HarmonyMethod(typeof(NetworkLogger), "SetResponse_Prefix"));
                            }
                            catch
                            {
                                // ...
                            }
                        }
                    }
                }
                catch (ReflectionTypeLoadException)
                {
                }
            }
        }

        
        public static bool SetResponse_Prefix(INetQuery __instance, string str)
        {
            PrintNetworkLog(HttpMessageType.Response, _nowServer, __instance.URL, str);
            return true;
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PacketBase), "create", 
            typeof(INetQuery), typeof(ushort), typeof(string), 
            typeof(string), typeof(string), 
            typeof(PacketBase.OnFinishDeligate), typeof(CommonStatic.Server))]
        public static bool PacketBase_create_Prefix(PacketBase __instance, 
            ref INetQuery query, ref ushort port, ref string host, 
            ref string hostPort, ref string basePath, 
            ref PacketBase.OnFinishDeligate onFinishDelegate, 
            ref CommonStatic.Server server)
        {
            _url = hostPort + basePath;
            _nowServer = server;
            return true;
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PacketBase), "procImpl")]
        public static bool PacketBase_procImpl_Prefix(PacketBase __instance)
        {
            switch (__instance.state)
            {
                case PacketBase.State.Ready:
                {
                    var query = (INetQuery)AccessTools.Field(typeof(PacketBase), "query_").GetValue(__instance);
                    PrintNetworkLog(HttpMessageType.Request, _nowServer, query.URL, query.getRequest());
                    break;  
                } 
            }
            return true;
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PacketBase), "procImpl")]
        public static void PacketBase_procImpl_Postfix(PacketBase __instance)
        {
            if (__instance.state == PacketBase.State.Error)
            {
                var query = (INetQuery)AccessTools.Field(typeof(PacketBase), "query_").GetValue(__instance);
                PrintNetworkLog(HttpMessageType.Response, _nowServer, query.URL, $"Error: {__instance.status}, HttpStatusCode: {__instance.httpStatus}");
            }
        }
        
        private static void PrintNetworkLog(HttpMessageType httpMessageType, CommonStatic.Server server, string api, string content)
        {
            if (!Enable) return;
            
            if (httpMessageType == HttpMessageType.Null) return;
            try
            {
                var now = DateTime.Now;
                var logText = now.ToString("[HH:mm:ss.fff] ");
                logText += $"[{server}] [{httpMessageType}] [{_url + api}] -> {content}\n";
                
                if (EnablePrintToConsole)
                {
                    CmMaiKiraMod.Log.LogInfo($"[NetworkLogger] [{server}] [{httpMessageType}] [{api}] -> {content}");
                }

                lock (LOGLock)
                {
                    var logDir = Path.Combine("CmMaiKiraMod", "NetworkLogs");
                    if (!Directory.Exists(logDir))
                    {
                        Directory.CreateDirectory(logDir);
                    }
                    File.AppendAllText(Path.Combine(logDir, now.ToString("yyyy-MM-dd") + ".log"), logText);
                }
            }
            catch (Exception e)
            {
                CmMaiKiraMod.Log.LogError(e);
            }
        }
    }
}