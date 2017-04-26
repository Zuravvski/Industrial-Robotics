﻿using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Driver;
using ICSharpCode.AvalonEdit.CodeCompletion;
using IDE.Common.Models.Code_Completion;
using IDE.Common.Models.Services;
using IDE.Common.Models.Syntax_Check;
using IDE.Common.Models.Value_Objects;
using IDE.Common.Utilities;
using IDE.Common.Utilities.Extensions;
using Microsoft.Win32;

namespace IDE.Common.Models
{
    public class ProgramEditor : TextEditor
    {

        #region Fields
        private readonly HighlightingE highlighting;
        private SyntaxCheckerModeE syntaxCheckerMode;
        private readonly UseIntellisense useIntellisense;
        private Program currentProgram;
        private Macro currentMacro;
        private readonly SyntaxChecker syntaxChecker;
        private IList<ICompletionData> data;
        private readonly Intellisense intellisense;
        private IEnumerable<Command> commands;
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

        public enum UseIntellisense
        {
            Yes,
            No
        }

        #endregion

        #region Constructor

        public ProgramEditor(HighlightingE highlighting, UseIntellisense useIntellisense)
        {
            this.highlighting = highlighting;
            this.useIntellisense = useIntellisense;
            InitializeAvalon();
            syntaxChecker = new SyntaxChecker();

            Session.Instance.Highlighting.HighlightingChanged += LoadHighligtingDefinition;
            TextChanged += OnTextChanged;

            if (useIntellisense == UseIntellisense.Yes)
            {
                intellisense = new Intellisense();
                TextArea.TextEntering += TextArea_TextEntering;
                TextArea.TextEntered += TextArea_TextEntered;
                TextArea.PreviewKeyDown += TextArea_PreviewKeyDown;
            }
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


        public CompletionWindow completionWindow { get; set; }

        #endregion

        public bool DoSyntaxCheck { get; set; }
        public bool IsOneLine { get; set; }

        #region Actions

        private void InitializeAvalon()
        {
            ShowLineNumbers = true;
            Foreground = new SolidColorBrush(Color.FromRgb(193, 193, 193));
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            Padding = new Thickness(5);

            if (highlighting == HighlightingE.On)
            {
                try
                {
                    LoadHighligtingDefinition();
                }
                catch (FileNotFoundException)
                {
                    MissingFileManager.CreateHighlightingDefinitionFile();
                    Console.Error.WriteLine("Could not load highlighting definition. Loading defaults.");
                    LoadHighligtingDefinition();
                }
            }   
        }

        private void LoadHighligtingDefinition()
        {
            var filePath = Session.Instance.Highlighting.FilePath;

            using (var reader = new XmlTextReader(filePath))
            {
                var definition = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                SyntaxHighlighting = definition;
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
            completionWindow?.Show();
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
                for(var i = 0; i < TextArea.Document.Lines.Count; i++)
                {
                    var lineText = TextArea.Document.GetText(TextArea.Document.Lines[i]);
                    var isValid = await syntaxChecker.ValidateAsync(lineText);
                    TextArea.TextView.LineTransformers.Add(new LineColorizer(i+1, 
                        isValid ? LineColorizer.ValidityE.Yes : LineColorizer.ValidityE.No));
                }
            }
        }

        public async void ValidateLine(int lineNum)
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

        private void OnTextChanged(object sender, EventArgs e)
        {
            if (currentProgram != null)
            {
                currentProgram.Content = Text;
            }
        }

        #endregion
    }
}
