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

        [STAThread]
        private static void Main()
        {
            string directory = AppDomain.CurrentDomain.BaseDirectory;
            string executable = Path.Combine(directory, "LAD.exe");
            try
            {
                UpdateIfNeeded(executable);
            }
            catch (Exception exception)
            {
                Log(exception);
                if (!File.Exists(executable))
                {
                    MessageBox.Show("Не удалось загрузить LAD с GitHub, и локальной версии нет. " +
                        "Проверьте подключение к интернету или распакуйте LAD заново.\n\n" + exception.Message,
                        "LAD", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

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
            }
        }

        private static void UpdateIfNeeded(string executable)
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(8);
                string cacheKey = Guid.NewGuid().ToString("N");
                string manifest = client.GetStringAsync(BuildUrl + "version.txt?" + cacheKey)
                    .GetAwaiter().GetResult();
                string[] fields = manifest.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if (fields.Length < 2 || !Regex.IsMatch(fields[0], "^[0-9a-fA-F]{40}$") ||
                    !Regex.IsMatch(fields[1], "^[0-9a-fA-F]{64}$"))
                {
                    throw new InvalidDataException("Неверный файл версии LAD на GitHub.");
                }

                string expectedHash = fields[1];
                if (File.Exists(executable) &&
                    string.Equals(GetHash(executable), expectedHash, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                byte[] downloaded = client.GetByteArrayAsync(BuildUrl + "LAD.exe?" + expectedHash)
                    .GetAwaiter().GetResult();
                string actualHash = GetHash(downloaded);
                if (!string.Equals(actualHash, expectedHash, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidDataException("Контрольная сумма обновления LAD не совпадает.");
                }

                string temporary = executable + "." + Guid.NewGuid().ToString("N") + ".new";
                string backup = executable + ".old";
                try
                {
                    File.WriteAllBytes(temporary, downloaded);
                    if (File.Exists(executable))
                    {
                        File.Replace(temporary, executable, backup);
                        try { File.Delete(backup); } catch (IOException) { }
                    }
                    else
                    {
                        File.Move(temporary, executable);
                    }
                }
                finally
                {
                    if (File.Exists(temporary)) File.Delete(temporary);
                }
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
