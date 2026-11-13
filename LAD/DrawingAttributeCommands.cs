using System;
using System.IO;
using TSD = Tekla.Structures.Drawing;
using TSM = Tekla.Structures.Model;

namespace LAD
{
    internal static class DrawingAttributeCommands
    {
        internal static void ApplyMark(string preset)
        {
            var handler = new TSD.DrawingHandler();
            var drawing = handler.GetActiveDrawing();
            if (drawing == null) throw new InvalidOperationException("Откройте чертёж Tekla.");

            int changed = 0;
            foreach (TSD.DrawingObject selected in handler.GetDrawingObjectSelector().GetSelected())
            {
                var mark = selected as TSD.Mark;
                if (mark == null) continue;
                if (!LoadDrawingAttributes(preset, mark.Attributes.LoadAttributes))
                    throw new InvalidOperationException("Не удалось загрузить набор свойств марки: " + preset +
                        ". Он виден в Tekla, но не найден через API. Проверьте папку attributes модели.");
                if (!mark.Modify())
                    throw new InvalidOperationException("Tekla не изменила выбранную марку.");
                changed++;
            }
            if (changed == 0) throw new InvalidOperationException("Выберите марку детали на чертеже.");
            drawing.CommitChanges();
        }

        private static bool LoadDrawingAttributes(string preset, Func<string, bool> load)
        {
            if (load(preset)) return true;

            string modelPath = new TSM.Model().GetInfo().ModelPath;
            if (string.IsNullOrEmpty(modelPath) || !Directory.Exists(modelPath)) return false;

            // An external EXE starts in its own folder. Tekla may resolve saved
            // drawing properties relative to the active model instead.
            string previousDirectory = Environment.CurrentDirectory;
            try
            {
                Environment.CurrentDirectory = modelPath;
                if (load(preset)) return true;

                string attributeDirectory = Path.Combine(modelPath, "attributes");
                if (!Directory.Exists(attributeDirectory)) return false;
                foreach (string file in Directory.EnumerateFiles(attributeDirectory, preset + ".*"))
                {
                    string baseName = Path.Combine(attributeDirectory, Path.GetFileNameWithoutExtension(file));
                    if (load(baseName) || load(file))
                        return true;
                }
                return false;
            }
            finally
            {
                Environment.CurrentDirectory = previousDirectory;
            }
        }

        internal static void ApplyDimensions(string preset)
        {
            var handler = new TSD.DrawingHandler();
            var drawing = handler.GetActiveDrawing();
            if (drawing == null) throw new InvalidOperationException("Откройте чертёж Tekla.");

            int changed = 0;
            foreach (TSD.DrawingObject selected in handler.GetDrawingObjectSelector().GetSelected())
            {
                bool loaded;
                var straightSet = selected as TSD.StraightDimensionSet;
                var curvedOrthogonal = selected as TSD.CurvedDimensionSetOrthogonal;
                var curvedRadial = selected as TSD.CurvedDimensionSetRadial;
                if (straightSet != null) loaded = LoadDrawingAttributes(preset, straightSet.Attributes.LoadAttributes);
                else if (curvedOrthogonal != null) loaded = LoadDrawingAttributes(preset, curvedOrthogonal.Attributes.LoadAttributes);
                else if (curvedRadial != null) loaded = LoadDrawingAttributes(preset, curvedRadial.Attributes.LoadAttributes);
                else continue;

                if (!loaded) throw new InvalidOperationException("Не найден набор свойств размера: " + preset);
                if (!selected.Modify()) throw new InvalidOperationException("Tekla не изменила выбранный размер.");
                changed++;
            }
            if (changed == 0) throw new InvalidOperationException("Выберите набор размеров на чертеже.");
            drawing.CommitChanges();
        }

        internal static void ApplyWeldPreset(string preset)
        {
            var handler = new TSD.DrawingHandler();
            var drawing = handler.GetActiveDrawing();
            if (drawing == null) throw new InvalidOperationException("Откройте чертёж Tekla.");

            int changed = 0;
            foreach (TSD.DrawingObject selected in handler.GetDrawingObjectSelector().GetSelected())
            {
                var weld = selected as TSD.WeldMark;
                if (weld == null) continue;
                if (!LoadDrawingAttributes(preset, weld.Attributes.LoadAttributes))
                    throw new InvalidOperationException("Не найден набор свойств сварной марки: " + preset);
                if (!weld.Modify())
                    throw new InvalidOperationException("Tekla не изменила сварную марку.");
                changed++;
            }
            if (changed == 0) throw new InvalidOperationException("Выберите сварную марку на чертеже.");
            drawing.CommitChanges();
        }
    }
}
