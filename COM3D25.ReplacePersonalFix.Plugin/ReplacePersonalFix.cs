using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using MaidStatus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COM3D25.ReplacePersonalFix.Plugin
{
    class MyAttribute
    {
        public const string PLAGIN_NAME = "ReplacePersonalFix";
        public const string PLAGIN_VERSION = "22.2.24";
        public const string PLAGIN_FULL_NAME = "COM3D2.ReplacePersonalFix.Plugin";
    }


    [BepInPlugin(MyAttribute.PLAGIN_FULL_NAME, MyAttribute.PLAGIN_NAME, MyAttribute.PLAGIN_VERSION)]

    public class ReplacePersonalFix : BaseUnityPlugin
    {
        public static Harmony harmony;
        public static ManualLogSource log;
        public static List<Personal.Data> personalDataEnable;
        public static HashSet<string> personalIds=new HashSet<string>();

        public void Awake()
        {
            log = Logger;
            log.LogMessage($"Awake");
            ReplacePersonalFixPatch.Awake(Logger, Config);
        }

        public void OnEnable()
        {
            log.LogMessage($"OnEnable");
            try
            {
                if (harmony==null)
                {
                    harmony = Harmony.CreateAndPatchAll(typeof(ReplacePersonalFixPatch));
                }
            }
            catch (Exception e)
            {
                log.LogFatal($"Harmony {e.ToString()}");
            }
        }

        public void Start()
        {
            log.LogMessage($"Start");
            personalDataEnable = Personal.GetAllDatas(true);
            foreach (var item in personalDataEnable)
            {
                personalIds.Add(item.replaceText);
            }
        }

        public void OnDisable()
        {
            log.LogMessage($"OnDisable");
            harmony?.UnpatchSelf();
        }
    }
}
