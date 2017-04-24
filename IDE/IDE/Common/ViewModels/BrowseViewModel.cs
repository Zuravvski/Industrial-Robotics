using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using IDE.Common.Models;
using IDE.Common.ViewModels.Commands;
using System.Windows.Media;
using Driver;
using IDE.Common.Utilities;
using IDE.Common.Models.Value_Objects;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Documents;
using ICSharpCode.AvalonEdit.CodeCompletion;
using IDE.Common.Models.Code_Completion;
using IDE.Common.Models.Services;
using IDE.Common.Utilities.Extensions;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace IDE.Common.ViewModels
{
    public class BrowseViewModel : ObservableObject
    {
        private ProgramEditor commandHistory, commandInput;
        private bool lineWasNotValid;
        private readonly E3JManipulator manipulator;
        private int messageSelectionArrows;
        private CompletionWindow completionWindow;
        private IList<ICompletionData> data;
        private readonly Intellisense intellisense;

        public BrowseViewModel()
        {
            DeclareCommands();
            InitializeCommandHistory();
            InitializeCommandInput();

            MessageList = new MessageList();    //list storing sent commands
            intellisense = new Intellisense();


            //to usun
            manipulator = new E3JManipulator(DriverSettings.CreateDefaultSettings());
            manipulator.Connect("COM4");
        }


        #region Properties

        public MessageList MessageList { get; }

        public ProgramEditor CommandHistory
        {
            set
            {
                commandHistory = value;
                NotifyPropertyChanged("CommandHistory");
            }
            get
            {
                return commandHistory;
            }
        }

        public ProgramEditor CommandInput
        {
            set
            {
                commandInput = value;
                NotifyPropertyChanged("UserInput");
            }
            get
            {
                return commandInput;
            }
        }

        #endregion

        #region Actions


        #region CommandWindow
        private void InitializeCommandInput()
        {
            CommandInput = new ProgramEditor(ProgramEditor.HighlightingE.On)
            {
                ShowLineNumbers = false,
                Background = new SolidColorBrush(Color.FromRgb(61, 61, 61)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(41, 41, 41)),
                BorderThickness = new Thickness(0, 0, 2, 0)
            };
            //make only right border visible
            CommandInput.PreviewKeyDown += CommandInput_PreviewKeyDown; //to ensure 'enter' triggers Send() event
            CommandInput.TextChanged += CommandInput_TextChanged;

            CommandInput.TextArea.TextEntered += TextArea_TextEntered;
            CommandInput.TextArea.TextEntering += TextArea_TextEntering;
        }

        private void TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (completionWindow == null)
            {
                completionWindow = new CompletionWindow(CommandInput.TextArea);
                data = completionWindow.CompletionList.CompletionData;

                var commands = intellisense.Commands;
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
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    completionWindow.CompletionList.RequestInsertion(e);
                }
            }
            // Do not set e.Handled=true.
            // We still want to insert the character that was typed.
        }

        private void TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            completionWindow?.Show();
        }

        private void CommandInput_TextChanged(object sender, EventArgs e)
        {
            if (lineWasNotValid)
            {
                var isLineValid = commandInput.CheckLineValidationManually(CommandInput.Text);

                if (isLineValid)
                {
                    CommandInput.TextArea.TextView.LineTransformers.Clear();
                    lineWasNotValid = false;
                }
            }
        }

        private void InitializeCommandHistory()
        {
            CommandHistory = new ProgramEditor(ProgramEditor.HighlightingE.On)
            {
                IsReadOnly = true,
                Background = new SolidColorBrush(Color.FromRgb(61, 61, 61)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(41, 41, 41)),
                BorderThickness = new Thickness(0, 0, 0, 2),  //make only bottom border visible
                ShowLineNumbers = false
            };

            CommandHistory.PreviewMouseWheel += CommandHistory_PreviewMouseWheel;
        }

        private void Refresh(object obj)
        {
            MissingFileCreator.CreateHighlightingDefinitionFile();
        }

        private void Download(object obj)
        {
            //tbi
        }
        private void Upload(object obj)
        {
            throw new NotImplementedException();
        }

        private void Send(object obj = null)
        {
            if (!string.IsNullOrEmpty(CommandInput.Text))
            {
                if (CommandInput.DoSyntaxCheck != true) //if user dont want to check syntax just send it right away
                {
                    CommandInput.TextArea.TextView.LineTransformers.Clear();
                    MessageList.AddMessage(new Message(DateTime.Now.ToString(CultureInfo.InvariantCulture), CommandInput.Text));
                    CommandHistory.Text += MessageList.Messages[MessageList.Messages.Count - 1].MyTime + ": " +
                        MessageList.Messages[MessageList.Messages.Count - 1].MyMessage + "\n";
                    CommandHistory.ScrollToEnd();
                    CommandInput.Text = string.Empty;
                }
                else //if user wants to check syntax
                {
                    bool isLineValid = commandInput.CheckLineValidationManually(CommandInput.Text);

                    if (isLineValid)    //if line is valid, send it
                    {
                        CommandInput.TextArea.TextView.LineTransformers.Clear();
                        MessageList.AddMessage(new Models.Value_Objects.Message(DateTime.Now.ToString(CultureInfo.InvariantCulture), CommandInput.Text));
                        CommandHistory.Text += MessageList.Messages[MessageList.Messages.Count - 1].MyTime + ": " +
                            MessageList.Messages[MessageList.Messages.Count - 1].MyMessage + "\n";
                        CommandHistory.ScrollToEnd();
                        CommandInput.Text = string.Empty;
                    }
                    else //if line is not valid colorize line and do nothing
                    {
                        CommandInput.TextArea.TextView.LineTransformers.Add(new LineColorizer(1, LineColorizer.ValidityE.No));
                        lineWasNotValid = true;
                    }
                }
            }
        }

        private void FontReduce(object obj = null)
        {
            if (CommandHistory.FontSize > 3)
            {
                CommandHistory.FontSize--;
                CommandInput.FontSize--;
            }

        }

        private void FontEnlarge(object obj = null)
        {
            if (CommandHistory.FontSize < 20)
            {
                CommandHistory.FontSize++;
                CommandInput.FontSize++;
            }
        }

        private void ExportHistory(object obj)
        {

            CommandHistory.ExportContent(DateTime.Now.ToString(CultureInfo.InvariantCulture).Replace(':', '-'), "txt");
        }

        private void ClearHistory(object obj)
        {
            CommandHistory.Text = string.Empty;
        }
        private void FontTimesNewRoman(object obj)
        {
            CommandHistory.TextArea.FontFamily = new FontFamily("Times New Roman");
        }

        private void FontArial(object obj)
        {
            CommandHistory.TextArea.FontFamily = new FontFamily("Arial");
        }

        private void FontCalibri(object obj)
        {
            CommandHistory.TextArea.FontFamily = new FontFamily("Calibri");
        }

        private void CommandInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                completionWindow?.Focus();
                Send();
                e.Handled = true;
                
            }

            if (e.Key == Key.Up)
            {
                if (messageSelectionArrows < MessageList.Messages.Count)
                {
                    CommandInput.Text = MessageList.Messages[MessageList.Messages.Count - ++messageSelectionArrows].MyMessage;
                    CommandInput.TextArea.Caret.Offset = CommandInput.Text.Length;  //bring carret to end of text
                }
            }
            else if (e.Key == Key.Down)
            {
                if (messageSelectionArrows > 1)
                {
                    CommandInput.Text = MessageList.Messages[MessageList.Messages.Count - --messageSelectionArrows].MyMessage;
                    CommandInput.TextArea.Caret.Offset = CommandInput.Text.Length;  //bring carret to end of text
                }
                else if (messageSelectionArrows > 0)
                {
                    --messageSelectionArrows;
                    CommandInput.Text = string.Empty;
                }
            }
        }

        private void CommandHistory_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            bool handle = (Keyboard.Modifiers & ModifierKeys.Control) > 0;
            if (!handle)
                return;

            if (e.Delta > 0)    //scrolls away from user
                FontEnlarge();
            else if (e.Delta < 0)
                FontReduce();   //scrolls toward user
        }

        private void OffSyntaxCheck(object obj)
        {
            CommandInput.DoSyntaxCheck = false;
        }

        private void OnSyntaxCheck(object obj)
        {
            CommandInput.DoSyntaxCheck = true;
        }

        #endregion


        #endregion

        #region Commands

        public ICommand RefreshClickCommand { get; private set; }
        public ICommand DownloadClickCommand { get; private set; }
        public ICommand UploadClickCommand { get; private set; }
        public ICommand SendClickCommand { get; private set; }
        public ICommand FontEnlargeCommand { get; private set; }
        public ICommand FontReduceCommand { get; private set; }
        public ICommand ClearHistoryCommand { get; private set; }
        public ICommand ExportHistoryCommand { get; private set; }
        public ICommand FontTNRomanCommand { get; private set; }
        public ICommand FontCalibriCommand { get; private set; }
        public ICommand FontArialCommand { get; private set; }
        public ICommand OnSyntaxCheckCommand { get; private set; }
        public ICommand OffSyntaxCheckCommand { get; private set; }

        private void DeclareCommands()
        {
            RefreshClickCommand = new RelayCommand(Refresh);
            DownloadClickCommand = new RelayCommand(Download);
            UploadClickCommand = new RelayCommand(Upload);
            SendClickCommand = new RelayCommand(Send, IsCommandInputNotEmpty);
            FontEnlargeCommand = new RelayCommand(FontEnlarge);
            FontReduceCommand = new RelayCommand(FontReduce);
            ClearHistoryCommand = new RelayCommand(ClearHistory);
            ExportHistoryCommand = new RelayCommand(ExportHistory);
            FontTNRomanCommand = new RelayCommand(FontTimesNewRoman);
            FontCalibriCommand = new RelayCommand(FontCalibri);
            FontArialCommand = new RelayCommand(FontArial);
            OnSyntaxCheckCommand = new RelayCommand(OnSyntaxCheck);
            OffSyntaxCheckCommand = new RelayCommand(OffSyntaxCheck);
        }

        private bool IsCommandInputNotEmpty(object obj)
        {
            return !string.IsNullOrWhiteSpace(CommandInput.Text);
        }



        #endregion

    }
}
