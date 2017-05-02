using IDE.Common.ViewModels.Commands;
using System.Windows.Input;
using System;
using System.Diagnostics;
using IDE.Common.Utilities;
using System.IO;

namespace IDE.Common.ViewModels
{
    public class HomeViewModel : ObservableObject
    {

        #region Fields



        #endregion

        #region Constructor

        public HomeViewModel()
        {
            DeclareCommands();
        }

        #endregion

        #region Properties

        public AppearanceViewModel Appearance => AppearanceViewModel.Instance;

        #endregion

        #region Actions



        #endregion

        #region Commands

        public ICommand OpenOverviewClipCommand { get; private set; }

        private void DeclareCommands()
        {
            OpenOverviewClipCommand = new RelayCommand(OpenOverviewClip);
        }

        private void OpenOverviewClip(object obj)
        {
            Process.Start("https://www.youtube.com/watch?v=7LKHpM1UeDA");
        }


        #endregion

    }
}
