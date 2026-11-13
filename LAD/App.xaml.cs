using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace LAD
{
    public partial class App : Application
    {
        public App()
        {
            DispatcherUnhandledException += OnUnhandledException;
        }

        internal static string WriteErrorLog(Exception exception)
        {
            try
            {
                string directory = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LAD");
                Directory.CreateDirectory(directory);
                string path = Path.Combine(directory, "error.log");
                File.AppendAllText(path, DateTime.Now + Environment.NewLine + exception + Environment.NewLine);
                return path;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            string logPath = WriteErrorLog(e.Exception);
            MessageBox.Show("Ошибка LAD: " + e.Exception.Message +
                (logPath == null ? "" : "\nПодробности: " + logPath),
                "LAD", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
            Shutdown();
        }
    }
}
