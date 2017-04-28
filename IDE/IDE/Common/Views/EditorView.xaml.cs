using System.Windows.Controls;
using IDE.Common.ViewModels;
using FirstFloor.ModernUI.Windows.Controls;

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
            DataContext = new Editor_v2ViewModel();
        }

        #endregion

    }
}
