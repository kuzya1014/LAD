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
    /// Логика взаимодействия для WindowForDims.xaml
    /// </summary>
    public partial class WindowForDims : Window
    {
        public WindowForDims()
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
            try
            {
                DrawingAttributeCommands.ApplyDimensions(shapeTag);
                this.Close();
            }
            catch (Exception ex)
            {
                App.WriteErrorLog(ex);
                MessageBox.Show(this, ex.Message, "LAD", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
        //Close button
        private void CloseButtonClick(object sender, RoutedEventArgs e) => this.Close();
        //Close window if press Escape
        private void Key_Down_MW(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) this.Close();
        }

    }
}
