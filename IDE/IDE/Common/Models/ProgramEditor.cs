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
using IDE.Common.Models.Services;
using IDE.Common.Models.Syntax_Check;
using IDE.Common.Models.Value_Objects;
using IDE.Common.Utilities;
using ICSharpCode.AvalonEdit.CodeCompletion;
using System.Collections.Generic;
using IDE.Common.Models.Code_Completion;
using IDE.Common.Utilities.Extensions;

namespace IDE.Common.Models
{
    public class ProgramEditor : TextEditor
    {

        #region Fields
        private readonly Highlighting highlighting;
        private readonly UseIntellisense useIntellisense;
        private Program currentProgram;
        private Macro currentMacro;
        private readonly SyntaxChecker syntaxChecker;
        public CompletionWindow completionWindow;
        private IList<ICompletionData> data;
        private Intellisense intellisense;
        private IEnumerable<Command> commands;

        #endregion

        #region enums

        public enum Highlighting
        {
            On,
            Off
        }

        public enum UseIntellisense
        {
            Yes,
            No
        }

        #endregion

        #region Constructor

        public ProgramEditor(Highlighting highlighting, UseIntellisense useIntellisense)
        {
            MissingFileCreator.CheckForRequiredFiles();
            this.highlighting = highlighting;
            this.useIntellisense = useIntellisense;
            InitializeAvalon();

            syntaxChecker = new SyntaxChecker();
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
        public bool IsOneLine { get; set; }
        
        #endregion

        #region Actions

        private void InitializeAvalon()
        {
            ShowLineNumbers = true;
            Foreground = new SolidColorBrush(Color.FromRgb(193, 193, 193));
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            Padding = new Thickness(5);

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
                    Console.Error.WriteLine("Error loading HighlightingDefinition file");
                }
            }

            if (useIntellisense == UseIntellisense.Yes)
            {
                intellisense = new Intellisense();
                TextArea.TextEntering += TextArea_TextEntering;
                TextArea.TextEntered += TextArea_TextEntered;
                TextArea.PreviewKeyDown += TextArea_PreviewKeyDown;
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

        private void TextArea_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (completionWindow != null)
                {
                    completionWindow?.Focus();
                    e.Handled = true;
                }

                if (IsOneLine)
                {
                    e.Handled = true;
                }
            }
        }

        private void TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (completionWindow == null)
            {
                completionWindow = new CompletionWindow(TextArea);
                data = completionWindow.CompletionList.CompletionData;

                commands = intellisense.Commands;
                foreach (var command in commands)
                {
                    data.Add(new MyCompletionData(command.Content, command.Description, command.Type.Description()));
                }
            }

            completionWindow.Closed += delegate
            {
                completionWindow = null;
                data = null;
            };


            if (e.Text.Length > 0 && completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open, insert the currently selected element.
                    completionWindow.CompletionList.RequestInsertion(e);
                }
            }
            // Do not set e.Handled=true, we still want to insert the character that was typed.
        }

        private void TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            if (completionWindow != null)
            {
                completionWindow.Show();
            }
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

        private async void ValidateAllLines()
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

        #region OLD_INTELLISENSE

        //private readonly Intellisense intellisense;
        //intellisense = new Intellisense();

        //private void OnIntellisense(object sender, TextCompositionEventArgs textCompositionEventArgs)
        //{
        //    //tbi
        //}


        //public async void RunIntellisense(bool isForced)
        //{
        //    var line = TextArea.Document.GetLineByNumber(TextArea.Caret.Line);
        //    var lineText = TextArea.Document.GetText(line);

        //    // Don't show intellisense if theres no text in the line unless user forces intellisense by pressing ctrl+space
        //    if (!string.IsNullOrWhiteSpace(lineText) || isForced)
        //    {
        //        var tips = await intellisense.GetCompletionAsync(lineText);

        //        var completionWindow = new CompletionWindow(TextArea);
        //        completionWindow.Closed += delegate
        //        {
        //            completionWindow = null;
        //        };
        //        var data = completionWindow.CompletionList.CompletionData;

        //        //await Task.Run(() =>
        //        //{
        //        //    foreach (var command in tips)
        //        //    {
        //        //        var completionData = new MyCompletionData(command.Content, command.Description, Command.TypeE.None);
        //        //        data.Add(completionData);
        //        //    }

        //        //});

        //        // Don't show empty results
        //        if (data.Count > 0)
        //            completionWindow.Show();
        //    }
        //}

        #endregion

    }
}
