using System.Windows.Controls;
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
            this.DataContext = new AboutViewModel();
        }
    }
}
