using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using IDE.Common.Models;
using IDE.Common.ViewModels.Commands;
using System.Windows.Media;
using Driver;
using IDE.Common.Models.Value_Objects;
using IDE.Common.Models.Services;
using IDE.Common.Models.Syntax_Check;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace IDE.Common.ViewModels
{
    public class BrowseViewModel : ObservableObject
    {

        #region Fields

        private ProgramEditor commandHistory, commandInput;
        private RemoteProgram selectedRemoteProgram;
        private bool lineWasNotValid;
        private E3JManipulator manipulator;
        private int messageSelectionArrows;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of BrowseViewModel class.
        /// </summary>
        public BrowseViewModel()
        {
            DeclareCommands();
            InitializeCommandHistory();
            InitializeCommandInput();

            MessageList = new MessageList();

            manipulator = new E3JManipulator(DriverSettings.CreateDefaultSettings());
            manipulator.Connect("COM4");
        }

        #endregion

        #region Properties

        /// <summary>
        /// List storing sent commands,
        /// </summary>
        public MessageList MessageList { get; }

        /// <summary>
        /// Read only editor for displaying send and received commands.
        /// </summary>
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

        /// <summary>
        /// Single line editor for user to send commands.
        /// </summary>
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


        /// <summary>
        /// Selected remote program from RV-E3J list of remote programs.
        /// </summary>
        public RemoteProgram SelectedProgram
        {
            set
            {
                selectedRemoteProgram = value;
                NotifyPropertyChanged("SelectedRemoteProgram");
            }
            get
            {
                return selectedRemoteProgram;
            }
        }

        #endregion

        #region Actions

        #region CommandWindow

        /// <summary>
        /// Initializes command input editor.
        /// </summary>
        private void InitializeCommandInput()
        {
            CommandInput = new ProgramEditor(ProgramEditor.HighlightingE.On, ProgramEditor.UseIntellisense.Yes)
            {
                ShowLineNumbers = false,
                Background = new SolidColorBrush(Color.FromRgb(61, 61, 61)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(41, 41, 41)),
                BorderThickness = new Thickness(0, 0, 2, 0), //right border visible = button will be separated from command input.
                IsOneLine = true
            };
            CommandInput.PreviewKeyDown += CommandInput_PreviewKeyDown;
            CommandInput.TextChanged += CommandInput_TextChanged;
        }

        /// <summary>
        /// Occurs when there is any text change in Command Input editor.
        /// </summary>
        private void CommandInput_TextChanged(object sender, EventArgs e)
        {
            if (lineWasNotValid)
            {
                var isLineValid = commandInput.CheckLineValidationManually(CommandInput.Text);

                if (isLineValid)
                {
                    lineWasNotValid = false;
                }

                CommandInput.TextArea.TextView.LineTransformers.Add(new LineColorizer(1,
                    isLineValid ? LineColorizer.ValidityE.Yes : LineColorizer.ValidityE.No));
            }
        }

        /// <summary>
        /// Initializes command history editor.
        /// </summary>
        private void InitializeCommandHistory()
        {
            CommandHistory = new ProgramEditor(ProgramEditor.HighlightingE.On, ProgramEditor.UseIntellisense.No)
            {
                IsReadOnly = true,
                Background = new SolidColorBrush(Color.FromRgb(61, 61, 61)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(41, 41, 41)),
                BorderThickness = new Thickness(0, 0, 0, 2),  //make only bottom border visible = C.History separated from C.Input.
                ShowLineNumbers = false
            };
            CommandHistory.PreviewMouseWheel += CommandHistory_PreviewMouseWheel;
        }

        /// <summary>
        /// Occurs after user triggers upload event.
        /// </summary>
        /// <param name="obj"></param>
        private void Refresh(object obj)
        {
            //TODO
        }

        /// <summary>
        /// Occurs after user triggers upload event.
        /// </summary>
        private void Download(object obj)
        {
            //TODO
        }

        /// <summary>
        /// Occurs after user triggers upload event.
        /// </summary>
        private void Upload(object obj)
        {
            //TODO
        }

        /// <summary>
        /// Occurs after user triggers send event.
        /// </summary>
        private void Send(object obj = null)
        {
            if (!string.IsNullOrWhiteSpace(CommandInput.Text))
            {
                if (CommandInput.DoSyntaxCheck != true) //if user dont want to check syntax just send it right away
                {
                    CommandInput.TextArea.TextView.LineTransformers.Add(new LineColorizer(1, LineColorizer.ValidityE.Yes));
                    MessageList.AddMessage(new Message(DateTime.Now, CommandInput.Text));
                    CommandHistory.Text += MessageList.Messages[MessageList.Messages.Count - 1].DisplayMessage();
                    CommandHistory.ScrollToEnd();
                    CommandInput.Text = string.Empty;
                }
                else //if user wants to check syntax
                {
                    bool isLineValid = commandInput.CheckLineValidationManually(CommandInput.Text);

                    if (isLineValid)    //if line is valid, send it
                    {
                        CommandInput.TextArea.TextView.LineTransformers.Add(new LineColorizer(1, LineColorizer.ValidityE.Yes));
                        MessageList.AddMessage(new Message(DateTime.Now, CommandInput.Text));
                        CommandHistory.Text += MessageList.Messages[MessageList.Messages.Count - 1].DisplayMessage();
                        CommandHistory.ScrollToEnd();
                        CommandInput.Text = string.Empty;
                    }
                    else //if line is not valid colorize line and don't send
                    {
                        CommandInput.TextArea.TextView.LineTransformers.Add(new LineColorizer(1, LineColorizer.ValidityE.No));
                        lineWasNotValid = true;
                    }
                }
            }
        }

        /// <summary>
        /// Occurs after user triggers font reduce event.
        /// </summary>
        private void FontReduce(object obj = null)
        {
            if (CommandHistory.FontSize > 3)
            {
                CommandHistory.FontSize--;
                CommandInput.FontSize--;
            }
        }

        /// <summary>
        /// Occurs after user triggers font enlarge event.
        /// </summary>
        private void FontEnlarge(object obj = null)
        {
            if (CommandHistory.FontSize < 20)
            {
                CommandHistory.FontSize++;
                CommandInput.FontSize++;
            }
        }

        /// <summary>
        /// Exports current content of Command History to a file.
        /// </summary>
        /// <param name="obj"></param>
        private void ExportHistory(object obj)
        {
            CommandHistory.ExportContent(DateTime.Now.ToString(CultureInfo.InvariantCulture).Replace(':', '-'), "txt");
        }

        /// <summary>
        /// Clears current content of Command History.
        /// </summary>
        /// <param name="obj"></param>
        private void ClearHistory(object obj)
        {
            CommandHistory.Text = string.Empty;
        }

        /// <summary>
        /// Sets current font as Times New Roman.
        /// </summary>
        private void FontTimesNewRoman(object obj)
        {
            CommandHistory.TextArea.FontFamily = new FontFamily("Times New Roman");
            CommandInput.TextArea.FontFamily = new FontFamily("Times New Roman");
        }

        /// <summary>
        /// Sets current font as Arial.
        /// </summary>
        private void FontArial(object obj)
        {
            CommandHistory.TextArea.FontFamily = new FontFamily("Arial");
            CommandInput.TextArea.FontFamily = new FontFamily("Times New Roman");
        }

        /// <summary>
        /// Sets current font as Calibri.
        /// </summary>
        private void FontCalibri(object obj)
        {
            CommandHistory.TextArea.FontFamily = new FontFamily("Calibri");
            CommandInput.TextArea.FontFamily = new FontFamily("Times New Roman");
        }

        /// <summary>
        /// Sets current font as Segoe UI.
        /// </summary>
        private void FontSegoeUI(object obj)
        {
            CommandHistory.TextArea.FontFamily = new FontFamily("Segoe UI");
            CommandInput.TextArea.FontFamily = new FontFamily("Times New Roman");
        }

        /// <summary>
        /// Occurs when there is any key down while having focus on Command Input editor.
        /// </summary>
        private void CommandInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && CommandInput.completionWindow == null)
                Send();

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

        /// <summary>
        /// Occurs when there is any mouse scroll press/movement while having focus on Command Input editor.
        /// </summary>
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

        /// <summary>
        /// Disables syntax check for command input.
        /// </summary>
        private void OffSyntaxCheck(object obj)
        {
            CommandInput.DoSyntaxCheck = false;
        }

        /// <summary>
        /// Enables syntax check for command input.
        /// </summary>
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
        public ICommand ClearHistoryCommand { get; private set; }
        public ICommand ExportHistoryCommand { get; private set; }
        public ICommand FontTNRomanCommand { get; private set; }
        public ICommand FontCalibriCommand { get; private set; }
        public ICommand FontArialCommand { get; private set; }
        public ICommand FontSegoeUICommand { get; private set; }
        public ICommand OnSyntaxCheckCommand { get; private set; }
        public ICommand OffSyntaxCheckCommand { get; private set; }

        private void DeclareCommands()
        {
            RefreshClickCommand = new RelayCommand(Refresh, IsConnectionEstablished);
            DownloadClickCommand = new RelayCommand(Download, IsItemSelected);
            UploadClickCommand = new RelayCommand(Upload, IsConnectionEstablished);
            SendClickCommand = new RelayCommand(Send, IsCommandInputNotEmpty);
            ClearHistoryCommand = new RelayCommand(ClearHistory, IsCommandHistoryNotEmpty);
            ExportHistoryCommand = new RelayCommand(ExportHistory, IsCommandHistoryNotEmpty);
            FontTNRomanCommand = new RelayCommand(FontTimesNewRoman, IsCurrentFontNotTNRoman);
            FontCalibriCommand = new RelayCommand(FontCalibri, IsCurrentFontNotCalibri);
            FontArialCommand = new RelayCommand(FontArial, IsCurrentFontNotArial);
            FontSegoeUICommand = new RelayCommand(FontSegoeUI, IsCurrentFontNotSegoeUI);
            OnSyntaxCheckCommand = new RelayCommand(OnSyntaxCheck);
            OffSyntaxCheckCommand = new RelayCommand(OffSyntaxCheck);
        }

        /// <summary>
        /// Return a value based upon wheter current font is Calibri or not.
        /// </summary>
        private bool IsCurrentFontNotCalibri(object obj)
        {
            if (CommandHistory.TextArea.FontFamily.ToString() == "Calibri")
                return false;
            else
                return true;
        }

        /// <summary>
        /// Return a value based upon wheter current font is Times New Roman or not.
        /// </summary>
        private bool IsCurrentFontNotTNRoman(object obj)
        {
            if (CommandHistory.TextArea.FontFamily.ToString() == "Times New Roman")
                return false;
            else
                return true;
        }

        /// <summary>
        /// Return a value based upon wheter current font is Segoe UI or not.
        /// </summary>
        private bool IsCurrentFontNotSegoeUI(object obj)
        {
            if (CommandHistory.TextArea.FontFamily.ToString() == "Segoe UI")
                return false;
            else
                return true;
        }

        /// <summary>
        /// Return a value based upon wheter current font is Arial or not.
        /// </summary>
        private bool IsCurrentFontNotArial(object obj)
        {
            if (CommandHistory.TextArea.FontFamily.ToString() == "Arial")
                return false;
            else
                return true;
        }

        /// <summary>
        /// Return a value based upon wheter Command History is empty or not.
        /// </summary>
        private bool IsCommandHistoryNotEmpty(object obj)
        {
            return !string.IsNullOrWhiteSpace(CommandHistory.Text);
        }

        /// <summary>
        /// Return a value based upon wheter a connection between computer and RV-E3J manipulator was established or not.
        /// </summary>
        private bool IsConnectionEstablished(object obj)
        {
            //TODO
            return false;
        }

        /// <summary>
        /// Returns a value based upon wheter a item is selected in remote program list or not.
        /// </summary>
        private bool IsItemSelected(object obj)
        {
            if (SelectedProgram != null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Returns a value based upon wheter a Command Input is empty or not.
        /// </summary>
        private bool IsCommandInputNotEmpty(object obj)
        {
            return !string.IsNullOrWhiteSpace(CommandInput.Text);
        }



        #endregion
    }
}
