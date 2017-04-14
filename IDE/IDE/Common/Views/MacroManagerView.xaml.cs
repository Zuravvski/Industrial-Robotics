using IDE.Common.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IDE.Common.Views
{
    /// <summary>
    /// Interaction logic for MacroManagerView.xaml
    /// </summary>
    public partial class MacroManagerView : UserControl
    {
        public MacroManagerView()
        {
            InitializeComponent();
            DataContext = new MacroManagerViewModel();
        }
    }
}
