using System.Windows.Controls;
using IDE.Common.ViewModels;

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
            InitializeComponent();
            DataContext = new EditorViewModel();
        }

        #endregion
    }
}
