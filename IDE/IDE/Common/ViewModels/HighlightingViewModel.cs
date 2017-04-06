using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace IDE.Common.ViewModels
{
    class HighlightingViewModel : INotifyPropertyChanged
    {
        Button button;
        public ObservableCollection<string> ColorCollection { get; set; }
        public ICommand TestCommand { get; private set; }

        public HighlightingViewModel()
        {
            ColorCollection = new ObservableCollection<string>();
            ColorCollection.Add("Some dummy string");
        }
        


        #region PropertyChangedEvents

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}
