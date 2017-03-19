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
        private readonly E2JManipulator manipulator;
        public MainWindow()
        {
            InitializeComponent();
            manipulator = new E2JManipulator();
            manipulator.Connect("COM3");
            it = 6;
            speed = 5;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            switch (it++)
            {
                case 1:
                    manipulator.GrabOpen();
                    break;
                case 2:
                    manipulator.Move(1);
                    break;
                case 3:
                    manipulator.GrabClose();
                    break;
                case 4:
                    manipulator.Draw(0,0,25);
                    break;
                case 5:
                    manipulator.MoveRA(2, E2JManipulator.GrabE.Closed);
                    break;
                case 6:
                    manipulator.GrabOpen();
                    break;
                case 7:
                    manipulator.Draw(0,0,25);
                    break;
                case 8:
                    manipulator.Move(3);
                    break;
                case 9:
                    manipulator.GrabClose();
                    break;
                case 10:
                    manipulator.Draw(0,0,25);
                    break;
                case 11:
                    manipulator.Move(1, E2JManipulator.GrabE.Closed);
                    break;
                case 12:
                    manipulator.GrabOpen();
                    break;
                case 13:
                    manipulator.Draw(0,0,25);
                    break;
                case 14:
                    manipulator.Move(2);
                    break;
                case 15:
                    manipulator.GrabClose();
                    break;
                case 16:
                    manipulator.Draw(0,0,35);
                    break;
                case 17:
                    manipulator.Move(3, E2JManipulator.GrabE.Closed);
                    break;
                case 18:
                    manipulator.GrabOpen();
                    break;
                case 19:
                    manipulator.Draw(0, 0, 55);
                    break;
                default:
                    it = 1;
                    break;
            }
        }

        private void savePosition_button_Click(object sender, RoutedEventArgs e)
        {
            //manipulator.Here(3);
            manipulator.Where();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            speed = speed < 30 ? speed += 5 : 20;
            manipulator.Speed(speed);
        }
    }
}
