using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using StayALiVE_Lib;

namespace StayALiVE_Updater
{ 
    internal class VersionTemplate
    {
        public string Version { get; set; } = "0.0.0";
        public List<string> Changes { get; set; } = new List<string>();
        internal bool Check(string version) => !Version.Equals(version);
    }

    internal class Program
    {
        internal static Program GetInstance() { return new Program(); }
        internal static ProgramAssembly Assembly = null;
        internal static ProgramArguments Arguments = null;
         

        private static string AppEXE => Path.Combine(Assembly.Path(), "StayALiVE.exe");
        private static string AppData => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StayALiVE");
        private static string AppConfig => Path.Combine(AppData, "version.json");
        private const string VersionURL = "https://raw.githubusercontent.com/Ni1kko/StayALiVE/master/version.json";


        private protected static async Task Init(List<string> arguments)
        {
            Assembly = ProgramAssembly.GetInstance();
            Arguments = new ProgramArguments(arguments);
            await Task.CompletedTask;
            if (!Directory.Exists(AppData)) Directory.CreateDirectory(AppData);
            if (!File.Exists(AppConfig)) NewConfig(new VersionTemplate());
        }

        private static async Task<VersionTemplate> DownloadConfig()
        {
            var response = await new RestClient(VersionURL).ExecuteAsync(new RestRequest());
            return response.StatusCode == HttpStatusCode.OK ? JsonConvert.DeserializeObject<VersionTemplate>(response.Content) : null;
        }

        private static void NewConfig(VersionTemplate config) => File.WriteAllText(AppConfig, JsonConvert.SerializeObject(config, Formatting.Indented));

        private protected static async Task<bool> HasUpdate(VersionTemplate remote)
        {
            var local = new VersionTemplate() { Version = "-1.0.0" };

            if (File.Exists(AppConfig))
            {
                var localconfig = JsonConvert.DeserializeObject<VersionTemplate>(File.ReadAllText(AppConfig));
                local.Version = localconfig.Version;
                local.Changes = localconfig.Changes;
            }

            var res = remote.Check(local.Version);

            if (res)
            {
                Print($"Update v{remote.Version} ready to be downloaded");
                if (remote.Changes.Any())
                {
                    Print($"ChangeLog:");
                    remote.Changes.ForEach(x => Print($"\t- {x}"));
                }
            }

            return res;
        }

        private protected static async Task<bool> StartUpdate(VersionTemplate remote)
        {
            //purge
            if (Arguments.Purge && File.Exists(AppEXE)) 
                File.Delete(AppEXE);


            //download

            //store version
             NewConfig(remote);
            
            //check
            return !await HasUpdate(remote);
        }

        private protected static ConsoleKeyInfo Print(string message,bool waitforinput = false, ConsoleKey defaultKey = ConsoleKey.NoName)
        {
            var keyinfo = new ConsoleKeyInfo(' ', defaultKey, false, false, false);
            
            if (!Arguments.Silent)
            {
                Console.WriteLine(message);
                if(waitforinput) 
                    keyinfo = Console.ReadKey();
            }

            return keyinfo;
        }

        /// <summary>
        /// Entry point
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        internal static async Task Main(string[] arguments)
        {
            await Init(arguments.ToList());
             
            var remote = await DownloadConfig();

            //-- site down
            if (remote == null)
            {
                Print($"Error Site {VersionURL}\nUnreachable!", true);
                return;
            }

            //-- download update
            if (File.Exists(AppEXE))
            {
                if (await HasUpdate(remote))
                {
                    Print("Has Update!");
                    if (await StartUpdate(remote))
                        Print("Updated!", true);
                }
                else
                    Print("Already updated!", true);
            }
            else
            {
                Print($"Downloading v{remote.Version}!");
                if (remote.Changes.Any())
                {
                    Print("\nChangeLog:");
                    remote.Changes.ForEach(x => Print($"\t- {x}"));
                    Print(Environment.NewLine);
                }
                if (await StartUpdate(remote))
                { 
                    do { Console.Clear(); } while (Print("Downloaded, Press `E` to exit!", true, ConsoleKey.E).Key != ConsoleKey.E);
                }
            } 

            //-- start updated exe
            if (Arguments.Restart)
            {
                if (File.Exists(AppConfig))
                    Process.Start(AppEXE);
            }
        }
    }
}
