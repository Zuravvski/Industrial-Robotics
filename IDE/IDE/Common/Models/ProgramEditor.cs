using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace IDE.Common.Models
{
    public class ProgramEditor : TextEditor
    {
        private readonly Highlighting highlighting;
        private readonly ReadOnly readOnly;
        private Program currentProgram;

        #region enums

        public enum Highlighting
        {
            On,
            Off
        }

        public enum ReadOnly
        {
            Yes,
            No
        }

        #endregion
        
        public ProgramEditor(Highlighting highlighting, ReadOnly readOnly)
        {
            this.highlighting = highlighting;
            this.readOnly = readOnly;
            InitializeAvalon();
        }

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
            //Background = new SolidColorBrush(Color.FromRgb(30, 30, 30));
            Foreground = new SolidColorBrush(Color.FromRgb(193, 193, 193));
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            if (readOnly == ReadOnly.Yes)
            {
                IsReadOnly = true;
            }

            if (highlighting == Highlighting.On)
            {
                try
                {
                    var definition = HighlightingLoader.Load(XmlReader.Create("CustomHighlighting.xshd"), 
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
            //currentProgram.Content = Text;
        }

        private void TextEntering(object sender, TextCompositionEventArgs e)
        {
            //tbi
        }

        #endregion

    }
}
