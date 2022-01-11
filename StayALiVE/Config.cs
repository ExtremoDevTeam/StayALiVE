using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace StayALiVE
{

    internal class Config
    {
        public List<string> LoadedServerMods { get; set; } = new List<string>() { };
        private static readonly string Defaultpath = "c:\\Arma3Server\\@server\\server_configs";
        private static string ConfigPath { get; set; }

        internal class Holder
        {
            public int Port { get; set; } = 2302;

            public string ProfileName { get; set; } = "Admin";
            public string ProfilesPath { get; set; } = Path.Combine(Defaultpath, "serverData");
            public string CfgsPath { get; set; } = Defaultpath;
            public string CfgPerformance { get; set; } = "A3_Performance.cfg";
            public string CfgDedicatedServer { get; set; } = "A3_DedicatedServer.cfg";
            public string CustomServerDirectory { get; set; } = string.Empty;
            public List<string> ServerMods { get; set; } = new List<string>()
            {
                "@server"
            };
            public bool EnableHyperThreading { get; set; } = false;
            public bool EnableLargePage { get; set; } = false;
            public bool EnableExperimentalAlg { get; set; } = true;
            public bool EnableAutoInit { get; set; } = true; 
            public bool Enable64Bit { get; set; } = true;
        }

        private bool ExistsConfig() => File.Exists(ConfigPath);

        private void DelConfig() => File.Delete(ConfigPath);
        
        private void NewConfig() => File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(new Holder() { }, Formatting.Indented));

        internal Holder GetConfig() => JsonConvert.DeserializeObject<Holder>(File.ReadAllText(ConfigPath));

        internal Config(string path)
        {
            ConfigPath = path;

            if (!ExistsConfig())
            {
                NewConfig();
                Environment.Exit(0);
            }
        }

        internal string GetConfigAsString(Holder config)
        {
            if(config == null)
                Environment.Exit(0);

            string output = $"-port={config.Port} " +
                            $"-profiles={config.ProfilesPath} " +
                            $"-name={config.ProfileName} " +
                            $"-cfg={Path.Combine(config.CfgsPath, config.CfgPerformance)} " +
                            $"-config={Path.Combine(config.CfgsPath, config.CfgDedicatedServer)} ";

            string svrmods = "";
            foreach (string mod in config.ServerMods)
            {
                if (LoadedServerMods.Contains(mod)) continue;
                LoadedServerMods.Add(mod);
                svrmods += $"{mod};";
            }

            if (svrmods.EndsWith(";"))
                svrmods = svrmods.TrimEnd(';');

            if (svrmods.Length > 0)
                output += $"-servermod={svrmods} ";

            if (config.EnableHyperThreading)
                output += $"-enableHT ";

            if (config.EnableLargePage)
                output += $"-hugepages ";

            if (config.EnableExperimentalAlg)
                output += $"-bandwidthAlg=2 ";

            if (config.EnableAutoInit)
                output += $"-autoinit";

            return output;
        }
    }
}
