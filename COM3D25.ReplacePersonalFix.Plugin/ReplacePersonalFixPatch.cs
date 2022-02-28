using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace COM3D2.ReplacePersonalFix.Plugin
{
    internal class ReplacePersonalFixPatch
    {
        internal static ManualLogSource log;
        internal static ConfigFile config;

        internal static void Awake(BepInEx.Logging.ManualLogSource logger, BepInEx.Configuration.ConfigFile Config)
        {
            log = logger;
            config = Config;

            ScriptManager_ReplacePersonal_Log = config.Bind("ScriptManager", "ReplacePersonal Log", false);
        }

        internal static ConfigEntry<bool> ScriptManager_ReplacePersonal_Log;

        [HarmonyPatch(typeof(ScriptManager), "ReplacePersonal", typeof(Maid[]), typeof(string))]
        [HarmonyPrefix]
        public static void ReplacePersonal_pre(Maid[] maid_array, string text)
        {
            ks = text;

            if (ScriptManager_ReplacePersonal_Log.Value)
                log.LogMessage($"ScriptManager.ReplacePersonal_pre , {maid_array.Length} , {text}");
        }

        static string ks;

        [HarmonyPatch(typeof(ScriptManager), "ReplacePersonal", typeof(Maid[]), typeof(string))]
        [HarmonyPostfix]
        public static void ReplacePersonal_post(ref string __result, Maid[] maid_array)
        {
            if (string.IsNullOrEmpty(__result)) return;
            if (IsExistFile(__result)) return;
            log.LogWarning($"Not IsExistFile {ks} to {__result}");

            if (ScriptManager_ReplacePersonal_Log.Value)
                log.LogMessage($"ScriptManager.ReplacePersonal_post , {maid_array.Length} , {__result}");

            HashSet<string> ids = new HashSet<string>(ReplacePersonalFix.personalIds);
            bool isNotFile;
            do
            {
                __result = ReplacePersonal(ks, ids);

                if (ScriptManager_ReplacePersonal_Log.Value)
                    log.LogMessage($"ScriptManager.ReplacePersonal_post fix , {ks} , {__result}");
            }
            while (isNotFile = !IsExistFile(__result) && ids.Count > 0);
            if (isNotFile)
            {
                log.LogError($"Not have script : {ks}");
            }
            else
            {
                log.LogWarning($"Change {ks} to {__result}");
            }
        }

        private static bool IsExistFile(string text)
        {
            if (!text.EndsWith(".ks"))
              text += ".ks";            
            return GameUty.IsExistFile(text);
        }

        public static string ReplacePersonal( string text, HashSet<string> ids)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }            
            var id = ids.ElementAt(UnityEngine.Random.Range(0, ids.Count - 1));
            ids.Remove(id);            
            return text.Replace("?", id);
        }

    }
}
