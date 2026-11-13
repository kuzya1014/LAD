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
                    foreach (TSD.DrawingObject myDrawingObject in myDrawingHandler.GetDrawingObjectSelector().GetSelected())
                    {
                        if (myDrawingObject is TSD.Mark) OpenWindowForPartMark();
                        //рд можно добавить
                        //ай походу тоже через макросы нужно будет делать
                        else if (myDrawingObject is TSD.DimensionSetBase) OpenWindowDims();
                        else if (myDrawingObject is TSD.Part) OpenWindowForParts();
                        else if (myDrawingObject is TSD.WeldMark) OpenWindowForWelds();
                        //{
                        //    TSD.WeldMark SelectedWeld = (TSD.WeldMark)myDrawingObject;
                        //    SelectedWeld.Attributes.Font.Color = TSD.DrawingColors.Blue;
                        //    //SelectedWeld.Delete();
                        //    SelectedWeld.Modify();
                        //    myDrawing.CommitChanges();
                        //}
                        else if (myDrawingObject is TSD.Text) OpenWindowForText();
                        else if (myDrawingObject is TSD.Bolt)
                        {
                            TSD.Bolt selBolt = myDrawingObject as TSD.Bolt;
                            if (selBolt.Attributes.Representation is TSD.Bolt.Representation.Symbol3)
                                selBolt.Attributes.Representation = TSD.Bolt.Representation.ExactSolid;
                            else selBolt.Attributes.Representation = TSD.Bolt.Representation.Symbol3;
                            selBolt.Modify();
                            myDrawing.CommitChanges();
                        }
                    }
                }
            }
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


