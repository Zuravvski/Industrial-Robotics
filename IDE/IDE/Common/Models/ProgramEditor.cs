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
using IDE.Common.ViewModels;

namespace IDE.Common.Models
{
    public class ProgramEditor : TextEditor
    {

        #region Fields
        private Highlighting highlighting;
        private bool isHighlightingEnabled;
        private UseIntellisense useIntellisense;
        private bool isIntellisenseEnabled;
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
            this.highlighting = highlighting;
            this.useIntellisense = useIntellisense;
        }

        public ProgramEditor()
        {
            MissingFileCreator.CheckForRequiredFiles();
            syntaxChecker = new SyntaxChecker();
        }
        
        
        #endregion

        #region Properties

        public bool IsHighlightingEnabled
        {
            get
            {
                return isHighlightingEnabled;
            }
            set
            {
                isHighlightingEnabled = value;
                InitializeHighlighting();
            }
        }

        public bool IsIntellisenseEnabled
        {
            get
            {
                return isIntellisenseEnabled;
            }
            set
            {
                isIntellisenseEnabled = value;
                InitializeIntellisense();
            }
        }

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

        private void InitializeHighlighting()
        {
            if (IsHighlightingEnabled)
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
        }

        private void InitializeIntellisense()
        {
            if (isIntellisenseEnabled)
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
        
    }
}
