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
        private readonly E2JManipulator manipulator;
        public MainWindow()
        {
            InitializeComponent();
            manipulator = new E2JManipulator();
            manipulator.Connect("COM3");
            it = 1;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            manipulator.MoveTool(1, 20, E2JManipulator.GrabE.Open);
        }

        private void savePosition_button_Click(object sender, RoutedEventArgs e)
        {
            manipulator.Here(it++);
            //manipulator.Where();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            manipulator.Speed(5);
        }
    }
}
