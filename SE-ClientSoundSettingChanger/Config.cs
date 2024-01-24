using Newtonsoft.Json;
using System;
using System.IO;
using VRage.Utils;

namespace SE_RealisticSoundToggle
{
    public class Config
    {
        public bool OverrideWorldSound { get; set; } = false;
        public bool EnableRealisticSound { get; set; } = false;

        private Config()
        {

        }

        public static Config Load(string path)
        {
            Config conf;
            try
            {
                using (var sr = new StreamReader(path))
                {
                    using (var jr = new JsonTextReader(sr))
                    {
                        conf = new JsonSerializer().Deserialize<Config>(jr);
                    }
                }
            }
            catch (FileNotFoundException e)
            {
                MyLog.Default.Warning(e.ToString());
                MyLog.Default.Info("RealisticSoundToggle config file not found, creating default config.");
                conf = new Config();
                conf.Save(path);
            }
            catch (Exception e)
            {
                MyLog.Default.Error(e.ToString());
                MyLog.Default.Info("Creating default config for RealisticSoundToggle plugin.");
                conf = new Config();
                conf.Save(path);
            }

            return conf;
        }

        public void Save(string path)
        {
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var serializer = new JsonSerializer()
            {
                Formatting = Formatting.Indented,
            };

            try
            {
                using (var sw = new StreamWriter(path))
                {
                    using (var jw = new JsonTextWriter(sw))
                    {
                        serializer.Serialize(jw, this);
                    }
                }
            }
            catch (Exception e)
            {
                MyLog.Default.Error("Error occurred while saving RealisticSoundToggle config." + e.ToString());
            }
        }
    }
}
