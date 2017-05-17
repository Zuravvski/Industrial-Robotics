using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;
using ICSharpCode.AvalonEdit.Document;
using IDE.Common.Models.Code_Completion;
using IDE.Common.Models.Syntax_Check;
using IDE.Common.Utilities;
using Microsoft.Win32;

namespace IDE.Common.Models
{
    public class ProgramEditor : TextEditor
    {

        #region Fields

        private SyntaxCheckerModeE syntaxCheckerMode;
        private readonly SyntaxChecker syntaxChecker;
        private readonly Intellisense intellisense;
        private readonly SyntaxCheckVisualizer syntaxCheckVisualizer;
        private bool isHighlightingEnabled;
        private bool isIntellisenseEnabled;

        #endregion

        #region Enums
        
        /// <summary>
        /// Describes wheter syntax check will occcur on the real time or on demand.
        /// </summary>
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
            SyntaxCheckerMode = SyntaxCheckerModeE.OnDemand;

            // Events
            Session.Instance.Highlighting.HighlightingChanged += LoadHighligtingDefinition;
            DataObject.AddPastingHandler(this, OnPaste);
        }
        
        #endregion

        #region Properties

        public bool IsHighlightingEnabled
        {
            get { return isHighlightingEnabled; }
            set
            {
                isHighlightingEnabled = value;
                LoadHighligtingDefinition();
            }
        }

        public bool IsIntellisenseEnabled
        {
            get { return isIntellisenseEnabled; }
            set
            {
                isIntellisenseEnabled = value;
                if (isIntellisenseEnabled)
                {
                    //Subscribes to intellisense events.
                    TextArea.TextEntering += OnIntellisensePreparation;
                    TextArea.TextEntered += OnIntellisenseShow;
                    TextArea.PreviewKeyDown += OnIntellisenseSubmition;
                }
                else
                {
                    //Unsubscribes from intellisense events.
                    TextArea.TextEntering -= OnIntellisensePreparation;
                    TextArea.TextEntered -= OnIntellisenseShow;
                    TextArea.PreviewKeyDown -= OnIntellisenseSubmition;
                }
            }
        }

        public bool IsIntellisenseShowing => intellisense.IsShowing;

        public static readonly DependencyProperty DoSyntaxCheckProperty =
             DependencyProperty.Register("DoSyntaxCheck", typeof(bool),
             typeof(ProgramEditor), new FrameworkPropertyMetadata(true));

        public bool DoSyntaxCheck
        {
            get { return (bool)GetValue(DoSyntaxCheckProperty); }
            set
            {
                SetValue(DoSyntaxCheckProperty, value);
                if (!value)
                {
                    foreach (var line in Document.Lines)
                    {
                        syntaxCheckVisualizer.Visualize(true, line);
                    }
                }
                else
                {
                    ValidateAllLines();
                }
            }
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
                }
                else
                {
                    TextChanged -= OnSyntaxCheck;
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
            else
            {
                SyntaxHighlighting = null;
            }

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
            var line = Document.GetLineByNumber(TextArea.Caret.Line);
            // show only if nothing valid was typed in current line yet
            if (!syntaxChecker.Validate(Document.GetText(line)))
            {
                intellisense.Show();
            }
            else
            {
                intellisense.Close();
            }
        }

        private async void OnSyntaxCheck(object sender, EventArgs e)
        {
            if (DoSyntaxCheck)
            {
                await ValidateLine(TextArea.Caret.Line);
            }    
        }

        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            var isText = e.SourceDataObject.GetDataPresent(DataFormats.UnicodeText, true);
            if (!isText)
                return;

            var text = e.SourceDataObject.GetData(DataFormats.UnicodeText) as string;

            if (!string.IsNullOrWhiteSpace(text))
            {
                ValidateAllLines();
            }
        }

        public async void ValidateAllLines()
        {
            if (DoSyntaxCheck)
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
