using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace manipulatorDriver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Manipulator manipulator;
        public MainWindow()
        {
            InitializeComponent();
            manipulator = new Manipulator();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            manipulator.OpenPort("COM4");
            //manipulator.MovePosition(360,-60,638,95,138);
            manipulator.Draw(0,0,10);
        }
    }
}
