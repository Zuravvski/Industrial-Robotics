using System.Diagnostics;
using System.Windows;
using Driver;

namespace POCs
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private E3JManipulator manipulator;

        public MainWindow()
        {
            InitializeComponent();
            manipulator = new E3JManipulator();
        }

        private void buttonConnect_Click(object sender, RoutedEventArgs e)
        {
            manipulator.Connect("COM5");
            Debug.WriteLine("Connected: manipulator.Port.Connected");
        }

        private void buttonGO_Click(object sender, RoutedEventArgs e)
        {
            manipulator.GrabOpen();
        }

        private void buttonGC_Click(object sender, RoutedEventArgs e)
        {
            manipulator.GrabClose();
        }

        private void buttonWH_Click(object sender, RoutedEventArgs e)
        {
            manipulator.Where();
        }
    }
}
