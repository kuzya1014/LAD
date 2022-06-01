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
            //this.Close();
        
        TSM.Model myModel = new TSM.Model();
        
        if(myModel.GetConnectionStatus())
            {
                TSD.DrawingHandler myDrawingHandler = new TSD.DrawingHandler();
                TSD.Drawing myDrawing = myDrawingHandler.GetActiveDrawing();

                if (myDrawing != null)
                {
                    foreach (TSD.DrawingObject myDrawingObject in myDrawingHandler.GetDrawingObjectSelector().GetSelected())
                    {
                        if (myDrawingObject is TSD.Mark)
                        {
                            //окно для партмарок
                            PMOpenWindow();
                        }
                        
                        //рд можно добавить
                        //ай походу тоже через макросы нужно будет делать
                        else if (myDrawingObject is TSD.DimensionSetBase)
                        {
                            TSD.DimensionSetBase selDimension = myDrawingObject as TSD.DimensionSetBase;
                            TSD.StraightDimensionSet selStrDimension = selDimension as TSD.StraightDimensionSet;
                            selStrDimension.Attributes.Color = TSD.DrawingColors.Blue;
                            selStrDimension.Attributes.ShortDimension = TSD.DimensionSetBaseAttributes.ShortDimensionTypes.Inside;
                            selStrDimension.Attributes.DimensionType = TSD.DimensionSetBaseAttributes.DimensionTypes.AbsoluteWithAllRelativesAbove;
                            selStrDimension.Modify();
                            myDrawing.CommitChanges();
                        }
                        else if (myDrawingObject is TSD.WeldMark) 
                        {
                            TSD.WeldMark myWeldMark = myDrawingObject as TSD.WeldMark;
                            myWeldMark.Attributes.ShowWorkshopSite = true;
                            myWeldMark.Attributes.ShowEdgeAround = true;
                            myWeldMark.Attributes.Font.Color = TSD.DrawingColors.Blue;
                            myWeldMark.Modify();
                            myDrawing.CommitChanges();
                            
                        }
                        
                        else if (myDrawingObject is TSD.Text)
                        {
                            //Добавить метод по добавлению окна для текста
                            ChangeSelText("work"); 
                        }
                        //Рисовалки чет не о чем
                        //else if (myDrawingObject is TSD.Line)
                        //else if (myDrawingObject is TSD.Rectangle)
                        //else if (myDrawingObject is TSD.Polygon)

                    }
                }
            }
        }

        //закрывает по нажатию Escape
        private void Key_Down_MW(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) this.Close();
        }

        void ChangeSelText(string newText)
        {
            TSD.DrawingHandler myDrawingHandler = new TSD.DrawingHandler();
            TSD.Drawing myDrawing = myDrawingHandler.GetActiveDrawing();
            foreach (TSD.DrawingObject myDrawingObject in myDrawingHandler.GetDrawingObjectSelector().GetSelected())
            {
                TSD.Text selText = myDrawingObject as TSD.Text;
                selText.TextString = $"{newText}";
                selText.Attributes.UseWordWrapping = true;
                selText.Attributes.RulerWidth = 25.4 * 1.5;
                selText.Attributes.Font.Height = 25.4 * 3 / 32;
                selText.Attributes.Font.Color = TSD.DrawingColors.Green;
                selText.Attributes.Font.Italic = false;
                selText.Attributes.Font.Name = "Arial";
                selText.Modify();
                myDrawing.CommitChanges();
            }
        }

        //Открывает окно для парт марок в месте расположения курсора
        void PMOpenWindow()
        {
            Window1 WindowPartMark = new Window1();
            Point point1 = new Point();
            point1.X = SWF.Cursor.Position.X;
            point1.Y = SWF.Cursor.Position.Y;
            WindowPartMark.Left = point1.X-150;
            WindowPartMark.Top = point1.Y-150;
            WindowPartMark.Show();
            this.Close();
        }

    }
}

