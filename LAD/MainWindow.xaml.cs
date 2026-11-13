using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

using System.Windows.Shapes;
using System.IO;
using SWF=System.Windows.Forms;
using SD=System.Drawing;


using TSD = Tekla.Structures.Drawing;
using TSM = Tekla.Structures.Model;

namespace LAD
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
        InitializeComponent();
            MainCode();
            this.Close();
        }
        //Собственно весь главный код
        void MainCode()
        {
            TSM.Model myModel = new TSM.Model();
            if (myModel.GetConnectionStatus())
            {
                TSD.DrawingHandler myDrawingHandler = new TSD.DrawingHandler();
                TSD.Drawing myDrawing = myDrawingHandler.GetActiveDrawing();

                if (myDrawing != null)
                {
                    // Choose one tool for the first supported type in the selection.
                    foreach (TSD.DrawingObject selected in myDrawingHandler.GetDrawingObjectSelector().GetSelected())
                    {
                        if (selected is TSD.Mark) { OpenWindowForPartMark(); return; }
                        if (selected is TSD.DimensionSetBase) { OpenWindowDims(); return; }
                        if (selected is TSD.Part) { OpenWindowForParts(); return; }
                        if (selected is TSD.WeldMark) { OpenWindowForWelds(); return; }
                        if (selected is TSD.Text) { OpenWindowForText(); return; }
                        if (selected is TSD.Bolt)
                        {
                            ToggleSelectedBolts(myDrawingHandler, myDrawing);
                            return;
                        }
                    }
                }
            }
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
        //Работает весь код при нажатии на пробел
        private void Key_Down_Space(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.Space) MainCode();
        }
        //Открывает окно для парт марок в месте расположения курсора
        void OpenWindowForPartMark()
        {
            WindowForPM WindowPartMark = new WindowForPM();
            Point point1 = new Point();
            point1.X = SWF.Cursor.Position.X;
            point1.Y = SWF.Cursor.Position.Y;
            WindowPartMark.Left = point1.X-150;
            WindowPartMark.Top = point1.Y-150;
            WindowPartMark.Show();
            //this.Close();
        }
        void OpenWindowForParts()
        {
            WindowForPart WindowT = new WindowForPart();
            Point point1 = new Point();
            point1.X = SWF.Cursor.Position.X;
            point1.Y = SWF.Cursor.Position.Y;
            WindowT.Left = point1.X - 150;
            WindowT.Top = point1.Y - 150;
            WindowT.Show();
            //this.Close();
        }
        void OpenWindowForWelds()
        {
            WindowForWeld WindowT = new WindowForWeld();
            Point point1 = new Point();
            point1.X = SWF.Cursor.Position.X;
            point1.Y = SWF.Cursor.Position.Y;
            WindowT.Left = point1.X - 150;
            WindowT.Top = point1.Y - 150;
            WindowT.Show();
            //this.Close();
        }
        void OpenWindowForText()
        {
            WindowForText WindowT = new WindowForText();
            Point point1 = new Point();
            point1.X = SWF.Cursor.Position.X;
            point1.Y = SWF.Cursor.Position.Y;
            WindowT.Left = point1.X - 150;
            WindowT.Top = point1.Y - 150;
            WindowT.Show();
            //this.Close();
        }
        void OpenWindowDims()
        {
            WindowForDims WindowDims = new WindowForDims();
            Point point1 = new Point();
            point1.X = SWF.Cursor.Position.X;
            point1.Y = SWF.Cursor.Position.Y;
            WindowDims.Left = point1.X - 150;
            WindowDims.Top = point1.Y - 150;
            WindowDims.Show();
            //this.Close();
        }
        //Закрывает по нажатию Escape
        private void Key_Down_Close_Escape(object sender, KeyEventArgs e)
        {
             if (e.Key == Key.Escape) this.Close();
        }
     }
}


