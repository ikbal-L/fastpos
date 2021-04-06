using System;
using System.Diagnostics;
using System.IO;
using Caliburn.Micro;
using NLog;
using FastPosFrontend.ViewModels;
using System.Windows;
using FastPosFrontend.Helpers;

namespace FastPosFrontend
{
    public class ShellBootstrapper : BootstrapperBase
    {
        private Process _backendServerProcess;

        public ShellBootstrapper()
        {
            var config = new NLog.Config.LoggingConfiguration();
            var logfile = new NLog.Targets.FileTarget("logfile") {FileName = "/logs/fastpos.txt"};
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
            NLog.LogManager.Configuration = config;

            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            //var splashScreen = new SplashScreenView();
            //splashScreen.Show();
            //System.Threading.Thread.Sleep(7000);
            //splashScreen.Close();


            //var databaseProcess = Process.Start(@"C:\xampp\mysql_start.bat");
            //databaseProcess.StartInfo.UseShellExecute = false;
            //databaseProcess.StartInfo.CreateNoWindow = true;
            //ReadProcessId();
            var databaseProcess = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    WorkingDirectory = @"C:\xampp",
                    FileName = @"C:\xampp\mysql_start.bat",
                    Arguments = "bootrun",
                    ErrorDialog = true,
                    CreateNoWindow = true,
                    UseShellExecute = false,

                },
            };

            _backendServerProcess = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    WorkingDirectory = @"C:\Users\User\source\fastpos-backend",
                    FileName = @"C:\Users\User\source\fastpos-backend\gradlew.bat",
                    Arguments = "bootrun",
                    ErrorDialog = true,
                    CreateNoWindow = true,
                    UseShellExecute = false
                    
                }
            };
            if (!ConnectionHelper.PingHost(portNumber: 3306))
            {
                databaseProcess?.Start();
            }

            if (!ConnectionHelper.PingHost())
            {
                _backendServerProcess.Start();
                //WriteProcessId(_backendServerProcess.Id);
            }

            var stopWatch = Stopwatch.StartNew();
            stopWatch.Start();
            while (!ConnectionHelper.PingHost())
            {
                if (stopWatch.ElapsedMilliseconds >= 30000)
                {
                    stopWatch.Stop();
                    //backendServerProcess.Kill();
                    break;
                }
            }

            if (stopWatch.IsRunning)
            {
                stopWatch.Stop();
                DisplayRootViewFor<MainViewModel>();
            }
            else
            {
                MessageBox.Show("Error! Unable to start the server", "Server Connection Error", MessageBoxButton.OK,MessageBoxImage.Error);
                this.Application.Shutdown(1);
            }

            //DisplayRootViewFor<MainViewModel>();
            


        }

        protected override void OnExit(object sender, EventArgs e)
        {
            //EXCEPTION
           
            _backendServerProcess.Kill();
            base.OnExit(sender, e);
        }

        private static void WriteProcessId(int id)
        {
            File.WriteAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}temp_pos.txt",$"{id}");
        }
        private static void ReadProcessId()
        {
            var stringId = File.ReadAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}temp_pos.txt"); 
            ;
            if (int.TryParse(stringId, out var id))
            {
                try
                {
                    var process = Process.GetProcessById(id);
                    process.Kill();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}