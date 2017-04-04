using System.Windows;
using ManipulatorDriver;

namespace manipulatorDriver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private uint it;
        private uint speed;
        private readonly E3JManipulator manipulator;
        public MainWindow()
        {
            InitializeComponent();
            manipulator = new E3JManipulator();
            manipulator.Connect("COM4");
            it = 6;
            speed = 5;
        }
    }
}
