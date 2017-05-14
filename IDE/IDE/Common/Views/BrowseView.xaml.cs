using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using IDE.Common.ViewModels;

namespace IDE.Common.Views
{
    /// <summary>
    /// Interaction logic for Browse.xaml
    /// </summary>
    public partial class Browse : UserControl 
    {
        #region Constructor

        public Browse()
        {
            InitializeComponent();
            DataContext = new BrowseViewModel(CommandHistory, CommandInput);
        }

        #endregion
    }
}
