using System.Windows;
using ManipulatorDriver;

namespace manipulatorDriver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private E2JManipulator manipulator;
        public MainWindow()
        {
            InitializeComponent();
            manipulator = new E2JManipulator();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            manipulator.Connect("COM4");
            //manipulator.MovePosition(360,-60,638,95,138);
            manipulator.Draw(0,0,10);
        }
    }
}
