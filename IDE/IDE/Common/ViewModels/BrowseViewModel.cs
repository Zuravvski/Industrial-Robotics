using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using IDE.Common.Models;
using IDE.Common.ViewModels.Commands;

namespace IDE.Common.ViewModels
{
    public class BrowseViewModel : ObservableObject
    {
        private string userInputText;
        private double fontSize;
        ProgramEditor programEditor;

        public BrowseViewModel()
        {
            RefreshClickCommand = new RelayCommand(Refresh);
            DownloadClickCommand = new RelayCommand(Download);
            UploadClickCommand = new RelayCommand(Upload);
            SendClickCommand = new RelayCommand(Send);
            FontEnlargeCommand = new RelayCommand(FontEnlarge);
            FontReduceCommand = new RelayCommand(FontReduce);

            MessageList = new MessageList();
            ProgramEditor = new ProgramEditor(ProgramEditor.Highlighting.On, ProgramEditor.ReadOnly.Yes);
        }

        private void FontReduce(object obj)
        {
            if (ProgramEditor.FontSize > 3)
                ProgramEditor.FontSize--;
        }

        private void FontEnlarge(object obj)
        {
            if (ProgramEditor.FontSize < 20)
                ProgramEditor.FontSize++;
        }

        public MessageList MessageList { get; }
        public ProgramEditor ProgramEditor
        {
            set
            {
                programEditor = value;
                NotifyPropertyChanged("ProgramEditor");
            }
            get
            {
                return programEditor;
            }
        }
        public string UserInputText
        {
            set
            {
                userInputText = value;
                NotifyPropertyChanged("UserInputText");
            }
            get
            {
                return userInputText;
            }
        }



        private void Refresh(object obj)
        {
            MessageBox.Show("pop");
        }

        private void Download(object obj)
        {
            MessageBox.Show("pop");
        }
        private void Upload(object obj)
        {
            MessageBox.Show("pop");
        }

        private void Send(object obj)
        {
            if (!string.IsNullOrEmpty(UserInputText))
            {
                MessageList.AddMessage(new Message(DateTime.Now.ToString(CultureInfo.InvariantCulture), UserInputText));
                ProgramEditor.Text += MessageList.Messages[MessageList.Messages.Count - 1].MyTime.ToString() + ": " + 
                    MessageList.Messages[MessageList.Messages.Count - 1].MyMessage.ToString() + "\n";
                ProgramEditor.ScrollToEnd();
                UserInputText = string.Empty;
            }
        }



        public ICommand RefreshClickCommand { get; private set; }
        public ICommand DownloadClickCommand { get; private set; }
        public ICommand UploadClickCommand { get; private set; }
        public ICommand SendClickCommand { get; private set; }
        public ICommand FontEnlargeCommand { get; private set; }
        public ICommand FontReduceCommand { get; private set; }

    }
}
