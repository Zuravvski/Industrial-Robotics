using System.Windows.Controls;
using IDE.Common.ViewModel;

namespace IDE.Views
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
