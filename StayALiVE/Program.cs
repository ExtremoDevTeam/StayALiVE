using System;
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace StayALiVE
{
    internal class ProgramAssembly
    {
        internal static ProgramAssembly GetInstance() { return new ProgramAssembly(); }
        internal Assembly _assembly = typeof(Program).Assembly;
        internal Program program = null;
        internal Ui ui = null;

        internal ProgramAssembly()
        {
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolver;
            program = Program.GetInstance();
        }
        private Assembly AssemblyResolver(object sender, ResolveEventArgs args)
        {
            var askedAssembly = new AssemblyName(args.Name);
            lock (this)
            {
                var stream = _assembly.GetManifestResourceStream($"{Assembly.GetExecutingAssembly().GetName().Name}.Embedded.Assemblies.{askedAssembly.Name}.dll");
                if (stream == null) return null;

                Assembly assembly = null;
                try
                {
                    var assemblyData = new byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);
                    assembly = Assembly.Load(assemblyData);
                }
                catch (Exception e)
                {
                    throw new Exception($"Loading embedded assembly: {askedAssembly.Name}{Environment.NewLine}Has thrown a unhandled exception: {e}");
                }
                return assembly;
            }
        }
    }

    internal class Program
    { 
        internal static Program GetInstance() { return new Program(); }
        internal static ProgramAssembly programAssembly = null;
        private static int procID = -1;
        private static bool killswitch = false;
        private static string CMDLine = "";

        [STAThread]
        internal static void Main()
        {
            programAssembly = ProgramAssembly.GetInstance();

            void MakeDefaultCMDLine(string path)
            {
                if (!File.Exists(path))
                {
                    string defaultCMDline = "" +
                        "-port=2302 " +
                        "-profiles=c:\\Arma3Server\\@server\\server_configs\\serverData " +
                        "-name=Admin " +
                        "-cfg=c:\\Arma3Server\\@server\\server_configs\\A3_Performance.cfg " +
                        "-config=c:\\Arma3Server\\@server\\server_configs\\A3_DedicatedServer.cfg " +
                        "-servermod=@server " +
                        "-enableHT " +
                        "-hugepages " +
                        "-bandwidthAlg=2 " +
                        "-autoinit";
                    File.WriteAllText(path, defaultCMDline, Encoding.UTF8);
                    Environment.Exit(2);
                }
            }
            void LoadCMDLine()
            {
                string path = Path.Combine(Path.GetDirectoryName(programAssembly._assembly.Location), "StayALiVE.cmdline");
                MakeDefaultCMDLine(path);
                CMDLine = File.ReadAllText(path);
            }
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            LoadCMDLine();
            programAssembly.ui = Ui.GetInstance();
            Application.Run(programAssembly.ui);
        }
        internal static void SwitchOnlineState(object sender, EventArgs e)
        {
            if (programAssembly.ui.StartSwitch.Checked) {
                killswitch = false;
                Run();
            } else {
                killswitch = true;
                Die();
            }
            programAssembly.ui.pidValueBox.Text = procID.ToString();
            programAssembly.ui.Text = killswitch ? "Offline" : "Online";
        }
        private static void Run()
        {
            if (killswitch || IsRunning()) return;
            void Die(object sender, EventArgs e) => Run();
            var serverProcess = new Process { StartInfo = new ProcessStartInfo { FileName = "arma3server_x64.exe", Arguments = CMDLine }, EnableRaisingEvents = true };
            serverProcess.Exited += Die;
            serverProcess.Start();
            procID = serverProcess.Id;
            programAssembly.ui.pidValueBox.Text = procID.ToString();
            programAssembly.ui.Text = procID == -1 ? "Offline" : "Online";
        }
        private static void Die()
        {
            if (!killswitch || !IsRunning()) return;
            Task.Delay(1000);
            Process.GetProcessById(procID).Kill();
            procID = -1;
        }
        private static bool IsRunning()
        {
            try { Process.GetProcessById(procID); }
            catch (InvalidOperationException) { return false; }
            catch (ArgumentException) { return false; }
            return true;
        }
    }
}