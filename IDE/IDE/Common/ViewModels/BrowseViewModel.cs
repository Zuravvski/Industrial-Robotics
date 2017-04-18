using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using IDE.Common.Models;
using IDE.Common.ViewModels.Commands;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using IDE.Common.Models.Intellisense;
using IDE.Common.Models.Services;
using IDE.Common.Models.Value_Objects;

namespace IDE.Common.ViewModels
{
    public class BrowseViewModel : ObservableObject
    {
        private ProgramEditor commandHistory, commandInput;
        private bool lineWasNotValid;
        private bool runIntellisense;
        private readonly SyntaxChecker syntaxChecker;
        private readonly Intellisense intellisense;
        private CompletionWindow completionWindow;

        public BrowseViewModel()
        {
            DeclareCommands();
            InitializeCommandHistory();
            InitializeCommandInput();

            MessageList = new MessageList();    //list storing sent commands
            syntaxChecker = new SyntaxChecker();
            intellisense = new Intellisense();
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
            CommandInput = new ProgramEditor(ProgramEditor.Highlighting.On)
            {
                ShowLineNumbers = false,
                Background = new SolidColorBrush(Color.FromRgb(61, 61, 61)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(41, 41, 41)),
                BorderThickness = new Thickness(0, 0, 2, 0)
            };
            //make only right border visible
            CommandInput.PreviewKeyDown += CommandInput_PreviewKeyDown; //to ensure 'enter' triggers Send() event
        }

        private void InitializeCommandHistory()
        {
            CommandHistory = new ProgramEditor(ProgramEditor.Highlighting.On)
            {
                IsReadOnly = true,
                Background = new SolidColorBrush(Color.FromRgb(61, 61, 61)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(41, 41, 41)),
                BorderThickness = new Thickness(0, 0, 0, 2),
                ShowLineNumbers = false
            };
            //make only bottom border visible
            CommandHistory.PreviewMouseWheel += CommandHistory_PreviewMouseWheel; CommandHistory.TextArea.FontFamily = new FontFamily("Cambria");
        }

        private void Refresh(object obj)
        {
            //tbi
        }

        private void Download(object obj)
        {
            //tbi
        }
        private void Upload(object obj)
        {
            //tbi
        }

        private async void Send(object obj = null)
        {
            if (!string.IsNullOrEmpty(CommandInput.Text))
            {
                if (CommandInput.DoSyntaxCheck != true) //if user dont want to check syntax just send it right away
                {
                    var colorizer = new LineColorizer(1, LineColorizer.ValidityE.Yes, CommandInput.Background);
                    CommandInput.TextArea.TextView.LineTransformers.Add(colorizer);
                    MessageList.AddMessage(new Message(DateTime.Now.ToString(CultureInfo.InvariantCulture), CommandInput.Text));
                    CommandHistory.Text += MessageList.Messages[MessageList.Messages.Count - 1].MyTime.ToString() + ": " +
                        MessageList.Messages[MessageList.Messages.Count - 1].MyMessage.ToString() + "\n";
                    CommandHistory.ScrollToEnd();
                    CommandInput.Text = string.Empty;
                }
                else
                {
                    var isLineValid = await syntaxChecker.ValidateAsync(CommandInput.Text);

                    if (isLineValid)    //if line is valid, send it
                    {
                        var colorizer = new LineColorizer(1, LineColorizer.ValidityE.Yes, CommandInput.Background);
                        MessageList.AddMessage(new Message(DateTime.Now.ToString(CultureInfo.InvariantCulture), CommandInput.Text));
                        CommandHistory.Text += MessageList.Messages[MessageList.Messages.Count - 1].MyTime + ": " +
                            MessageList.Messages[MessageList.Messages.Count - 1].MyMessage + "\n";
                        CommandHistory.ScrollToEnd();
                        CommandInput.Text = string.Empty;
                    }
                    else //if line is not valid colorize line and do nothing
                    {
                        var colorizer = new LineColorizer(1, LineColorizer.ValidityE.No, CommandInput.Background);
                        CommandInput.TextArea.TextView.LineTransformers.Add(colorizer);
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
                Send();
                e.Handled = true;
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
            SendClickCommand = new RelayCommand(Send);
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

        #endregion

    }
}
