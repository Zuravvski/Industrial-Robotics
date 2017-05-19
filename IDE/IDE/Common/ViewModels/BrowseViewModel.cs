using System;
using System.Globalization;
using System.Windows.Input;
using IDE.Common.Models;
using IDE.Common.ViewModels.Commands;
using Driver;
using System.Linq;
using IDE.Common.Models.Value_Objects;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using Microsoft.Win32;
using IDE.Common.Utilities;
using System.Threading;
using System.Threading.Tasks;
using IDE.Common.Kinect;

namespace IDE.Common.ViewModels
{
    /// <summary>
    /// BrowseViewModel class
    /// </summary>
    /// <seealso cref="IDE.Common.ViewModels.Commands.ObservableObject" />
    public class BrowseViewModel : ObservableObject
    {

        #region Fields

        /// <summary>
        /// The command history text
        /// </summary>
        private string commandHistoryText;
        /// <summary>
        /// The command input text
        /// </summary>
        private string commandInputText;
        /// <summary>
        /// The line was not valid
        /// </summary>
        private bool lineWasNotValid;
        /// <summary>
        /// The message selection arrows
        /// </summary>
        private int messageSelectionArrows;
        /// <summary>
        /// The remote programs
        /// </summary>
        private ObservableCollection<RemoteProgram> remotePrograms;
        /// <summary>
        /// The available COM ports
        /// </summary>
        private ObservableCollection<string> availableCOMPorts;
        /// <summary>
        /// The selected remote program
        /// </summary>
        private RemoteProgram selectedRemoteProgram;
        /// <summary>
        /// The manipulator
        /// </summary>
        private E3JManipulator manipulator;
        /// <summary>
        /// The settings
        /// </summary>
        private DriverSettings settings;
        /// <summary>
        /// The program service
        /// </summary>
        private ProgramService programService;
        /// <summary>
        /// The command history
        /// </summary>
        private readonly ProgramEditor commandHistory, commandInput;

        /// <summary>
        /// The selected COM port
        /// </summary>
        private string selectedCOMPort;
        /// <summary>
        /// The badge text
        /// </summary>
        private string badgeText;
        /// <summary>
        /// The dialog host
        /// </summary>
        private DialogHost dialogHost;
        /// <summary>
        /// The dialog host is open
        /// </summary>
        private bool dialogHostIsOpen;
        /// <summary>
        /// The connection toggle is checked
        /// </summary>
        private bool connectionToggleIsChecked;
        /// <summary>
        /// The kinect handler
        /// </summary>
        private KinectHandler kinectHandler;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of BrowseViewModel class.
        /// </summary>
        /// <param name="commandHistory">The command history.</param>
        /// <param name="commandInput">The command input.</param>
        public BrowseViewModel(ProgramEditor commandHistory, ProgramEditor commandInput)
        {
            DeclareCommands();

            this.commandInput = commandInput;
            commandInput.PreviewKeyDown += commandInput_PreviewKeyDown;
            commandInput.TextChanged += commandInput_TextChanged;

            this.commandHistory = commandHistory;
            commandHistory.PreviewMouseWheel += commandHistory_PreviewMouseWheel;

            MessageList = new MessageList();
            Settings = DriverSettings.CreateDefaultSettings();
            //KinectHandler = new KinectHandler();
            
            //this should be removed later on
            manipulator = new E3JManipulator(DriverSettings.CreateDefaultSettings());
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the kinect handler.
        /// </summary>
        /// <value>
        /// The kinect handler.
        /// </value>
        public KinectHandler KinectHandler
        {
            get { return kinectHandler; }
            set
            {
                kinectHandler = value;
                NotifyPropertyChanged("KinectHandler");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [connection toggle is checked].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [connection toggle is checked]; otherwise, <c>false</c>.
        /// </value>
        public bool ConnectionToggleIsChecked
        {
            get { return connectionToggleIsChecked; }
            set
            {
                connectionToggleIsChecked = value;
                if (connectionToggleIsChecked)
                    BadgeText = string.Empty;
                NotifyPropertyChanged("ConnectionToggleIsChecked");
            }
        }

        /// <summary>
        /// Gets or sets the badge text.
        /// </summary>
        /// <value>
        /// The badge text.
        /// </value>
        public string BadgeText
        {
            get { return badgeText; }
            set
            {
                badgeText = value;
                NotifyPropertyChanged("BadgeText");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [dialog host is open].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [dialog host is open]; otherwise, <c>false</c>.
        /// </value>
        public bool DialogHostIsOpen
        {
            get { return dialogHostIsOpen; }
            set
            {
                dialogHostIsOpen = value;
                if (!DialogHostIsOpen)
                {
                    DialogHost?.Cancel();
                }
                NotifyPropertyChanged("DialogHostIsOpen");
            }
        }

        /// <summary>
        /// Gets or sets the dialog host.
        /// </summary>
        /// <value>
        /// The dialog host.
        /// </value>
        public DialogHost DialogHost
        {
            get { return dialogHost; }
            set
            {
                dialogHost = value;
                NotifyPropertyChanged("DialogHost");
            }
        }

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        /// <value>
        /// The settings.
        /// </value>
        public DriverSettings Settings
        {
            get { return settings; }
            set
            {
                settings = value;
                NotifyPropertyChanged("Settings");
            }
        }

        /// <summary>
        /// Gets or sets the selected COM port.
        /// </summary>
        /// <value>
        /// The selected COM port.
        /// </value>
        public string SelectedCOMPort
        {
            get { return selectedCOMPort; }
            set
            {
                selectedCOMPort = value;
                NotifyPropertyChanged("SelectedCOMPort");
            }
        }

        /// <summary>
        /// Gets or sets the available COM ports.
        /// </summary>
        /// <value>
        /// The available COM ports.
        /// </value>
        public ObservableCollection<string> AvailableCOMPorts
        {
            get { return availableCOMPorts; }
            set
            {
                availableCOMPorts = value;
                NotifyPropertyChanged("AvailableCOMPorts");
            }
        }

        /// <summary>
        /// Gets or sets the command input text.
        /// </summary>
        /// <value>
        /// The command input text.
        /// </value>
        public string CommandInputText
        {
            get
            {
                return commandInputText;
            }
            set
            {
                commandInputText = value;
                NotifyPropertyChanged("CommandInputText");
            }
        }

        /// <summary>
        /// Gets or sets the command history text.
        /// </summary>
        /// <value>
        /// The command history text.
        /// </value>
        public string CommandHistoryText
        {
            get
            {
                return commandHistoryText;
            }
            set
            {
                commandHistoryText = value;
                NotifyPropertyChanged("CommandHistoryText");
            }
        }

        /// <summary>
        /// Gets the appearance.
        /// </summary>
        /// <value>
        /// The appearance.
        /// </value>
        public AppearanceViewModel Appearance => AppearanceViewModel.Instance;

        /// <summary>
        /// Gets or sets the selected remote program.
        /// </summary>
        /// <value>
        /// The selected remote program.
        /// </value>
        public RemoteProgram SelectedRemoteProgram
        {
            get
            {
                return selectedRemoteProgram;
            }
            set
            {
                selectedRemoteProgram = value;
                NotifyPropertyChanged("SelectedRemoteProgram");
            }
        }

        /// <summary>
        /// Gets or sets the remote programs.
        /// </summary>
        /// <value>
        /// The remote programs.
        /// </value>
        public ObservableCollection<RemoteProgram> RemotePrograms
        {
            get
            {
                return remotePrograms;
            }
            set
            {
                remotePrograms = value;
                NotifyPropertyChanged("RemotePrograms");
            }
        }

        /// <summary>
        /// Gets the manipulator.
        /// </summary>
        /// <value>
        /// The manipulator.
        /// </value>
        public E3JManipulator Manipulator
        {
            get
            {
                return manipulator;
            }
            private set
            {
                manipulator = value;
                programService = new ProgramService(manipulator);
                programService.StepUpdate += ProgramService_StepUpdate;
                NotifyPropertyChanged("Manipulator");
            }
        }

        /// <summary>
        /// List storing sent commands,
        /// </summary>
        /// <value>
        /// The message list.
        /// </value>
        public MessageList MessageList { get; }

        #endregion

        #region Actions

        /// <summary>
        /// Ports the connection status changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void Port_ConnectionStatusChanged(object sender, ConnectionStatusChangedArgs e)
        {
            if (e.OldStatus == true && e.NewStatus == false)
            {
                Manipulator.Port.DataReceived -= Port_DataReceived;
                ConnectionToggleIsChecked = false;
                SelectedCOMPort = null;
            }
        }

        /// <summary>
        /// Handles the StepUpdate event of the ProgramService control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="NotificationEventArgs"/> instance containing the event data.</param>
        private async void ProgramService_StepUpdate(object sender, NotificationEventArgs e)
        {
            int progress = (int)(e.CurrentStep / (float)e.NumberOfSteps * 100);
            var dialog = CreateDialogHost(false, e.ActionName, progress);

            if (e.CurrentStep == e.NumberOfSteps)
            {
                await Task.Delay(2000, dialog.CancellationToken);
                Refresh(dialog);
            }
        }

        /// <summary>
        /// Creates the dialog host.
        /// </summary>
        /// <param name="isIndeterminate">if set to <c>true</c> [is indeterminate].</param>
        /// <param name="currentAction">The current action.</param>
        /// <param name="currentProgress">The current progress.</param>
        /// <returns></returns>
        private DialogHost CreateDialogHost(bool isIndeterminate, string currentAction, int currentProgress = 0)
        {
            var message = "";
            var progress = "";

            if (isIndeterminate)
            {
                message = "Just a moment...";   //default indeterminate dialog 
            }
            else
            {
                if (currentProgress < 30)
                    message = "Hold on. Looks like it might take a while.";
                else if (currentProgress < 60)
                    message = "How about you get yourself some coffee?";
                else if (currentProgress < 90)
                    message = "Well, worst part is over, right?";
                else
                    message = "Get ready. We are almost done.";

                progress = currentProgress.ToString() + "%";
            }

            DialogHost = new DialogHost()
            {
                CurrentAction = currentAction,
                CurrentProgress = progress,
                Message = message,
            };
            return DialogHost;
        }

        /// <summary>
        /// Occurs when there is any text change in Command Input editor.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private async void commandInput_TextChanged(object sender, EventArgs e)
        {
            if (lineWasNotValid)
            {
                var isLineValid = await commandInput.ValidateLine(1);

                if (isLineValid)
                {
                    lineWasNotValid = false;
                }
            }

            if (CommandInputText.Equals(string.Empty))
                messageSelectionArrows = 0;
        }


        /// <summary>
        /// Occurs after user triggers upload event.
        /// </summary>
        /// <param name="obj">The object.</param>
        private async void Refresh(object obj)
        {
            DialogHostIsOpen = true;
            DialogHost host;
            if(obj != null && obj is DialogHost)
            {
                host = obj as DialogHost;
            }
            else
            {
                host = CreateDialogHost(true, "Refreshing program list");
            }
            RemotePrograms.Clear();
            RemotePrograms = new ObservableCollection<RemoteProgram>(new List<RemoteProgram>(await programService.ReadProgramInfo(host.CancellationToken)));
            DialogHostIsOpen = false;
        }

        /// <summary>
        /// Occurs after user triggers upload event.
        /// </summary>
        /// <param name="obj">The object.</param>
        private async void Download(object obj)
        {  
            var dialog = new SaveFileDialog()
            {
                Filter = "Text file (.txt)|*.txt",
                FileName = SelectedRemoteProgram.Name
            };

            if (dialog.ShowDialog().GetValueOrDefault(false))
            {
                DialogHostIsOpen = true;
                var host = CreateDialogHost(true, $"Downloading {SelectedRemoteProgram.Name}");
                var program = await programService.DownloadProgram(SelectedRemoteProgram, host.CancellationToken);
                var programWithoutLineNumbers = ProgramContentConverter.ToPC(program.Content);
                program.Content = programWithoutLineNumbers;
                File.WriteAllText(dialog.FileName, program.Content);
            }

            DialogHostIsOpen = false;
        }

        /// <summary>
        /// Deletes specified program and position data.
        /// </summary>
        /// <param name="obj">The object.</param>
        private async void Delete(object obj)
        {
            DialogHostIsOpen = true;
            var host = CreateDialogHost(true, $"Deleting {SelectedRemoteProgram.Name}");
            programService.DeleteProgram(SelectedRemoteProgram.Name, host.CancellationToken).RunSynchronously();
            await Task.Delay(2000);
            Refresh(null);
        }

        /// <summary>
        /// Occurs after user triggers upload event.
        /// </summary>
        /// <param name="obj">The object.</param>
        private async void Upload(object obj)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Text files (.txt)|*.txt",
                CheckFileExists = true
            };
            

            if (dialog.ShowDialog().GetValueOrDefault(false))
            {
                var path = dialog.FileName;

                if (string.IsNullOrWhiteSpace(path) || Uri.IsWellFormedUriString(path, UriKind.RelativeOrAbsolute))
                    return;

                var name = Path.GetFileNameWithoutExtension(path);
                var content = File.ReadAllText(dialog.FileName);
                var program = new Program(name) {Content = content};

                DialogHostIsOpen = true;
                var host = CreateDialogHost(false, $"Uploading program");
                var contentWithLineNumbers = ProgramContentConverter.ToManipulator(program.Content);
                program.Content = contentWithLineNumbers;
                
                await programService.UploadProgram(program, host.CancellationToken);
            }
        }

        /// <summary>
        /// Occurs after user triggers send event.
        /// </summary>
        /// <param name="obj">The object.</param>
        private async void Send(object obj = null)
        {
            if (Manipulator.Connected && !string.IsNullOrWhiteSpace(commandInputText))
            {
                if (commandInput.DoSyntaxCheck != true) //if user dont want to check syntax just send it right away
                {
                    //syntaxCheckVisualizer.Visualize(true, line);
                    MessageList.AddMessage(new Message(DateTime.Now, commandInput.Text, Message.Type.Send));
                    CommandHistoryText += MessageList.Messages[MessageList.Messages.Count - 1].DisplayMessage();
                    manipulator.SendCustom(MessageList.Messages[MessageList.Messages.Count - 1].MyMessage); //send
                    commandHistory.ScrollToEnd();
                    CommandInputText = string.Empty;
                    messageSelectionArrows = 0;
                }
                else //if user wants to check syntax
                {
                    var isLineValid = await commandInput.ValidateLine(1);

                    if (isLineValid)    //if line is valid, send it
                    {
                        MessageList.AddMessage(new Message(DateTime.Now, commandInput.Text, Message.Type.Send));
                        CommandHistoryText += MessageList.Messages[MessageList.Messages.Count - 1].DisplayMessage();
                        manipulator.SendCustom(MessageList.Messages[MessageList.Messages.Count - 1].MyMessage); //send
                        commandHistory.ScrollToEnd();
                        CommandInputText = string.Empty;
                        messageSelectionArrows = 0;
                    }
                    else //if line is not valid colorize line and don't send
                    {
                        lineWasNotValid = true;
                    }
                }
            }
        }

        /// <summary>
        /// Stops the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void Stop(object obj)
        {
            programService.StopProgram().RunSynchronously();
        }

        /// <summary>
        /// Runs the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void Run(object obj)
        {
            programService.RunProgram(SelectedRemoteProgram).RunSynchronously();
        }

        /// <summary>
        /// Occurs after user triggers font reduce event.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void FontReduce(object obj = null)
        {
            if (commandHistory.FontSize > 3)
            {
                commandHistory.FontReduce();
            }
        }

        /// <summary>
        /// Occurs after user triggers font enlarge event.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void FontEnlarge(object obj = null)
        {
            if (commandHistory.FontSize < 20)
            {
                commandHistory.FontEnlarge();
            }
        }

        /// <summary>
        /// Exports current content of Command History to a file.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void ExportHistory(object obj)
        {
            commandHistory.ExportContent(DateTime.Now.ToString(CultureInfo.InvariantCulture).Replace(':', '-'), "txt");
        }

        /// <summary>
        /// Clears current content of Command History.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void ClearHistory(object obj)
        {
            CommandHistoryText = string.Empty;
        }

        /// <summary>
        /// Sets current font.
        /// </summary>
        /// <param name="obj">Current font.</param>
        private void ChangeFont(object obj)
        {
            var font = obj as string;

            commandInput.ChangeFont(font);
            commandHistory.ChangeFont(font);
        }

        /// <summary>
        /// Occurs when there is any key down while having focus on Command Input editor.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        private void commandInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !commandInput.IsIntellisenseShowing)
                Send();


            if (!commandInput.IsIntellisenseShowing)  //if theres no completion window use arrows to show previous messages
            {
                var sentMessages = MessageList.Messages.Where(i => i.MyType == Message.Type.Send).ToList();

                if (e.Key == Key.Up)
                {
                    if (messageSelectionArrows < sentMessages.Count)
                    {
                        commandInput.Text = sentMessages[sentMessages.Count - ++messageSelectionArrows].MyMessage;
                        //commandInput.Text = MessageList.Messages[MessageList.Messages.Count - ++messageSelectionArrows].MyMessage;
                        commandInput.TextArea.Caret.Offset = commandInput.Text.Length;  //bring carret to end of text
                    }
                }
                else if (e.Key == Key.Down)
                {
                    if (messageSelectionArrows > 1)
                    {
                        commandInput.Text = sentMessages[sentMessages.Count - --messageSelectionArrows].MyMessage;
                        //commandInput.Text = MessageList.Messages[MessageList.Messages.Count - --messageSelectionArrows].MyMessage;
                        commandInput.TextArea.Caret.Offset = commandInput.Text.Length;  //bring carret to end of text
                    }
                    else if (messageSelectionArrows > 0)
                    {
                        --messageSelectionArrows;
                        commandInput.Text = string.Empty;
                    }
                }
            }
        }

        /// <summary>
        /// Occurs when there is any mouse scroll press/movement while having focus on Command Input editor.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseWheelEventArgs"/> instance containing the event data.</param>
        private void commandHistory_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var handle = (Keyboard.Modifiers & ModifierKeys.Control) > 0;
            if (!handle)
                return;

            if (e.Delta > 0)    //scrolls away from user
                FontEnlarge();
            else if (e.Delta < 0)
                FontReduce();   //scrolls toward user
        }

        #endregion

        #region Commands

        /// <summary>
        /// Gets the refresh click command.
        /// </summary>
        /// <value>
        /// The refresh click command.
        /// </value>
        public ICommand RefreshClickCommand { get; private set; }
        /// <summary>
        /// Gets the download click command.
        /// </summary>
        /// <value>
        /// The download click command.
        /// </value>
        public ICommand DownloadClickCommand { get; private set; }
        /// <summary>
        /// Gets the upload click command.
        /// </summary>
        /// <value>
        /// The upload click command.
        /// </value>
        public ICommand UploadClickCommand { get; private set; }
        /// <summary>
        /// Gets the send click command.
        /// </summary>
        /// <value>
        /// The send click command.
        /// </value>
        public ICommand SendClickCommand { get; private set; }
        /// <summary>
        /// Gets the run click command.
        /// </summary>
        /// <value>
        /// The run click command.
        /// </value>
        public ICommand RunClickCommand { get; private set; }
        /// <summary>
        /// Gets the delete click command.
        /// </summary>
        /// <value>
        /// The delete click command.
        /// </value>
        public ICommand DeleteClickCommand { get; private set; }
        /// <summary>
        /// Gets the clear history command.
        /// </summary>
        /// <value>
        /// The clear history command.
        /// </value>
        public ICommand ClearHistoryCommand { get; private set; }
        /// <summary>
        /// Gets the export history command.
        /// </summary>
        /// <value>
        /// The export history command.
        /// </value>
        public ICommand ExportHistoryCommand { get; private set; }
        /// <summary>
        /// Gets the change font command.
        /// </summary>
        /// <value>
        /// The change font command.
        /// </value>
        public ICommand ChangeFontCommand { get; private set; }
        /// <summary>
        /// Gets the connection command.
        /// </summary>
        /// <value>
        /// The connection command.
        /// </value>
        public ICommand ConnectionCommand { get; private set; }
        /// <summary>
        /// Gets the refresh COM ports command.
        /// </summary>
        /// <value>
        /// The refresh COM ports command.
        /// </value>
        public ICommand RefreshCOMPortsCommand { get; private set; }

        /// <summary>
        /// Declares the commands.
        /// </summary>
        private void DeclareCommands()
        {
            RefreshClickCommand = new RelayCommand(Refresh, IsConnectionEstablished);
            DownloadClickCommand = new RelayCommand(Download);
            UploadClickCommand = new RelayCommand(Upload, IsConnectionEstablished);
            SendClickCommand = new RelayCommand(Send, IscommandInputNotEmpty);
            RunClickCommand = new RelayCommand(Run, IsConnectionEstablished);
            DeleteClickCommand = new RelayCommand(Delete, IsConnectionEstablished);
            ClearHistoryCommand = new RelayCommand(ClearHistory, IscommandHistoryNotEmpty);
            ExportHistoryCommand = new RelayCommand(ExportHistory, IscommandHistoryNotEmpty);
            ChangeFontCommand = new RelayCommand(ChangeFont, CanChangeFont);
            ConnectionCommand = new RelayCommand(Connection);
            RefreshCOMPortsCommand = new RelayCommand(RefreshCOMPorts);
        }

        /// <summary>
        /// Refreshes the COM ports.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void RefreshCOMPorts(object obj)
        {
            AvailableCOMPorts = new ObservableCollection<string>(SerialPort.GetPortNames());
        }

        /// <summary>
        /// Connections the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        private async void Connection(object obj)
        {
            if (null != obj)
            {
                var state = (bool)obj;
                if (!state)
                {
                    Manipulator?.Disconnect();
                }
                else
                {
                    if (string.IsNullOrEmpty(SelectedCOMPort))
                    {
                        BadgeText = "!";
                        ConnectionToggleIsChecked = false;
                        return;
                    }

                    ConnectionToggleIsChecked = true;
                    Manipulator = new E3JManipulator(Settings);
                    Manipulator.Port.ConnectionStatusChanged += Port_ConnectionStatusChanged;
                    Manipulator.Connect(SelectedCOMPort);
                    Manipulator.Port.DataReceived += Port_DataReceived;
                }
            }
        }

        /// <summary>
        /// Ports the data received.
        /// </summary>
        /// <param name="data">The data.</param>
        private void Port_DataReceived(string data)
        {
            data = data.Replace("\r", string.Empty);
            MessageList.AddMessage(new Message(DateTime.Now, data, Message.Type.Received));

            var receivedMessages = MessageList.Messages.Where(i => i.MyType == Message.Type.Received).ToList();
            CommandHistoryText += receivedMessages[receivedMessages.Count - 1].DisplayMessage();
            commandHistory.Dispatcher.Invoke(() => commandHistory.ScrollToEnd());
        }

        /// <summary>
        /// Determines whether this instance [can change font] the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        ///   <c>true</c> if this instance [can change font] the specified object; otherwise, <c>false</c>.
        /// </returns>
        private bool CanChangeFont(object obj)
        {
            var text = commandHistory.FontFamily.ToString();
            return !text.Equals(obj as string);
        }

        /// <summary>
        /// Return a value based upon wheter Command History is empty or not.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        private bool IscommandHistoryNotEmpty(object obj)
        {
            return !string.IsNullOrWhiteSpace(CommandHistoryText);
        }

        /// <summary>
        /// Return a value based upon wheter a connection between computer and RV-E3J manipulator was established or not.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        ///   <c>true</c> if [is connection established] [the specified object]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsConnectionEstablished(object obj)
        {
            var isConnected = Manipulator.Connected;
            if (!isConnected)
                ConnectionToggleIsChecked = false;

            return isConnected;
        }


        /// <summary>
        /// Returns a value based upon wheter a Command Input is empty or not.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        private bool IscommandInputNotEmpty(object obj)
        {
            return !string.IsNullOrWhiteSpace(CommandInputText);
        }

        #endregion

    }
}
