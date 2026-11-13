using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using SWF = System.Windows.Forms;
using TSD = Tekla.Structures.Drawing;
using TSM = Tekla.Structures.Model;

namespace LAD
{
    public partial class MainWindow : Window
    {
        private volatile string currentStage = "запуск";
        private bool closed;

        private enum ToolKind { PartMark, Dimensions, Part, Weld, Text, Bolt }

        public MainWindow()
        {
            InitializeComponent();
            ContentRendered += MainWindow_ContentRendered;
            Closed += (sender, args) => closed = true;
        }

        private async void MainWindow_ContentRendered(object sender, EventArgs e)
        {
            ContentRendered -= MainWindow_ContentRendered;
            var completion = new TaskCompletionSource<ToolKind>(TaskCreationOptions.RunContinuationsAsynchronously);
            var worker = new Thread(() =>
            {
                try { completion.TrySetResult(ProbeSelection()); }
                catch (Exception ex) { completion.TrySetException(ex); }
            });
            worker.IsBackground = true;
            worker.SetApartmentState(ApartmentState.STA);
            worker.Start();

            Task finished = await Task.WhenAny(completion.Task, Task.Delay(TimeSpan.FromSeconds(10)));
            if (closed) return;
            if (finished != completion.Task)
            {
                ShowFailure(new TimeoutException("Tekla API не ответил за 10 секунд. Этап: " + currentStage));
                return;
            }

            try
            {
                ToolKind kind = await completion.Task;
                if (kind == ToolKind.Bolt)
                {
                    Close();
                    return;
                }

                Window tool = CreateTool(kind);
                ShowTool(tool);
                Hide();
            }
            catch (Exception ex)
            {
                ShowFailure(ex);
            }
        }

        private ToolKind ProbeSelection()
        {
            currentStage = "создание DrawingHandler";
            var handler = new TSD.DrawingHandler();
            currentStage = "получение активного чертежа";
            var drawing = handler.GetActiveDrawing();
            if (drawing == null)
            {
                currentStage = "проверка соединения";
                throw new InvalidOperationException("Не удалось получить активный чертёж Tekla. " + ConnectionDiagnostics(handler));
            }

            currentStage = "чтение выделения";
            bool hasSelection = false;
            foreach (TSD.DrawingObject selected in handler.GetDrawingObjectSelector().GetSelected())
            {
                hasSelection = true;
                if (selected is TSD.Mark) return ToolKind.PartMark;
                if (selected is TSD.DimensionSetBase) return ToolKind.Dimensions;
                if (selected is TSD.Part) return ToolKind.Part;
                if (selected is TSD.WeldMark) return ToolKind.Weld;
                if (selected is TSD.Text) return ToolKind.Text;
                if (selected is TSD.Bolt)
                {
                    currentStage = "изменение болтов";
                    ToggleSelectedBolts(handler, drawing);
                    return ToolKind.Bolt;
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

        private void ShowFailure(Exception exception)
        {
            if (closed) return;
            string logPath = App.WriteErrorLog(exception);
            MessageBox.Show(this, "Не удалось запустить LAD: " + exception.Message +
                (logPath == null ? "" : "\nПодробности: " + logPath),
                "LAD", MessageBoxButton.OK, MessageBoxImage.Error);
            Close();
        }

        private static Window CreateTool(ToolKind kind)
        {
            switch (kind)
            {
                case ToolKind.PartMark: return new WindowForPM();
                case ToolKind.Dimensions: return new WindowForDims();
                case ToolKind.Part: return new WindowForPart();
                case ToolKind.Weld: return new WindowForWeld();
                case ToolKind.Text: return new WindowForText();
                default: throw new InvalidOperationException("Неизвестный инструмент LAD.");
            }
        }

        private void ShowTool(Window tool)
        {
            var cursor = SWF.Cursor.Position;
            tool.Left = cursor.X - 150;
            tool.Top = cursor.Y - 150;
            tool.Closed += (sender, args) => Close();
            tool.Show();
            tool.Activate();
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

        private void Key_Down_Space(object sender, KeyEventArgs e) { }
        private void Key_Down_Close_Escape(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) Close();
        }
    }
}
