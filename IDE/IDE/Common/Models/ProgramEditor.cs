using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Driver;
using IDE.Common.Models.Code_Completion;
using IDE.Common.Models.Syntax_Check;
using IDE.Common.Models.Value_Objects;
using IDE.Common.Utilities;
using Microsoft.Win32;

namespace IDE.Common.Models
{
    public class ProgramEditor : TextEditor
    {

        #region Fields

        private SyntaxCheckerModeE syntaxCheckerMode;
        private Program currentProgram;
        private Macro currentMacro;
        private readonly SyntaxChecker syntaxChecker;
        private readonly Intellisense intellisense;
        private readonly SyntaxCheckVisualizer syntaxCheckVisualizer;
        private bool isIntellisenseEnabled;

        #endregion

        #region enums
        
        public enum SyntaxCheckerModeE
        {
            RealTime,
            OnDemand
        }

        #endregion

        #region Constructor

        public ProgramEditor()
        {
            syntaxChecker = new SyntaxChecker();
            intellisense = new Intellisense(TextArea);
            syntaxCheckVisualizer = new SyntaxCheckVisualizer(this);
            IsIntellisenseEnabled = true;
            IsHighlightingEnabled = true;
            Session.Instance.Highlighting.HighlightingChanged += LoadHighligtingDefinition;
            LoadHighligtingDefinition();
        }
        
        #endregion

        #region Properties

        public bool IsHighlightingEnabled { get; set; }

        public bool IsIntellisenseEnabled
        {
            get { return isIntellisenseEnabled; }
            set
            {
                isIntellisenseEnabled = value;
                if (isIntellisenseEnabled)
                {
                    TextArea.TextEntering += OnIntellisensePreparation;
                    TextArea.TextEntered += OnIntellisenseShow;
                    TextArea.PreviewKeyDown += OnIntellisenseSubmition;
                }
                else
                {
                    TextArea.TextEntering -= OnIntellisensePreparation;
                    TextArea.TextEntered -= OnIntellisenseShow;
                    TextArea.PreviewKeyDown -= OnIntellisenseSubmition;
                }
            }
        }

        public bool IsIntellisenseShowing => intellisense.IsShowing;

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

        public static readonly DependencyProperty DoSyntaxCheckProperty =
             DependencyProperty.Register("DoSyntaxCheck", typeof(bool),
             typeof(ProgramEditor), new FrameworkPropertyMetadata(true));

        public bool DoSyntaxCheck
        {
            get { return (bool)GetValue(DoSyntaxCheckProperty); }
            set { SetValue(DoSyntaxCheckProperty, value); }
        }

        public bool IsOneLine { get; set; }

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

        #endregion

        #region Actions

        public void FontEnlarge()
        {
            FontSize++;
        }

        public void FontReduce()
        {
            FontSize--;
        }

        /// <summary>
        /// Sets current font.
        /// </summary>
        public void ChangeFont(string fontName)
        {
            var fontFamily = FontFamily;

            try
            {
                FontFamily = new FontFamily(fontName);
            }
            catch
            {
                FontFamily = fontFamily;
            }
        }

        private void LoadHighligtingDefinition()
        {
            if (IsHighlightingEnabled)
            {
                var filePath = Session.Instance.Highlighting.FilePath;
                using (var reader = new XmlTextReader(filePath))
                {
                    var definition = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                    SyntaxHighlighting = definition;
                }
            }

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

        private void OnIntellisenseSubmition(object sender, KeyEventArgs e)
        {
            intellisense.Submit(e, IsOneLine);
        }

        private void OnIntellisensePreparation(object sender, TextCompositionEventArgs e)
        {
            intellisense.Prepare(e);
        }

        private void OnIntellisenseShow(object sender, TextCompositionEventArgs e)
        {
            intellisense.Show();
        }

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
                foreach (var line in TextArea.Document.Lines)
                {
                    var lineText = TextArea.Document.GetText(line);
                    var isValid = await syntaxChecker.ValidateAsync(lineText);
                    syntaxCheckVisualizer.Visualize(isValid, line);
                }
            }
        }

        public async Task<bool> ValidateLine(int lineNum)
        {
            var line = TextArea.Document.GetLineByNumber(lineNum);
            var lineText = TextArea.Document.GetText(line);
            var isValid = await syntaxChecker.ValidateAsync(lineText);
            syntaxCheckVisualizer.Visualize(!DoSyntaxCheck || isValid, line);
            return isValid;
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
