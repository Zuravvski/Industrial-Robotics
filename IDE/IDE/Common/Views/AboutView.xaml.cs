using System.Windows.Controls;
using IDE.Common.Utilities;
using IDE.Common.ViewModels;

namespace IDE.Common.Views
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : UserControl
    {
        public About()
        {
            InitializeComponent();
            DataContext = new AboutViewModel();
            Session.Instance.InitializeColors();
        }
    }
}
