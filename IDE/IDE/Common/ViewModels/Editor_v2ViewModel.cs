using Dragablz;
using FirstFloor.ModernUI.Windows.Controls;
using IDE.Common.Utilities;
using IDE.Common.ViewModels.Commands;
using IDE.Common.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace IDE.Common.ViewModels
{
    public class Editor_v2ViewModel : ObservableObject
    {

        #region Fields
        


        #endregion

        #region Constructor
        public Editor_v2ViewModel()
        {
            DeclareCommands();
        }

    

        #endregion

        #region Properties
        

        #endregion

        #region Actions


        #endregion

        #region Commands

        public ICommand CloseClickCommand { get; private set; }
        public ICommand NewTabFactory { get; private set; }

        private void DeclareCommands()
        {
            CloseClickCommand = new RelayCommand(Close);
            NewTabFactory = new RelayCommand(NewTab);
        }

        private void NewTab(object obj)
        {
            throw new NotImplementedException();
        }

        private void Close(object obj)
        {
            
        }

        #endregion

    }
}
