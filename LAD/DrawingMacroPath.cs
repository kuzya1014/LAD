using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using Tekla.Structures;
using Tekla.Structures.Model.Operations;

namespace LAD
{
    internal static class DrawingMacroPath
    {
        internal static string Create()
        {
            // This override must also be in Tekla's XS_MACRO_DIRECTORY search path.
            string overrideDirectory = Environment.GetEnvironmentVariable("LAD_MACRO_DIRECTORY");
            if (!string.IsNullOrWhiteSpace(overrideDirectory))
            {
                return CreateInDirectory(overrideDirectory);
            }

            List<string> macroDirectories;
            if (TeklaStructuresSettings.GetAdvancedOptionPaths(
                "XS_MACRO_DIRECTORY", out macroDirectories, LogInvalidPath) && macroDirectories != null)
            {
                foreach (string macroDirectory in macroDirectories)
                {
                    if (!Directory.Exists(macroDirectory))
                    {
                        continue;
                    }

                    string name = Path.GetFileName(macroDirectory.TrimEnd(
                        Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
                    string drawingDirectory;
                    if (string.Equals(name, "drawings", StringComparison.OrdinalIgnoreCase))
                    {
                        drawingDirectory = macroDirectory;
                    }
                    else if (string.Equals(name, "modeling", StringComparison.OrdinalIgnoreCase))
                    {
                        drawingDirectory = Path.Combine(Path.GetDirectoryName(macroDirectory), "drawings");
                    }
                    else
                    {
                        drawingDirectory = Path.Combine(macroDirectory, "drawings");
                    }
                    try
                    {
                        return CreateInDirectory(drawingDirectory);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // Try another configured directory.
                    }
                    catch (IOException)
                    {
                        // Try another configured directory.
                    }
                }
            }

            throw new InvalidOperationException(
                "Не найдена доступная для записи папка макросов Tekla. " +
                "Проверьте XS_MACRO_DIRECTORY или задайте LAD_MACRO_DIRECTORY " +
                "для папки drawings из списка макросов Tekla.");
        }

        internal static void Run(string path)
        {
            string generatedPath = path;
            path = CacheMacro(generatedPath);
            string fallbackName = @"..\drawings\" + Path.GetFileName(path);
            bool started = false;
            try
            {
                // Use the exact directory selected from XS_MACRO_DIRECTORY first.
                started = Operation.RunMacro(path);
            }
            catch (FileNotFoundException)
            {
                // Older Tekla versions accept only a name relative to the macro directory.
            }
            if (!started)
            {
                started = Operation.RunMacro(fallbackName);
            }
            LogRun(path, started);
            if (!started)
            {
                throw new FileNotFoundException("Tekla не нашла созданный макрос.", path);
            }
            // RunMacro starts the macro; deleting its source here can race with execution.
            // The cached source stays in place for Tekla's compiled macro cache.
            try { File.Delete(generatedPath); }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
        }

        private static string CacheMacro(string generatedPath)
        {
            byte[] source = File.ReadAllBytes(generatedPath);
            string hash;
            using (var sha256 = SHA256.Create())
            {
                hash = BitConverter.ToString(sha256.ComputeHash(source)).Replace("-", "");
            }
            string cachedPath = Path.Combine(Path.GetDirectoryName(generatedPath),
                "LAD_cached_" + hash.Substring(0, 24) + ".cs");
            if (!File.Exists(cachedPath))
            {
                try { File.Copy(generatedPath, cachedPath); }
                catch (IOException) { if (!File.Exists(cachedPath)) throw; }
            }
            return cachedPath;
        }

        private static void LogRun(string path, bool started)
        {
            try
            {
                string directory = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LAD");
                Directory.CreateDirectory(directory);
                File.AppendAllText(Path.Combine(directory, "macro.log"),
                    DateTime.Now + " RunMacro: " + path + "; accepted: " + started + Environment.NewLine);
            }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
        }

        private static string CreateInDirectory(string directory)
        {
            Directory.CreateDirectory(directory);
            string probe = Path.Combine(directory, "LAD_write_test_" + Guid.NewGuid().ToString("N"));
            using (new FileStream(probe, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
            }
            File.Delete(probe);
            CleanupOldMacros(directory);
            return Path.Combine(directory, "LAD_" + Guid.NewGuid().ToString("N") + ".cs");
        }

        private static void CleanupOldMacros(string directory)
        {
            foreach (string path in Directory.EnumerateFiles(directory, "LAD_*.cs"))
            {
                if (Path.GetFileName(path).StartsWith("LAD_cached_", StringComparison.Ordinal))
                    continue;
                if (File.GetLastWriteTimeUtc(path) > DateTime.UtcNow.AddDays(-1))
                {
                    continue;
                }

                foreach (string generatedFile in new[] { path, path + ".dll", path + ".pdb" })
                {
                    try
                    {
                        File.Delete(generatedFile);
                    }
                    catch (IOException)
                    {
                        // An old file may still be locked; retry on the next run.
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // A read-only file does not prevent running a new macro.
                    }
                }
            }
        }

        private static void LogInvalidPath(string option, string value, string error)
        {
            Trace.TraceWarning("Invalid {0} entry: {1} ({2})", option, value, error);
        }
    }
}
