using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System.Xml;
using System.Diagnostics;

namespace WPF_industrial_robotics
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CompletionWindow completionWindow;
        TextEditor textEditor;

        public MainWindow()
        {
            InitializeComponent();
            InitializeAvalon();
            
            
            textEditor.TextArea.TextEntering += textEditor_TextArea_TextEntering;
            textEditor.TextArea.TextEntered += textEditor_TextArea_TextEntered;
        }


        void textEditor_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == ">")
            {
                // Open code completion after the user has pressed > :
                completionWindow = new CompletionWindow(textEditor.TextArea);
                IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                data.Add(new MyCompletionData("GC", "Closes Grip of a hand"));
                //how to add many parameters switching at real time???????????????//
                data.Add(new MyCompletionData("MO", "Moves to specified location; PARAM: position, state-of-a-grab"));  

                completionWindow.Show();
                completionWindow.Closed += delegate {
                    completionWindow = null;
                };
            }

            if (e.Text == "(")
            {
                if (textEditor.Text.Contains(","))
                {
                    Debug.WriteLine(textEditor.Text);
                }
                    
            }
        }

        void textEditor_TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    completionWindow.CompletionList.RequestInsertion(e);
                }
            }
            // Do not set e.Handled=true.
            // We still want to insert the character that was typed.
        }


        private void InitializeAvalon()
        {
            textEditor = new TextEditor();
            textEditor.Width = 400;
            textEditor.Height = 200;
            textEditor.ShowLineNumbers = true;
            IHighlightingDefinition definition = HighlightingLoader.Load(XmlReader.Create("CustomHighlighting.xshd"), HighlightingManager.Instance);
            HighlightingManager.Instance.RegisterHighlighting("CustomHighlighting", new[] { ".txt" }, definition);
            textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("CustomHighlighting");
            panel.Children.Add(textEditor);
        }
    }
}
