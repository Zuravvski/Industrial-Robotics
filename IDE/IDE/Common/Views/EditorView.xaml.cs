using System.Windows.Controls;
using IDE.Common.ViewModels;
using System.Windows;

namespace IDE.Common.Views
{
    /// <summary>
    /// Interaction logic for Editor.xaml
    /// </summary>
    public partial class Editor : UserControl
    {

        #region Constructor

        public Editor()
        {
            DataContext = new EditorViewModel();
            InitializeComponent();
        }


        #endregion
        
    }
}
