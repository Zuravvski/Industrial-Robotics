using FirstFloor.ModernUI.Windows.Controls;
using System.Windows;

namespace IDE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow
    {
        private readonly Console console;
        public MainWindow()
        {
            InitializeComponent();
            console = new Console();
        }
    }
}
