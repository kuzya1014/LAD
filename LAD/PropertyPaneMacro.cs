using System;
using System.IO;

namespace LAD
{
    internal static class PropertyPaneMacro
    {
        internal static void ApplyPreset(string preset)
        {
            if (string.IsNullOrWhiteSpace(preset))
                throw new ArgumentException("Не выбраны свойства Tekla.", "preset");

            string escaped = preset.Replace("\\", "\\\\").Replace("\"", "\\\"")
                .Replace("\r", "\\r").Replace("\n", "\\n");
            string path = DrawingMacroPath.Create();
            string source = string.Join(Environment.NewLine, new[]
            {
                "#pragma warning disable 1633 // Unrecognized #pragma directive",
                "#pragma reference \"Tekla.Macros.Wpf.Runtime\"",
                "#pragma reference \"Tekla.Macros.Runtime\"",
                "#pragma warning restore 1633 // Unrecognized #pragma directive",
                "namespace UserMacros {",
                "    public sealed class Macro {",
                "        [Tekla.Macros.Runtime.MacroEntryPointAttribute()]",
                "        public static void Run(Tekla.Macros.Runtime.IMacroRuntime runtime) {",
                "            Tekla.Macros.Wpf.Runtime.IWpfMacroHost wpf = runtime.Get<Tekla.Macros.Wpf.Runtime.IWpfMacroHost>();",
                "            wpf.View(\"PropertyPane.PropertyPane\").Find(\"AID_PropertyPane_FileSelectionControl\").As.TextBox.SetText(\"" + escaped + "\");",
                "            wpf.View(\"PropertyPane.PropertyPane\").Find(\"AID_PropertyPane_FileSelectionControl\").As.Button.Invoke();",
                "            wpf.View(\"PropertyPane.PropertyPane\").Find(\"AID_PropertyPane_CommitChangesButton\").As.Button.Invoke();",
                "        }",
                "    }",
                "}"
            }) + Environment.NewLine;
            File.WriteAllText(path, source);
            DrawingMacroPath.Run(path);
        }
    }
}
