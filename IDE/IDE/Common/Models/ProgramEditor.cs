using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using Driver;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Microsoft.Win32;
using System;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit.CodeCompletion;
using IDE.Common.Models.Code_Completion;
using IDE.Common.Models.Services;
using IDE.Common.Models.Syntax_Check;
using IDE.Common.Models.Value_Objects;

namespace IDE.Common.Models
{
    public class ProgramEditor : TextEditor
    {

        #region Fields

        private readonly Highlighting highlighting;
        private Program currentProgram;
        private Macro currentMacro;
        private readonly SyntaxChecker syntaxChecker;
        private readonly Intellisense intellisense;

        #endregion

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

            syntaxChecker = new SyntaxChecker();
            intellisense = new Intellisense();
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
        public bool DoSyntaxCheck { get; set; }
        
        #endregion

        #region Actions

        private void InitializeAvalon()
        {
            ShowLineNumbers = true;
            Foreground = new SolidColorBrush(Color.FromRgb(193, 193, 193));
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;

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

            // Dispatching events
            TextChanged += OnSyntaxCheck;
            TextArea.TextEntered += OnIntellisense;
            TextArea.TextEntered += OnTextEntered;
            DataObject.AddPastingHandler(this, OnPaste);
        }

        #region Event Handlers
        private void OnIntellisense(object sender, TextCompositionEventArgs textCompositionEventArgs)
        {
            RunIntellisense(false);
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

        private void OnTextEntered(object sender, TextCompositionEventArgs e)
        {
            if (currentProgram != null)
            {
                currentProgram.Content = Text;
            }
        }
        #endregion

        private async void ValidateAllLines()
        {
            if (!string.IsNullOrEmpty(CurrentProgram?.Content) && DoSyntaxCheck)
            {
                for(var i = 0; i < TextArea.Document.Lines.Count; i++)
                {
                    var lineText = TextArea.Document.GetText(TextArea.Document.Lines[i]);
                    var isValid = await syntaxChecker.ValidateAsync(lineText);
                    TextArea.TextView.LineTransformers.Add(new LineColorizer(i+1, 
                        isValid ? LineColorizer.ValidityE.Yes : LineColorizer.ValidityE.No, Background));
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
                    isValid ? LineColorizer.ValidityE.Yes : LineColorizer.ValidityE.No, Background));
            }
            else
            {
                TextArea.TextView.LineTransformers.Add(
                    new LineColorizer(lineNum, LineColorizer.ValidityE.Yes, Background));
            }
        }

        public async void RunIntellisense(bool isForced)
        {
            var line = TextArea.Document.GetLineByNumber(TextArea.Caret.Line);
            var lineText = TextArea.Document.GetText(line);

            // Don't show intellisense if theres no text in the line unless user forces intellisense by pressing ctrl+space
            if (!string.IsNullOrWhiteSpace(lineText) || isForced)
            {
                var tips = await intellisense.GetCompletionAsync(lineText);

                var completionWindow = new CompletionWindow(TextArea);
                completionWindow.Closed += delegate
                {
                    completionWindow = null;
                };
                var data = completionWindow.CompletionList.CompletionData;

                await Task.Run(() =>
                {
                    foreach (var command in tips)
                    {
                        var completionData = new MyCompletionData(command.Content, command.Description);
                        data.Add(completionData);
                    }

                });

                // Don't show empty results
                if(data.Count > 0)
                    completionWindow.Show();
            }
        }

        // TODO: Review this
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
