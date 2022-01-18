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
        internal static Ui ui = null;
        private static int procID = -1;
        private static bool killswitch = false;
        private static string CMDLine = "";

        private static async Task Setup()
        {
            var programAssembly = ProgramAssembly.GetInstance();
            var config = Config.GetInstance(programAssembly.Path());

            await Task.Delay(1000);

            Config.Settings = config.GetConfig();
            CMDLine = config.GetConfigAsString(Config.Settings);
            ui = Ui.GetInstance();
        }

        [STAThread]
        internal static async Task Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            await Setup();
            Application.Run(ui);
        }

        internal static void SwitchOnlineState(object sender, EventArgs e)
        {
            if (ui.StartSwitch.Checked) {
                killswitch = false;
                Run();
            } else {
                killswitch = true;
                Die();
            }
            ui.pidValueBox.Text = procID.ToString();
            ui.Text = killswitch ? "Offline" : "Online";
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
            ui.pidValueBox.Text = procID.ToString();
            ui.Text = procID == -1 ? "Offline" : "Online";
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