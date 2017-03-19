using System.Windows;
using ManipulatorDriver;
using System.Diagnostics;

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
                case 1: //do nad-pierwszego
                    manipulator.MoveTool(1, 0, E2JManipulator.GrabE.Open);
                    break;
                case 2: //do pierwszego
                    manipulator.MoveTool(2, 0, E2JManipulator.GrabE.Open);
                    break;
                case 3: //zamknij + w gore
                    manipulator.MoveTool(1, 0, E2JManipulator.GrabE.Closed);
                    break;
                case 4: //do nad-drugiego
                    manipulator.MoveTool(3, 0, E2JManipulator.GrabE.Closed);
                    break;
                case 5: //do drugiego
                    manipulator.MoveTool(4, 0, E2JManipulator.GrabE.Closed);
                    break;
                case 6: //otworz + w gore
                    manipulator.MoveTool(3, 0, E2JManipulator.GrabE.Open);
                    break;
                case 7: //do nad-trzeciego
                    manipulator.MoveTool(5, 0, E2JManipulator.GrabE.Open);
                    break;
                case 8: //do trzeciego
                    manipulator.MoveTool(6, 0, E2JManipulator.GrabE.Open);
                    break;
                case 9: //zamknij + w gore
                    manipulator.MoveTool(5, 0, E2JManipulator.GrabE.Closed);
                    break;
                case 10: //do nad-pierwszego
                    manipulator.MoveTool(1, 0, E2JManipulator.GrabE.Closed);
                    break;
                case 11: //do pierwszego
                    manipulator.MoveTool(7, 0, E2JManipulator.GrabE.Closed);
                    break;
                case 12: //otworz + w gore
                    manipulator.MoveTool(1, 0, E2JManipulator.GrabE.Open);
                    break;
                case 13: //do nad-drugiego
                    manipulator.MoveTool(3, 0, E2JManipulator.GrabE.Open);
                    break;
                case 14: //do drugiego
                    manipulator.MoveTool(4, 0, E2JManipulator.GrabE.Open);
                    break;
                case 15: //zamknij + w gore
                    manipulator.MoveTool(3, 0, E2JManipulator.GrabE.Closed);
                    break;
                case 16: //do nad-trzeciego
                    manipulator.MoveTool(5, 0, E2JManipulator.GrabE.Closed);
                    break;
                case 17: //do trzeciego
                    manipulator.MoveTool(6, 0, E2JManipulator.GrabE.Closed);
                    break;
                case 18: //otworz + odjedz daleko
                    manipulator.MoveTool(1, 0, E2JManipulator.GrabE.Open);
                    break;
                default:
                    it = 1;
                    break;
            }
            //manipulator.PositionClear(1);
            //manipulator.PositionClear(2);
            //manipulator.PositionClear(3);
            //manipulator.MoveTool(1, 20, E2JManipulator.GrabE.Open);
        }

        private void savePosition_button_Click(object sender, RoutedEventArgs e)
        {
            //manipulator.PositionRead(it++);
            manipulator.LineRead();
            //manipulator.Here(7);
            //manipulator.Where();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            //it = 1;
            //manipulator.Speed(20);
            manipulator.New();
        }
    }
}
