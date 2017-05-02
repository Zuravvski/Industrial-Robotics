using IDE.Common.ViewModels;
using System.Windows.Controls;

namespace IDE.Common.Views
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {
        public Home()
        {
            InitializeComponent();
            DataContext = new HomeViewModel();
        }
    }
}
