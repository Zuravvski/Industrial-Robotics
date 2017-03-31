using IDE.Common.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace IDE.Views.Settings
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : UserControl
    {
        public About()
        {
            InitializeComponent();
            this.DataContext = new AboutViewModel();
        }
    }
}
