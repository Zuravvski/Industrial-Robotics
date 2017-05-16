using System.Windows.Controls;
using FirstFloor.ModernUI.Presentation;
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

        private void ContextMenu_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {

        }
    }
}
