using System;
using TSD = Tekla.Structures.Drawing;

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
                if (!mark.Attributes.LoadAttributes(preset))
                    throw new InvalidOperationException("Не найден набор свойств марки: " + preset);
                if (!mark.Modify())
                    throw new InvalidOperationException("Tekla не изменила выбранную марку.");
                changed++;
            }
            if (changed == 0) throw new InvalidOperationException("Выберите марку детали на чертеже.");
            drawing.CommitChanges();
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
                if (straightSet != null) loaded = straightSet.Attributes.LoadAttributes(preset);
                else if (curvedOrthogonal != null) loaded = curvedOrthogonal.Attributes.LoadAttributes(preset);
                else if (curvedRadial != null) loaded = curvedRadial.Attributes.LoadAttributes(preset);
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
                if (!weld.Attributes.LoadAttributes(preset))
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
