using System.Windows.Controls;
using IDE.Common.ViewModels;
using System.Windows;
using MaterialMenu;

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
            DataContext = new EditorViewModel();
            InitializeComponent();
        }


        #endregion

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Menu.Toggle();
        }

        private void MenuButton_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }
    }
}
