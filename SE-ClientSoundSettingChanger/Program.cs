using Sandbox;
using Sandbox.Engine.Utils;
using Sandbox.Game.World;
using Sandbox.Graphics.GUI;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage.FileSystem;
using VRage.Game.Components;
using VRage.Plugins;
using VRageMath;

namespace SE_RealisticSoundToggle
{
    public class Main : IPlugin
    {
        public Config _config;
        public string _configPath;

        public void Init(object gameInstance)
        {
            _configPath = Path.Combine(MyFileSystem.UserDataPath, "Storage", "RealisticSoundToggle.cfg");
            _config = Config.Load(_configPath);
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

    [MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
    public class Session : MySessionComponentBase
    {
        public static bool? IsSessionRealisticSound = null;

        public override void BeforeStart()
        {
            IsSessionRealisticSound = MySession.Static.Settings.RealisticSound;
        }

        protected override void UnloadData()
        {
            IsSessionRealisticSound = null;
        }
    }
}
