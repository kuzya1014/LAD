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
    /// Логика взаимодействия для WindowForWeld.xaml
    /// </summary>
    public partial class WindowForWeld : Window
    {
        public WindowForWeld()
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
                    PartMarkMacro(shapeTag);
                    this.Close();
                }
                catch (Exception ex)
                {
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
            //Метод - Создание макроса для партмарок с определенным аттрибутом
            void PartMarkMacro(string shapeTag)
            {
                TSD.DrawingHandler myDrawingHandler = new TSD.DrawingHandler();
                TSD.Drawing myDrawing = myDrawingHandler.GetActiveDrawing();
                if (myDrawing != null)
                {
                    foreach (TSD.DrawingObject myDrawingObject in myDrawingHandler.GetDrawingObjectSelector().GetSelected())
                    {
                        if (shapeTag == "Edge")
                        {
                        string path = DrawingMacroPath.Create();
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
                        string st13 = $@"akit.ValueChange(""wld_dial"", ""w_around"", ""0"");" + Environment.NewLine;
                        File.AppendAllText(path, st13);

                        string st14 = @"akit.PushButton(""wld_modify"", ""wld_dial"");" + Environment.NewLine;
                        File.AppendAllText(path, st14);

                        string st15 = @"akit.PushButton(""wld_ok"", ""wld_dial"");" + Environment.NewLine;
                        File.AppendAllText(path, st15);

                        string st17 = @"}" + Environment.NewLine;
                        File.AppendAllText(path, st17);

                        string st18 = @"}" + Environment.NewLine;
                        File.AppendAllText(path, st18);

                        string st19 = @"}" + Environment.NewLine;
                        File.AppendAllText(path, st19);

                        DrawingMacroPath.Run(path);

                    }
                        if (shapeTag == "Around")
                    {
                        string path = DrawingMacroPath.Create();
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
                        string st13 = $@"akit.ValueChange(""wld_dial"", ""w_around"", ""1 "");" + Environment.NewLine;
                        File.AppendAllText(path, st13);

                        string st14 = @"akit.PushButton(""wld_modify"", ""wld_dial"");" + Environment.NewLine;
                        File.AppendAllText(path, st14);

                        string st15 = @"akit.PushButton(""wld_ok"", ""wld_dial"");" + Environment.NewLine;
                        File.AppendAllText(path, st15);

                        string st17 = @"}" + Environment.NewLine;
                        File.AppendAllText(path, st17);

                        string st18 = @"}" + Environment.NewLine;
                        File.AppendAllText(path, st18);

                        string st19 = @"}" + Environment.NewLine;
                        File.AppendAllText(path, st19);

                        DrawingMacroPath.Run(path);

                    }
                        if (shapeTag == "1/4")
                    {
                        string path = DrawingMacroPath.Create();
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
                        string st13 = $@"akit.ValueChange(""wld_dial"", ""w_size2"", ""0.25"");" + Environment.NewLine;
                        File.AppendAllText(path, st13);

                        string st14 = $@"akit.ValueChange(""wld_dial"", ""w_size"", ""0.25"");" + Environment.NewLine;
                        File.AppendAllText(path, st14);

                        string st15 = @"akit.PushButton(""wld_modify"", ""wld_dial"");" + Environment.NewLine;
                        File.AppendAllText(path, st15);

                        string st16 = @"akit.PushButton(""wld_ok"", ""wld_dial"");" + Environment.NewLine;
                        File.AppendAllText(path, st16);

                        string st17 = @"}" + Environment.NewLine;
                        File.AppendAllText(path, st17);

                        string st18 = @"}" + Environment.NewLine;
                        File.AppendAllText(path, st18);

                        string st19 = @"}" + Environment.NewLine;
                        File.AppendAllText(path, st19);

                        DrawingMacroPath.Run(path);

                    }
                        if (shapeTag =="3/16")
                    {
                        string path = DrawingMacroPath.Create();
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
                        string st13 = $@"akit.ValueChange(""wld_dial"", ""w_size2"", ""0.1875"");" + Environment.NewLine;
                        File.AppendAllText(path, st13);

                        string st14 = $@"akit.ValueChange(""wld_dial"", ""w_size"", ""0.1875"");" + Environment.NewLine;
                        File.AppendAllText(path, st14);

                        string st15 = @"akit.PushButton(""wld_modify"", ""wld_dial"");" + Environment.NewLine;
                        File.AppendAllText(path, st15);

                        string st16 = @"akit.PushButton(""wld_ok"", ""wld_dial"");" + Environment.NewLine;
                        File.AppendAllText(path, st16);

                        string st17 = @"}" + Environment.NewLine;
                        File.AppendAllText(path, st17);

                        string st18 = @"}" + Environment.NewLine;
                        File.AppendAllText(path, st18);

                        string st19 = @"}" + Environment.NewLine;
                        File.AppendAllText(path, st19);

                        DrawingMacroPath.Run(path);

                    }
                        if (shapeTag =="2Fillet")
                    {
                        string path = DrawingMacroPath.Create();
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
                        string st13 = $@"akit.ValueChange(""wld_dial"", ""w_type2"", ""10"");" + Environment.NewLine;
                        File.AppendAllText(path, st13);

                        string st14 = $@"akit.ValueChange(""wld_dial"", ""w_type"", ""10"");" + Environment.NewLine;
                        File.AppendAllText(path, st14);

                        string st15 = @"akit.PushButton(""wld_modify"", ""wld_dial"");" + Environment.NewLine;
                        File.AppendAllText(path, st15);

                        string st16 = @"akit.PushButton(""wld_ok"", ""wld_dial"");" + Environment.NewLine;
                        File.AppendAllText(path, st16);

                        string st17 = @"}" + Environment.NewLine;
                        File.AppendAllText(path, st17);

                        string st18 = @"}" + Environment.NewLine;
                        File.AppendAllText(path, st18);

                        string st19 = @"}" + Environment.NewLine;
                        File.AppendAllText(path, st19);

                        DrawingMacroPath.Run(path);

                    }
                        if (shapeTag == "Fillet")
                    {
                        string path = DrawingMacroPath.Create();
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
                        string st13 = $@"akit.ValueChange(""wld_dial"", ""w_type2"", ""10"");" + Environment.NewLine;
                        File.AppendAllText(path, st13);

                        string st14 = $@"akit.ValueChange(""wld_dial"", ""w_type"", ""0"");" + Environment.NewLine;
                        File.AppendAllText(path, st14);

                        string st15 = @"akit.PushButton(""wld_modify"", ""wld_dial"");" + Environment.NewLine;
                        File.AppendAllText(path, st15);

                        string st16 = @"akit.PushButton(""wld_ok"", ""wld_dial"");" + Environment.NewLine;
                        File.AppendAllText(path, st16);

                        string st17 = @"}" + Environment.NewLine;
                        File.AppendAllText(path, st17);

                        string st18 = @"}" + Environment.NewLine;
                        File.AppendAllText(path, st18);

                        string st19 = @"}" + Environment.NewLine;
                        File.AppendAllText(path, st19);

                        DrawingMacroPath.Run(path);

                    }
                        if (shapeTag == "Field")
                    {
                        string path = DrawingMacroPath.Create();
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
                        string st13 = $@"akit.ValueChange(""wld_dial"", ""w_workshop"", ""1"");" + Environment.NewLine;
                        File.AppendAllText(path, st13);

                        string st15 = @"akit.PushButton(""wld_modify"", ""wld_dial"");" + Environment.NewLine;
                        File.AppendAllText(path, st15);

                        string st16 = @"akit.PushButton(""wld_ok"", ""wld_dial"");" + Environment.NewLine;
                        File.AppendAllText(path, st16);

                        string st17 = @"}" + Environment.NewLine;
                        File.AppendAllText(path, st17);

                        string st18 = @"}" + Environment.NewLine;
                        File.AppendAllText(path, st18);

                        string st19 = @"}" + Environment.NewLine;
                        File.AppendAllText(path, st19);

                        DrawingMacroPath.Run(path);

                    }
                        if(shapeTag =="Shop")
                    {
                        string path = DrawingMacroPath.Create();
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
                        string st13 = $@"akit.ValueChange(""wld_dial"", ""w_workshop"", ""0"");" + Environment.NewLine;
                        File.AppendAllText(path, st13);

                        string st15 = @"akit.PushButton(""wld_modify"", ""wld_dial"");" + Environment.NewLine;
                        File.AppendAllText(path, st15);

                        string st16 = @"akit.PushButton(""wld_ok"", ""wld_dial"");" + Environment.NewLine;
                        File.AppendAllText(path, st16);

                        string st17 = @"}" + Environment.NewLine;
                        File.AppendAllText(path, st17);

                        string st18 = @"}" + Environment.NewLine;
                        File.AppendAllText(path, st18);

                        string st19 = @"}" + Environment.NewLine;
                        File.AppendAllText(path, st19);

                        DrawingMacroPath.Run(path);

                    }
                        if(shapeTag =="SW")
                    {
                        string path = DrawingMacroPath.Create();
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
                        string st13 = $@"akit.ValueChange(""wld_dial"", ""gr_wld_get_menu"", ""SEAL WELD"");" + Environment.NewLine;
                        File.AppendAllText(path, st13);

                        string st14 = @"akit.PushButton(""gr_wld_get"", ""wld_dial"");" + Environment.NewLine;
                        File.AppendAllText(path, st14);

                        string st15 = @"akit.PushButton(""wld_modify"", ""wld_dial"");" + Environment.NewLine;
                        File.AppendAllText(path, st15);

                        string st16 = @"akit.PushButton(""wld_ok"", ""wld_dial"");" + Environment.NewLine;
                        File.AppendAllText(path, st16);

                        string st17 = @"}" + Environment.NewLine;
                        File.AppendAllText(path, st17);

                        string st18 = @"}" + Environment.NewLine;
                        File.AppendAllText(path, st18);

                        string st19 = @"}" + Environment.NewLine;
                        File.AppendAllText(path, st19);

                        DrawingMacroPath.Run(path);

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

