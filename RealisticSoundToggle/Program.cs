using HarmonyLib;
using Sandbox;
using Sandbox.Engine.Utils;
using Sandbox.Game.World;
using Sandbox.Graphics.GUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VRage.FileSystem;
using VRage.Game;
using VRage.Game.Components;
using VRage.Plugins;

namespace SE_RealisticSoundToggle
{
    public class Program : IPlugin
    {
        public static Config _config;
        public static string _configPath;

        public void Init(object gameInstance)
        {
            _configPath = Path.Combine(MyFileSystem.UserDataPath, "Storage", "RealisticSoundToggle.cfg");
            _config = Config.Load(_configPath);

            new Harmony("RealisticSoundToggle").PatchAll(Assembly.GetExecutingAssembly());
        }

        public void Update()
        {

        }

        public void Dispose()
        {
            _config = null;
            _configPath = null;
        }

        public void OpenConfigDialog()
        {
            MyGuiSandbox.AddScreen(new ConfigScreen(_config, _configPath));
        }
    }

    [HarmonyPatch(typeof(MySession))]
    [HarmonyPatch("GetCheckpoint")]
    public class Patch_MySession_GetCheckpoint
    {
        //[HarmonyTargetMethod]
        //public static MethodInfo TargetMethod()
        //{
        //    return AccessTools.Method(typeof(MySession), "GetCheckpoint");
        //}

        [HarmonyPostfix]
        public static void Postfix(MyObjectBuilder_Checkpoint __result)
        {
            if (SessionComp.IsSessionRealisticSound.HasValue)
            {
                __result.Settings.RealisticSound = SessionComp.IsSessionRealisticSound.Value;
            }
        }
    }

    [MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
    public class SessionComp : MySessionComponentBase
    {
        public static bool? IsSessionRealisticSound = null;
        private static SessionComp _instance;

        public override void LoadData()
        {
            IsSessionRealisticSound = MySession.Static.Settings.RealisticSound;
            _instance = this;

            if (Program._config.OverrideWorldSound)
            {
                MySession.Static.Settings.RealisticSound = Program._config.EnableRealisticSound;
            }
        }

        protected override void UnloadData()
        {
            IsSessionRealisticSound = null;
            _instance = null;
        }

        public static void UpdateSoundSetting()
        {
            if (_instance == null || !IsSessionRealisticSound.HasValue || MySession.Static == null)
            {
                return;
            }

            MySession.Static.Settings.RealisticSound = Program._config.OverrideWorldSound ? Program._config.EnableRealisticSound : IsSessionRealisticSound.Value;
        }
    }
}
