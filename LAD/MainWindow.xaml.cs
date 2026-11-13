using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using SWF = System.Windows.Forms;
using TSD = Tekla.Structures.Drawing;
using TSM = Tekla.Structures.Model;

namespace LAD
{
    public partial class MainWindow : Window
    {
        private bool toolOpened;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= MainWindow_Loaded;
            try
            {
                OpenToolForSelection();
                if (toolOpened)
                {
                    Hide(); // Keep the application alive until the tool window closes.
                }
                else
                {
                    Close(); // The bolt command does not open a tool window.
                }
            }
            catch (Exception ex)
            {
                string logPath = App.WriteErrorLog(ex);
                MessageBox.Show("Не удалось запустить LAD: " + ex.Message +
                    (logPath == null ? "" : "\nПодробности: " + logPath),
                    "LAD", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private void OpenToolForSelection()
        {
            var handler = new TSD.DrawingHandler();
            var drawing = handler.GetActiveDrawing();
            if (drawing == null)
            {
                throw new InvalidOperationException("Не удалось получить активный чертёж Tekla. " + ConnectionDiagnostics(handler));
            }

            bool hasSelection = false;
            // One launch opens one tool for the first supported object type.
            foreach (TSD.DrawingObject selected in handler.GetDrawingObjectSelector().GetSelected())
            {
                hasSelection = true;
                if (selected is TSD.Mark) { ShowTool(new WindowForPM()); return; }
                if (selected is TSD.DimensionSetBase) { ShowTool(new WindowForDims()); return; }
                if (selected is TSD.Part) { ShowTool(new WindowForPart()); return; }
                if (selected is TSD.WeldMark) { ShowTool(new WindowForWeld()); return; }
                if (selected is TSD.Text) { ShowTool(new WindowForText()); return; }
                if (selected is TSD.Bolt)
                {
                    ToggleSelectedBolts(handler, drawing);
                    return;
                }
            }

            throw new InvalidOperationException(hasSelection
                ? "Для выбранного объекта нет команды LAD. Выберите деталь, марку, размер, текст или сварку."
                : "Ничего не выделено на чертеже Tekla.");
        }

        private static string ConnectionDiagnostics(TSD.DrawingHandler handler)
        {
            string drawingConnection;
            string modelConnection;
            string drawingOpen;
            try { drawingConnection = handler.GetConnectionStatus().ToString(); }
            catch (Exception ex) { drawingConnection = ex.GetType().Name; }
            try { modelConnection = new TSM.Model().GetConnectionStatus().ToString(); }
            catch (Exception ex) { modelConnection = ex.GetType().Name; }
            try { drawingOpen = handler.IsAnyDrawingOpen().ToString(); }
            catch (Exception ex) { drawingOpen = ex.GetType().Name; }

            string version;
            string teklaProcesses;
            try { version = FileVersionInfo.GetVersionInfo(typeof(TSD.DrawingHandler).Assembly.Location).FileVersion; }
            catch (Exception ex) { version = ex.GetType().Name; }
            try { teklaProcesses = Process.GetProcessesByName("TeklaStructures").Length.ToString(); }
            catch (Exception ex) { teklaProcesses = ex.GetType().Name; }
            return string.Format("Соединение чертежа: {0}; модели: {1}; открыт чертёж: {2}; " +
                "процессов TeklaStructures: {3}; версия API: {4}.",
                drawingConnection, modelConnection, drawingOpen, teklaProcesses, version);
        }

        private void ShowTool(Window tool)
        {
            var cursor = SWF.Cursor.Position;
            tool.Left = cursor.X - 150;
            tool.Top = cursor.Y - 150;
            tool.Closed += (sender, args) => Close();
            tool.Show();
            tool.Activate();
            toolOpened = true;
        }

        private static void ToggleSelectedBolts(TSD.DrawingHandler handler, TSD.Drawing drawing)
        {
            bool modified = false;
            foreach (TSD.DrawingObject selected in handler.GetDrawingObjectSelector().GetSelected())
            {
                TSD.Bolt bolt = selected as TSD.Bolt;
                if (bolt == null) continue;

                bolt.Attributes.Representation = bolt.Attributes.Representation is TSD.Bolt.Representation.Symbol3
                    ? TSD.Bolt.Representation.ExactSolid
                    : TSD.Bolt.Representation.Symbol3;
                bolt.Modify();
                modified = true;
            }

            if (modified) drawing.CommitChanges();
        }

        private void Key_Down_Space(object sender, KeyEventArgs e)
        {
        }

        private void Key_Down_Close_Escape(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) Close();
        }
    }
}
