using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

//-- https://github.com/Ni1kko/StayALiVE

namespace StayALiVE
{
    internal class Program
    { 
        internal static Program GetInstance() { return new Program(); } 
        internal static ProgramAssembly programAssembly = null;
        internal static Config config = null;
        private static int procID = -1;
        private static bool killswitch = false;
        private static string CMDLine = "";

        [STAThread]
        internal static void Main()
        {
            programAssembly = ProgramAssembly.GetInstance();
            
            var config = Config.GetInstance(Path.GetDirectoryName(programAssembly._assembly.Location));
            Config.Settings = config.GetConfig();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
             
            CMDLine = config.GetConfigAsString(Config.Settings);
            
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
            var procname = Config.Settings.Enable64Bit ? "arma3server_x64.exe" : "arma3server.exe";
            var serverProcess = new Process { StartInfo = new ProcessStartInfo { FileName = Config.Settings.CustomServerDirectory.Equals(string.Empty) ? procname : Path.Combine(Config.Settings.CustomServerDirectory, procname), Arguments = CMDLine }, EnableRaisingEvents = true };
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