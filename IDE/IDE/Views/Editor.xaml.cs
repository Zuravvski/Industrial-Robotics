using System.Windows.Controls;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System.Xml;
using System;
using System.Windows.Input;
using System.Collections.Generic;

namespace IDE.Views
{
    /// <summary>
    /// Interaction logic for Editor.xaml
    /// </summary>
    public partial class Editor : UserControl
    {
        CompletionWindow completionWindow;
        TextEditor textEditor;

        public Editor()
        {
            InitializeComponent();
            InitializeAvalon();

            textEditor.TextArea.TextEntering += TextEntering;
            textEditor.TextArea.TextEntered += TextEntered;

            grid.Children.Add(textEditor);  
        }

        private void TextEntered(object sender, TextCompositionEventArgs e)
        {
            //add text entered handling
        }

        private void TextEntering(object sender, TextCompositionEventArgs e)
        {
            //add text entering handling
        }

        private void InitializeAvalon()
        {
            textEditor = new TextEditor();
            textEditor.ShowLineNumbers = true;
            textEditor.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));  //here you can set backgroud color
            IHighlightingDefinition definition = HighlightingLoader.Load(XmlReader.Create("CustomHighlighting.xshd"), HighlightingManager.Instance);
            HighlightingManager.Instance.RegisterHighlighting("CustomHighlighting", new[] { ".txt" }, definition);
            textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("CustomHighlighting");
        }
    }
}
