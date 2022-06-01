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


using TSD = Tekla.Structures.Drawing;
using TSM = Tekla.Structures.Model;

namespace LAD
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            foreach (UIElement shape1 in mygrid.Children)
            {
                if (shape1 is Shape)
                {
                    shape1.MouseLeftButtonDown += Shape_Click;
                    shape1.MouseEnter += Shape_Enter;
                    shape1.MouseLeave += Shape_Leave;
                }
            }
        }

        private void Shape_Leave(object sender, MouseEventArgs e)
        {
            Shape shape2 = (Shape)e.Source;
            MouseLeaveSegment(shape2);
        }

        private void Shape_Enter(object sender, MouseEventArgs e)
        {
            Shape shape2 = (Shape)e.Source;
            MouseEnterSegment(shape2);
        }

        private void Shape_Click(object sender, MouseButtonEventArgs e)
        {
            string attrNameTag = (string)((Shape)e.OriginalSource).Tag;         
            PartMarkMacro(attrNameTag);
        }
 
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
        void PartMarkMacro(string attrName)
        {
            string path = @"C:\TeklaStructures\2020.0\Environments\common\macros\drawings\Paul_Macro.cs";
            string pathdll = @"C:\TeklaStructures\2020.0\Environments\common\macros\drawings\Paul_Macro.cs.dll";
            string pathpdb = @"C:\TeklaStructures\2020.0\Environments\common\macros\drawings\Paul_Macro.cs.pdb";
            // This text is added only once to the file.
            if (!File.Exists(path))
            {
                // Create a file to write to.
                string st1 = "#pragma warning disable 1633 // Unrecognized #pragma directive" + Environment.NewLine;
                File.WriteAllText(path, st1);
            }

            string st2 = @"#pragma reference ""Tekla.Macros.Wpf.Runtime""" + Environment.NewLine;
            File.AppendAllText(path, st2);

            string st3 = @"#pragma reference ""Tekla.Macros.Akit""" + Environment.NewLine;
            File.AppendAllText(path, st3);

            string st4 = @"#pragma reference ""Tekla.Macros.Runtime""" + Environment.NewLine;
            File.AppendAllText(path, st4);

            string st5 = @"#pragma warning restore 1633 // Unrecognized #pragma directive" + Environment.NewLine;
            File.AppendAllText(path, st5);

            string st6 = @"namespace UserMacros {" + Environment.NewLine;
            File.AppendAllText(path, st6);

            string st7 = @"public sealed class Macro {" + Environment.NewLine;
            File.AppendAllText(path, st7);

            string st8 = @"[Tekla.Macros.Runtime.MacroEntryPointAttribute()]" + Environment.NewLine;
            File.AppendAllText(path, st8);

            string st9 = @" public static void Run(Tekla.Macros.Runtime.IMacroRuntime runtime) {" + Environment.NewLine;
            File.AppendAllText(path, st9);

            string st10 = @"Tekla.Macros.Akit.IAkitScriptHost akit = runtime.Get<Tekla.Macros.Akit.IAkitScriptHost>();" + Environment.NewLine;
            File.AppendAllText(path, st10);

            string st11 = @"Tekla.Macros.Wpf.Runtime.IWpfMacroHost wpf = runtime.Get<Tekla.Macros.Wpf.Runtime.IWpfMacroHost>();" + Environment.NewLine;
            File.AppendAllText(path, st11);

            string st12 = @"wpf.InvokeCommand(""CommandRepository"", ""Common.Properties_Drawing"");" + Environment.NewLine;
            File.AppendAllText(path, st12);
            //Тут название меняем
            string st13 = $@"akit.ValueChange(""pmark_dial"", ""gr_pmark_get_menu"", ""{attrName}"");" + Environment.NewLine;
            File.AppendAllText(path, st13);

            string st14 = @"akit.PushButton(""gr_pmark_get"", ""pmark_dial"");" + Environment.NewLine;
            File.AppendAllText(path, st14);

            string st15 = @"akit.PushButton(""pmark_modify"", ""pmark_dial"");" + Environment.NewLine;
            File.AppendAllText(path, st15);

            string st16 = @" akit.PushButton(""pmark_ok"", ""pmark_dial"");" + Environment.NewLine;
            File.AppendAllText(path, st16);

            string st17 = @"}" + Environment.NewLine;
            File.AppendAllText(path, st17);

            string st18 = @"}" + Environment.NewLine;
            File.AppendAllText(path, st18);

            string st19 = @"}" + Environment.NewLine;
            File.AppendAllText(path, st19);

            TSM.Operations.Operation.RunMacro(@"..\drawings\Paul_Macro.cs");

            File.Delete(path);
            File.Delete(pathdll);
            File.Delete(pathpdb);

        }

        //Close button
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //Close window if press Escape
        private void Key_Down_MW(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) this.Close();

        }
    }
}
