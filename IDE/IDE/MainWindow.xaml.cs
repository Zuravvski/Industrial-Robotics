using System.Windows;

namespace IDE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Console console;
        public MainWindow()
        {
            InitializeComponent();
            console = new Console();
            mainView.Children.Add(console);
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            console.uploadProgram();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            console.downloadProgram();
        }
    }
}
