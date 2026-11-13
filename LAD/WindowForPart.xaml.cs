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
using System.Windows.Shapes;
using System.IO;


using TSD = Tekla.Structures.Drawing;
using TSM = Tekla.Structures.Model;

namespace LAD
{
    /// <summary>
    /// Логика взаимодействия для WindowForPart.xaml
    /// </summary>
    public partial class WindowForPart : Window
    {
        public WindowForPart()
        {
            InitializeComponent();
            foreach (UIElement shape1 in myGrid.Children)
            {
                if (shape1 is Shape)
                {
                    shape1.MouseLeftButtonDown += Segment_Click;
                    shape1.MouseEnter += Segment_Enter;
                    shape1.MouseLeave += Segment_Leave;

                }
                //Tag to Textblock
                foreach (UIElement textblock1 in myGrid.Children)
                {
                    if (shape1 is Shape & textblock1 is TextBlock)
                    {
                        var shape2 = shape1 as Shape;
                        var textblock2 = textblock1 as TextBlock;
                        if (shape2.Name.Substring(shape2.Name.Length - 2) == textblock2.Name.Substring(textblock2.Name.Length - 2))
                        {
                            SegmentTagToTextBlock(shape2, textblock2);
                        }
                    }
                }
            }
        }
        private void Segment_Leave(object sender, MouseEventArgs e)
        {
            Shape shape2 = (Shape)e.Source;
            MouseLeaveSegment(shape2);
        }
        private void Segment_Enter(object sender, MouseEventArgs e)
        {
            Shape shape2 = (Shape)e.Source;
            MouseEnterSegment(shape2);
        }
        private void Segment_Click(object sender, MouseButtonEventArgs e)
        {
            string shapeTag = (string)((Shape)e.OriginalSource).Tag;
            PartMarkMacro(shapeTag);
            this.Close();
        }
        //Метод tag to textblocktext
        void SegmentTagToTextBlock(Shape shape, TextBlock textBlock) => textBlock.Text = shape.Tag.ToString();
        //Метод при наведение мыши на сегмент 
        void MouseEnterSegment(Shape shape)
        {
            shape.Stroke = new SolidColorBrush(Colors.Lime);
            shape.StrokeThickness = 2.5;
        }
        //Метод при вывода мыши с сегмента 
        void MouseLeaveSegment(Shape shape)
        {
            shape.Stroke = new SolidColorBrush(Colors.Lime);
            shape.StrokeThickness = 1;
        }
        //Метод - Создание макроса для партмарок с определенным аттрибутом
        void PartMarkMacro(string shapeTag)
        {
            TSD.DrawingHandler myDrawingHandler = new TSD.DrawingHandler();
            TSD.Drawing myDrawing = myDrawingHandler.GetActiveDrawing();
            if (myDrawing != null)
            {
                foreach (TSD.DrawingObject myDrawingObject in myDrawingHandler.GetDrawingObjectSelector().GetSelected())
                {
                    TSD.Part SelectedPart = myDrawingObject as TSD.Part;
                    if (SelectedPart == null) continue;
                    if(shapeTag == "Yellow")
                    {
                        SelectedPart.Attributes.VisibleLines.Color = TSD.DrawingColors.Yellow;
                        SelectedPart.Attributes.SectionFaceHatch.Color = TSD.DrawingHatchColors.Red;
                        SelectedPart.Attributes.FaceHatch.Color = TSD.DrawingHatchColors.Red;
                        SelectedPart.Attributes.VisibleLines.Type = TSD.LineTypes.SolidLine;
                        SelectedPart.Modify();
                        myDrawing.CommitChanges();
                    }
                    if(shapeTag =="Red")
                    {
                        SelectedPart.Attributes.VisibleLines.Color = TSD.DrawingColors.Red;
                        SelectedPart.Attributes.SectionFaceHatch.Color = TSD.DrawingHatchColors.Red;
                        SelectedPart.Attributes.FaceHatch.Color = TSD.DrawingHatchColors.Red;
                        SelectedPart.Attributes.VisibleLines.Type = TSD.LineTypes.SolidLine;
                        SelectedPart.Modify();
                        myDrawing.CommitChanges();
                    }
                    if(shapeTag == "Blue")
                    {
                        SelectedPart.Attributes.VisibleLines.Color = TSD.DrawingColors.Blue;
                        SelectedPart.Modify();
                        myDrawing.CommitChanges();
                    }
                    if(shapeTag == "Gray")
                    {
                        SelectedPart.Attributes.VisibleLines.Color = TSD.DrawingColors.Gray50;
                        SelectedPart.Attributes.SectionFaceHatch.Color = TSD.DrawingHatchColors.Gray50;
                        SelectedPart.Attributes.FaceHatch.Color = TSD.DrawingHatchColors.Gray50;
                        SelectedPart.Modify();
                        myDrawing.CommitChanges();
                    }
                    if(shapeTag == "Center")
                    {
                        if (SelectedPart.Attributes.DrawCenterLine == true)
                        {
                            SelectedPart.Attributes.DrawCenterLine = false;
                        }
                        else SelectedPart.Attributes.DrawCenterLine = true;
                        SelectedPart.Modify();
                        myDrawing.CommitChanges();
                    }
                    if(shapeTag =="Outline")
                    {
                        SelectedPart.Attributes.Representation = TSD.Part.Representation.Outline;
                        SelectedPart.Modify();
                        myDrawing.CommitChanges();
                    }
                    if(shapeTag == "Exact")
                    {
                        SelectedPart.Attributes.Representation = TSD.Part.Representation.Exact;
                        SelectedPart.Modify();
                        myDrawing.CommitChanges();
                    }
                    if(shapeTag == "Ref")
                    {
                        if (SelectedPart.Attributes.DrawReferenceLine == true)
                        {
                            SelectedPart.Attributes.DrawReferenceLine = false;
                        }
                        else SelectedPart.Attributes.DrawReferenceLine = true;
                        SelectedPart.Modify();
                        myDrawing.CommitChanges();
                    }
                    if(shapeTag == "Hid")
                    {
                        if (SelectedPart.Attributes.DrawHiddenLines == true)
                        {
                            SelectedPart.Attributes.DrawHiddenLines = false;
                        }
                        else SelectedPart.Attributes.DrawHiddenLines = true;
                        SelectedPart.Modify();
                        myDrawing.CommitChanges();
                    }
                    if(shapeTag == "ANSI")
                    {
                        SelectedPart.Attributes.SectionFaceHatch.Color = TSD.DrawingHatchColors.Red;
                        SelectedPart.Attributes.SectionFaceHatch.Name = "ANSI31";
                        SelectedPart.Attributes.SectionFaceHatch.ScaleX = 0.5;
                        SelectedPart.Attributes.SectionFaceHatch.ScaleY = 0.5;
                        SelectedPart.Attributes.SectionFaceHatch.Angle = 0;
                        SelectedPart.Attributes.SectionFaceHatch.AutomaticScaling = false;
                        SelectedPart.Modify();
                        myDrawing.CommitChanges();
                    }
                    if (shapeTag == "CONC")
                    {
                        SelectedPart.Attributes.SectionFaceHatch.Name = "AR-CONC";
                        SelectedPart.Attributes.SectionFaceHatch.ScaleX = 0.33;
                        SelectedPart.Attributes.SectionFaceHatch.ScaleY = 0.33;
                        SelectedPart.Attributes.SectionFaceHatch.Angle = 0;
                        SelectedPart.Attributes.SectionFaceHatch.AutomaticScaling = false;
                        SelectedPart.Modify();
                        myDrawing.CommitChanges();
                    }
                    if (shapeTag == "CMU")
                    {
                        SelectedPart.Attributes.SectionFaceHatch.Name = "CTYSQ6ST";
                        SelectedPart.Attributes.SectionFaceHatch.ScaleX = 0.25;
                        SelectedPart.Attributes.SectionFaceHatch.ScaleY = 0.25;
                        SelectedPart.Attributes.SectionFaceHatch.Angle = 45;
                        SelectedPart.Attributes.SectionFaceHatch.AutomaticScaling = false;
                        SelectedPart.Modify();
                        myDrawing.CommitChanges();
                    }
                }
            }
        }
        //Close button
        private void CloseButtonClick(object sender, RoutedEventArgs e) => this.Close();
        //Close window if press Escape
        private void Key_Down_MW(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) this.Close();
        }
    }
}
