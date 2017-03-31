using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using System.Windows.Input;
using IDE.Common.Model;

namespace IDE.Common.Utilities
{
    public class ProgramEditor : TextEditor
    {
        Highlighting highlighting;
        Program currentProgram;

        #region enums

        public enum Highlighting
        {
            On,
            Off
        }

        #endregion

        #region Constructor

        public ProgramEditor(Highlighting highlighting)
        {
            this.highlighting = highlighting;
            InitializeAvalon();
        }

        #endregion

        #region Properties

        public Program CurrentProgram
        {
            set
            {
                currentProgram = value;
                Text = CurrentProgram.Content;
            }
            get
            {
                return currentProgram;
            }
        }

        #endregion

        #region Actions

        private void InitializeAvalon()
        {
            ShowLineNumbers = true;
            Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));

            if (highlighting == Highlighting.On)
            {
                try
                {
                    IHighlightingDefinition definition = HighlightingLoader.Load(XmlReader.Create("CustomHighlighting.xshd"), 
                        HighlightingManager.Instance);
                    HighlightingManager.Instance.RegisterHighlighting("CustomHighlighting", new[] { ".txt" }, definition);
                    SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("CustomHighlighting");
                }
                catch (FileNotFoundException)
                {
                    MessageBox.Show("Highlighting definitions not found. Support for syntax highlighting will be switched off " +
                        "during this session.", "No definitions", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            TextArea.TextEntering += TextEntering;
            TextArea.TextEntered += TextEntered;
            TextArea.PreviewKeyDown += KeyIsDown;
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            //tbi
        }

        private void TextEntered(object sender, TextCompositionEventArgs e)
        {
            //tbi
        }

        private void TextEntering(object sender, TextCompositionEventArgs e)
        {
            //tbi
        }

        #endregion

    }
}
