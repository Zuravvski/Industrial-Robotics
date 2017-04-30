using System.Windows.Controls;
using IDE.Common.ViewModels;
using FirstFloor.ModernUI.Windows.Controls;
using System;
using RadialMenu.Controls;
using System.Collections.Generic;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Input;

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
