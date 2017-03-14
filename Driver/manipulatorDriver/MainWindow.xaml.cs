using System.Windows;
using ManipulatorDriver;
using System.Globalization;
using System.Threading;
using System;

namespace manipulatorDriver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int it;
        private E2JManipulator manipulator;
        public MainWindow()
        {
            InitializeComponent();
            manipulator = new E2JManipulator();
            manipulator.Connect("COM4");
            it = 0;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            manipulator.MoveToolStraight(1,-100);
            label_x.Content = manipulator.X;
            label_y.Content = manipulator.Y;
            label_z.Content = manipulator.Z;
            label_a.Content = manipulator.A;
            label_b.Content = manipulator.B;
           
        }

        private void savePosition_button_Click(object sender, RoutedEventArgs e)
        {
            manipulator.Here(1);
        }
    }
}
