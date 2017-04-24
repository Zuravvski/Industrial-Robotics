using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using Driver;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Microsoft.Win32;
using System;
using IDE.Common.Models.Code_Completion;
using IDE.Common.Models.Services;
using IDE.Common.Models.Syntax_Check;
using IDE.Common.Models.Value_Objects;
using IDE.Common.Utilities;

namespace IDE.Common.Models
{
    public class ProgramEditor : TextEditor
    {

        #region Fields

        private readonly HighlightingE highlighting;
        private SyntaxCheckerModeE syntaxCheckerMode;
        private Program currentProgram;
        private Macro currentMacro;
        private readonly SyntaxChecker syntaxChecker;
        private readonly Intellisense intellisense;

        #endregion

        #region enums

        public enum HighlightingE
        {
            On,
            Off
        }

        public enum SyntaxCheckerModeE
        {
            RealTime,
            OnDemand
        }

        #endregion


        #region Properties

        public Macro CurrentMacro
        {
            set
            {
                currentMacro = value;
                Text = value == null ? string.Empty : CurrentMacro.Content;
            }
            get
            {
                return currentMacro;
            }
        }
        public Program CurrentProgram
        {
            set
            {
                currentProgram = value;
                Text = value == null ? string.Empty : CurrentProgram.Content;
            }
            get
            {
                return currentProgram;
            }
        }

        public SyntaxCheckerModeE SyntaxCheckerMode
        {
            get { return syntaxCheckerMode; }
            set
            {
                if (value == SyntaxCheckerModeE.RealTime)
                {
                    TextChanged += OnSyntaxCheck;
                    DataObject.AddPastingHandler(this, OnPaste);
                }
                else
                {
                    TextChanged -= OnSyntaxCheck;
                    DataObject.RemovePastingHandler(this, OnPaste);
                }
                syntaxCheckerMode = value;
            }
        }

        public bool DoSyntaxCheck { get; set; }

        #endregion

        #region Constructor

        public ProgramEditor(HighlightingE highlighting)
        {
            InitializeAvalon();
            this.highlighting = highlighting;
            syntaxChecker = new SyntaxChecker();
            intellisense = new Intellisense();
            syntaxCheckerMode = SyntaxCheckerModeE.OnDemand;

            TextChanged += OnTextChanged;
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            if (currentProgram != null)
            {
                currentProgram.Content = Text;
            }
        }

        #endregion

        #region Actions

        private void InitializeAvalon()
        {
            ShowLineNumbers = true;
            Foreground = new SolidColorBrush(Color.FromRgb(193, 193, 193));
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            Padding = new Thickness(5);

            if (highlighting == HighlightingE.On && !MissingFileCreator.HighlightingErrorWasAlreadyShown)
            {
                try
                {
                    LoadHighligtingDefinition();
                }
                catch (FileNotFoundException)
                {
                    MissingFileCreator.CreateHighlightingDefinitionFile();
                    LoadHighligtingDefinition();
                }

            }   
        }

        private void LoadHighligtingDefinition()
        {
            var definition = HighlightingLoader.Load(XmlReader.Create("CustomHighlighting.xshd"),
                HighlightingManager.Instance);
            HighlightingManager.Instance.RegisterHighlighting("CustomHighlighting", new[] { ".txt" }, definition);
            SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("CustomHighlighting");
        }

        public bool CheckLineValidationManually(string line)
        {
            if (!string.IsNullOrEmpty(line))
            {
                return syntaxChecker.Validate(line);
            }
            return false;
        }

        #endregion

        #region Event Handlers

        private void OnSyntaxCheck(object sender, EventArgs e)
        {
            ValidateLine(TextArea.Caret.Line);
        }

        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            var isText = e.SourceDataObject.GetDataPresent(DataFormats.UnicodeText, true);
            if (!isText) return;

            var text = e.SourceDataObject.GetData(DataFormats.UnicodeText) as string;
            currentProgram.Content = text;
            ValidateAllLines();
        }

        public async void ValidateAllLines()
        {
            if (!string.IsNullOrEmpty(CurrentProgram?.Content) && DoSyntaxCheck)
            {
                for(var i = 0; i < TextArea.Document.Lines.Count; i++)
                {
                    var lineText = TextArea.Document.GetText(TextArea.Document.Lines[i]);
                    var isValid = await syntaxChecker.ValidateAsync(lineText);
                    TextArea.TextView.LineTransformers.Add(new LineColorizer(i+1, 
                        isValid ? LineColorizer.ValidityE.Yes : LineColorizer.ValidityE.No));
                }
            }
        }

        private async void ValidateLine(int lineNum)
        {
            if (DoSyntaxCheck)
            {
                var line = TextArea.Document.GetLineByNumber(lineNum);
                var lineText = TextArea.Document.GetText(line);
                var isValid = await syntaxChecker.ValidateAsync(lineText);

                TextArea.TextView.LineTransformers.Add(new LineColorizer(lineNum,
                    isValid ? LineColorizer.ValidityE.Yes : LineColorizer.ValidityE.No));
            }
            else
            {
                TextArea.TextView.LineTransformers.Add(
                    new LineColorizer(lineNum, LineColorizer.ValidityE.Yes));
            }
        }
        
        public void ExportContent(string defaultFileName, string extension)
        {
            try
            {
                var dialog = new SaveFileDialog
                {
                    // Default file name
                    FileName = defaultFileName,
                    // Default file extension
                    DefaultExt = extension,
                    // Filter files by extension
                    Filter = $"{extension} files (.{extension}|*.{extension}"
                };

                // Process save file dialog box results
                if (dialog.ShowDialog() == false)
                {
                    return;
                }

                var lines = Text.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);


                File.WriteAllLines($"{dialog.FileName}", lines);
            }
            catch (Exception)
            {
                MessageBox.Show("Something went very wrong here. Try again tommorow.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

    }
}
