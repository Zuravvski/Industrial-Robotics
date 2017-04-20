using System.Configuration;
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
using System.Timers;
using System.Collections.Generic;
using IDE.Common.Utilities;
using System.Diagnostics;

namespace IDE.Common.Models
{
    public class ProgramEditor : TextEditor
    {

        #region Fields

        private readonly Highlighting highlighting;
        private Program currentProgram;
        private Macro currentMacro;
        Timer regexTimer = new Timer();

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

            regexTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            regexTimer.Interval = 10000;
            regexTimer.Enabled = true;
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

        public bool IsOneLine { set; get; }
        public List<bool> IsLineValid { get; private set; }
        public bool DoSyntaxCheck { get; set; }
        
        #endregion

        #region Actions

        private void InitializeAvalon()
        {
            ShowLineNumbers = true;
            Foreground = new SolidColorBrush(Color.FromRgb(193, 193, 193));
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;

            if (highlighting == Highlighting.On && !MissingFileCreator.HighlightingErrorWasAlreadyShown)
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
                    MissingFileCreator.CreateHighlightingDefinitionFile();
                }
            }

            TextArea.TextEntering += TextEntering;
            TextArea.TextEntered += TextEntered;
            TextArea.PreviewKeyDown += KeyIsDown;
            TextChanged += TextHasChanged;
        }

        private void TextHasChanged(object sender, EventArgs e)
        {
            //if (IsOneLine)
            //    Text = Text.Replace(System.Environment.NewLine, "replacement text");
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            //tbi
        }

        private void TextEntered(object sender, TextCompositionEventArgs e)
        {
            if (currentProgram != null)
            {
                currentProgram.Content = Text;
            }
        }

        private void TextEntering(object sender, TextCompositionEventArgs e)
        {
            //tbi
        }

        public void ExportContent(string defaultFileName, string extension)
        {
            try
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.FileName = defaultFileName; // Default file name
                dialog.DefaultExt = extension; // Default file extension
                dialog.Filter = $"{extension} files (.{extension}|*.{extension}"; // Filter files by extension

                // Process save file dialog box results
                if (dialog.ShowDialog() == false)
                {
                    return;
                }

                string[] lines = Text.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);


                File.WriteAllLines($"{dialog.FileName}", lines);
            }
            catch (Exception)
            {
                MessageBox.Show("Something went very wrong here. Try again tommorow.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CheckLineValidation()
        {
            List<bool> isLineValid = new List<bool>();
            if (CurrentProgram != null && !string.IsNullOrEmpty(CurrentProgram.Content))
            {
                string[] lines = CurrentProgram.Content.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

                for (int i = 0; i < lines.Length; i++)
                {
                    isLineValid.Add(RegexMatching.InputMatching(lines[i]));
                }
            }

            IsLineValid = isLineValid;
        }

        public static bool CheckLineValidationManually(string line)
        {
            if (!string.IsNullOrEmpty(line))
            {
                return RegexMatching.InputMatching(line);
            }
            return false;
        }


        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            if (DoSyntaxCheck == true)
                CheckLineValidation();
        }

        #endregion

    }
}
