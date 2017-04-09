using IDE.Common.Models;
using IDE.Common.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace IDE.Common.ViewModel
{
    public class BrowseViewModel : ObservableObject
    {

        public BrowseViewModel()
        {
            RefreshClickCommand = new RelayCommand(Refresh);
            DownloadClickCommand = new RelayCommand(Download);
            UploadClickCommand = new RelayCommand(Upload);
            SendClickCommand = new RelayCommand(Send);

            MessageList = new MessageList();
        }

        public MessageList MessageList { get; }
        public string UserInputText { set; get; }



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
                MessageList.AddMessage(new Message(DateTime.Now.ToString(), UserInputText));
                //UserInputText = string.Empty;             nie dziala?
            }
        }






        public ICommand RefreshClickCommand { get; private set; }
        public ICommand DownloadClickCommand { get; private set; }
        public ICommand UploadClickCommand { get; private set; }
        public ICommand SendClickCommand { get; private set; }


    }
}
