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

        public BrowseViewModel()
        {
            RefreshClickCommand = new RelayCommand(Refresh);
            DownloadClickCommand = new RelayCommand(Download);
            UploadClickCommand = new RelayCommand(Upload);
            SendClickCommand = new RelayCommand(Send);

            MessageList = new MessageList();
        }

        public MessageList MessageList { get; }
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
                UserInputText = string.Empty;
            }
        }






        public ICommand RefreshClickCommand { get; private set; }
        public ICommand DownloadClickCommand { get; private set; }
        public ICommand UploadClickCommand { get; private set; }
        public ICommand SendClickCommand { get; private set; }


    }
}
