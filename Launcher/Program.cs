using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace LADLauncher
{
    internal static class Program
    {
        private const string BuildUrl = "https://raw.githubusercontent.com/kuzya1014/LAD/build-output/";
        private static readonly TimeSpan CheckInterval = TimeSpan.FromMinutes(10);

        [STAThread]
        private static int Main(string[] args)
        {
            bool checkOnly = args.Length == 1 && args[0] == "--check";
            string directory = AppDomain.CurrentDomain.BaseDirectory;
            string executable = Path.Combine(directory, "LAD.exe");
            string checkStamp = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "LAD", "last_update_check.txt");
            try
            {
                if (checkOnly || !File.Exists(executable) || !RecentlyChecked(checkStamp))
                {
                    UpdateIfNeeded(executable);
                    if (!checkOnly) RememberCheck(checkStamp);
                }
            }
            catch (Exception exception)
            {
                Log(exception);
                if (checkOnly) return 1;
                if (!File.Exists(executable))
                {
                    MessageBox.Show("Не удалось загрузить LAD с GitHub, и локальной версии нет. " +
                        "Проверьте подключение к интернету или распакуйте LAD заново.\n\n" + exception.Message,
                        "LAD", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return 1;
                }
                RememberCheck(checkStamp);
            }

            if (checkOnly) return File.Exists(executable) ? 0 : 1;

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = executable,
                    WorkingDirectory = directory,
                    UseShellExecute = true
                });
            }
            catch (Exception exception)
            {
                Log(exception);
                MessageBox.Show("Не удалось запустить LAD: " + exception.Message,
                    "LAD", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 1;
            }
            return 0;
        }

        private static void UpdateIfNeeded(string executable)
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(3);
                string cacheKey = Guid.NewGuid().ToString("N");
                string manifest = client.GetStringAsync(BuildUrl + "version.txt?" + cacheKey)
                    .GetAwaiter().GetResult();
                string[] fields = manifest.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if (fields.Length < 2 || !Regex.IsMatch(fields[0], "^[0-9a-fA-F]{40}$") ||
                    !Regex.IsMatch(fields[1], "^[0-9a-fA-F]{64}$") ||
                    (fields.Length >= 3 && !Regex.IsMatch(fields[2], "^[0-9a-fA-F]{64}$")))
                {
                    throw new InvalidDataException("Неверный файл версии LAD на GitHub.");
                }

                if (fields.Length >= 3)
                {
                    UpdateFile(client, "LAD.exe.config", executable + ".config", fields[2]);
                }
                UpdateFile(client, "LAD.exe", executable, fields[1]);
            }
        }

        private static bool RecentlyChecked(string path)
        {
            try
            {
                return File.Exists(path) && DateTime.UtcNow - File.GetLastWriteTimeUtc(path) < CheckInterval;
            }
            catch (IOException) { return false; }
            catch (UnauthorizedAccessException) { return false; }
        }

        private static void RememberCheck(string path)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllText(path, DateTime.UtcNow.ToString("O"));
            }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
        }

        private static void UpdateFile(HttpClient client, string name, string destination, string expectedHash)
        {
            if (File.Exists(destination) &&
                string.Equals(GetHash(destination), expectedHash, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            byte[] downloaded = client.GetByteArrayAsync(BuildUrl + name + "?" + expectedHash)
                .GetAwaiter().GetResult();
            if (!string.Equals(GetHash(downloaded), expectedHash, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidDataException("Контрольная сумма обновления " + name + " не совпадает.");
            }

            string temporary = destination + "." + Guid.NewGuid().ToString("N") + ".new";
            string backup = destination + ".old";
            try
            {
                File.WriteAllBytes(temporary, downloaded);
                if (File.Exists(destination))
                {
                    File.Replace(temporary, destination, backup);
                    try { File.Delete(backup); } catch (IOException) { }
                }
                else
                {
                    File.Move(temporary, destination);
                }
            }
            finally
            {
                if (File.Exists(temporary)) File.Delete(temporary);
            }
        }

        private static string GetHash(string path)
        {
            using (var stream = File.OpenRead(path))
            using (var sha256 = SHA256.Create())
            {
                return BitConverter.ToString(sha256.ComputeHash(stream)).Replace("-", "");
            }
        }

        private static string GetHash(byte[] bytes)
        {
            using (var sha256 = SHA256.Create())
            {
                return BitConverter.ToString(sha256.ComputeHash(bytes)).Replace("-", "");
            }
        }

        private static void Log(Exception exception)
        {
            try
            {
                string directory = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LAD");
                Directory.CreateDirectory(directory);
                File.AppendAllText(Path.Combine(directory, "launcher.log"),
                    DateTime.Now + Environment.NewLine + exception + Environment.NewLine);
            }
            catch (Exception)
            {
                // Updating remains optional when a local LAD.exe is available.
            }
        }
    }
}
